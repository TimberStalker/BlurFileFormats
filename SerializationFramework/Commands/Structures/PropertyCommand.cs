using BlurFileFormats.SerializationFramework.Command;
using System.Reflection;

namespace BlurFileFormats.SerializationFramework.Commands.Structures;

public class PropertyCommand : ISerializationValueCommand
{
    public PropertyInfo Property { get; }
    public ISerializationCommand Command { get; }
    public PropertyCommand(PropertyInfo property, ISerializationCommand command)
    {
        Property = property;
        Command = command;
    }

    public object Read(BinaryReader reader, ReadTree tree)
    {
        object result = 0;
        if(Command is ISerializationReadCommand r)
        {
            Property.SetValue(tree.CurrentObject, result = r.Read(reader, tree));
        }
        return result;
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        if(Command is ISerializationWriteCommand w)
        {
            w.Write(writer, tree, Property.GetValue(value)!);
        }
    }
}
