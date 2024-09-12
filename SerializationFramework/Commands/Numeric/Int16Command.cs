using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Int16Command : ISerializationValueCommand<short>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public short Read(BinaryReader reader, ReadTree tree) => reader.ReadInt16();

    void ISerializationWriteCommand.Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (short)value);
    public void Write(BinaryWriter writer, ReadTree tree, short value) => writer.Write(value);
}
