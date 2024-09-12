using BlurFileFormats.SerializationFramework.Command;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Commands.Meta.Position;

public class UInt8PositionCommand : ISerializationReadCommand<byte>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public byte Read(BinaryReader reader, ReadTree tree) => (byte)reader.BaseStream.Position;
}
