using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Serialization.Entities;

public class BitVectorEntity
{
    [Read] public int BitCount { get; set; }
    [Read] public int V1 => 0x4152;
    [AllowNull]
    [Read] public byte[] Bytes { get; set; }
}
