using BlurFileFormats.Models.Entities.General;
using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Entities;

public class ModelsEntity
{
    [AllowNull]
    [Read] public SectionEntity Section { get; set; }

    [Length(nameof(ModelDataEntity.ModelCount))]
    [AllowNull]
    [Read] public SubModelEntity[] Models { get; set; }
}
