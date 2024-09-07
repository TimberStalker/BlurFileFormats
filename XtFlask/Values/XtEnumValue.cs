using BlurFileFormats.XtFlask.Types;
using System;

namespace BlurFileFormats.XtFlask.Values;

public class XtEnumValue : IXtValue
{
    public XtEnumType XtType { get; }

    public uint Value = 0;
    IXtType IXtValue.Type => XtType;
    object IXtValue.Value => Value;
    public XtEnumValue(XtEnumType xtType)
    {
        XtType = xtType;
    }
    public string GetLabel() => XtType.Labels[(int)Value];
    public void SetLabel(string label) => Value = (uint)XtType.Labels.IndexOf(label);
    public override string ToString() => GetLabel();
}
