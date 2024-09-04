using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Serialization.Entities;
public enum PrimitveType
{
    TriangleList,
    TriangleStrip,
    PointList,
    QuadList,
    QuadStrip
}
public enum CullMode
{
    None,
    Normal,
    Reversed,
}
public enum BlendOperation
{
    Add,
    Subtract,
    Min,
    Max,
    ReverseSubtract,
}
public enum BlendFactor
{
    Zero,
    One,
    SourceColor,
    InverseSourceColor,
    SourceAlpha,
    InverseSourceAlpha,
    DestinationAlpha,
    InverseDestinationAlpha,
    DestinationColor,
    InverseDestinationColor,
}
public class PrimitiveEntity
{
    [Read] public int V1 => 0x014152;
    [Read] public int Effect { get; set; }
    [Read] public int VertexDeclaration { get; set; }
    [Read] public PrimitveType PrimitveType { get; set; }
    [Read] public int IndexBufferIndex { get; set; }
    [Read] public int Flags { get; set; }

    [Read] public int V2 => 0x4152;

    [Read] public int BaseMaterialListIndex { get; set; }
    [Read] public int BaseMaterialIndex { get; set; }

    [Read] public int V3 => 0x4152;
    [Read] public int InstanceMaterialListIndex { get; set; }
    [Read] public int InstanceMaterialIndex { get; set; }

    [Read] public int V4 => 0x4152;
    [Read] public int PrimitveStream { get; set; }
    [Read] public BlendFactor BlendFactor { get; set; }
    [Read] public BlendOperation BlendOperation { get; set; }
    [Read] public int Unknown13 { get; set; }
    [Read] public CullMode CullMode { get; set; }
    [Read] public int ListIndex { get; set; }

    [Read] public int V5 => 0x4152;
    [AllowNull]
    [Read] public LodEntity[] Lods { get; set; }
    [Read] public int V6 => 0x4152;
    [AllowNull]
    [Read] public PrimitveStreamEntity[] PrimitveStreams { get; set; }
}