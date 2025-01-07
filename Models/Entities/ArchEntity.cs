using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Entities;

public class ArchEntity
{
    [Length(4)]
    [AllowNull]
    [Read] public string Arch { get; set; }
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
    public override string ToString() => $"{Arch} {Unknown1:X}-{Unknown2:X}";
}
