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
    void Read(BinaryReader reader, ReadTree tree, IReadTarget target);
}
public interface ISerializationWriteCommand : ISerializationCommand
{
    void Write(BinaryWriter writer, ReadTree tree, object value);
}
public interface ISerializationValueCommand : ISerializationReadCommand, ISerializationWriteCommand
{

}