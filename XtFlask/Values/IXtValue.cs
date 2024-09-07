using BlurFileFormats.XtFlask.Types;

namespace BlurFileFormats.XtFlask.Values;

public interface IXtValue
{
    IXtType Type { get; }
    object Value { get; }
}
