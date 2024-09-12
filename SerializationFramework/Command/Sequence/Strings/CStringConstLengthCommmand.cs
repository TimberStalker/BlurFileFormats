using System.Text;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Sequence.Strings;

public class CStringConstLengthCommmand : ISerializationValueCommand
{
    public Encoding Encoding { get; }
    public int Length { get; }

    public CStringConstLengthCommmand(Encoding encoding, int length)
    {
        Encoding = encoding;
        Length = length;
    }

    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        var bytes = reader.ReadBytes(Length);
        int byteCount = bytes.TakeWhile(b => b != 0).Count();
        target.SetValue(Encoding.GetString(bytes.ToArray(), 0, byteCount));
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
