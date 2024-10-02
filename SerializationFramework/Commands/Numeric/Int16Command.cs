using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Int16Command : ISerializeCommand<short>
{
    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);
    public short Read(BinaryReader reader, ReadTree tree, object parent) => reader.ReadInt16();

    void ISerializeCommand.Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (short)value);
    public void Write(BinaryWriter writer, WriteTree tree, object parent, short value) => writer.Write(value);
}
