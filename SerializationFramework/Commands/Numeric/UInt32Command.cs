using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class UInt32Command : ISerializeCommand<uint>
{
    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);
    public uint Read(BinaryReader reader, ReadTree tree, object parent) => reader.ReadUInt32();

    void ISerializeCommand.Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (uint)value);
    public void Write(BinaryWriter writer, WriteTree tree, object parent, uint value) => writer.Write(value);
}
