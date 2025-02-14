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
    [EndianSwitch]
    [Length(4)]
    [Read] public string Header => "2KAP";
    [Read] public int Version => 2;
    [Read] public uint Align { get; set; }
    [Read] public uint Dummy4 { get; set; }
    [Read] public uint Dummy5 { get; set; }
    [Read] public uint Dummy6 { get; set; }
    [Read] public uint Dummy7 { get; set; }
    [Read] public uint Dummy8 { get; set; }
    [Read] public uint Dummy9 { get; set; }
    [Read] public uint Dummy10 { get; set; }
    [Read] public uint Dummy11 { get; set; }
    [Read] public uint Dummy12 { get; set; }
    [Read] public uint Dummy13 { get; set; }
    [Read] public uint Dummy14 { get; set; }
    public string XorKey => @"VXo40j3@$%\\%`x";
    [Align(nameof(Align))]
    [Xor(nameof(XorKey))]
    [Read] public string Names { get; set; } = "";
}
