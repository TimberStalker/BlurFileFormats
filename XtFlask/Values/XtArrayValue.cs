using BlurFileFormats.XtFlask.Types;

namespace BlurFileFormats.XtFlask.Values;

public class XtArrayValue : IXtValue, IXtMultiValue
{
    public XtArrayType Type { get; }

    public List<XtArrayValueItem> Values { get; } = [];
    object IXtValue.Value => Values;

    IReadOnlyList<IXtValue> IXtMultiValue.Values => Values;

    IXtType IXtValue.Type => Type;

    public XtArrayValue(XtArrayType xtType)
    {
        Type = xtType;
    }
}
