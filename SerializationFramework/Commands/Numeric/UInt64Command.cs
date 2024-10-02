using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class UInt64Command : ISerializeCommand<ulong>
{
    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);
    public ulong Read(BinaryReader reader, ReadTree tree, object parent) => reader.ReadUInt64();

    void ISerializeCommand.Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (ulong)value);
    public void Write(BinaryWriter writer, WriteTree tree, object parent, ulong value) => writer.Write(value);
}
