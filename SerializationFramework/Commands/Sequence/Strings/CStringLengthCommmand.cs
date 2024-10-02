using System.Reflection.PortableExecutable;
using System.Text;
using BlurFileFormats.SerializationFramework.Commands;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Sequence.Strings;

public class CStringLengthCommmand : ISerializeCommand<string>
{
    public SerializerCommandReference<Encoding> EncodingCommand { get; }
    public SerializerCommandReference<int> LengthCommand { get; }

    public CStringLengthCommmand(SerializerCommandReference<Encoding> encodingCommand, SerializerCommandReference<int> lengthCommand)
    {
        EncodingCommand = encodingCommand;
        LengthCommand = lengthCommand;
    }

    public string Read(BinaryReader reader, ReadTree tree, object parent)
    {
        int length = LengthCommand.GetOrRead(reader, tree, parent);
        var encoding = EncodingCommand.GetOrRead(reader, tree, parent);


        var bytes = reader.ReadBytes(length);
        int byteCount = bytes.TakeWhile(b => b != 0).Count();
        return encoding.GetString(bytes.ToArray(), 0, byteCount);
    }

    public void Write(BinaryWriter writer, WriteTree tree, object parent, string value)
    {
        EncodingCommand.GetOrWrite(writer, tree, parent, Encoding.ASCII, out var encoding);
        var bytes = encoding.GetBytes(value);

        LengthCommand.GetOrWrite(writer, tree, parent, bytes.Length, out var length);

        if (bytes.Length > length)
        {
            throw new Exception("CString Length is too big");
        }

        writer.Write(bytes);
        for (int i = bytes.Length; i < length; i++)
        {
            writer.Write((byte)0);
        }
    }

    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);

    void ISerializeCommand.Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (string)value);
}