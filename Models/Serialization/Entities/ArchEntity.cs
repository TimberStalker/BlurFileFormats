using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class ArchEntity
{
    [Length(4)]
    [Read] public string Arch { get; set; }
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
    public override string ToString() => $"{Arch} {Unknown1:X}-{Unknown2:X}";
}
