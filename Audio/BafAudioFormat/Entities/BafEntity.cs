using BlurFileFormats.SerializationFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.Audio.BafAudioFormat.Entities;
public class BafEntity
{
    [Read] public string Magic => "DATA";
    [Read] public BafHeaderEntity Header { get; set; } = null!;
}
public class BafHeaderEntity
{
    [Read] public int Size { get; set; }
    [Read] public int Unknown { get; set; }
    [Read] public int WavelCount { get; set; }
    [Read] public short Unknown2 { get; set; }

    [StringLength(34)]
    [CString]
    [Read] public string Name { get; set; } = "";
    [Length(nameof(WavelCount))]
    [Read] public BafWavelEntity[] Wavels { get; set; } = [];
    [Length(nameof(WavelCount))]
    [Read] public BafDataEntity[] Data { get; set; } = [];
}
public class BafWavelEntity
{
    [StringLength(8)]
    [CString]
    [Read] public string Header => "WAVEL";
    [Read] public int Unknown1 { get; set; }
    [StringLength(0x20)]
    [CString]
    [Read] public string Name { get; set; } = "";
    [Read] public int Start { get; set;  }
    [Read] public int Size { get; set; }
    public int End  => Start + Size;
    [Read] public float Length { get; set; }
    [Read] public int Unk5 { get; set; }
    [Read] public int Unk6 { get; set; }
    [Read] public int Frequency { get; set; }
    [Read] public int Unk8 { get; set; }
    [Read] public int Unk9 { get; set; }
}
public class BafDataEntity
{
    [Read] public string Header => "DATA";
    [Read] public int EntityLength { get; set; }
    public int DataLength => EntityLength - 8;
    [StringLength(nameof(DataLength))]
    [Read] public byte[] Data { get; set; } = [];
}
