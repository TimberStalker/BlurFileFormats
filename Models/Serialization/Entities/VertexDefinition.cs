using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class VertexDefinition
{
    [Read] public int Unknown1 { get; set; }
    [Read] public VertexDefintionPart[] Parts { get; set; }
}
