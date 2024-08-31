using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class VertexStreamDefinitionEntity
{
    [Read] public short Unknown1_0 { get; set; }
    [Read] public short Unknown1_1 { get; set; }

    [Read] public int DataType { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Channel { get; set; }
    [Read] public int SubChannel { get; set; }
    public override string ToString() => $"{DataType:X} {Unknown2:X} {Channel:X}-{SubChannel:X} {Unknown1_0:X4}{Unknown1_1:X4}";
}