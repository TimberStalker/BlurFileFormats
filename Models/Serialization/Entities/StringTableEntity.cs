using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class StringTableEntity
{
    [Read] public SectionEntity Section { get; set; }
    [Read] public int[] StringOffsets { get; set; }
    [Read] public byte[] Bytes { get; set; }

}
