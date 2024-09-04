using BlurFileFormats.Models.Serialization.Entities.General;
using BlurFileFormats.Models.Serialization.Entities.Shaders;
using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

namespace BlurFileFormats.Models.Serialization.Entities;

public class ModelDataEntity
{
    [AllowNull]
    [Read] public SectionEntity Section { get; set; }
    [AllowNull]
    [Read] public HeaderEntity Header { get; set; }
    [Read] public int ModelCount { get; set; }
    [Read] public int ElementCount { get; set; }
    [Read] public int Unknown { get; set; }
    [AllowNull]
    [Read] public BoundingBoxEntity BoundingBox { get; set; }
    [AllowNull]
    [Read] public StringTableEntity StringTable { get; set; }
    [AllowNull]
    [Read] public ModelsEntity Models { get; set; }
    [AllowNull]
    [Read] public ElementsEntity Elements { get; set; }
    [AllowNull]
    [Read] public ConstructionEntity Construction { get; set; }

}
public class ConstructionEntity
{
    [AllowNull]
    [Read] public SectionEntity Section { get; set; }
    [AllowNull]
    [Read] public RenderParentEntity RenderParent { get; set; }
}
public class RenderParentEntity
{
    [AllowNull]
    [Read] public SectionEntity Section { get; set; }
    [AllowNull]
    [Read] public RenderEntity Render { get; set; }
}
public class RenderEntity
{
    [AllowNull]
    [Read] public SectionEntity Section { get; set; }
    [AllowNull]
    [Read] public HeaderEntity Header { get; set; }
    [AllowNull]
    [Read] public SceneEntity Scene { get; set; }
}
public class SceneEntity
{
    [AllowNull]
    [Read] public SectionEntity Section { get; set; }
    [Position]
    [Read] public int SceneStart { get; set; }
    [AllowNull]
    [Read] public ArchEntity Arch1 { get; set; }
    [AllowNull]
    [Read] public ArchEntity Arch2 { get; set; }
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Unknown3 { get; set; }

    [Read] public int Unknown4 { get; set; }
    [Read] public int Unknown5 { get; set; }

    [Read] public int Unknown6 { get; set; }
    [Read] public int Unknown7 { get; set; }

    [Read] public int Unknown8 { get; set; }
    [Read] public int Unknown9 { get; set; }

    [Read] public int Unknown10 { get; set; }

    [AllowNull]
    [Read] public ArchItemEntity[] ArchItems { get; set; }

    [Read] public int Unknown11 { get; set; }
    [Read] public int Unknown12 { get; set; }
    [Read] public int Unknown13 { get; set; }

    [Read] public int Unknown14 { get; set; }
    [Read] public int Unknown15 { get; set; }

    [AllowNull]
    [Read] public RangedBoundingBoxEntity BoundingBox { get; set; }

    [Read] public int Unknown24 { get; set; }
    [AllowNull]
    [Read] public BitVectorEntity Unknown25 { get; set; }
    [Read] public int Unknown27 { get; set; }

    [Read] public int Unknown28 { get; set; }
    [Read] public int Unknown29 { get; set; }

    [Read] public int Unknown30 { get; set; }
    [Read] public int Unknown31 { get; set; }

    [AllowNull]
    [Read] public VertexDeclarationListEntity VertexDeclarations { get; set; }
    [Read] public int Unknown33 { get; set; }
    [AllowNull]
    [Read] public FxFileEntity[] FxFiles { get; set; }
    [Read] public int Unknown34 { get; set; }
    [Read] public int Unknown35 { get; set; }
    [Read] public int Unknown36 { get; set; }
    [Read] public int Unknown37 { get; set; }
    [AllowNull]
    [Read] public TextureEntity[] Textures { get; set; }
    [Read] public int Unknown38 { get; set; }
    [Read] public int Unknown39 { get; set; }
    [Read] public int Unknown40 { get; set; }
    [Read] public int Unknown41 { get; set; }
    [Read] public int Unknown42 { get; set; }
    [Read] public int Unknown43 { get; set; }
    [Read] public int Unknown44 { get; set; }
    [Read] public int Unknown45 { get; set; }
    [Read] public int Unknown46 { get; set; }
    [Read] public int Unknown47 { get; set; }
    [Read] public int Unknown48 { get; set; }
    [Read] public int Unknown49 { get; set; }
    [Read] public int Unknown50 { get; set; }
    [Read] public int Unknown51 { get; set; }
    [Read] public int Unknown52 { get; set; }

    [Read] public short Unknown53_0 { get; set; }
    [Read] public short Unknown53_1 { get; set; }
    [Read] public short Unknown53_2 { get; set; }
    [Read] public byte Unknown53_3 { get; set; }

