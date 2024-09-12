using BlurFileFormats.SerializationFramework.Command;
using BlurFileFormats.SerializationFramework.Read;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Commands.Structures;
public class StructureCommand : ISerializationValueCommand
{
    public Type Type { get; }
    public IReadOnlyList<ISerializationCommand> SubCommands { get; }

    public StructureCommand(Type type, IReadOnlyList<ISerializationCommand> subCommands)
    {
        Type = type;
        SubCommands = subCommands;
    }

    public object Read(BinaryReader reader, ReadTree tree)
    {
        var value = Activator.CreateInstance(Type)!;
        tree.Push(value);
        foreach (var command in SubCommands.OfType<ISerializationReadCommand>())
        {
            command.Read(reader, tree);
        }
        tree.Pop();
        return value;
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        foreach (var command in SubCommands.OfType<ISerializationWriteCommand>())
        {
            command.Write(writer, tree, value);
        }
    }
}
