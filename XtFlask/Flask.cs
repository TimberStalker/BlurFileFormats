using BlurFileFormats.SerializationFramework;
using BlurFileFormats.Utils;
using BlurFileFormats.XtFlask.Components;
using BlurFileFormats.XtFlask.Entities;
using BlurFileFormats.XtFlask.Types;
using BlurFileFormats.XtFlask.Types.Fields;
using BlurFileFormats.XtFlask.Values;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.XtFlask;
public static class Flask
{
    static DataSerializer<FlaskEntity> FlaskSerializer { get; } = DataSerializer.Build<FlaskEntity>();
    public static XtDb Import(string file)
    {
        using var fileStream = File.OpenRead(file);
        return Import(fileStream);
    }
    public static XtDb Import(Stream source)
    {
        var flaskEntity = FlaskSerializer.Deserialize(source);

        var db = new XtDb();
        
        CreateFlaskTypes(flaskEntity, db.Types);
        CreateRefs(flaskEntity, db.Types, db.References);

        return db;
    }

    private static void CreateRefs(FlaskEntity flaskEntity, List<IXtType> types, List<IXtRef> references)
    {
        int currentRecord = 0;
        int currentComponent = 0;

        var dataStream = new MemoryStream(flaskEntity.Data);
        var dataReader = new BinaryReader(dataStream);
        var encoding = new FlaskEncoding();

        List<ValueResolver.ResolverContext> resolveValues = [];

        for (int i = 0; i < flaskEntity.Refs.Length; i++)
        {
            var reference = flaskEntity.Refs[i];
            var type = types[reference.Type];
            if (reference.Record == ushort.MaxValue)
            {
                references.Add(new XtRefHandle(reference.Id, type));
                continue;
            }
            var record = flaskEntity.Records[currentRecord];

            List<List<IRecordComponent>> refRecords = [];

            var bytes = dataReader.ReadBytes((int)record.DataBytes);

            string strings = encoding.GetString(dataReader.ReadBytes((int)record.StringBytes));

            using var recordBytesStream = new MemoryStream(bytes);
            using var dataBytesReader = new BinaryReader(recordBytesStream);

            var resolver = new ValueResolver(strings, resolveValues, references, refRecords);
            for (int j = 0; j < record.ComponentCount; j++)
            {
                var component = flaskEntity.Components[currentComponent];
                var componentType = types[component.Type];

                List<IRecordComponent> recordComponents = [];

                for (int k = 0; k < component.Count; k++)
                {
                    switch (component.Behavior)
                    {
                        case ComponentBehavior.Object:
                            var item = componentType.ReadValue(dataBytesReader, resolver);
                            recordComponents.Add(new ObjectComponent(item));
                            break;
                        case ComponentBehavior.Ponter:
                            recordComponents.Add(new PointerComponent(refRecords, dataBytesReader.ReadUInt16(), dataBytesReader.ReadUInt16()));
                            break;
                        case ComponentBehavior.Handle:
                            recordComponents.Add(new HandleComponent(dataBytesReader.ReadUInt32()));
                            break;
                        default:
                            throw new NotSupportedException($"{component.Behavior} is not a valid component behavior.");
                    }
                }


                refRecords.Add(recordComponents);
                currentComponent++;
            }
            
            if (recordBytesStream.Length != recordBytesStream.Position) throw new Exception();

            references.Add(new XtRef(reference.Id, refRecords[0][0].GetValue(references, refRecords)));
            currentRecord++;
        }

        foreach (var item in resolveValues)
        {
            item.Resolve();
        }
    }
    
    private static void CreateFlaskTypes(FlaskEntity flaskEntity, List<IXtType> types)
    {
        foreach (var type in flaskEntity.Types)
        {
            string name = GetCString(flaskEntity.Strings, type.Name);
            switch (type.Behavior)
            {
                case TypeBehavior.Atom:
                    types.Add(XtAtomType.CreateType(type.Atom, name));
                    break;
                case TypeBehavior.Enum:
                    types.Add(new XtEnumType(name));
                    break;
                case TypeBehavior.Flags:
                    types.Add(new XtFlagsType(name));
                    break;
                case TypeBehavior.Struct:
                    types.Add(new XtStructType(name));
                    break;
            }
        }
        int k = 0;
        for (int i = 0; i < flaskEntity.Types.Length; i++)
        {
            FlaskTypeEntity type = flaskEntity.Types[i];
            if (type.BaseCount == 0) continue;
            if (types[i] is not XtStructType t) throw new NotSupportedException();

            for (int j = 0; j < type.BaseCount; j++)
            {
                var baseEntity = flaskEntity.Bases[k];
                if (types[baseEntity.Type] is not XtStructType bt) throw new NotSupportedException();
                t.Bases.Add(bt);
                k++;
            }
        }
        k = 0;
        for (int i = 0; i < flaskEntity.Types.Length; i++)
        {
            FlaskTypeEntity type = flaskEntity.Types[i];
            if (type.FieldCount == 0) continue;
            IFieldHandler handler;
            switch (types[i])
            {
                case XtStructType st:
                    handler = new StructFieldHandler(st, flaskEntity.Strings, types);
                    break;
                case IXtEnumType et:
                    handler = new EnumFieldHandler(et, flaskEntity.Strings);
                    break;
                default:
                    throw new NotSupportedException();
            }

            for (int j = 0; j < type.FieldCount; j++)
            {
                var fieldEntity = flaskEntity.Fields[k];
                handler.Add(fieldEntity);
                k++;
            }
        }
    }

