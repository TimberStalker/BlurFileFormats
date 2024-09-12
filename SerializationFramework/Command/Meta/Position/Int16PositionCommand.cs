using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Meta.Position;

public class Int16PositionCommand : ISerializationReadCommand
{
    public Int16PositionCommand()
    {
    }

    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        target.SetValue((short)reader.BaseStream.Position);
    }

}
