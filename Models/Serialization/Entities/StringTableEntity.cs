using BlurFileFormats.Models.Serialization.Entities.General;
using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Serialization.Entities;

public class StringTableEntity
{
    [AllowNull]
    [Read] public SectionEntity Section { get; set; }
    [AllowNull]
    [Read] public int[] StringOffsets { get; set; }
    [AllowNull]
    [Read] public byte[] Bytes { get; set; }

}
