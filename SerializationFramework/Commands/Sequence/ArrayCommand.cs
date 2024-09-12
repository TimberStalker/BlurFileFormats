using BlurFileFormats.SerializationFramework.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Commands.Sequence;
public class ArrayCommand : ISerializationValueCommand<Array>
{
    public Type ElementType { get; }
    public ISerializationValueCommand<int> LengthCommand { get; }
    public ISerializationValueCommand ElementCommand { get; }
    public ArrayCommand(Type elementType, ISerializationValueCommand<int> length, ISerializationValueCommand value)
    {
        ElementType = elementType;
        LengthCommand = length;
        ElementCommand = value;
    }
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public Array Read(BinaryReader reader, ReadTree tree)
    {
        int length = LengthCommand.Read(reader, tree);
        var array = Array.CreateInstance(ElementType, length);
        for (int i = 0; i < length; i++)
        {
            var value = ElementCommand.Read(reader, tree);
            array.SetValue(value, i);
        }
        return array;
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (Array)value);
    public void Write(BinaryWriter writer, ReadTree tree, Array value)
    {
        if (value.GetType().GetElementType() != ElementType) throw new NotSupportedException();
        LengthCommand.Write(writer, tree, value.Length);
        for(int i = 0; i < value.Length; i++)
        {
            ElementCommand.Write(writer, tree, value.GetValue(i)!);
        }
    }

}
