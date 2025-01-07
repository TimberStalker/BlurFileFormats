using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Entities.General;

public class BoundingBoxEntity
{
    [AllowNull]
    [Read] public VectorEntity Start { get; set; }
    [AllowNull]
    [Read] public VectorEntity End { get; set; }
    public override string ToString()
    {
        return $"{{{Start} <> {End}}}";
    }
}
