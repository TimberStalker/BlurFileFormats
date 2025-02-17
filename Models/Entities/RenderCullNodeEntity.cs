using BlurFileFormats.Models.Entities.General;
using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Entities;
[Target("RenderingData::CullNode")]
public class RenderCullNodeEntity : IRenderingNodeDataEntity
{
    [Read] public int V1 => 0x024152;
    [Read] public int V2 => 0x4152;
    [AllowNull]
    [Read] public string Name { get; set; }
    [Read] public int V3 => 0x4152;
    [AllowNull]
    [Read] public ResourceIndex[] SubNodes { get; set; }

    [AllowNull]
    [Read] public ResourceIndex Parent { get; set; }
    [AllowNull]
    [Read] public ResourceIndex Location { get; set; }

    [Read] public int V4 => 0x4152;
    [AllowNull]
    [Read] public RangedBoundingBoxEntity BoundingBox { get; set; }
    [Read] public int V5 => 0x4152;
    [AllowNull]
    [Read] public BitVectorEntity PVSBits { get; set; }
    [Read] public int GroupIndex { get; set; }
    [Read] public int V6 => 0x4152;
    [AllowNull]
    [Read] public BitVectorEntity PortalBits { get; set; }
}
