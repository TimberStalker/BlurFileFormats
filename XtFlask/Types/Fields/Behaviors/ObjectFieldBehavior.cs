using BlurFileFormats.XtFlask.Types;
using BlurFileFormats.XtFlask.Values;

namespace BlurFileFormats.XtFlask.Types.Fields.Behaviors;

public class ObjectFieldBehavior : IFieldBehavior
{
    public IXtValue CreateDefault(IXtType type) => type.CreateValue();

    public IXtValue ReadValue(XtStructValue value, XtField field, BinaryReader reader, ValueResolver resolver)
    {
        return field.Type.ReadValue(reader, resolver);
    }
}
