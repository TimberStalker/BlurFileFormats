using BlurFileFormats.Models.Serialization.Entities.General;
using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Serialization.Entities;

public class ModelEntity
{
    [AllowNull]
    [Read] public SectionEntity Section { get; set; }
    [AllowNull]
    [Read] public HeaderEntity Header { get; set; }
    [AllowNull]
    [Read] public ModelDataEntity ModelData { get; set; }
}
