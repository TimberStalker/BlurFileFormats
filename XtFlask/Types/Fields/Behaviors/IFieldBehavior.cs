using BlurFileFormats.XtFlask.Values;

namespace BlurFileFormats.XtFlask.Types.Fields.Behaviors;

public interface IFieldBehavior
{
    IXtValue CreateDefault(IXtType type);
    IXtValue ReadValue(XtStructValue value, XtField field, BinaryReader reader, ValueResolver resolver);
}
