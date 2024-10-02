using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Float32Command : ISerializeCommand<float>
{
    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);
    public float Read(BinaryReader reader, ReadTree tree, object parent) => reader.ReadSingle();

    void ISerializeCommand.Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (float)value);
    public void Write(BinaryWriter writer, WriteTree tree, object parent, float value) => writer.Write(value);
}
