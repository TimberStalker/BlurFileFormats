using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command;
public interface ISerializationCommand
{
}
public interface ISerializationReadCommand : ISerializationCommand
{
    object Read(BinaryReader reader, ReadTree tree);
}
public interface ISerializationWriteCommand : ISerializationCommand
{
    void Write(BinaryWriter writer, ReadTree tree, object value);
}
public interface ISerializationValueCommand : ISerializationReadCommand, ISerializationWriteCommand
{
}
public interface ISerializationReadCommand<T> : ISerializationReadCommand
{
    new T Read(BinaryReader reader, ReadTree tree);
}
public interface ISerializationWriteCommand<T> : ISerializationWriteCommand
{
    void Write(BinaryWriter writer, ReadTree tree, T value);
}
public interface ISerializationValueCommand<T> : ISerializationValueCommand, ISerializationReadCommand<T>, ISerializationWriteCommand<T>
{
}