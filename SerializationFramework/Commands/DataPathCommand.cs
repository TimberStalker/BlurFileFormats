using BlurFileFormats.SerializationFramework.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Commands;
public class DataPathCommand<T> : IGetCommand<T> where T : notnull
{
    public ISerializerCommand<T> Command { get; }

    public DataPathCommand(ISerializerCommand<T> command)
    {
        Command = command;
    }

    public T Get(BinaryReader reader, ReadTree tree, object parent) => tree.GetValue(Command);

    public T Get(BinaryWriter writer, WriteTree tree, object parent) => tree.GetValue(Command);

    object IGetCommand.Get(BinaryReader reader, ReadTree tree, object parent) => Get(reader, tree, parent);

    object IGetCommand.Get(BinaryWriter writer, WriteTree tree, object parent) => Get(writer, tree, parent);

    public static ISerializerCommand<T> Create(DataPath path, TypeTree tree)
    {
        var command = tree.GetCommand<T>(path);
        return new DataPathCommand<T>(command);
    }
}