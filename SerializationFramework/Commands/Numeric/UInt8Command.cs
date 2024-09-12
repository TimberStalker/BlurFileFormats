using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class UInt8Command : ISerializationValueCommand<byte>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public byte Read(BinaryReader reader, ReadTree tree) => reader.ReadByte();

    void ISerializationWriteCommand.Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (byte)value);
    public void Write(BinaryWriter writer, ReadTree tree, byte value) => writer.Write(value);
}
