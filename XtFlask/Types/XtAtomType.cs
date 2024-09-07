using BlurFileFormats.XtFlask.Entities;
using BlurFileFormats.XtFlask.Values;
using System.Diagnostics;

namespace BlurFileFormats.XtFlask.Types;

public static class XtAtomType
{
    public static IXtType CreateType(AtomType atomType, string name) => atomType switch
    {
        AtomType.Bool => new XtBoolType(name),
        AtomType.Int8 => new XtInt8Type(name),
        AtomType.Int16 => new XtInt16Type(name),
        AtomType.Int32 => new XtInt32Type(name),
        AtomType.Int64 => new XtInt64Type(name),
        AtomType.Unsigned8 => new XtUInt8Type(name),
        AtomType.Unsigned16 => new XtUInt16Type(name),
        AtomType.Unsigned32 => new XtUInt32Type(name),
        AtomType.Unsigned64 => new XtUInt64Type(name),
        AtomType.Float32 => new XtFloat32Type(name),
        AtomType.Float64 => new XtFloat64Type(name),
        AtomType.StringSz8 => new XtSz8Type(name),
        AtomType.StringSz16 => new XtSz16Type(name),
        AtomType.LocId => new XtLocIdType(name),
        _ => throw new UnreachableException()
    };
}

public abstract class XtAtomType<T> : IXtType where T : notnull
{
    public abstract AtomType Atom { get; }
    public string Name { get; }

    public XtAtomType(string name)
    {
        Name = name;
    }
    public abstract T Read(BinaryReader reader, ValueResolver resolver);
    public abstract T DefaultValue();


    IXtValue IXtType.CreateValue() => CreateValue();

    public XtAtomValue<T> CreateValue() =>
        new (this, DefaultValue());


    IXtValue IXtType.ReadValue(BinaryReader reader, ValueResolver resolver) => ReadValue(reader, resolver);
    public XtAtomValue<T> ReadValue(BinaryReader reader, ValueResolver resolver)
    {
        var value = CreateValue();
        value.Value = Read(reader, resolver);
        return value;
    }
}
public sealed class XtBoolType : XtAtomType<bool>
{
    public override AtomType Atom => AtomType.Bool;
    public XtBoolType(string name) : base(name) { }
    public override bool Read(BinaryReader reader, ValueResolver resolver) => reader.ReadInt32() > 0;

    public override bool DefaultValue() => false;
}

public sealed class XtInt8Type : XtAtomType<sbyte>
{
    public override AtomType Atom => AtomType.Int8;
    public XtInt8Type(string name) : base(name) { }
    public override sbyte Read(BinaryReader reader, ValueResolver resolver) => unchecked((sbyte)reader.ReadUInt32());

    public override sbyte DefaultValue() => 0;
}

public sealed class XtInt16Type : XtAtomType<short>
{
    public override AtomType Atom => AtomType.Int16;
    public XtInt16Type(string name) : base(name) { }
    public override short Read(BinaryReader reader, ValueResolver resolver) => unchecked((short)reader.ReadUInt32());

    public override short DefaultValue() => 0;
}

public sealed class XtInt32Type : XtAtomType<int>
{
    public override AtomType Atom => AtomType.Int32;
    public XtInt32Type(string name) : base(name) { }
    public override int Read(BinaryReader reader, ValueResolver resolver) => reader.ReadInt32();

    public override int DefaultValue() => 0;
}

public sealed class XtInt64Type : XtAtomType<long>
{
    public override AtomType Atom => AtomType.Int64;
    public XtInt64Type(string name) : base(name) { }
    public override long Read(BinaryReader reader, ValueResolver resolver) => reader.ReadInt64();

    public override long DefaultValue() => 0;
}

public sealed class XtUInt8Type : XtAtomType<byte>
{
    public override AtomType Atom => AtomType.Unsigned8;
    public XtUInt8Type(string name) : base(name) { }
    public override byte Read(BinaryReader reader, ValueResolver resolver) => unchecked((byte)reader.ReadUInt32());

    public override byte DefaultValue() => 0;
}

public sealed class XtUInt16Type : XtAtomType<ushort>
{
    public override AtomType Atom => AtomType.Unsigned16;
    public XtUInt16Type(string name) : base(name) { }
    public override ushort Read(BinaryReader reader, ValueResolver resolver) => unchecked((ushort)reader.ReadUInt32());

    public override ushort DefaultValue() => 0;
}

public sealed class XtUInt32Type : XtAtomType<uint>
{
    public override AtomType Atom => AtomType.Unsigned32;
    public XtUInt32Type(string name) : base(name) { }
    public override uint Read(BinaryReader reader, ValueResolver resolver) => reader.ReadUInt32();

    public override uint DefaultValue() => 0;
}

public sealed class XtUInt64Type : XtAtomType<ulong>
{
    public override AtomType Atom => AtomType.Unsigned64;
    public XtUInt64Type(string name) : base(name) { }
    public override ulong Read(BinaryReader reader, ValueResolver resolver) => reader.ReadUInt64();

    public override ulong DefaultValue() => 0;
}

public sealed class XtFloat32Type : XtAtomType<float>
{
    public override AtomType Atom => AtomType.Float32;
    public XtFloat32Type(string name) : base(name) { }
    public override float Read(BinaryReader reader, ValueResolver resolver) => reader.ReadSingle();

    public override float DefaultValue() => 0;
}

public sealed class XtFloat64Type : XtAtomType<double>
{
    public override AtomType Atom => AtomType.Float64;
    public XtFloat64Type(string name) : base(name) { }
    public override double Read(BinaryReader reader, ValueResolver resolver) => reader.ReadDouble();

    public override double DefaultValue() => 0;
}
public sealed class XtSz8Type : XtAtomType<string>
{
    public override AtomType Atom => AtomType.StringSz8;
    public XtSz8Type(string name) : base(name) { }
    public override string Read(BinaryReader reader, ValueResolver resolver) => resolver.GetString(reader.ReadUInt32());

    public override string DefaultValue() => "";
}

public sealed class XtSz16Type : XtAtomType<string>
{
    public override AtomType Atom => AtomType.StringSz16;
    public XtSz16Type(string name) : base(name) { }
    public override string Read(BinaryReader reader, ValueResolver resolver) => resolver.GetString(reader.ReadUInt32());

    public override string DefaultValue() => "";
}

public sealed class XtLocIdType : XtAtomType<uint>
{
    public override AtomType Atom => AtomType.LocId;
    public XtLocIdType(string name) : base(name) { }
    public override uint Read(BinaryReader reader, ValueResolver resolver) => reader.ReadUInt32();

    public override uint DefaultValue() => 0;
}
