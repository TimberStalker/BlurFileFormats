namespace BlurFileFormats.SerializationFramework.Commands;

public abstract class AtomicValueCommand<T> : ISerializerCommand
{
    object? ISerializerCommand.Read(BinaryReader reader, SerializerInfo readInfo) => Read(reader, readInfo);
    void ISerializerCommand.Write(BinaryWriter writer, SerializerInfo writeInfo, object? value) => Write(writer, writeInfo, (T)value!);
    public abstract T Read(BinaryReader reader, SerializerInfo readInfo);
    public abstract void Write(BinaryWriter writer, SerializerInfo writeInfo, T value);
}
