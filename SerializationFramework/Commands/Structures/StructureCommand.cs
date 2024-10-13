using BlurFileFormats.SerializationFramework.Command;
using BlurFileFormats.SerializationFramework.Read;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Commands.Structures;
public class StructureCommand : ISerializeCommand
{
    public Type Type { get; }
    public IReadOnlyList<ISerializerPropertyCommand> SubCommands { get; }

    public StructureCommand(Type type, IReadOnlyList<ISerializerPropertyCommand> subCommands)
    {
        Type = type;
        SubCommands = subCommands;
    }

    public object Read(BinaryReader reader, ReadTree tree, object parent)
    {
        var value = Activator.CreateInstance(Type)!;
        var subTree = new ReadTree(tree);
        tree.Add(this, subTree);
        foreach (var command in SubCommands)
        {
            command.Read(reader, subTree, value);
        }
        return value;
    }

    public void Write(BinaryWriter writer, WriteTree tree, object parent, object value)
    {
        foreach (var command in SubCommands)
        {
            command.Write(writer, tree, value);
        }
    }
}
public interface ISerializerPropertyCommand : ISerializerCommand
{
    public void Read(BinaryReader reader, ReadTree tree, object value);
    public void Write(BinaryWriter reader, WriteTree tree, object value);
}
public class PropertyCommand : ISerializerPropertyCommand
{
    public PropertyInfo Property { get; }
    public ISerializeCommand Command { get; }
    public PropertyCommand(PropertyInfo property, ISerializeCommand command)
    {
        Property = property;
        Command = command;
    }

    public void Write(BinaryWriter writer, WriteTree tree, object value)
    {
        object writeValue = Property.GetValue(value)!;
        tree.Add(Command, writeValue);
        Command.Write(writer, tree, writeValue, writeValue);
    }

    public void Read(BinaryReader reader, ReadTree tree, object value)
    {
        object readValue = Command.Read(reader, tree, value);
        tree.Add(Command, readValue);
        Property.SetValue(value, readValue);
    }
}
public class MetaPropertyCommand : ISerializerPropertyCommand
{
    public PropertyInfo Property { get; }
    public IGetCommand Command { get; }
    public MetaPropertyCommand(PropertyInfo property, IGetCommand command)
    {
        Property = property;
        Command = command;
    }

    public void Read(BinaryReader reader, ReadTree tree, object value)
    {
        object readValue = Command.Get(reader, tree, value);
        tree.Add(Command, readValue);
        //Property.SetValue(value, readValue);
    }

    public void Write(BinaryWriter reader, WriteTree tree, object value) { }
}
public class ValidatePropertyCommand : ISerializerPropertyCommand
{
    public PropertyInfo Property { get; }
    public ISerializeCommand Command { get; }
    public ValidatePropertyCommand(PropertyInfo property, ISerializeCommand command)
    {
        Property = property;
        Command = command;
    }

    public void Write(BinaryWriter writer, WriteTree tree, object value)
    {
        object writeValue = Property.GetValue(value)!;
        tree.Add(Command, writeValue);
        Command.Write(writer, tree, writeValue, writeValue);
    }

    public void Read(BinaryReader reader, ReadTree tree, object value)
    {
        object readValue = Command.Read(reader, tree, value);
        tree.Add(Command, readValue);
        if(!Property.GetValue(value)!.Equals(readValue))
        {
            throw new Exception($"Expected: {Property.GetValue(value)}, Recieved: {readValue}");
        }
    }
}
public class ValidateMetaPropertyCommand : ISerializerPropertyCommand
{
    public PropertyInfo Property { get; }
    public IGetCommand Command { get; }
    public ValidateMetaPropertyCommand(PropertyInfo property, IGetCommand command)
    {
        Property = property;
        Command = command;
    }

    public void Read(BinaryReader reader, ReadTree tree, object value)
    {
        object readValue = Command.Get(reader, tree, value);
        tree.Add(Command, readValue);
        if (!Property.GetValue(value)!.Equals(readValue))
        {
            throw new Exception($"Expected: {Property.GetValue(value)}, Recieved: {readValue}");
        }
    }

    public void Write(BinaryWriter reader, WriteTree tree, object value) { }
}