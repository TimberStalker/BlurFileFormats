using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BlurFileFormats.Models.Serialization.Entities;

public class FxFileEntity
{
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Encoding("utf-8")]
    [AllowNull]
    [Read] public string FileName { get; set; }
    public override string ToString() => $"{Unknown1:X}-{Unknown2:X} {FileName}";
}
