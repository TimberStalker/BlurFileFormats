namespace BlurFileFormats.SerializationFramework.Commands;

public class PositionCommand : ISerializerCommand
{
    public Func<long, object> MapPosition { get; }

    public PositionCommand(Func<long, object> mapPosition)
    {
        MapPosition = mapPosition;
    }

    public object? Read(BinaryReader reader, SerializerInfo readInfo) => MapPosition(reader.BaseStream.Position - readInfo.CurrentAnchor);

    public void Write(BinaryWriter writer, SerializerInfo writeInfo, object? value) { }
}
