using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class ModelsEntity
{
    [Read] public SectionEntity Section { get; set; }
    [Length(nameof(ModelDataEntity.ModelCount))]
    [Read] public SubModelEntity[] Models { get; set; }
}
