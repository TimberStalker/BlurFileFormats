using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Entities;

public class IndexBufferEntity
{
    [Read] public byte ID { get; set; }
    [Read] public int V1 => 0x014152;
    [Read] public int BufferSize { get; set; }
    [Read] public IndexType IndexType { get; set; }
    [Read] public int V2 => 0x4152;
    [Read] public int Size { get; set; }

    [Read] public int Alignment { get; set; }
    [Read] public CompressionType CompressionType { get; set; }
    [Read] public bool EndianSwap { get; set; }
    [Align(nameof(Alignment))]
    [AllowNull]
    [Read] public byte AlignBytes { get; set; }
    [Length(nameof(Size))]
    [AllowNull]
    [Read] public byte[] VertexStream { get; set; }
}
public enum IndexType
{
    U16,
    U32
}
public enum CompressionType
{
    None,
    ZLib,
    XMem
}