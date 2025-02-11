using System.Diagnostics;
using BlurFileFormats.XtFlask.Entities;
using BlurFileFormats.XtFlask.Values;

namespace BlurFileFormats.XtFlask.Types;

[DebuggerDisplay("flags {Name}")]
public class XtFlagsType : IXtEnumType
{
    public string Name { get; }
    public IList<string> Labels { get; } = [];
    public XtFlagsType(string name)
    {
        Name = name;
    }
    IXtValue IXtType.CreateDefault() => CreateValue();
    public XtFlagsValue CreateValue() => new XtFlagsValue(this);

    IXtValue IXtType.ReadValue(BinaryReader reader, ValueResolver resolver) => ReadValue(reader, resolver);
    public XtFlagsValue ReadValue(BinaryReader reader, ValueResolver resolver)
    {
        var value = CreateValue();
        value.Value = reader.ReadUInt32();
        return value;
    }

    public void Emit(TypeTableBuilder types, List<FlaskBaseEntity> bases, List<FlaskFieldEntity> fields, StringTableBuilder stringTable)
    {
        throw new NotImplementedException();
    }
}
