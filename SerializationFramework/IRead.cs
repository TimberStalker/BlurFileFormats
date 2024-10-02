using BlurFileFormats.SerializationFramework.Command;
using BlurFileFormats.SerializationFramework.Commands.Structures;

namespace BlurFileFormats.SerializationFramework;

public interface IRead
{
    int Order { get; }
    void Build(List<ISerializerPropertyCommand> commands, TypeTree tree);
}
