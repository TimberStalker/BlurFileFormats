using BlurFileFormats.XtFlask.Types;

namespace BlurFileFormats.XtFlask.Values;

public class XtArrayValueItem : IXtValue
{
    public IXtValue Value { get; set; }

    public IXtType Type => Value.Type;
    object IXtValue.Value => Value;
    public XtArrayValueItem(IXtValue value)
    {
        Value = value;
    }
}
