using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Entities.Shaders;

public class ShaderParameterEntity
{
    [Read] public int Unknown1 { get; set; }
    [AllowNull]
    [Read] public string Name { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Unknown3 { get; set; }
}
