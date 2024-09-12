using BlurFileFormats.SerializationFramework.Read;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Command;
public class StructureCommand : ISerializationValueCommand
{
    public Type Type { get; }
    
    public IReadOnlyList<StructureSubCommand> SubCommands { get; }
    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        var value = Activator.CreateInstance(Type)!;
        tree.Push(value);
        foreach (var (property, command) in SubCommands
            .Where(c => c.Command is ISerializationReadCommand)
            .Select(c => (c.Property, (ISerializationReadCommand)c.Command)))
        {
            command.Read(reader, tree, target);
        }
        target.SetValue(value);
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        foreach (var (property, command) in SubCommands
            .Where(c => c.Command is ISerializationWriteCommand)
            .Select(c => (c.Property, (ISerializationWriteCommand)c.Command)))
        {
            command.Write(writer, tree, target);
        }
    }
}
public class StructureSubCommand
{
    public PropertyInfo Property { get; }
    public ISerializationCommand Command { get; }
    public StructureSubCommand(PropertyInfo property, ISerializationCommand command)
    {
        Property = property;
        Command = command;
    }
}