using BlurFileFormats.SerializationFramework.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Commands;
public class BoolCommand : ISerializationValueCommand<bool>
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public bool Read(BinaryReader reader, ReadTree tree)
    {
        return reader.ReadByte() > 0;
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (bool)value);
    public void Write(BinaryWriter writer, ReadTree tree, bool value)
    {
        writer.Write((byte)(value ? 1 : 0));
    }
}
public class LargeBoolCommand : ISerializationValueCommand<bool>
{
    ISerializationReadCommand<int> LengthCommand { get; }

    public LargeBoolCommand(ISerializationReadCommand<int> lengthCommand)
    {
        LengthCommand = lengthCommand;
    }

    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public bool Read(BinaryReader reader, ReadTree tree)
    {
        int length = LengthCommand.Read(reader, tree);
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

    public void Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (bool)value);
    public void Write(BinaryWriter writer, ReadTree tree, bool value)
    {
        writer.Write((byte)(value ? 1 : 0));
    }
}
