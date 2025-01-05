using BlurFileFormats.SerializationFrameworkOld.Read;
using BlurFileFormats.SerializationFrameworkOld.Commands;
using System.Reflection.PortableExecutable;

namespace BlurFileFormats.SerializationFrameworkOld.Commands.Meta.Position;

public class Int16PositionCommand : IGetCommand<short>
{
    public short Get(BinaryReader reader, ReadTree tree, object parent)
    {
        return (short)reader.BaseStream.Position;
    }
    public short Get(BinaryWriter writer, WriteTree tree, object parent)
    {
        return (short)writer.BaseStream.Position;
    }

    object IGetCommand.Get(BinaryReader reader, ReadTree tree, object parent) => Get(reader, tree, parent);
    object IGetCommand.Get(BinaryWriter writer, WriteTree tree, object parent) => Get(writer, tree, parent);
}
