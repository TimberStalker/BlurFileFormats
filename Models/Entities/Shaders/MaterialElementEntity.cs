using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Entities.Shaders;

public class MaterialElementEntity
{
    [Read] public int V1 => 0x4152;
    [AllowNull]
    [Read] public string Name { get; set; }
    [Read] public MaterialElementType Type { get; set; }
    [Read] public int Count { get; set; }
}
public enum MaterialElementType
{
    S8,
    U8,
    S16,
    U16,
    S32,
    U32,
    F32,
    TextureIndex,
    BasicMax,

    MaterialIndex,
    DataIndex,
    TexturePointer,
    Max
}