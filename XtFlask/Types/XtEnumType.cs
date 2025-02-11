using System.Diagnostics;
using BlurFileFormats.XtFlask.Entities;
using BlurFileFormats.XtFlask.Values;

namespace BlurFileFormats.XtFlask.Types;

[DebuggerDisplay("enum {Name}")]
public class XtEnumType : IXtEnumType
{
    public string Name { get; }
    public IList<string> Labels { get; } = [];
    public XtEnumType(string name)
    {
        Name = name;
    }

    public XtEnumValue CreateValue() => new XtEnumValue(this);
    IXtValue IXtType.CreateDefault() => CreateValue();

    IXtValue IXtType.ReadValue(BinaryReader reader, ValueResolver resolver) => ReadValue(reader, resolver);
    public XtEnumValue ReadValue(BinaryReader reader, ValueResolver resolver)
    {
        var value = CreateValue();
        value.Value = reader.ReadUInt32();
        return value;
    }

    public void Emit(TypeTableBuilder types, List<FlaskBaseEntity> bases, List<FlaskFieldEntity> fields, StringTableBuilder stringTable)
    {
        if (types.Contains(this)) return;
        int typeIndex = types.Count;
        types.Add(new FlaskTypeEntity()
        {
            Atom = AtomType.NotAtom,
            BaseCount = 0,
            Behavior = TypeBehavior.Enum,
            FieldCount = (ushort)Labels.Count,
            Name = (ushort)stringTable.GetIndex(Name),
            Size = 4
        }, this);
        foreach(var label in Labels)
        {
            var field = new FlaskFieldEntity()
            {
                Array = false,
                Behavior = FieldBehavior.Enum,
                Name = (short)stringTable.GetIndex(label),
                Offset = 0,
                Type = (short)typeIndex
            };
            fields.Add(field);
        }
    }
}
