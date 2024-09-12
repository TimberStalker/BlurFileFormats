using BlurFileFormats.Models.Entities.Shaders;
using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Entities;

[Default]
public class CorrectShaderDataEntity : IShaderDataEntity
{
    [Length(nameof(MaterialEntity.Type))]
    [AllowNull]
    [Read] public string FxName { get; set; }
    [Read] public int Unknown1 { get; set; }
    [AllowNull]
    [Read] public ShaderParameterEntity[] Parameters { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Unknown3 { get; set; }
    [Read] public int Unknown4 { get; set; }
    [AllowNull]
    [Read] public short[] Offsets { get; set; }
    [Read] public int Unknown6 { get; set; }
    [Read] public int Unknown7 { get; set; }
}
