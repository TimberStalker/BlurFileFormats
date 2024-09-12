using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Meta.Position;

public class Int8PositionCommand : ISerializationReadCommand<sbyte>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public sbyte Read(BinaryReader reader, ReadTree tree) => (sbyte)reader.BaseStream.Position;

}