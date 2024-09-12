using BlurFileFormats.SerializationFramework.Command;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Commands.Meta.Position;

public class UInt32PositionCommand : ISerializationReadCommand<uint>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public uint Read(BinaryReader reader, ReadTree tree) => (uint)reader.BaseStream.Position;
}
