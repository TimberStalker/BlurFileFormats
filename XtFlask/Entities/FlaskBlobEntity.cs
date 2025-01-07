using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.XtFlask.Entities;

public class FlaskBlobEntity
{
    [Read] public int Offset { get; set; }
    [Read] public int Count { get; set; }
}
