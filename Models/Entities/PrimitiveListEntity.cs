using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Entities;

public class PrimitiveListEntity
{
    [Read] public int V1 => 0x4152;
    [AllowNull]
    [Read] public PrimitiveEntity[] Primitives { get; set; }
}
