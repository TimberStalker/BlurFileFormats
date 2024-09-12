using System.Reflection.PortableExecutable;
using System.Text;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Sequence.Strings;

public class CStringLengthCommmand : ISerializationValueCommand
{
    public ISerializationValueCommand<Encoding> EncodingCommand { get; }
    public ISerializationValueCommand<int> LengthCommand { get; }

    public CStringLengthCommmand(ISerializationValueCommand<Encoding> encodingCommand, ISerializationValueCommand<int> lengthCommand)
    {
        EncodingCommand = encodingCommand;
        LengthCommand = lengthCommand;
    }

    public object Read(BinaryReader reader, ReadTree tree)
    {
        int length = LengthCommand.Read(reader, tree);
        var encoding = EncodingCommand.Read(reader, tree);


        var bytes = reader.ReadBytes(length);
        int byteCount = bytes.TakeWhile(b => b != 0).Count();
        return encoding.GetString(bytes.ToArray(), 0, byteCount);
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        int maxLength = 0;//tree.GetValue<int>(Length);
        var encoding = EncodingCommand.Read(null, tree);

        var bytes = encoding.GetBytes((string)value);
        int length = Math.Min(bytes.Length, maxLength);
        writer.Write(bytes[0..length]);
        for (int i = length; i < maxLength; i++)
        {
            writer.Write((byte)0);
        }
    }
}