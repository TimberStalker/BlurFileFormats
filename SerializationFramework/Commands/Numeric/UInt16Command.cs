using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class UInt16Command : ISerializationValueCommand<ushort>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public ushort Read(BinaryReader reader, ReadTree tree) => reader.ReadUInt16();

    void ISerializationWriteCommand.Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (ushort)value);
    public void Write(BinaryWriter writer, ReadTree tree, ushort value) => writer.Write(value);
}
