using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Entities.Shaders;

public class MaterialTypeEntity
{
    [Read] public int V1 => 0x4152;
    [AllowNull]
    [Read] public string FxName { get; set; }
    [Read] public int V2 => 0x4152;
    [AllowNull]
    [Read] public MaterialElementEntity[] Parameters { get; set; }
    [Read] public int DataSize { get; set; }
    [Read] public int MaxAlign { get; set; }
    [Read] public int V3 => 0x4152;
    [AllowNull]
    [Read] public short[] Offsets { get; set; }
    [Read] public int Effect { get; set; }
    [Read] public int ListIndex { get; set; }
}
