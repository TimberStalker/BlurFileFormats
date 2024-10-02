using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Int32Command : ISerializeCommand<int>
{
    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);
    public int Read(BinaryReader reader, ReadTree tree, object parent) => reader.ReadInt32();

    void ISerializeCommand.Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (int)value);
    public void Write(BinaryWriter writer, WriteTree tree, object parent, int value) => writer.Write(value);
}
