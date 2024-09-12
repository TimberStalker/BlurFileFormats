using System.Text;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Sequence.Strings;

public class CStringCommmand : ISerializationValueCommand
{
    public Encoding Encoding { get; }

    public CStringCommmand(Encoding encoding)
    {
        Encoding = encoding;
    }

    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        List<byte> bytes = new List<byte>(10);
        byte b = reader.ReadByte();
        while (b != 0)
        {
            bytes.Add(b);
            b = reader.ReadByte();
        }
        target.SetValue(Encoding.GetString(bytes.ToArray()));
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        writer.Write(Encoding.GetBytes((string)value));
        writer.Write((byte)0);
    }
}
