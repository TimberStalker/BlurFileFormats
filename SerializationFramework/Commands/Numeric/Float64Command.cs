using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Float64Command : ISerializationValueCommand<double>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public double Read(BinaryReader reader, ReadTree tree) => reader.ReadSingle();

    void ISerializationWriteCommand.Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (double)value);
    public void Write(BinaryWriter writer, ReadTree tree, double value) => writer.Write(value);
}
