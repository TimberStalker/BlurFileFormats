using BlurFileFormats.SerializationFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
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
    [Read] public uint SectorSize { get; set; }
}
public class PakHeader
{
    [Length(20)]
    [Read] public byte[] Hash { get; set; }
    [Read] public byte Codec { get; set; }
    [Read] public byte FatEntryType { get; set; }
    public uint FatEntrySize()
    {
        uint v1 = (uint)((int)(FatEntryType ^ 1) >> 0x1f);
        uint v2 = (uint)(((int)(v1 - (v1 ^ FatEntryType ^ 1)) >> 0x1f & 0xffffffc8) + 0x38);
        if(FatEntryType == 0)
        {
            return 0x20;
        }
        return v2;
    }
    [Read] public char PathSeparator { get; set; }
    [Read] public byte Unused { get; set; }
    [Read] public uint CodecFlags { get; set; }
    [Read] public uint MetaStrOffset { get; set; }
    [Read] public ulong FileTime { get; set; }
}
enum FatEntryType : byte
{
    Normal,
    Secure
}