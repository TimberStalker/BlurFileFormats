using BlurFileFormats.SerializationFramework.Command;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Commands.Meta.Position;

public class UInt8PositionCommand : IGetCommand<byte>
{
    public byte Get(BinaryReader reader, ReadTree tree, object parent)
    {
        return (byte)reader.BaseStream.Position;
    }
    public byte Get(BinaryWriter writer, WriteTree tree, object parent)
    {
        return (byte)writer.BaseStream.Position;
    }

    object IGetCommand.Get(BinaryReader reader, ReadTree tree, object parent) => Get(reader, tree, parent);
    object IGetCommand.Get(BinaryWriter writer, WriteTree tree, object parent) => Get(writer, tree, parent);
}
