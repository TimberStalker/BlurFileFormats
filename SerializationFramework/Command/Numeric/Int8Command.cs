using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Int8Command : ISerializationValueCommand
{
    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        target.SetValue(reader.ReadSByte());
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        writer.Write((sbyte)value);
    }
}