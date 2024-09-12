using BlurFileFormats.SerializationFramework.Attributes;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Meta;

public class AlignCommand : ISerializationReadCommand
{
    public AlignAttribute Align { get; }
    public AlignCommand(AlignAttribute align)
    {
        Align = align;
    }


    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        Align.AlignStream(tree, reader);
    }

}
public class Int32PositionCommand : ISerializationReadCommand
{
    public Int32PositionCommand()
    {
    }

    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        target.SetValue((int)reader.BaseStream.Position);
    }

}
