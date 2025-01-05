using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlurFileFormats.SerializationFramework;
using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Localizations.Entities;
public class LocEntity
{
    [Read] public string Format => "OLTX";
    [Read] public int Version => 1;
    [Read] int HeaderSize { get; set; }
    [Read] int LanguagesStart { get; set; }

    [Read] int UnknownC { get; set; }
    [Read] int LanguageCount { get; set; }
    [Read] int Unknown84 { get; set; }

    [Length(nameof(LanguageCount))]
    [Read] LanguageEntity[] Languages { get; set; }

    [Read] int StringCount { get; set; }
    [Read] int StringHeaderByteSize { get; set; }

    [Length(nameof (StringCount))]
    [Read] HeaderEntity[] Headers { get; set; }
    [Length(nameof (StringCount))]
    [Read] string[] HeaderTexts { get; set; }
}
public class LanguageContentEntity
{
    [Read] public int StringCount { get; set; }
    [Read] public int ByteSize { get; set; }

    [Length(nameof(StringCount))]
    [Read] HeaderEntity[] Headers { get; set; }
    [Length(nameof(StringCount))]
    [Encoding("utf-8")]
    [CString]
    [Read] string[] HeaderTexts { get; set; }
}
public class HeaderEntity
{
    public uint Id { get; set; }
    public uint Position { get; set; }
}
public class LanguageEntity
{
    [StringLength(4)]
    [Read] public string Name { get; set; }

    [Read] int Unknown1 { get; set; }
    [Read] int Unknown2 { get; set; }
    [Read] int Unknown3 { get; set; }

    [Read] int Start { get; set; }
    [Read] int End { get; set; }
}