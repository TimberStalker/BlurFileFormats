using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Serialization.Entities.Shaders;

public class MaterialEntity
{
    [Read] public int V1 => 0x4152;
    [Read] public int Type { get; set; }
    [Switch(nameof(Type))]
    [AllowNull]
    [Read] public IShaderDataEntity ShaderData { get; set; }
}
