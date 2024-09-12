using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Meta.Position;

public class Int16PositionCommand : ISerializationReadCommand<short>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public short Read(BinaryReader reader, ReadTree tree)
    {
        return (short)reader.BaseStream.Position;
    }
}
