using BlurFileFormats.SerializationFramework.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Commands;
public class ConstantExpressionCommand : IGetCommand
{
    public Func<object, object> Value { get; }

    public ConstantExpressionCommand(Func<object, object> value)
    {
        Value = value;
    }
    public object Get(BinaryReader reader, ReadTree tree, object parent) => Value(parent);

    public object Get(BinaryWriter writer, WriteTree tree, object parent) => Value(parent);

}
public class ConstantValueCommand<T> : IGetCommand<T> where T : notnull
{
    public T Value { get; }

    public ConstantValueCommand(T value)
    {
        Value = value;
    }
    public T Get(BinaryReader reader, ReadTree tree, object parent) => Value;

    public T Get(BinaryWriter writer, WriteTree tree, object parent) => Value;

    object IGetCommand.Get(BinaryReader reader, ReadTree tree, object parent) => Get(reader, tree, parent);

    object IGetCommand.Get(BinaryWriter writer, WriteTree tree, object parent) => Get(writer, tree, parent);
}
