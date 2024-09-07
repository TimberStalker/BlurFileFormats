using BlurFileFormats.XtFlask.Types;
using System.Linq;

namespace BlurFileFormats.XtFlask.Values;

public class XtFlagsValue : IXtValue
{
    public XtFlagsType XtType { get; }

    public uint Value = 0;
    IXtType IXtValue.Type => XtType;
    object IXtValue.Value => Value;
    public XtFlagsValue(XtFlagsType xtType)
    {
        XtType = xtType;
    }
    public ReadOnlySpan<string> GetLabels()
    {
        uint value = Value;
        int count = 0;
        string[] labels = new string[XtType.Labels.Count];
        for (int i = 0; i < XtType.Labels.Count; i++)
        {
            if ((value & 1) > 0)
            {
                labels[count] = XtType.Labels[i];
                count++;
            }
            value >>= 1;
        }
        return labels[0..count];
    }

    public bool IsFlagSet(int i) => (Value >> i & 1) > 0;
    public bool IsFlagClear(int i) => (Value >> i & 1) == 0;
    public void SetFlag(int i) => Value |= (uint)1 << i;
    public void ClearFlag(int i) => Value &= ~((uint)1 << i);
    public void ToggleFlag(int i) => Value &= ~((uint)1 << i);

    public void IsFlagSet(string name) => IsFlagSet(GetIndex(name));
    public void IsFlagClear(string name) => IsFlagClear(GetIndex(name));
    public void SetFlag(string name) => SetFlag(GetIndex(name));
    public void ClearFlag(string name) => ClearFlag(GetIndex(name));
    public void ToggleFlag(string name) => ToggleFlag(GetIndex(name));

    private int GetIndex(string name)
    {
        var index = XtType.Labels.IndexOf(name);
        if (index == -1)
        {
            throw new ArgumentException($"{XtType.Name} does not contain the label {name}.");
        }

        return index;
    }
    public override string ToString() => string.Join(',', GetLabels().ToArray());
}
