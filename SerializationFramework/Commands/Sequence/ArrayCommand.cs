using BlurFileFormats.SerializationFramework.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Commands.Sequence;
public class ArrayCommand : ISerializeCommand<Array>
{
    public Type ElementType { get; }
    public SerializerCommandReference<int> LengthCommand { get; }
    public ISerializeCommand ElementCommand { get; }
    public ArrayCommand(Type elementType, SerializerCommandReference<int> length, ISerializeCommand value)
    {
        ElementType = elementType;
        LengthCommand = length;
        ElementCommand = value;
    }
    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);
    public Array Read(BinaryReader reader, ReadTree tree, object parent)
    {
        int length = LengthCommand.GetOrRead(reader, tree, parent);
        var array = Array.CreateInstance(ElementType, length);
        var childTree = new ChildTree(tree);
        for (int i = 0; i < length; i++)
        {
            var value = ElementCommand.Read(reader, childTree, parent);
            array.SetValue(value, i);
        }
        return array;
    }

    void ISerializeCommand.Write(BinaryWriter writer, WriteTree tree, object value, object parent) => Write(writer, tree, parent, (Array)value);
    public void Write(BinaryWriter writer, WriteTree tree, object parent, Array value)
    {
        if (value.GetType().GetElementType() != ElementType) throw new NotSupportedException();

        LengthCommand.GetOrWrite(writer, tree, parent, value.Length, out int length);

        if (value.Length != length) throw new Exception("Length does not match.");
        for(int i = 0; i < length; i++)
        {
            ElementCommand.Write(writer, tree, i, value.GetValue(i)!);
        }
    }

}
