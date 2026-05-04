using BlurFileFormats.SF2.Attributes;

namespace BlurFileFormats.Models.Entities;

public class BlockHeader
{
    [Length(8)]
    [Read] public string Name { get; set;  }
    [Read] public uint Size { get; set; }
    [Read] public ushort Count { get; set; }
    [Read] public byte Type { get; set; }
    [Read] public byte CompressionType { get; set; }
};