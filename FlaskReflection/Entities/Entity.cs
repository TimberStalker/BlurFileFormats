using BlurFileFormats.FlaskReflection.Types;
using BlurFileFormats.FlaskReflection.Values;

namespace BlurFileFormats.FlaskReflection.Entities;

public interface IEntity
{
    IDataValue Value { get; set; }
    IDataType DataType { get; }
}
public class NullEntity : IEntity
{
    public IDataValue Value { get => NullDataValue.NullValue; set => throw new NotImplementedException(); }

    public IDataType DataType { get; }

    public NullEntity(IDataType dataType)
    {
        DataType = dataType;
    }
}