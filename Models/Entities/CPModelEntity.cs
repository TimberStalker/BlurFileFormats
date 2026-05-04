using System;
using System.Collections.Generic;
using System.Text;
using BlurFileFormats.SF2.Attributes;

namespace BlurFileFormats.Models.Entities;

public class CPModelEntity
{
    [Read] public string Magic => "  CP";
    [Read] public ModelEntity Model { get; set; }
}

public class ModelEntity
{
}

public class Block
{
    [Read] public BlockHeader BlockHeader { get; set; }
    [Length(nameof(BlockHeader.Size))]
    [Read] public byte[] Data { get; set; }
}