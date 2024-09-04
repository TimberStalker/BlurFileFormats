using BlurFileFormats.SerializationFramework.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace BlurFileFormats.Models.Serialization.Entities;

public class RenderingNodes
{
    [Read] public byte ID { get; set; }
    [AllowNull]
    [Read] public string Name { get; set; }
    [Switch(nameof(Name))]
    [AllowNull]
    [Read] public IRenderingNodeDataEntity NodeData { get; set; }
}
