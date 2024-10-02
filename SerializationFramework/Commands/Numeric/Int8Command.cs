using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Int8Command : ISerializeCommand<sbyte>
{
    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);
    public sbyte Read(BinaryReader reader, ReadTree tree, object parent) => reader.ReadSByte();

    void ISerializeCommand.Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (sbyte)value);
    public void Write(BinaryWriter writer, WriteTree tree, object parent, sbyte value) => writer.Write(value);
}