    interface IFieldHandler
    {
        public void Add(FlaskFieldEntity field);
    }
    class StructFieldHandler : IFieldHandler
    {
        public XtStructType Type { get; }
        public string Strings { get; }
        public IReadOnlyList<IXtType> Types { get; }

        public StructFieldHandler(XtStructType type, string strings, IReadOnlyList<IXtType> types)
        {
            Type = type;
            Strings = strings;
            Types = types;
        }
        public void Add(FlaskFieldEntity field)
        {
            var fieldType = Types[field.Type];
            switch (field.Behavior)
            {
                case FieldBehavior.Atom:
                case FieldBehavior.Enum:
                case FieldBehavior.Flags:
                case FieldBehavior.Struct:
                    break;
                case FieldBehavior.Pointer:
                    fieldType = new XtPointerType(fieldType);
                    break;
                case FieldBehavior.Handle:
                    fieldType = new XtHandleType(fieldType);
                    break;
                default:
                    throw new NotSupportedException();
            }
            if(field.Array)
            {
                fieldType = new XtArrayType(fieldType);
            }
            Type.Fields.Add(new XtField(Type, fieldType, GetCString(Strings, field.Name)));
        }
    }
    class EnumFieldHandler : IFieldHandler
    {
        public IXtEnumType Type { get; }
        public string Strings { get; }

        public EnumFieldHandler(IXtEnumType type, string strings)
        {
            Type = type;
            Strings = strings;
        }
        public void Add(FlaskFieldEntity field)
        {
            Type.Labels.Add(GetCString(Strings, field.Name));
        }
    }
    public static string GetCString(string s, int offset) => string.Concat(s.Skip(offset).TakeWhile(c => c != '\0'));
}
public class ValueResolver
{
    public string Strings { get; }
    public List<ResolverContext> ResolveItems { get; }
    public IReadOnlyList<IXtRef> References { get; }
    public List<List<IRecordComponent>> RefRecords { get; }

    public ValueResolver(string strings, List<ResolverContext> resolveItems, IReadOnlyList<IXtRef> references, List<List<IRecordComponent>> refRecords)
    {
        Strings = strings;
        ResolveItems = resolveItems;
        References = references;
        RefRecords = refRecords;
    }
    public void AddResolver(IResolverItem resolverItem)
    {
        ResolveItems.Add(new ResolverContext(resolverItem, References, RefRecords));
    }
    public string GetString(uint index)
    {
        if (index == uint.MaxValue) return "";
        return GetCString(Strings, (int)index);
    }
    public static string GetCString(string s, int offset) => string.Concat(s.Skip(offset).TakeWhile(c => c != '\0'));

    public class ResolverContext
    {

        IResolverItem Resolver { get; }
        IReadOnlyList<IXtRef> References { get; }
        List<List<IRecordComponent>> RefRecords { get; }
        public ResolverContext(IResolverItem resolver, IReadOnlyList<IXtRef> references, List<List<IRecordComponent>> refRecords)
        {
            Resolver = resolver;
            References = references;
            RefRecords = refRecords;
        }
        public void Resolve()
        {
            Resolver.Resolve(References, RefRecords);
        }
    }
}
public interface IResolverItem
{
    public void Resolve(IReadOnlyList<IXtRef> references, List<List<IRecordComponent>> refRecords);
}
public interface IXtRef : IXtValue
{
    public uint Id { get; }
    new public IXtValue Value { get; }
}
public class XtRef : IXtRef
{
    public uint Id { get; set; }
    public IXtValue Value { get; set; }
    public IXtType Type => Value.Type;
    object IXtValue.Value => Value;

    public XtRef(uint id, IXtValue value)
    {
        Id = id;
        Value = value;
    }
}
public class XtRefNull : IXtRef
{
    public static XtRefNull Instance { get; } = new XtRefNull();

    public uint Id => 0;
    public IXtValue Value => XtNullValue.Instance;
    public IXtType Type => Value.Type;
    object IXtValue.Value => Value;
}
public class XtRefHandle : IXtRef
{
    public uint Id { get; set; }
    public IXtType Type { get; }
    public IXtValue Value => XtNullValue.Instance;

    object IXtValue.Value => Value;

    public XtRefHandle(uint id, IXtType type)
    {
        Id = id;
        Type = type;
    }
}
public class XtDb
{
    public List<IXtType> Types { get; } = [];
    public List<IXtRef> References { get; } = [];
}
