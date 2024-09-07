using BlurFileFormats.SerializationFramework;
using BlurFileFormats.Utils;
using BlurFileFormats.XtFlask.Components;
using BlurFileFormats.XtFlask.Entities;
using BlurFileFormats.XtFlask.Types;
using BlurFileFormats.XtFlask.Types.Fields;
using BlurFileFormats.XtFlask.Types.Fields.Behaviors;
using BlurFileFormats.XtFlask.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.XtFlask;
public static class Flask
{
    public static XtDb Import(string file)
    {
        using var fileStream = File.OpenRead(file);
        return Import(fileStream);
    }
    public static XtDb Import(Stream source)
    {
        var flaskEntity = DataSerializer.Deserialize<FlaskEntity>(source);

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

        List<ValueResolver.IResolverItem> resolveValues = [];

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
            IFieldBehavior behavior;
            switch (field.Behavior)
            {
                case FieldBehavior.Atom:
                case FieldBehavior.Enum:
                case FieldBehavior.Flags:
                case FieldBehavior.Struct:
                    behavior = new ObjectFieldBehavior();
                    break;
                case FieldBehavior.Pointer:
                    behavior = new PointerFieldBehavior();
                    break;
                case FieldBehavior.Handle:
                    behavior = new HandleFieldBehavior();
                    break;
                default:
                    throw new NotSupportedException();
            }
            if(field.Array)
            {
                behavior = new ArrayFieldBehavior(behavior);
            }
            Type.Fields.Add(new XtField(Type, fieldType, GetCString(Strings, field.Name), behavior));
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
    public List<IResolverItem> ResolveItems { get; }
    public IReadOnlyList<IXtRef> References { get; }
    public List<List<IRecordComponent>> RefRecords { get; }

    public ValueResolver(string strings, List<IResolverItem> resolveItems, IReadOnlyList<IXtRef> references, List<List<IRecordComponent>> refRecords)
    {
        Strings = strings;
        ResolveItems = resolveItems;
        References = references;
        RefRecords = refRecords;
    }
    public void AddPointer(XtStructValue value, XtField field, BinaryReader reader)
    {
        ResolveItems.Add(PointerItem.Read(References, RefRecords, value, field, reader));
    }
    public void AddHandle(XtStructValue value, XtField field, BinaryReader reader)
    {
        ResolveItems.Add(HandleItem.Read(References, RefRecords, value, field, reader));
    }
    public void AddArray(XtStructValue value, XtField field, BinaryReader reader)
    {
        ResolveItems.Add(ArrayItem.Read(References, RefRecords, value, field, reader));
    }
    public string GetString(uint index)
    {
        if (index == uint.MaxValue) return "";
        return GetCString(Strings, (int)index);
    }
    public static string GetCString(string s, int offset) => string.Concat(s.Skip(offset).TakeWhile(c => c != '\0'));

    public interface IResolverItem
    {
        public void Resolve();
    }

    public record PointerItem(IReadOnlyList<IXtRef> References, List<List<IRecordComponent>> RefRecords, XtStructValue Value, XtField Field, ushort Component, ushort Offset) : IResolverItem
    {
        public static PointerItem Read(IReadOnlyList<IXtRef> references, List<List<IRecordComponent>> refRecords, XtStructValue value, XtField field, BinaryReader reader)
        {
            return new PointerItem(references, refRecords, value, field, reader.ReadUInt16(), reader.ReadUInt16());
        }

        public void Resolve()
        {
            if (Component == ushort.MaxValue || Offset == ushort.MaxValue) return;
            Value.SetField(Field, RefRecords[Component][Offset].GetValue(References, RefRecords));
        }
    }
    public record HandleItem(IReadOnlyList<IXtRef> References, List<List<IRecordComponent>> RefRecords, XtStructValue Value, XtField Field, uint Id) : IResolverItem
    {
        public static HandleItem Read(IReadOnlyList<IXtRef> references, List<List<IRecordComponent>> refRecords, XtStructValue value, XtField field, BinaryReader reader)
        {
            return new HandleItem(references, refRecords, value, field, reader.ReadUInt32());
        }

        public void Resolve()
        {
            if (Id == uint.MaxValue) return;
            Value.SetField(Field, new XtRefValue(References[(int)Id]));
        }
    }
    public record ArrayItem(IReadOnlyList<IXtRef> References, List<List<IRecordComponent>> RefRecords, XtStructValue Value, XtField Field, ushort Component, ushort Offset, uint Length) : IResolverItem
    {
        public static ArrayItem Read(IReadOnlyList<IXtRef> references, List<List<IRecordComponent>> refRecords, XtStructValue value, XtField field, BinaryReader reader)
        {
            return new ArrayItem(references, refRecords, value, field, reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt32());
        }

        public void Resolve()
        {
            var array = new XtArrayValue(Value.XtType);
            for(int i = Offset; i < Length; i++)
            {
                array.Values.Add(new XtArrayValueItem(RefRecords[Component][i].GetValue(References, RefRecords)));
            }
            Value.SetField(Field, array);
        }
    }
}
public interface IXtRef
{
    public uint Id { get; }
    public IXtType Type { get; }
    public IXtValue Value { get; }
}
public class XtRef : IXtRef
{
    public uint Id { get; set; }
    public IXtValue Value { get; set; }
    IXtType IXtRef.Type => Value.Type;

    public XtRef(uint id, IXtValue value)
    {
        Id = id;
        Value = value;
    }
}
public class XtRefHandle : IXtRef
{
    public uint Id { get; set; }
    public IXtType Type { get; }
    public IXtValue Value => XtNullValue.Instance;

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
