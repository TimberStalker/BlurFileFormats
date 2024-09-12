using BlurFileFormats.SerializationFramework.Command;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Commands.Meta.Position;

public class UInt16PositionCommand : ISerializationReadCommand<ushort>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public ushort Read(BinaryReader reader, ReadTree tree) => (ushort)reader.BaseStream.Position;
}
