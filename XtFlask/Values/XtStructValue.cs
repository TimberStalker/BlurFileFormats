using BlurFileFormats.XtFlask.Types;
using BlurFileFormats.XtFlask.Types.Fields;

namespace BlurFileFormats.XtFlask.Values;

public class XtStructValue : IXtValue
{
    public XtStructType XtType { get; }

    public List<XtStructValueItem> Values { get; }
    IXtType IXtValue.Type => XtType;
    object IXtValue.Value => Values;
    public XtStructValue(XtStructType xtType)
    {
        XtType = xtType;
        Values = xtType
            .GetFields()
            .Select(f => new XtStructValueItem(f, f.CreateDefault()))
            .ToList();
    }

    public void SetField(XtField field, IXtValue xtValue)
    {
        Values.First(v => v.Field == field).Value = xtValue;
    }
}
