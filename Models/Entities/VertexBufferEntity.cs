using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Entities;

public class VertexBufferEntity
{
    [Read] public byte Id { get; set; }

    [Read] public int V1 => 0x14152;
    [Read] public int V2 => 0x4152;

    [Read] public int BufferSize { get; set; }

    [Read] public int V3 => 0x4152;
    [AllowNull]
    [Read] public VertexStreamElementEntity[] VertexStreamDefinitions { get; set; }
    [Read] public int V4 => 0x4152;

    [AllowNull]
    [Read] public short[] VertexOffsets { get; set; }

    [Read] public int VertexCount { get; set; }
    [Read] public int V5 => 0x4152;

    [Read] public int Size { get; set; }

    [Read] public int Alignment { get; set; }
    [Read] public CompressionType CompressionType { get; set; }
    [Read] public bool EndianSwap { get; set; }
    [Get(nameof(SceneEntity.SceneStart))]
    [Read] public int SceneStart { get; set; }
    [Position]
    [Read] public int Position { get; set; }
    public int SceneOffset => Position - SceneStart;
    public int Skip => (SceneOffset + Alignment - 1 & -Alignment) - SceneOffset;
    [Length(nameof(Skip))]
    [AllowNull]
    [Read] public byte[] SkipBytes { get; set; }
    [Length(nameof(Size))]
    [AllowNull]
    [Read] public byte[] VertexStream { get; set; }
}
