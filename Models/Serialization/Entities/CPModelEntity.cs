﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlurFileFormats.SerializationFramework;
using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;
public class CPModelEntity
{
    [FileSpecifier("  CP")]
    [Length(4)]
    [Read] public string File { get; set; } = "";
    [Read] public ModelEntity Model { get; set; }
}