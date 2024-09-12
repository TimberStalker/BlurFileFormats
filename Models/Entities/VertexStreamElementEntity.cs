using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Entities;

public class VertexStreamElementEntity
{
    [Read] public int V1 => 0x4152;

    [Read] public VertexElementType Type { get; set; }
    [Read] public int Method { get; set; }
    [Read] public VertexElementUsage Usage { get; set; }
    [Read] public int UsageIndex { get; set; }
}