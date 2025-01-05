using BlurFileFormats.SerializationFrameworkOld.Read;
using BlurFileFormats.SerializationFrameworkOld.Commands;

namespace BlurFileFormats.SerializationFrameworkOld.Commands.Meta.Position;

public class UInt16PositionCommand : IGetCommand<ushort>
{
    public ushort Get(BinaryReader reader, ReadTree tree, object parent)
    {
        return (ushort)reader.BaseStream.Position;
    }
    public ushort Get(BinaryWriter writer, WriteTree tree, object parent)
    {
        return (ushort)writer.BaseStream.Position;
    }

    object IGetCommand.Get(BinaryReader reader, ReadTree tree, object parent) => Get(reader, tree, parent);
    object IGetCommand.Get(BinaryWriter writer, WriteTree tree, object parent) => Get(writer, tree, parent);
}
