using System.Text;
using BlurFileFormats.SerializationFramework.Commands;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Sequence.Strings;

public class CStringCommmand : ISerializeCommand<string>
{
    public SerializerCommandReference<Encoding> EncodingCommand { get; }

    public CStringCommmand(SerializerCommandReference<Encoding> encoding)
    {
        EncodingCommand = encoding;
    }


    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);
    public string Read(BinaryReader reader, ReadTree tree, object parent)
    {
        List<byte> bytes = new List<byte>(10);
        byte b = reader.ReadByte();
        while (b != 0)
        {
            bytes.Add(b);
            b = reader.ReadByte();
        }
        var encoding = EncodingCommand.GetOrRead(reader, tree, parent);
        return encoding.GetString(bytes.ToArray());
    }

    void ISerializeCommand.Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (string)value);
    public void Write(BinaryWriter writer, WriteTree tree, object parent, string value)
    {
        EncodingCommand.GetOrWrite(writer, tree, parent, Encoding.ASCII, out var encoding);
        var bytes = encoding.GetBytes(value);

        writer.Write(encoding.GetBytes(value));
        writer.Write((byte)0);
    }
}
