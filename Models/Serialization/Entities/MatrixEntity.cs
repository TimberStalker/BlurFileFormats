using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class MatrixEntity
{
    [Read] public VectorEntity M0 { get; set; }
    [Read] public VectorEntity M1 { get; set; }
    [Read] public VectorEntity M2 { get; set; }
    [Read] public VectorEntity M3 { get; set; }
}
