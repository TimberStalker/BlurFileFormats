using BlurFileFormats.XtFlask.Types;
using BlurFileFormats.XtFlask.Types.Fields;

namespace BlurFileFormats.XtFlask.Values;

public class XtStructValue : IXtValue, IXtMultiValue
{
    public XtStructType XtType { get; }

    public List<XtStructValueItem> Values { get; }
    IXtType IXtValue.Type => XtType;
    object IXtValue.Value => Values;

    IReadOnlyList<IXtValue> IXtMultiValue.Values => Values;

    public XtStructValue(XtStructType xtType)
    {
        XtType = xtType;
        Values = xtType
            .GetFields()
            .Select(f => new XtStructValueItem(f, f.CreateDefault()))
            .ToList();
    }

    public void SetField(XtField field, IXtValue xtValue) => Values.First(v => v.Field == field).Value = xtValue;
    public void SetField(string field, IXtValue xtValue) => Values.First(v => v.Field.Name == field).Value = xtValue;
    public IXtValue GetField(XtField field) => Values.First(v => v.Field == field).Value;
    public IXtValue GetField(string field) => Values.First(v => v.Field.Name == field).Value;
    public XtStructValueItem GetFieldItem(XtField field) => Values.First(v => v.Field == field);
    public XtStructValueItem GetFieldItem(string field) => Values.First(v => v.Field.Name == field);
}
