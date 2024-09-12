using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Meta.Position;

public class Int64PositionCommand : ISerializationReadCommand<long>
{
    public Int64PositionCommand()
    {
    }

    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public long Read(BinaryReader reader, ReadTree tree) => reader.BaseStream.Position;
}
