using BlurFileFormats.SerializationFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.Packing.Entities;
public class PakEntity
{
    [AllowNull]
    [Read] public PreHeaderEntity PreHeader { get; set; }
}
public class PreHeaderEntity
{
    [Read] public string Header => "2KAP";
    [Read] public int Version => 2;
    [Read] public uint SectorSize { get; set; }
}
