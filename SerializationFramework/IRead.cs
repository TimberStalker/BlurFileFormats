namespace BlurFileFormats.SerializationFramework;

public interface IRead
{
    void Read(BinaryReader reader, ReadTree tree);
}
