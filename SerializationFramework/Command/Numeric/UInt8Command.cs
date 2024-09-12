using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class UInt8Command : ISerializationValueCommand
{
    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        target.SetValue(reader.ReadByte());
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        writer.Write((byte)value);
    }
}
