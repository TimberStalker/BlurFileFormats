using BlurFileFormats.SerializationFramework.Commands;

namespace BlurFileFormats.SerializationFramework.Sources;

public class SerializerCommandSource : ISerializerSource
{
    ISerializerCommand Command { get; }

    public SerializerCommandSource(ISerializerCommand command)
    {
        Command = command;
    }

    public object? Read(BinaryReader reader, SerializerInfo readInfo)
    {
        return Command.Read(reader, readInfo);
    }

    public object? Write(BinaryWriter writer, SerializerInfo writeInfo, object? value)
    {
        Command.Write(writer, writeInfo, value);
        return value;
    }
}
