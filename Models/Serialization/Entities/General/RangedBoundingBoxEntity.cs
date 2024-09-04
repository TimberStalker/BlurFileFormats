using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Serialization.Entities.General;

public class RangedBoundingBoxEntity
{
    [AllowNull]
    [Read] public BoundingBoxEntity BoundingBox { get; set; }
    [Read] public float Min { get; set; }
    [Read] public float Max { get; set; }
}
