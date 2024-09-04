using BlurFileFormats.Models.Serialization.Entities.General;
using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Serialization.Entities;

public class ElementsEntity
{
    [AllowNull]
    [Read] public SectionEntity Section { get; set; }
    [Length(nameof(ModelDataEntity.ElementCount))]
    [AllowNull]
    [Read] public ElementEntity[] Elements { get; set; }
}
