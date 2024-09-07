using BlurFileFormats.XtFlask.Values;

namespace BlurFileFormats.XtFlask.Types;

public interface IXtType
{
    string Name { get; }
    IXtValue CreateValue();
    IXtValue ReadValue(BinaryReader reader, ValueResolver resolver);
}
