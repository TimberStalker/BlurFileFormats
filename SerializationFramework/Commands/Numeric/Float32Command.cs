using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Float32Command : ISerializationValueCommand<float>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public float Read(BinaryReader reader, ReadTree tree) => reader.ReadSingle();

    void ISerializationWriteCommand.Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (float)value);
    public void Write(BinaryWriter writer, ReadTree tree, float value) => writer.Write(value);
}
