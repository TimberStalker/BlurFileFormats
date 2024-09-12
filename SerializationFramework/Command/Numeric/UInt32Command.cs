using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class UInt32Command : ISerializationValueCommand
{
    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        target.SetValue(reader.ReadUInt32());
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        writer.Write((uint)value);
    }
}
