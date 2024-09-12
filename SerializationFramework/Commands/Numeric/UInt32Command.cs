using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class UInt32Command : ISerializationValueCommand<uint>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public uint Read(BinaryReader reader, ReadTree tree) => reader.ReadUInt32();

    void ISerializationWriteCommand.Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (uint)value);
    public void Write(BinaryWriter writer, ReadTree tree, uint value) => writer.Write(value);
}
