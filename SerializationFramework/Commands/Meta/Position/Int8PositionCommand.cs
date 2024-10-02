using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Meta.Position;

public class Int8PositionCommand : IGetCommand<sbyte>
{
    public sbyte Get(BinaryReader reader, ReadTree tree, object parent)
    {
        return (sbyte)reader.BaseStream.Position;
    }
    public sbyte Get(BinaryWriter writer, WriteTree tree, object parent)
    {
        return (sbyte)writer.BaseStream.Position;
    }

    object IGetCommand.Get(BinaryReader reader, ReadTree tree, object parent) => Get(reader, tree, parent);
    object IGetCommand.Get(BinaryWriter writer, WriteTree tree, object parent) => Get(writer, tree, parent);

}