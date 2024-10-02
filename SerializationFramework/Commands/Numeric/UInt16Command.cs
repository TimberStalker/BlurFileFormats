using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class UInt16Command : ISerializeCommand<ushort>
{
    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);
    public ushort Read(BinaryReader reader, ReadTree tree, object parent) => reader.ReadUInt16();

    void ISerializeCommand.Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (ushort)value);
    public void Write(BinaryWriter writer, WriteTree tree, object parent, ushort value) => writer.Write(value);
}
