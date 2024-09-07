using BlurFileFormats.XtFlask.Types;

namespace BlurFileFormats.XtFlask.Values;

public sealed class XtAtomValue<T> : IXtValue where T : notnull
{
    public XtAtomType<T> Type { get; }
    public T Value { get; set; }
    IXtType IXtValue.Type => Type;
    object IXtValue.Value => Value;
    public XtAtomValue(XtAtomType<T> type, T value)
    {
        Type = type;
        Value = value;
    }
    public override string ToString() => Value.ToString() ?? "";
}
