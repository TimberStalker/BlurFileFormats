using BlurFileFormats.XtFlask.Types;

namespace BlurFileFormats.XtFlask.Values;

public class XtArrayValue : IXtValue
{
    public IXtType Type { get; }

    public List<XtArrayValueItem> Values { get; } = [];
    object IXtValue.Value => Values;
    public XtArrayValue(IXtType xtType)
    {
        Type = xtType;
    }
}
