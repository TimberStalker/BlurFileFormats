using BlurFileFormats.SerializationFramework.Command;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Commands.Meta.Position;

public class UInt64PositionCommand : ISerializationReadCommand<ulong>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public ulong Read(BinaryReader reader, ReadTree tree) => (ulong)reader.BaseStream.Position;
}
