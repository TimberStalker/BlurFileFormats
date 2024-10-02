using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command;

public interface ISerializerCommand
{

}
public interface ISerializerCommand<T>
{

}
public interface IGetCommand : ISerializerCommand
{
    object Get(BinaryReader reader, ReadTree tree, object parent);
    object Get(BinaryWriter writer, WriteTree tree, object parent);
}
public interface IGetCommand<T> : IGetCommand, ISerializerCommand<T>
{
    new T Get(BinaryReader reader, ReadTree tree, object parent);
    new T Get(BinaryWriter writer, WriteTree tree, object parent);
}
public interface ISerializeCommand : ISerializerCommand
{
    object Read(BinaryReader reader, ReadTree tree, object parent);
    void Write(BinaryWriter writer, WriteTree tree, object parent, object value);
}
public interface ISerializeCommand<T> : ISerializeCommand, ISerializerCommand<T>
{
    new T Read(BinaryReader reader, ReadTree tree, object parent);
    void Write(BinaryWriter writer, WriteTree tree, object parent, T value);
}