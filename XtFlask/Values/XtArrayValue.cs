using BlurFileFormats.XtFlask.Types;

namespace BlurFileFormats.XtFlask.Values;

public class XtArrayValue : IXtValue, IXtMultiValue
{
    public IXtType Type { get; }

    public List<XtArrayValueItem> Values { get; } = [];
    object IXtValue.Value => Values;

    IReadOnlyList<IXtValue> IXtMultiValue.Values => Values;

    public XtArrayValue(IXtType xtType)
    {
        Type = xtType;
    }
}
