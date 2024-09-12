using BlurFileFormats.SerializationFramework.Command;
using System.Reflection;

namespace BlurFileFormats.SerializationFramework.Commands.Structures;

public class ReadPropertyCommand : ISerializationReadCommand
{

    public PropertyInfo Property { get; }
    public ISerializationReadCommand Command { get; }
    public ReadPropertyCommand(PropertyInfo property, ISerializationReadCommand command)
    {
        Property = property;
        Command = command;
    }

    public object Read(BinaryReader reader, ReadTree tree)
    {
        return Command.Read(reader, tree);
    }
}