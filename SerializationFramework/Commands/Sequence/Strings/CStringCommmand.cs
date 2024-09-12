using System.Text;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Sequence.Strings;

public class CStringCommmand : ISerializationValueCommand<string>
{
    public ISerializationValueCommand<Encoding> EncodingCommand { get; }

    public CStringCommmand(ISerializationValueCommand<Encoding> encoding)
    {
        EncodingCommand = encoding;
    }


    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public string Read(BinaryReader reader, ReadTree tree)
    {
        List<byte> bytes = new List<byte>(10);
        byte b = reader.ReadByte();
        while (b != 0)
        {
            bytes.Add(b);
            b = reader.ReadByte();
        }
        var encoding = EncodingCommand.Read(reader, tree);
        return encoding.GetString(bytes.ToArray());
    }

    void ISerializationWriteCommand.Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (string)value);
    public void Write(BinaryWriter writer, ReadTree tree, string value)
    {
        var encoding = EncodingCommand.Read(null, tree);
        writer.Write(encoding.GetBytes(value));
        writer.Write((byte)0);
    }
}
