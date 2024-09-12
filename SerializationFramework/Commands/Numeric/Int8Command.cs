using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Int8Command : ISerializationValueCommand<sbyte>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public sbyte Read(BinaryReader reader, ReadTree tree) => reader.ReadSByte();

    void ISerializationWriteCommand.Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (sbyte)value);
    public void Write(BinaryWriter writer, ReadTree tree, sbyte value) => writer.Write(value);
}