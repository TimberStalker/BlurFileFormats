using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities.General;

public class HeaderEntity
{
    [CString]
    [Length(8)]
    [Read] public string Name { get; set; } = "";
    [Read] public int Length { get; set; }
    [Read] public short UnknownA { get; set; }
    [Read] public short UnknownB { get; set; }
    [Read] public int Depth { get; set; }
}
