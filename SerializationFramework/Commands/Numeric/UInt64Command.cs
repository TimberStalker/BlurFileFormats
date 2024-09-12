using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class UInt64Command : ISerializationValueCommand<ulong>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public ulong Read(BinaryReader reader, ReadTree tree) => reader.ReadUInt64();

    void ISerializationWriteCommand.Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (ulong)value);
    public void Write(BinaryWriter writer, ReadTree tree, ulong value) => writer.Write(value);
}
