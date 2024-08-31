using BlurFileFormats.FlaskReflection.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.FlaskReflection.Values;
public interface IPrimitiveValue : IDataValue { }
public class BoolValue : IPrimitiveValue
{
    public PrimitiveType Type { get; }
    IDataType IDataValue.Type => Type;
    public bool Value { get; set; }
    object IDataValue.Value { get => Value; set => Value = (bool)value; }
    public BoolValue(PrimitiveType type)
    {
        Type = type;
    }
}
public class ByteValue : IPrimitiveValue
{
    public PrimitiveType Type { get; }
    IDataType IDataValue.Type => Type;
    public byte Value { get; set; }
    object IDataValue.Value { get => Value; set => Value = (byte)value; }
    public ByteValue(PrimitiveType type)
    {
        Type = type;
    }
}
public class ShortValue : IPrimitiveValue
{
    public PrimitiveType Type { get; }
    IDataType IDataValue.Type => Type;
    public short Value { get; set; }
    object IDataValue.Value { get => Value; set => Value = (short)value; }
    public ShortValue(PrimitiveType type)
    {
        Type = type;
    }
}
public class IntValue : IPrimitiveValue
{
    public PrimitiveType Type { get; }
    IDataType IDataValue.Type => Type;
    public int Value { get; set; }
    object IDataValue.Value { get => Value; set => Value = (int)value; }
    public IntValue(PrimitiveType type)
    {
        Type = type;
    }
}
public class LongValue : IPrimitiveValue
{
    public PrimitiveType Type { get; }
    IDataType IDataValue.Type => Type;
    public long Value { get; set; }
    object IDataValue.Value { get => Value; set => Value = (long)value; }
    public LongValue(PrimitiveType type)
    {
        Type = type;
    }
}
public class SByteValue : IPrimitiveValue
{
    public PrimitiveType Type { get; }
    IDataType IDataValue.Type => Type;
    public sbyte Value { get; set; }
    object IDataValue.Value { get => Value; set => Value = (sbyte)value; }
    public SByteValue(PrimitiveType type)
    {
        Type = type;
    }
}
public class UShortValue : IPrimitiveValue
{
    public PrimitiveType Type { get; }
    IDataType IDataValue.Type => Type;
    public ushort Value { get; set; }
    object IDataValue.Value { get => Value; set => Value = (ushort)value; }
    public UShortValue(PrimitiveType type)
    {
        Type = type;
    }
}

public class UIntValue : IPrimitiveValue
{
    public PrimitiveType Type { get; }
    IDataType IDataValue.Type => Type;
    public uint Value { get; set; }
    object IDataValue.Value { get => Value; set => Value = (uint)value; }
    public UIntValue(PrimitiveType type)
    {
        Type = type;
    }
}

public class ULongValue : IPrimitiveValue
{
    public PrimitiveType Type { get; }
    IDataType IDataValue.Type => Type;
    public ulong Value { get; set; }
    object IDataValue.Value { get => Value; set => Value = (ulong)value; }
    public ULongValue(PrimitiveType type)
    {
        Type = type;
    }
}

public class FloatValue : IPrimitiveValue
{
    public PrimitiveType Type { get; }
    IDataType IDataValue.Type => Type;
    public float Value { get; set; }
    object IDataValue.Value { get => Value; set => Value = (float)value; }
    public FloatValue(PrimitiveType type)
    {
        Type = type;
    }
}

public class DoubleValue : IPrimitiveValue
{
    public PrimitiveType Type { get; }
    IDataType IDataValue.Type => Type;
    public double Value { get; set; }
    object IDataValue.Value { get => Value; set => Value = (double)value; }
    public DoubleValue(PrimitiveType type)
    {
        Type = type;
    }
}

public class StringValue : IPrimitiveValue
{
    public PrimitiveType Type { get; }
    IDataType IDataValue.Type => Type;
    public string Value { get; set; } = "";
    object IDataValue.Value { get => Value; set => Value = (string)value; }
    public StringValue(PrimitiveType type)
    {
        Type = type;
    }
}

public class LocalizationValue : IPrimitiveValue
{
    public PrimitiveType Type { get; }
    IDataType IDataValue.Type => Type;
    public uint Value { get; set; }
    object IDataValue.Value { get => Value; set => Value = (uint)value; }
    public LocalizationValue(PrimitiveType type)
    {
        Type = type;
    }
}
