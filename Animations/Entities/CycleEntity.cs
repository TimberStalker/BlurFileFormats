using BlurFileFormats.SerializationFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.Animations.Entities;
public class CycleEntity
{
    [Read] public uint Version => 0xF;
    [Length(22)]
    [Read] public float[] Values { get; set; }
    [Read] public uint UnknownSize { get; set; }
    [Read] public float UnknownF1 { get; set; }
    [Read] public float UnknownF2 { get; set; }
    [Read] public byte[] PropertyByte { get; set; }
    [Read] public string[] Properties { get; set; }
}
