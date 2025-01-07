namespace BlurFileFormats.SerializationFramework.Sources;

public interface ISerializerSource
{
    object? Read(BinaryReader reader, SerializerInfo readInfo);
    object? Write(BinaryWriter writer, SerializerInfo writeInfo, object? value);
}
