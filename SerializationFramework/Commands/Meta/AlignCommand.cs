using BlurFileFormats.SerializationFramework.Attributes;
using BlurFileFormats.SerializationFramework.Read;
using System.Reflection.PortableExecutable;

namespace BlurFileFormats.SerializationFramework.Command.Meta;

public class AlignCommand : ISerializeCommand
{
    public IGetCommand<int> AlignPositionCommand { get; }
    public AlignCommand(IGetCommand<int> alignCommand)
    {
        AlignPositionCommand = alignCommand;
    }

    public object Read(BinaryReader reader, ReadTree tree, object parent)
    {
        AlignAttribute.AlignStream(AlignPositionCommand.Get(reader, tree, parent), reader);
        return 0;
    }

    public void Write(BinaryWriter writer, WriteTree tree, object parent, object value)
    {
        AlignAttribute.AlignStream(AlignPositionCommand.Get(writer, tree, parent), writer);
    }
}
