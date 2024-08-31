using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class SubModelEntity
{
    [Read] public MatrixEntity TransformMatrix { get; set; }
    [Read] public BoundingBoxEntity BoundingBox { get; set; }
    [Read] public int NameIndex { get; set; }
    [Read] public int ModelIndex { get; set; }
    [Read] public int ElementCount { get; set; }
    [Read] public int HierarchyIndex { get; set; }
    [Read] public int ParentIndex { get; set; }
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
}
