using BlurFileFormats.SerializationFramework.Command;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Commands.Meta.Position;

public class UInt64PositionCommand : IGetCommand<ulong>
{
    public ulong Get(BinaryReader reader, ReadTree tree, object parent)
    {
        return (ulong)reader.BaseStream.Position;
    }
    public ulong Get(BinaryWriter writer, WriteTree tree, object parent)
    {
        return (ulong)writer.BaseStream.Position;
    }

    object IGetCommand.Get(BinaryReader reader, ReadTree tree, object parent) => Get(reader, tree, parent);
    object IGetCommand.Get(BinaryWriter writer, WriteTree tree, object parent) => Get(writer, tree, parent);
}
