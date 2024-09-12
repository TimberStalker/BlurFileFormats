using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BlurFileFormats.Models.Entities;

public class TextureEntity
{
    [Encoding("utf-8")]
    [AllowNull]
    [Read] public string FileName { get; set; }
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Unknown3 { get; set; }
    [Read] public int Unknown4 { get; set; }
    [Encoding("utf-8")]
    [AllowNull]
    [Read] public string FileName2 { get; set; }
    [Read] public int Unknown5 { get; set; }
    [Read] public int Unknown6 { get; set; }
    [Read] public int Unknown7 { get; set; }
    [Read] public int Unknown8 { get; set; }
    [Read] public int Unknown9 { get; set; }
    [Read] public int Unknown10 { get; set; }
    [Read] public int Unknown11 { get; set; }
    [Read] public int Unknown12 { get; set; }
    [Read] public int Unknown13 { get; set; }
    [Read] public int Unknown14 { get; set; }
    [Read] public int Unknown15 { get; set; }
    [Read] public int Unknown16 { get; set; }

    [Read] public int Length { get; set; }
    [Read] public int Height { get; set; }
    [Read] public int Width { get; set; }

    public int Pitch => (Width * 1024 + 7) / 8;

    [Read] public int Unknown17 { get; set; }

    [Read] public int Mipmaps { get; set; }
    [Length(4)]
    [AllowNull]
    [Read] public string DxtVersion { get; set; }

    [Read] public int Unknown18 { get; set; }
    [Read] public int Unknown19 { get; set; }
    public int TextureLength => Length - 0x1C;

    [Length(nameof(TextureLength))]
    [AllowNull]
    [Read] public byte[] TextureBytes { get; set; }
}
