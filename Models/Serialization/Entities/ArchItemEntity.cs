using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class ArchItemEntity
{
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
    public override string ToString() => $"{Unknown1}-{Unknown2}";
}
