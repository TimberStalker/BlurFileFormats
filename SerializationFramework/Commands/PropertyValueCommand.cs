using BlurFileFormats.SerializationFramework.Command;
using System.Reflection;

namespace BlurFileFormats.SerializationFramework.Commands;
internal class PropertyValueCommand : ISerializationReadCommand
{
    public PropertyValueCommand(PropertyInfo property)
    {
        Property = property;
    }

    public PropertyInfo Property { get; }

    public object Read(BinaryReader reader, ReadTree tree)
    {
        return Property.GetValue(tree.CurrentObject)!;
    }
}