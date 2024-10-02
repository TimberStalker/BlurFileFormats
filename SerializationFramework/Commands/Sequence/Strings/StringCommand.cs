using System.Reflection.PortableExecutable;
using System.Text;
using BlurFileFormats.SerializationFramework.Command.Numeric;
using BlurFileFormats.SerializationFramework.Commands;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Sequence.Strings;
public class StringCommand : ISerializeCommand<string>
{
    public SerializerCommandReference<Encoding> EncodingCommand { get; }
    public SerializerCommandReference<int> LengthCommand { get; }

    public StringCommand(SerializerCommandReference<Encoding> encoding, SerializerCommandReference<int> length)
    {
        EncodingCommand = encoding;
        LengthCommand = length;
    }

    public string Read(BinaryReader reader, ReadTree tree, object parent)
    {
        int length = LengthCommand.GetOrRead(reader, tree, parent);
        return EncodingCommand.GetOrRead(reader, tree, parent).GetString(reader.ReadBytes(length));
    }

    public void Write(BinaryWriter writer, WriteTree tree, object parent, string value)
    {
        EncodingCommand.GetOrWrite(writer, tree, parent, Encoding.ASCII, out var encoding);
        var bytes = encoding.GetBytes(value);

        LengthCommand.GetOrWrite(writer, tree, parent, bytes.Length, out var length);

        if(length >= bytes.Length)
        {
            writer.Write(bytes);
            for(int i = length; i < bytes.Length; i++)
            {
                writer.Write(0);
            }
        }
        else
        {
            throw new Exception("Length is not valid.");
        }
    }

    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);

    void ISerializeCommand.Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (string)value);
}
