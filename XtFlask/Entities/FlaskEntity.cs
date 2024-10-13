using BlurFileFormats.SerializationFramework.Attributes;
using BlurFileFormats.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.XtFlask.Entities;
public class FlaskEntity
{
    [EndianSwitch]
    [Length(4)]
    [Read] public string FileType => "KSLF";
    [Read] public int Version => 2;
    [AllowNull]
    [Read] public FlaskBlobEntity TypesBlob { get; set; }
    [AllowNull]
    [Read] public FlaskBlobEntity BasesBlob { get; set; }
    [AllowNull]
    [Read] public FlaskBlobEntity FieldsBlob { get; set; }
    [AllowNull]
    [Read] public FlaskBlobEntity StringsBlob { get; set; }
    [AllowNull]
    [Read] public FlaskBlobEntity RefsBlob { get; set; }
    [AllowNull]
    [Read] public FlaskBlobEntity RecordsBlob { get; set; }
    [AllowNull]
    [Read] public FlaskBlobEntity ComponentsBlob { get; set; }
    [AllowNull]
    [Read] public FlaskBlobEntity DataBlob { get; set; }

    [Length(nameof(TypesBlob.Count))]
    [AllowNull]
    [Read] public FlaskTypeEntity[] Types { get; set; }

    [Length(nameof(BasesBlob.Count))]
    [AllowNull]
    [Read] public FlaskBaseEntity[] Bases { get; set; }

    [Length(nameof(FieldsBlob.Count))]
    [AllowNull]
    [Read] public FlaskFieldEntity[] Fields { get; set; }
    [IgnorePrint]
    public Encoding Encoding { get; } = new FlaskEncoding();
    [Length(nameof(StringsBlob.Count))]
    [Encoding(nameof(Encoding))]
    [AllowNull]
    [Read] public string Strings { get; set; }

    [Length(nameof(RefsBlob.Count))]
    [AllowNull]
    [Read] public FlaskRefEntity[] Refs { get; set; }

    [Length(nameof(RecordsBlob.Count))]
    [AllowNull]
    [Read] public FlaskRecordEntity[] Records { get; set; }

    [Length(nameof(ComponentsBlob.Count))]
    [AllowNull]
    [Read] public FlaskComponentEntity[] Components { get; set; }

    [Align(4)]
    [Read] public int Align { get; set; }

    [Length(nameof(DataBlob.Count))]
    [AllowNull]
    [Read] public byte[] Data { get; set; }
}
public class FlaskComponentEntity
{
    [Read] public ushort Type { get; set; }
    [Read] public ushort Count { get; set; }
    [Read] public ComponentBehavior Behavior { get; set; }
}
public enum ComponentBehavior : ushort
{
    Object,
    Ponter,
    Handle,
    ComponentBehaviorCount,
}
public class FlaskRecordEntity
{
    [Read] public uint Ref { get; set; }
    [Read] public uint ComponentCount { get; set; }
    [Read] public uint DataBytes { get; set; }
    [Read] public uint StringBytes { get; set; }
}
[DebuggerDisplay("{Record}-{Type} [{Id}]")]
public class FlaskRefEntity
{
    [Read] public uint Id { get; set; }
    [Read] public ushort Type { get; set; }
    [Read] public ushort Record { get; set; }
}
public class FlaskTypeEntity
{
    [Read] public ushort Name { get; set; }
    [Read] public ushort FieldCount { get; set; }
    [Read] public ushort BaseCount { get; set; }
    [Read] public TypeBehavior Behavior { get; set; }
    [Read] public AtomType Atom { get; set; }
    [Read] public ushort Size { get; set; }
}
public class FlaskFieldEntity
{
    [Read] public short Name { get; set; }
    [Read] public short Type { get; set; }
    [Read] public short Offset { get; set; }
    [Read] public FieldBehavior Behavior { get; set; }
    [Read] public bool Array { get; set; }
}
public enum TypeBehavior : ushort
{
    Atom,
    Enum,
    Flags,
    Struct,
    TypeBehaviorCount
}
public enum FieldBehavior : byte
{
    Atom,
    Enum,
    Flags,
    Struct,
    Pointer,
    Handle,
    Label,
    BehaviorCount,
    ImplInfer,
    ImplEnumLabel,
    ImplFlagLabel,
    ImplAtom,
    ImplBase,
    ImplCallback,
    ImplArray,
    ImplWeak,
    ImplFlagStorage,
    ImplEnd,
}
public enum AtomType : ushort
{
    NotAtom,
    Bool,
    Int8,
    Int16,
    Int32,
    Int64,
    Unsigned8,
    Unsigned16,
    Unsigned32,
    Unsigned64,
    Float32,
    Float64,
    StringSz8,
    StringSz16,
    LocId,
    AtomTypeCount,
}
