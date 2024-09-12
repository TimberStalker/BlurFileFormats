using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Int32Command : ISerializationValueCommand<int>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public int Read(BinaryReader reader, ReadTree tree) => reader.ReadInt32();

    void ISerializationWriteCommand.Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (int)value);
    public void Write(BinaryWriter writer, ReadTree tree, int value) => writer.Write(value);
}
