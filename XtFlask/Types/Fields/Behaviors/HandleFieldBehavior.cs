using BlurFileFormats.XtFlask.Types;
using BlurFileFormats.XtFlask.Values;

namespace BlurFileFormats.XtFlask.Types.Fields.Behaviors;

public class HandleFieldBehavior : IFieldBehavior
{
    public IXtValue CreateDefault(IXtType type) => XtNullValue.Instance;

    public IXtValue ReadValue(XtStructValue value, XtField field, BinaryReader reader, ValueResolver resolver)
    {
        resolver.AddHandle(value, field, reader);
        return XtNullValue.Instance;
    }
}