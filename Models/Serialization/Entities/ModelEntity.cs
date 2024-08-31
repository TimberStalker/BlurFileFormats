using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class ModelEntity
{
    [Read] public SectionEntity Section { get; set; }
    [Read] public HeaderEntity Header { get; set; }
    [Read] public ModelDataEntity ModelData { get; set; }
}
