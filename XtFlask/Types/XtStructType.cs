using System.Diagnostics;
using BlurFileFormats.XtFlask.Types.Fields;
using BlurFileFormats.XtFlask.Values;

namespace BlurFileFormats.XtFlask.Types;

[DebuggerDisplay("struct {Name}")]
public class XtStructType : IXtType
{
    public IList<XtStructType> Bases { get; } = [];
    public IList<XtField> Fields { get; } = [];
    public string Name { get; }
    
    
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
}
