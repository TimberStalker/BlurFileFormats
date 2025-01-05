using BlurFileFormats.SerializationFrameworkOld.Command;
using BlurFileFormats.SerializationFrameworkOld.Commands.Structures;

namespace BlurFileFormats.SerializationFrameworkOld;

public interface IRead
{
    int Order { get; }
    void Build(List<ISerializerPropertyCommand> commands, TypeTree tree);
}
