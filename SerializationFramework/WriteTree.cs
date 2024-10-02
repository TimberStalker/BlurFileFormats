using BlurFileFormats.SerializationFramework.Command;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.SerializationFramework;

public class WriteTree : ReadTree
{
    Dictionary<ISerializerCommand, SerializarionInfo> Writes { get; } = [];

    public void AddInfo(ISerializerCommand command, BinaryWriter writer)
    {
        Writes.Add(command, new SerializarionInfo(writer.BaseStream.Position));
    }

    public bool TryGetWriteInfo(ISerializerCommand command, [NotNullWhen(true)]out SerializarionInfo? info)
    {
        if(Writes.TryGetValue(command, out info)) return true;
        info = null;
        return false;
    }

    public record SerializarionInfo(long Position)
    {
        
    }
}
