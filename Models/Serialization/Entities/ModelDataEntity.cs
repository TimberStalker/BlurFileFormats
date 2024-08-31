using BlurFileFormats.SerializationFramework.Attributes;
using System.Threading.Channels;

namespace BlurFileFormats.Models.Serialization.Entities;

public class ModelDataEntity
{
    [Read] public SectionEntity Section { get; set; }
    [Read] public HeaderEntity Header { get; set; }
    [Read] public int ModelCount { get; set; }
    [Read] public int ElementCount { get; set; }
    [Read] public int Unknown { get; set; }
    [Read] public BoundingBoxEntity BoundingBox { get; set; }
    [Read] public StringTableEntity StringTable { get; set; }
    [Read] public ModelsEntity Models { get; set; }
    [Read] public ElementsEntity Elements { get; set; }
    [Read] public ConstructionEntity Construction { get; set; }

}
public class ConstructionEntity
{
    [Read] public SectionEntity Section { get; set; }
    [Read] public RenderParentEntity RenderParent { get; set; }
}
public class RenderParentEntity
{
    [Read] public SectionEntity Section { get; set; }
    [Read] public RenderEntity Render { get; set; }
}
public class RenderEntity
{
    [Read] public SectionEntity Section { get; set; }
    [Read] public HeaderEntity Header { get; set; }
    [Read] public SceneEntity Scene { get; set; }
}
public class SceneEntity
{
    [Read] public SectionEntity Section { get; set; }
    [Read] public ArchEntity Arch1 { get; set; }
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

    [Read] public ArchItemEntity[] ArchItems { get; set; }

    [Read] public int Unknown11 { get; set; }
    [Read] public int Unknown12 { get; set; }
    [Read] public int Unknown13 { get; set; }

    [Read] public int Unknown14 { get; set; }
    [Read] public int Unknown15 { get; set; }

    [Read] public byte Unknown16_0 { get; set; }
    [Read] public byte Unknown16_1 { get; set; }
    [Read] public byte Unknown16_3 { get; set; }
    [Read] public byte Unknown16_4 { get; set; }

    [Read] public byte Unknown17_0 { get; set; }
    [Read] public byte Unknown17_1 { get; set; }
    [Read] public byte Unknown17_2 { get; set; }
    [Read] public byte Unknown17_3 { get; set; }

    [Read] public byte Unknown18_0 { get; set; }
    [Read] public byte Unknown18_1 { get; set; }
    [Read] public byte Unknown18_2 { get; set; }
    [Read] public byte Unknown18_3 { get; set; }

    [Read] public byte Unknown19_0 { get; set; }
    [Read] public byte Unknown19_1 { get; set; }
    [Read] public byte Unknown19_2 { get; set; }
    [Read] public byte Unknown19_3 { get; set; }

    [Read] public byte Unknown20_0 { get; set; }
    [Read] public byte Unknown20_1 { get; set; }
    [Read] public byte Unknown20_2 { get; set; }
    [Read] public byte Unknown20_3 { get; set; }

    [Read] public byte Unknown21_0 { get; set; }
    [Read] public byte Unknown21_1 { get; set; }
    [Read] public byte Unknown21_2 { get; set; }
    [Read] public byte Unknown21_3 { get; set; }

    [Read] public byte Unknown22_0 { get; set; }
    [Read] public byte Unknown22_1 { get; set; }
    [Read] public byte Unknown22_2 { get; set; }
    [Read] public byte Unknown22_3 { get; set; }

    [Read] public byte Unknown23_0 { get; set; }
    [Read] public byte Unknown23_1 { get; set; }
    [Read] public byte Unknown23_2 { get; set; }
    [Read] public byte Unknown23_3 { get; set; }

    [Read] public int Unknown24 { get; set; }
    [Read] public int Unknown25 { get; set; }

    [Read] public int Unknown26 { get; set; }
    [Read] public byte[] FFs { get; set; }
    [Read] public int Unknown27 { get; set; }

    [Read] public int Unknown28 { get; set; }
    [Read] public int Unknown29 { get; set; }

    [Read] public int Unknown30 { get; set; }
    [Read] public int Unknown31 { get; set; }

    [Read] public int Unknown32 { get; set; }
    [Read] public VertexDefinition[] VertexDefinitions { get; set; }
    [Read] public int Unknown33 { get; set; }
    [Read] public FxFileEntity[] FxFiles { get; set; }
    [Read] public int Unknown34 { get; set; }
    [Read] public int Unknown35 { get; set; }
    [Read] public int Unknown36 { get; set; }
    [Read] public int Unknown37 { get; set; }
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
    [Read] public int Unknown65 { get; set; }
    [Read] public int Unknown66 { get; set; }
    [Read] public int Unknown67 { get; set; }
    [Read] public int Unknown68 { get; set; }
    //[Read] public int Length { get; set; }
    [Read] public VertexStreamEntity[] VertexStreams { get; set; }
    [Read] public int Unknown69 { get; set; }
    [Read] public FaceStreamEntity[] FaceStreams { get; set; }
    [Read] public int Unknown70 { get; set; }
    //[Read] public int Length { get; set; }
    [Read] public RenderingDataEntity[] RenderingData { get; set; }
}
public class RenderingDataEntity
{
    [Read] public byte ID { get; set; }
    [Read] public string Name { get; set; }
    [Read] public short Unknown1_0 { get; set; }
    [Read] public short Unknown1_1 { get; set; }
    [Read] public short Unknown2_0 { get; set; }
    [Read] public short Unknown2_1 { get; set; }
    [Length(nameof(Unknown2_1))]
    [Read] public UnknownDat2[] UnknownGroup1 { get; set; }
    [Read] public string ModelName { get; set; }
    [Read] public int Unknown3 { get; set; }
    [Read] public UnknownDat[] Unknown4 { get; set; }

    [Read] public int Unknown5 { get; set; }
    [Read] public int Unknown6 { get; set; }
    [Read] public int Unknown7 { get; set; }
    [Read] public int Unknown8 { get; set; }

    [Read] public int Unknown9 { get; set; }

    [Read] public BoundingBoxEntity BoundingBox { get; set; }
    [Read] public float Unknown10 { get; set; }
    [Read] public float Unknown11 { get; set; }

    [Read] public int Unknown12 { get; set; }
    [Read] public int Unknown13 { get; set; }
    [Read] public int Unknown14 { get; set; }
    [Read] public byte[] FFs { get; set; }
    [Read] public int Unknown15 { get; set; }

    [Read] public int Unknown16 { get; set; }
    [Read] public int Unknown17 { get; set; }
    [Read] public int Unknown18 { get; set; }
    [Read] public int Unknown19 { get; set; }
}
public class UnknownDat
{
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
}
public class UnknownDat2
{
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Unknown3 { get; set; }
    [Read] public int Unknown4 { get; set; }
    [Read] public short Unknown5 { get; set; }
}