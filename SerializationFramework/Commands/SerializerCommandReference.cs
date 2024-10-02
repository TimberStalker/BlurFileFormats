using BlurFileFormats.SerializationFramework.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Commands;
public class SerializerCommandReference : ISerializerCommand
{
    public ISerializerCommand Command { get; }

    public SerializerCommandReference(ISerializerCommand command)
    {
        Command = command;
    }

    public object GetOrRead(BinaryReader reader, ReadTree tree, object parent)
    {
        if (Command is IGetCommand g)
        {
            return g.Get(reader, tree, parent);
        }
        else if (Command is ISerializeCommand s)
        {
            return s.Read(reader, tree, parent);
        }
        else
        {
            throw new UnreachableException();
        }
    }

    public void GetOrWrite(BinaryWriter writer, WriteTree tree, object parent, object defaultValue, out object value)
    {
        value = defaultValue;
        if (Command is IGetCommand g)
        {
            value = g.Get(writer, tree, parent);
        }
        else if (Command is ISerializeCommand s)
        {
            s.Write(writer, tree, value, parent);
        }
        else
        {
            throw new UnreachableException();
        }
    }
}
public class SerializerCommandReference<T> : ISerializerCommand<T> where T : notnull
{
    public ISerializerCommand<T> Command { get; }

    public SerializerCommandReference(ISerializerCommand<T> command)
    {
        Command = command;
    }

    public T GetOrRead(BinaryReader reader, ReadTree tree, object parent)
    {
        if (Command is IGetCommand<T> g)
        {
            return g.Get(reader, tree, parent);
        }
        else if (Command is ISerializeCommand<T> s)
        {
            return s.Read(reader, tree, parent);
        }
        else
        {
            throw new UnreachableException();
        }
    }

    public void GetOrWrite(BinaryWriter writer, WriteTree tree, object parent, in T expected, out T value)
    {
        value = expected;
        if (Command is IGetCommand<T> g)
        {
            value = g.Get(writer, tree, parent);
        }
        else if (Command is ISerializeCommand<T> s)
        {
            s.Write(writer, tree, parent, value);
        }
        else
        {
            throw new UnreachableException();
        }
    }
}
