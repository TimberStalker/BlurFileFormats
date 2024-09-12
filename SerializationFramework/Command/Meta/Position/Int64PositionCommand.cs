using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Meta.Position;

public class Int64PositionCommand : ISerializationReadCommand
{
    public Int64PositionCommand()
    {
    }

    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        target.SetValue(reader.BaseStream.Position);
    }

}
