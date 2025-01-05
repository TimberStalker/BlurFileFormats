using BlurFileFormats.SerializationFrameworkOld.Read;
using BlurFileFormats.SerializationFrameworkOld.Commands;

namespace BlurFileFormats.SerializationFrameworkOld.Commands.Meta.Position;

public class Int64PositionCommand : IGetCommand<long>
{
    public long Get(BinaryReader reader, ReadTree tree, object parent)
    {
        return reader.BaseStream.Position;
    }
    public long Get(BinaryWriter writer, WriteTree tree, object parent)
    {
        return writer.BaseStream.Position;
    }

    object IGetCommand.Get(BinaryReader reader, ReadTree tree, object parent) => Get(reader, tree, parent);
    object IGetCommand.Get(BinaryWriter writer, WriteTree tree, object parent) => Get(writer, tree, parent);
}
