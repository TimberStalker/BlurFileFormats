using BlurFileFormats.XtFlask.Types;
using BlurFileFormats.XtFlask.Values;

namespace BlurFileFormats.XtFlask.Types.Fields.Behaviors;

public class ArrayFieldBehavior : IFieldBehavior
{
    public IFieldBehavior ItemBehavior { get; }

    public ArrayFieldBehavior(IFieldBehavior itemBehavior)
    {
        ItemBehavior = itemBehavior;
    }

    public IXtValue CreateDefault(IXtType type) => XtNullValue.Instance;

    public IXtValue ReadValue(XtStructValue value, XtField field, BinaryReader reader, ValueResolver resolver)
    {
        resolver.AddArray(value, field, reader);
        return XtNullValue.Instance;
    }
}
