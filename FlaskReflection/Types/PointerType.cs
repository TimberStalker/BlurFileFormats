using BlurFileFormats.FlaskReflection.Values;

namespace BlurFileFormats.FlaskReflection.Types;

public class PointerType : IDataType
{
    public IDataType DataType { get; }
    public string Name { get => DataType.Name; }
    public PointerType(IDataType dataType)
    {
        DataType = dataType;
    }

    public virtual IDataValue CreateValue()
    {
        throw new NotImplementedException();
    }
}
public class ArrayPointerType : PointerType
{
    public ArrayPointerType(IDataType dataType) : base(dataType)
    {
    }
    public override IDataValue CreateValue()
    {
        return new ArrayValue(this) { };
    }
}