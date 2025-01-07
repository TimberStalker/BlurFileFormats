using BlurFileFormats.Models.Entities.General;
using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Entities;

public class SubModelEntity
{
    [AllowNull]
    [Read] public MatrixEntity TransformMatrix { get; set; }
    [AllowNull]
    [Read] public BoundingBoxEntity BoundingBox { get; set; }
    [Read] public int NameIndex { get; set; }
    [Read] public int ModelIndex { get; set; }
    [Read] public int ElementCount { get; set; }
    [Read] public int HierarchyIndex { get; set; }
    [Read] public int ParentIndex { get; set; }
    [Read] public int Unknown1 { get; set; }
    [Read] public int Unknown2 { get; set; }
}
