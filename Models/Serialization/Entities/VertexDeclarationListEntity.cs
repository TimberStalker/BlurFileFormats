using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Serialization.Entities;

public class VertexDeclarationListEntity
{
    [Read] public int V1 => 0x4152;
    [AllowNull]
    [Read] public VertexDeclarationEntity[] VertexDeclarations { get; set; }
}
