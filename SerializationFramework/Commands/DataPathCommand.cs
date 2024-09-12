using BlurFileFormats.SerializationFramework.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Commands;
public class DataPathCommand<T> : ISerializationValueCommand<T> where T : notnull
{
    public DataPath Path { get; }

    public DataPathCommand(DataPath path)
    {
        Path = path;
    }

    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public T Read(BinaryReader reader, ReadTree tree) => tree.GetValue<T>(Path);


    public void Write(BinaryWriter writer, ReadTree tree, T value) { }

    public void Write(BinaryWriter writer, ReadTree tree, object value) { }
}public class DataPathCommandOverwrite<T> : ISerializationValueCommand<T> where T : notnull
{
    public DataPath Path { get; }
    public ISerializationWriteCommand<T> WriteCommand { get; }

    public DataPathCommandOverwrite(DataPath path, ISerializationWriteCommand<T> writeCommand)
    {
        Path = path;
        WriteCommand = writeCommand;
    }

    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public T Read(BinaryReader reader, ReadTree tree) => tree.GetValue<T>(Path);


    public void Write(BinaryWriter writer, ReadTree tree, T value)
    {
        throw new NotImplementedException();
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        throw new NotImplementedException();
    }
}
