using BlurFileFormats.SerializationFramework.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Commands;
public class ConstantValueCommand<T> : ISerializationValueCommand<T> where T : notnull
{
    public T Value { get; }

    public ConstantValueCommand(T value)
    {
        Value = value;
    }

    public T Read(BinaryReader reader, ReadTree tree) => Value;
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Value;

    public void Write(BinaryWriter writer, ReadTree tree, T value) { }

    public void Write(BinaryWriter writer, ReadTree tree, object value) { }
}
