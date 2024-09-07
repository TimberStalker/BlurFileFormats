using System.Diagnostics;
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
    IXtValue IXtType.CreateValue() => CreateValue();

    IXtValue IXtType.ReadValue(BinaryReader reader, ValueResolver resolver) => ReadValue(reader, resolver);
    public XtEnumValue ReadValue(BinaryReader reader, ValueResolver resolver)
    {
        var value = CreateValue();
        value.Value = reader.ReadUInt32();
        return value;
    }
}
