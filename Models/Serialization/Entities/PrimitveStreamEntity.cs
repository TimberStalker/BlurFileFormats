using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class PrimitveStreamEntity
{
    [Read] public int V1 => 0x4152;
    [Read] public int ListIndex { get; set; }
    [Length(4)]
    [Read] public bool AlphaTestEnable { get; set; }
    [Read] public int VertexOffset { get; set; }
    [Read] public int Frequency { get; set; }
    [Read] public int V2 => 0x4152;
    [Read] public int AlphaReference { get; set; }
    [Read] public BlendFactor DestinationBlend { get; set; }
}