    [Read] public int Unknown54 { get; set; }
    [Read] public int Unknown55 { get; set; }
    [Read] public int Unknown56 { get; set; }
    [Read] public int Unknown57 { get; set; }
    [Read] public int Unknown58 { get; set; }
    [Read] public int Unknown59 { get; set; }
    [Read] public int Unknown60 { get; set; }
    [Read] public int Unknown61 { get; set; }
    [Read] public int Unknown62 { get; set; }
    [Read] public int Unknown63 { get; set; }
    [Read] public int Unknown64 { get; set; }
    [Read] public int V1 => 0x4152;
    [Read] public PlatformType PlatformType { get; set; }
    [Length(4)]
    [Read] public bool IgnoreBuffers { get; set; }
    [Read] public int V2 => 0x4152;
    [AllowNull]
    [Read] public VertexBufferEntity[] VertexBuffers { get; set; }
    [Read] public int V3 => 0x4152;
    [AllowNull]
    [Read] public IndexBufferEntity[] IndexBuffers { get; set; }
    [Read] public int V4 => 0x4152;
    [AllowNull]
    [Read] public RenderingNodes[] Nodes { get; set; }
}
public class TextureListEntity
{
    [Read] public int V1 => 0x4152;
}
public enum TextureAdress
{
    Wrap,
    Mirror,
    Clamp,
    Border
}
[Target("RenderingData::RenderListNode_Common")]
public class RenderListNodeEntity : IRenderingNodeDataEntity
{
    [Read] public int V1 => 0x4152;
    [Read] public int V2 => 0x044152;
    [AllowNull]
    [Read] public RenderCullNodeEntity SuperNode { get; set; }
    [Read] public int V3 => 0x4152;
    [AllowNull]
    [Read] public MaterialListEntity RenderPasses { get; set; }
    [Read] public int Unknown32 { get; set; }
    [Read] public int Unknown33 { get; set; }
    [AllowNull]
    [Read] public PrimitiveListEntity PrimitiveList { get; set; }
    //[AllowNull]
    //[Read] public AfterMeshEntity AfterMeshData { get; set; }
}
public enum PlatformType
{
    Unknown = -1,
    X360,
    PS3,
    PC,
}
public class PlatformTypeEntity
{
    [Read] public int V1 => 0x4152;
    [Read] public PlatformType PlatformType { get; set; }
}
public class MaterialListEntity
{
    [Read] public int V1 => 0x4152;
    [AllowNull]
    [Read] public MaterialEntity[] MaterialTypes { get; set; }
}
public class AfterMeshEntity
{
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Unknown3 { get; set; }
    [Read] public int Unknown4 { get; set; }
    [Read] public int Unknown5 { get; set; }
    [Read] public int Unknown6 { get; set; }
    [Read] public int Unknown7 { get; set; }
    [Switch(nameof(Unknown7))]
    [AllowNull]
    [Read] public IAfterMeshDataEntity AfterMeshData { get; set; }
}
public interface IAfterMeshDataEntity
{
}
[Target(0x02)]
public class AfterMeshData02 : IAfterMeshDataEntity
{
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Unknown3 { get; set; }
    [Read] public int Unknown4 { get; set; }
    [Read] public float Unknown5 { get; set; }
    [Read] public float Unknown6 { get; set; }
    [Read] public int Unknown7 { get; set; }
    [Read] public int Unknown8 { get; set; }
    [Read] public int Unknown9 { get; set; }
    [Read] public float Unknown10 { get; set; }
    [Read] public int Unknown11 { get; set; }
    [Read] public int Unknown12 { get; set; }
    [Read] public int Unknown13 { get; set; }
    [Read] public float Unknown14 { get; set; }
    [Read] public int Unknown15 { get; set; }
    [Read] public int Unknown16 { get; set; }
    [Read] public int Unknown17 { get; set; }
}

[Target(0x03)]
public class AfterMeshData03 : IAfterMeshDataEntity
{
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Unknown3 { get; set; }
    [Read] public int Unknown4 { get; set; }
    [Read] public float Unknown5 { get; set; }
    [Read] public float Unknown6 { get; set; }
    [Read] public float Unknown7 { get; set; }
    [Read] public float Unknown8 { get; set; }
    [Read] public float Unknown9 { get; set; }
    [Read] public float Unknown10 { get; set; }
    [Read] public float Unknown11 { get; set; }
    [Read] public float Unknown12 { get; set; }
    [Read] public float Unknown13 { get; set; }
}
[Target(0x09)]
public class AfterMeshData09 : IAfterMeshDataEntity
{
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Unknown3 { get; set; }
    [Read] public int Unknown4 { get; set; }
    [Read] public float Unknown5 { get; set; }
    [Read] public float Unknown6 { get; set; }
    [Read] public int Unknown7 { get; set; }
    [Read] public int Unknown8 { get; set; }
    [Read] public int Unknown9 { get; set; }
}
public class UnknownDatEntity
{
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
}
public class UnknownDatEntity2
{
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Unknown3 { get; set; }
    [Read] public int Unknown4 { get; set; }
    [Read] public short Unknown5 { get; set; }
}