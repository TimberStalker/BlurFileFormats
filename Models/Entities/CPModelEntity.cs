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
    [Length(4)]
    [Read] public string File => "  CP";
    [AllowNull]
    [Read] public ModelEntity Model { get; set; }
}
