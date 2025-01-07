namespace BlurFileFormats.SerializationFramework.Sources;

public class SerializerDynamicSource : ISerializerSource
{
    public string Path { get; }
    public SerializerDynamicSource(string path)
    {
        Path = path;
    }

    public object? Read(BinaryReader reader, SerializerInfo readInfo) => readInfo.GetValue(Path);

    public object? Write(BinaryWriter writer, SerializerInfo writeInfo, object? value)
    {
        if (value is not null)
        {
            writeInfo.SetValue(Path, value);
            return value;
        }
        return writeInfo.GetValue(Path);
    }
}