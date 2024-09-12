using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class Int32Command : ISerializationValueCommand
{
    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        target.SetValue(reader.ReadInt32());
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        writer.Write((int)value);
    }
}
