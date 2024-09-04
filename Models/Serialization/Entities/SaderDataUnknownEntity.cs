using BlurFileFormats.Models.Serialization.Entities.Shaders;
using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

[Target(0)]
public class SaderDataUnknownEntity : IShaderDataEntity
{
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Unknown3 { get; set; }
    [Read] public int Unknown4 { get; set; }
    [Read] public int Unknown5 { get; set; }
    [Read] public int Unknown6 { get; set; }
}
