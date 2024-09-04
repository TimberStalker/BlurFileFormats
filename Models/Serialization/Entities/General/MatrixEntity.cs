using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Serialization.Entities.General;

public class MatrixEntity
{
    [AllowNull]
    [Read] public VectorEntity M0 { get; set; }
    [AllowNull]
    [Read] public VectorEntity M1 { get; set; }
    [AllowNull]
    [Read] public VectorEntity M2 { get; set; }
    [AllowNull]
    [Read] public VectorEntity M3 { get; set; }
}
