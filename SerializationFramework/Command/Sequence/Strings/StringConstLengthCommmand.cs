using System.Text;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Sequence.Strings;

public class StringConstLengthCommmand : ISerializationValueCommand
{
    public Encoding Encoding { get; }
    public int Length { get; }
    public StringConstLengthCommmand(Encoding encoding, int length)
    {
        Encoding = encoding;
        Length = length;
    }
    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        var bytes = reader.ReadBytes(Length);
        target.SetValue(Encoding.GetString(bytes.ToArray()));
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        var bytes = Encoding.GetBytes((string)value);
        int length = Math.Min(bytes.Length, Length);
        writer.Write(bytes[0..length]);
        for (int i = length; i < Length; i++)
        {
            writer.Write((byte)0);
        }
    }
}

public class StringRefLengthCommmand : ISerializationValueCommand
{
    public Encoding Encoding { get; }
    public DataPath Length { get; }
    public StringRefLengthCommmand(Encoding encoding, DataPath length)
    {
        Encoding = encoding;
        Length = length;
    }
    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        int length = tree.GetValue<int>(Length);
        var bytes = reader.ReadBytes(length);
        target.SetValue(Encoding.GetString(bytes.ToArray()));
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        int maxLength = tree.GetValue<int>(Length);
        var bytes = Encoding.GetBytes((string)value);
        int length = Math.Min(bytes.Length, maxLength);
        writer.Write(bytes[0..length]);
        for (int i = length; i < maxLength; i++)
        {
            writer.Write((byte)0);
        }
    }
}
