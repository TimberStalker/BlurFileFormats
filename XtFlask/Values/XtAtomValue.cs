using BlurFileFormats.XtFlask.Types;

namespace BlurFileFormats.XtFlask.Values;

public sealed class XtAtomValue<T> : IXtValue where T : notnull
{
    public XtAtomType<T> Type { get; }
    T value;
    public T Value { get => value; set => this.value = value; }
    IXtType IXtValue.Type => Type;
    object IXtValue.Value => Value;
    public XtAtomValue(XtAtomType<T> type, T value)
    {
        Type = type;
        this.value = value;
    }
    public ref T GetReference() => ref value;
    public override string ToString() => Value.ToString() ?? "";
}
