using BlurFileFormats.XtFlask.Types;

namespace BlurFileFormats.XtFlask.Values;

public class XtNullValue : IXtValue
{
    public static XtNullValue Instance { get; } = new XtNullValue();
    public IXtType Type => throw new NullReferenceException();
    public object Value => throw new NullReferenceException();
}
