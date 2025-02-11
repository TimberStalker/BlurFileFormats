using System.Diagnostics;
using BlurFileFormats.XtFlask.Entities;
using BlurFileFormats.XtFlask.Types.Fields;
using BlurFileFormats.XtFlask.Values;
using Microsoft.VisualBasic.FileIO;

namespace BlurFileFormats.XtFlask.Types;

[DebuggerDisplay("struct {Name}")]
public class XtStructType : IXtType
{
    public IList<XtStructType> Bases { get; } = [];
    public IList<XtField> Fields { get; } = [];
    public string Name { get; }
    public int Size => Bases.Sum(b => b.Size) + Fields.Sum(f => f.Size);
    
    public XtStructType(string name)
    {
        Name = name;
    }

    public IEnumerable<XtField> GetFields() =>
        Bases
            .SelectMany(b => b.GetFields())
            .Concat(Fields);

    IXtValue IXtType.CreateDefault() => CreateValue();
    public XtStructValue CreateValue() => new XtStructValue(this);

    IXtValue IXtType.ReadValue(BinaryReader reader, ValueResolver resolver) => ReadValue(reader, resolver);
    public XtStructValue ReadValue(BinaryReader reader, ValueResolver resolver)
    {
        var value = CreateValue();
        foreach (var fieldItem in value.Values)
        {
            value.SetField(fieldItem.Field, fieldItem.Field.Type.ReadValue(reader, resolver));
        }
        return value;
    }

    public void Emit(TypeTableBuilder types, List<FlaskBaseEntity> bases, List<FlaskFieldEntity> fields, StringTableBuilder stringTable)
    {
        if (types.Contains(this)) return;
        int typeIndex = types.Count;
        types.Add(new FlaskTypeEntity()
        {
            Atom = AtomType.NotAtom,
            BaseCount = (ushort)Bases.Count,
            Behavior = TypeBehavior.Struct,
            FieldCount = (ushort)Fields.Count,
            Name = (ushort)stringTable.GetIndex(Name),
            Size = (ushort)Size,
        }, this);
        int offset = 0;
        foreach(var baseType in Bases)
        {
            var baseEntity = new FlaskBaseEntity()
            {
                Offset = (ushort)offset,
            };
            offset += baseType.Size;
            types.GetIndex(baseType, i => baseEntity.Type = (ushort)i);
            bases.Add(baseEntity);
        }
        foreach (var field in Fields)
        {
            var testType = field.Type;
            if(field.Type is XtArrayType arrType)
            {
                testType = arrType.ElementType;
            }

            FlaskFieldEntity item = new FlaskFieldEntity()
            {
                Array = field.Type is XtArrayType,
                Behavior = testType switch
                {
                    XtStructType => FieldBehavior.Struct,
                    XtEnumType => FieldBehavior.Enum,
                    XtFlagsType => FieldBehavior.Flags,
                    XtPointerType => FieldBehavior.Pointer,
                    XtHandleType => FieldBehavior.Handle,
                    _ => FieldBehavior.Atom
                },
                Name = (short)stringTable.GetIndex(field.Name),
                Offset = (short)offset,
            };
            types.GetIndex(testType, i => item.Type = (short)i);
            fields.Add(item);
        }
    }
}
