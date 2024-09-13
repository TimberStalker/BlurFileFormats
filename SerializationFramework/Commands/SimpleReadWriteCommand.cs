using BlurFileFormats.SerializationFramework.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Commands;

public class SimpleReadWriteCommand<T> : ISerializationReadCommand<T> where T : notnull
{
    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);

    public T Read(BinaryReader reader, ReadTree tree)
    {
        throw new NotImplementedException();
    }
}
public class PairedReadWriteCommand<T> : ISerializationReadCommand<T>, ISerializationWriteCommand<T> where T : notnull
{
    public ISerializationReadCommand<T> ReadCommand { get; }
    public ISerializationWriteCommand<T> WriteCommand { get; }

    public PairedReadWriteCommand(ISerializationReadCommand<T> readCommand, ISerializationWriteCommand<T> writeCommand)
    {
        ReadCommand = readCommand;
        WriteCommand = writeCommand;
    }

    object ISerializationReadCommand.Read(BinaryReader reader, ReadTree tree) => Read(reader, tree);
    public T Read(BinaryReader reader, ReadTree tree) => ReadCommand.Read(reader, tree);


    public void Write(BinaryWriter writer, ReadTree tree, object value) => Write(writer, tree, (T)value);
    public void Write(BinaryWriter writer, ReadTree tree, T value) => WriteCommand.Write(writer, tree, value);
}