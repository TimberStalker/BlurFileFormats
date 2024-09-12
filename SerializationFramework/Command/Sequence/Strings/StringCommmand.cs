using System.Text;
using BlurFileFormats.SerializationFramework.Command.Numeric;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Sequence.Strings;

public class StringCommmand : ISerializationValueCommand
{
    public Encoding Encoding { get; }
    public Int32Command Length { get; }

    public StringCommmand(Encoding encoding, Int32Command length)
    {
        Encoding = encoding;
        Length = length;
    }

    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        var lengthTarget = new ValueTarget();
        Length.Read(reader, tree, lengthTarget);
        int length = (int)lengthTarget.Value;
        target.SetValue(Encoding.GetString(reader.ReadBytes(length)));
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        var v = (string)value;
        Length.Write(writer, tree, v.Length);
        writer.Write(Encoding.GetBytes(v));
    }
}
