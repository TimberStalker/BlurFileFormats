using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Entities;

public class VertexDeclarationElementEntity
{
    [Read] public int V1 => 0x4152;
    [Read] public short TypePrefix { get; set; }
    [Read] public short Offset { get; set; }
    [Read] public VertexElementType Type { get; set; }
    [Read] public int Method { get; set; }
    [Read] public VertexElementUsage Usage { get; set; }
    [Read] public byte UsageIndex { get; set; }
}