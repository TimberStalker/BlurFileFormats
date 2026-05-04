namespace BlurFileFormats.Shared;

public struct ResourceIndex
{
    public int block;
    public int index;
    public override string ToString() => $"{block}:{index}";
}