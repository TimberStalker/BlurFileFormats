using BlurFileFormats.SerializationFramework.Attributes;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Meta;

public class AlignCommand : ISerializationValueCommand
{
    public AlignAttribute Align { get; }
    public AlignCommand(AlignAttribute align)
    {
        Align = align;
    }

    public object Read(BinaryReader reader, ReadTree tree)
    {
        Align.AlignStream(tree, reader);
        return 0;
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        Align.AlignStream(tree, writer);
    }
}
