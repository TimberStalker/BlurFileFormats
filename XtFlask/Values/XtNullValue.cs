using BlurFileFormats.XtFlask.Types;

namespace BlurFileFormats.XtFlask.Values;

public class XtNullValue : IXtValue
{
    public static XtNullValue Instance { get; } = new XtNullValue();
    public IXtType Type => XtNullType.Instance;
    public object Value => throw new NullReferenceException();
}
