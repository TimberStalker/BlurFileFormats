using BlurFileFormats.SerializationFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.Audio.BafAudioFormat.Entities;
public class BafEntity
{
    [StringLength(4)]
    [CString]
    [Read] public string Magic => "BANK";
    [Read] public BafHeaderEntity Header { get; set; } = null!;
    [Length(nameof(Header.WavelCount))]
    [Read] public BafWavelEntity[] Wavels { get; set; } = [];
    [Length(nameof(Header.WavelCount))]
    [Read] public BafDataEntity[] Data { get; set; } = [];
}
public class BafHeaderEntity
{
    [Read] public int Size { get; set; }
    [Read] public int Version { get; set; }
    [Read] public int WavelCount { get; set; }
    [Read] public short Unknown2 { get; set; }

    [StringLength(34)]
    [CString]
    [Read] public string Name { get; set; } = "";
}
public class BafWavelEntity
{
    [StringLength(8)]
    [CString]
    [Read] public string Header => "WAVEL";
    [Read] public uint Codec { get; set; }
    [StringLength(0x20)]
    [CString]
    [Read] public string Name { get; set; } = "";
    [Read] public uint Start { get; set;  }
    [Read] public uint Size { get; set; }
    [Read] public float Unknown1 => 1;
    [Read] public int Unknown2 => 25;
    [Read] public int Version { get; set; }
    [Read] public uint SampleRate { get; set; }
    [Read] public uint SampleCount { get; set; }
    [Read] public bool Loops { get; set; }
    [Read] public byte Tracks { get; set; }
    [Read] public byte Pad { get; set; }
    [Read] public byte ChannelCount { get; set; }

}
public class BafDataEntity
{
    [StringLength(4)]
    [Read] public string Header => "DATA";
    [Read] public uint EntityLength { get; set; }
    public uint DataLength => EntityLength - 8;
    [Length(nameof(DataLength))]
    [Read] public byte[] Bytes { get; set; } = [];
}
