using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlurFileFormats.FlaskReflection.Entities;

namespace BlurFileFormats.XtDb;

public class XtDatabase
{
    public Dictionary<uint, XtRecord> Records { get; } = [];
}
public class XtRecord
{
    public uint Handle { get; }
    public XtValue? Value { get; set; }
    public XtRecord(uint handle, XtValue value)
    {
        Handle = handle;
        Value = value;
    }
}
public interface XtType
{
    public string Name { get; set; }
    public int Size { get; }

    public XtValue CreateDefault();
}
public enum XtReferenceFor
{
    Value,
    Array,
    Record
}
public class XtNull
{
    public static readonly XtNull Instance = new XtNull();
    private XtNull() { }
}
public class XtAtomType : XtType
{
    public string Name { get; set; }
    public AtomType AtomType { get; }
    public int Size => 4;
    public XtAtomType(string name, AtomType atomType)
    {
        Name = name;
        AtomType = atomType;
    }
    public XtValue CreateDefault() => new XtValue(this, 0u);
}
public class XtReferenceType : XtType
{
    public string Name { get; set; }
    public XtType ReferencedType { get; }
    public XtReferenceFor For { get; set; }
    public int Size => For == XtReferenceFor.Array ? 8 : 4;
    public XtReferenceType(string name, XtType referencedType, XtReferenceFor referenceFor)
    {
        Name = name;
        ReferencedType = referencedType;
        For = referenceFor;
    }

    public XtValue CreateDefault() => new XtValue(this, XtNull.Instance);
}
/// <summary>
/// Value is <see cref="List{T}"/>
/// </summary>
public class XtStructType : XtType
{
    public string Name { get; set; }
    public List<XtStructType> Bases { get; } = [];
    public List<XtField> Fields { get; } = [];

    public IEnumerable<XtField> AllFields => Bases.SelectMany(b => b.AllFields).Concat(Fields);

    public int Size => Fields.Sum(f => f.Type.Size);
    public XtStructType(string name)
    {
        Name = name;
    }
    public XtValue CreateDefault() => new XtValue(this, AllFields.Select(f => f.Type.CreateDefault()).ToList());
}
public class XtField
{
    public string Name { get; set; }
    public XtStructType Base { get; }
    public XtType Type { get; }
    public XtField(string name, XtStructType baseType, XtType type)
    {
        Name = name;
        Type = type;
        Base = baseType;
    }

}
public readonly ref struct XtValueSlot
{
    private readonly Func<XtValue> getValue;
    private readonly Action<XtValue> setValue;

    public XtValue Parent { get; }
    public object Key { get; }
    public readonly XtValue Value {
        get => getValue();
        set => setValue(value);
    }
    public XtValueSlot(XtValue parent, object key, Func<XtValue> getValue, Action<XtValue> setValue)
    {
        Parent = parent;
        Key = key;
        this.getValue = getValue;
        this.setValue = setValue;
    }
}
public record struct XtValue(XtType Type, object Value);