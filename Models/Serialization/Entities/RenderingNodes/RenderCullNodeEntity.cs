using BlurFileFormats.Models.Serialization.Entities.General;
using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Serialization.Entities;

[Target("RenderingData::CullNode")]
public class RenderCullNodeEntity : IRenderingNodeDataEntity
{
    [Read] public int V1 => 0x024152;
    [Read] public int V2 => 0x4152;
    [AllowNull]
    [Read] public string Name { get; set; }
    [Read] public int V3 => 0x4152;
    [AllowNull]
    [Read] public UnknownDatEntity[] SubNodes { get; set; }

    [Read] public int ParentBlock { get; set; }
    [Read] public int ParentIndex { get; set; }
    [Read] public int Unknown7 { get; set; }
    [Read] public int Unknown8 { get; set; }

    [Read] public int Unknown9 { get; set; }

    [AllowNull]
    [Read] public RangedBoundingBoxEntity BoundingBox { get; set; }
    [Read] public int V4 => 0x4152;
    [AllowNull]
    [Read] public BitVectorEntity PVSBits { get; set; }
    [Read] public int GroupIndex { get; set; }
    [Read] public int V5 => 0x4152;
    [AllowNull]
    [Read] public BitVectorEntity PortalBits { get; set; }
}
