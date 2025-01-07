namespace BlurFileFormats.SerializationFramework.Sources;

public class SerilaizerConstantSource<T> : ISerializerSource
{
    public T Value { get; }
    public SerilaizerConstantSource(T value)
    {
        Value = value;
    }


    public object? Read(BinaryReader reader, SerializerInfo readInfo) => Value;

    public object? Write(BinaryWriter writer, SerializerInfo writeInfo, object? value) => Value;

}
