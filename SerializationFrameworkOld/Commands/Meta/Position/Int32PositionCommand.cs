using BlurFileFormats.SerializationFrameworkOld.Read;
using BlurFileFormats.SerializationFrameworkOld.Commands;

namespace BlurFileFormats.SerializationFrameworkOld.Commands.Meta.Position;

public class Int32PositionCommand : IGetCommand<int>
{
    public int Get(BinaryReader reader, ReadTree tree, object parent)
    {
        return (int)reader.BaseStream.Position;
    }
    public int Get(BinaryWriter writer, WriteTree tree, object parent)
    {
        return (int)writer.BaseStream.Position;
    }

    object IGetCommand.Get(BinaryReader reader, ReadTree tree, object parent) => Get(reader, tree, parent);
    object IGetCommand.Get(BinaryWriter writer, WriteTree tree, object parent) => Get(writer, tree, parent);

}
