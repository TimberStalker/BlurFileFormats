using BlurFileFormats.XtFlask.Types;
using BlurFileFormats.XtFlask.Types.Fields;
using System.Diagnostics;

namespace BlurFileFormats.XtFlask.Values;

[DebuggerDisplay("{DebuggerDisplay()}")]
public class XtStructValueItem : IXtValue
{
    public XtField Field { get; }
    public IXtValue Value { get; set; }

    public IXtType Type => Value.Type;
    object IXtValue.Value => Value;
    public XtStructValueItem(XtField field, IXtValue value)
    {
        Field = field;
        Value = value;
    }
    public XtStructValueItem(XtField field) : this(field, XtNullValue.Instance) { }
    public string DebuggerDisplay() => $"{Field.Name} = {Value}"; 
}
