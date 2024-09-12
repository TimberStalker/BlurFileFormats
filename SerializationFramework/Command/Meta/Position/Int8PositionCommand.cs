using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Meta.Position;

public class Int8PositionCommand : ISerializationReadCommand
{
    public Int8PositionCommand()
    {
    }

    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        target.SetValue((sbyte)reader.BaseStream.Position);
    }

}