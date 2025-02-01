using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Entities;

[Target("RenderingData::RenderListNode_Common")]
public class RenderListNodeEntity : IRenderingNodeDataEntity
{
    [Read] public int V1 => 0x4152;
    [Read] public int V2 => 0x044152;
    [AllowNull]
    [Read] public RenderCullNodeEntity SuperNode { get; set; }
    [Read] public int V3 => 0x4152;
    [AllowNull]
    [Read] public MaterialListEntity MaterialTypes { get; set; }
    [AllowNull]
    [Read] public PrimitiveListEntity PrimitiveList { get; set; }
    [AllowNull]
    [Read] public AfterMeshEntity AfterMeshData { get; set; }
}
