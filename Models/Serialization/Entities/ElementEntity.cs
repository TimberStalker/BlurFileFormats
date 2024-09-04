using BlurFileFormats.Models.Serialization.Entities.General;
using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Serialization.Entities;

public class ElementEntity
{
    [Read] public int ModelIndex { get; set; }
    [AllowNull]
    [Read] public MatrixEntity TransformMatrix { get; set; }
    [AllowNull]
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
