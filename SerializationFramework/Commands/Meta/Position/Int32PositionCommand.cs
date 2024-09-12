using BlurFileFormats.SerializationFramework.Command;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Commands.Meta.Position;

public class Int32PositionCommand : ISerializationReadCommand<int>
{
    public Int32PositionCommand()
    {
    }

    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public int Read(BinaryReader reader, ReadTree tree) => (int)reader.BaseStream.Position;

}
