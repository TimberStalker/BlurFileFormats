using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class VertexDefintionPart
{
    [Read] public int Unknown1 { get; set; }
    [Read] public short TypePrefix { get; set; }
    [Read] public short Offset { get; set; }
    [Read] public int DataType { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Channel { get; set; }
    [Read] public byte SubChannel { get; set; }
    public override string ToString()
    {
        return $"{DataType:X}   {TypePrefix}-{Offset,-2}   {Channel}-{SubChannel} | {Unknown1:X}, {Unknown2:X}";
    }
}