using BlurFileFormats.SerializationFrameworkOld.Read;
using BlurFileFormats.SerializationFrameworkOld.Commands;

namespace BlurFileFormats.SerializationFrameworkOld.Commands.Meta.Position;

public class UInt32PositionCommand : IGetCommand<uint>
{
    public uint Get(BinaryReader reader, ReadTree tree, object parent)
    {
        return (uint)reader.BaseStream.Position;
    }
    public uint Get(BinaryWriter writer, WriteTree tree, object parent)
    {
        return (uint)writer.BaseStream.Position;
    }

    object IGetCommand.Get(BinaryReader reader, ReadTree tree, object parent) => Get(reader, tree, parent);
    object IGetCommand.Get(BinaryWriter writer, WriteTree tree, object parent) => Get(writer, tree, parent);
}
