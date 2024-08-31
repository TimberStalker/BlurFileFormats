using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class ElementEntity
{
    [Read] public int ModelIndex { get; set; }
    [Read] public MatrixEntity TransformMatrix { get; set; }
    [Read] public BoundingBoxEntity BoundingBox { get; set; }
    [Read] public int NameIndex { get; set; }
    [Read] public int ElementIndex { get; set; }
    [Read] public int ParentIndex { get; set; }
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Unknown3 { get; set; }
    [Read] public short Unknown4_0 { get; set; }
    [Read] public short Unknown4_1 { get; set; }
}
