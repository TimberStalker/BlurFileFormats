using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class BoundingBoxEntity
{
    [Read] public VectorEntity Start { get; set; }
    [Read] public VectorEntity End { get; set; }
    public override string ToString()
    {
        return $"{{{Start} <> {End}}}";
    }
}
