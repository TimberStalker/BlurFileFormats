using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Float64Command : ISerializationValueCommand
{
    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        target.SetValue(reader.ReadDouble());
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        writer.Write((double)value);
    }
}
