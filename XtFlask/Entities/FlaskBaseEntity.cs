using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.XtFlask.Entities;

public class FlaskBaseEntity
{
    [Read] public ushort Type { get; set; }
    [Read] public ushort Offset { get; set; }
}
