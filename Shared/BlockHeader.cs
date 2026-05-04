namespace BlurFileFormats.Shared;

public struct BlockHeader
{
    public string name;
    public uint size;
    public ushort count;
    public byte type;
    public byte compressionType;
}