using System.Reflection.PortableExecutable;
using System.Text;
using BlurFileFormats.SerializationFramework.Command.Numeric;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Sequence.Strings;
public class StringCommand : ISerializationValueCommand<string>
{
    public ISerializationReadCommand<Encoding> Encoding { get; }
    public ISerializationValueCommand<int> Length { get; }

    public StringCommand(ISerializationReadCommand<Encoding> encoding, ISerializationValueCommand<int> length)
    {
        Encoding = encoding;
        Length = length;
    }

    public string Read(BinaryReader reader, ReadTree tree)
    {
        int length = Length.Read(reader, tree);
        return Encoding.Read(reader, tree).GetString(reader.ReadBytes(length));
    }

    public void Write(BinaryWriter writer, ReadTree tree, string value)
    {
        var bytes = Encoding.Read(null, tree).GetBytes((string)value);
        Length.Write(writer, tree, bytes.Length);
        writer.Write(bytes);
    }

    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);

    void ISerializationWriteCommand.Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (string)value);
}
