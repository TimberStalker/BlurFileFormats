using BlurFileFormats.SerializationFramework.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Commands;
public class VerifyPropertyCommand : ISerializationReadCommand
{
    public PropertyInfo Property { get; }
    public ISerializationReadCommand ReadCommand { get; }
    public VerifyPropertyCommand(PropertyInfo property, ISerializationReadCommand readCommand)
    {
        Property = property;
        ReadCommand = readCommand;
    }

    public object Read(BinaryReader reader, ReadTree tree)
    {
        var value = Property.GetValue(tree.CurrentObject)!;
        var read = ReadCommand.Read(reader, tree);

        if (!value.Equals(read)) throw new Exception();

        return read;
    }
}
