using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class ElementsEntity
{
    [Read] public SectionEntity Section { get; set; }
    [Length(nameof(ModelDataEntity.ElementCount))]
    [Read] public ElementEntity[] Elements { get; set; }
}
