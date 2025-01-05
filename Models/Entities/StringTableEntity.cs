using BlurFileFormats.Models.Entities.General;
using BlurFileFormats.SerializationFrameworkOld.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Entities;

public class StringTableEntity
{
    [AllowNull]
    [Read] public SectionEntity Section { get; set; }
    [AllowNull]
    [Read] public int[] StringOffsets { get; set; }
    [AllowNull]
    [Read] public byte[] Bytes { get; set; }

}
