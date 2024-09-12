using BlurFileFormats.SerializationFramework.Command;

namespace BlurFileFormats.SerializationFramework;

public interface IRead
{
    void Build(List<ISerializationCommand> commands);
}
