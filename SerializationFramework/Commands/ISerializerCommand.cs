using BlurFileFormats.XtFlask.Values;
using System.Reflection.PortableExecutable;

namespace BlurFileFormats.SerializationFramework.Commands;
public interface ISerializerCommand
{
    object? Read(BinaryReader reader, SerializerInfo readInfo);
    void Write(BinaryWriter writer, SerializerInfo writeInfo, object? value);
}
