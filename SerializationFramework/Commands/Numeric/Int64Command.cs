using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Int64Command : ISerializationValueCommand<long>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public long Read(BinaryReader reader, ReadTree tree) => reader.ReadInt64();

    void ISerializationWriteCommand.Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (long)value);
    public void Write(BinaryWriter writer, ReadTree tree, long value) => writer.Write(value);
}
