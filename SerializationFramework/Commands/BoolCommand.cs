using BlurFileFormats.SerializationFramework.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Commands;
public class BoolCommand : ISerializeCommand<bool>
{
    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);
    public bool Read(BinaryReader reader, ReadTree tree, object parent)
    {
        return reader.ReadByte() > 0;
    }

    public void Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (bool)value);
    public void Write(BinaryWriter writer, WriteTree tree, object parent, bool value)
    {
        writer.Write((byte)(value ? 1 : 0));
    }
}
public class LargeBoolCommand : ISerializeCommand<bool>
{
    SerializerCommandReference<int> LengthCommand { get; }

    public LargeBoolCommand(SerializerCommandReference<int> lengthCommand)
    {
        LengthCommand = lengthCommand;
    }

    object ISerializeCommand.Read(BinaryReader reader, ReadTree tree, object parent) => Read(reader, tree, parent);
    public bool Read(BinaryReader reader, ReadTree tree, object parent)
    {
        int length = LengthCommand.GetOrRead(reader, tree, parent);
        bool isTrue = false;
        for (int i = 0; i < length; i++)
        {
            if (reader.ReadByte() > 0)
            {
                isTrue = true;
            }
        }
        return isTrue;
    }

    public void Write(BinaryWriter writer, WriteTree tree, object parent, object value) => Write(writer, tree, parent, (bool)value);
    public void Write(BinaryWriter writer, WriteTree tree, object parent, bool value)
    {
        writer.Write((byte)(value ? 1 : 0));
    }
}
