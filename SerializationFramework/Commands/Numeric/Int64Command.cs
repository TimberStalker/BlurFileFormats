using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Int64Command : ISerializeCommand<long>
{
    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);
    public long Read(BinaryReader reader, ReadTree tree, object parent) => reader.ReadInt64();

    void ISerializeCommand.Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (long)value);
    public void Write(BinaryWriter writer, WriteTree tree, object parent, long value) => writer.Write(value);
}
