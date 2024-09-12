using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Entities;

public class LodEntity
{
    [Read] public int V1 => 0x4152;
    [Read] public int IndexOffset { get; set; }
    [Read] public int IndexCount { get; set; }
    [Read] public int VertexOffset { get; set; }
    [Read] public int VertexCount { get; set; }
}
