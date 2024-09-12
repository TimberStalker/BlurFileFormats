using System.Text;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Sequence.Strings;

public class CStringRefLengthCommmand : ISerializationValueCommand
{
    public Encoding Encoding { get; }
    public DataPath Length { get; }

    public CStringRefLengthCommmand(Encoding encoding, DataPath length)
    {
        Encoding = encoding;
        Length = length;
    }

    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        int length = tree.GetValue<int>(Length);
        var bytes = reader.ReadBytes(length);
        int byteCount = bytes.TakeWhile(b => b != 0).Count();
        target.SetValue(Encoding.GetString(bytes.ToArray(), 0, byteCount));
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