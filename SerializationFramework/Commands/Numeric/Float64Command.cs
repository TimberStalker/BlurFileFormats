using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Float64Command : ISerializeCommand<double>
{
    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);
    public double Read(BinaryReader reader, ReadTree tree, object parent) => reader.ReadSingle();

    void ISerializeCommand.Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (double)value);
    public void Write(BinaryWriter writer, WriteTree tree, object parent, double value) => writer.Write(value);
}
