using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class UInt8Command : ISerializeCommand<byte>
{
    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);
    public byte Read(BinaryReader reader, ReadTree tree, object parent) => reader.ReadByte();

    void ISerializeCommand.Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (byte)value);
    public void Write(BinaryWriter writer, WriteTree tree, object parent, byte value) => writer.Write(value);
}
