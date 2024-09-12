using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlurFileFormats.SerializationFramework;
using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Entities;
public class CPModelEntity
{
    [FileSpecifier("  CP")]
    [Length(4)]
    [AllowNull]
    [Read] public string File { get; set; }
    [AllowNull]
    [Read] public ModelEntity Model { get; set; }
}
