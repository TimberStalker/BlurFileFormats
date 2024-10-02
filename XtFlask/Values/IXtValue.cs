using BlurFileFormats.XtFlask.Types;

namespace BlurFileFormats.XtFlask.Values;

public interface IXtValue
{
    IXtType Type { get; }
    object Value { get; }
}
public interface IXtMultiValue : IXtValue
{
    IReadOnlyList<IXtValue> Values { get; }
}
public static class XtValue
{
    public static void Explore(this IXtValue xtValue, Action action)
    {

    }
}