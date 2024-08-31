using BlurFileFormats.FlaskReflection.Entities;
using BlurFileFormats.FlaskReflection.Values;

namespace BlurFileFormats.FlaskReflection.Types;

public interface IDataType
{
    string Name { get; }
    IDataValue CreateValue();
}
