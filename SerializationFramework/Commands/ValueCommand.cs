namespace BlurFileFormats.SerializationFramework.Commands;

public class ValueCommand : ISerializerCommand
{
    public Func<BinaryReader, SerializerInfo, object?>? ReadAction { get; set; }
    public Action<BinaryWriter, SerializerInfo, object?>? WriteAction { get; set; }

    public object? Read(BinaryReader reader, SerializerInfo readInfo) => ReadAction?.Invoke(reader, readInfo);

    public void Write(BinaryWriter writer, SerializerInfo writeInfo, object? value) => WriteAction?.Invoke(writer, writeInfo, value);
}
