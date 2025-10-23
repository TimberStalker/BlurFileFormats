namespace BlurFileFormats.SF2.DataSerializers;

public interface IDataSerializer
{
    void Serialize(object? data, SerializingState state);
    object? Deserialize(DeserializingState state);
}

public class StructSerializer : IDataSerializer
{

    public object? Deserialize(DeserializingState state)
    {
        throw new NotImplementedException();
    }

    public void Serialize(object? data, SerializingState state)
    {
        throw new NotImplementedException();
    }
}

public abstract class ValueSerializer<T> : IDataSerializer
{
    public void Serialize(object? data, SerializingState state)
    {
        if (data is T value)
        {
            SerializeValue(value, state);
        }
        else
        {
            throw new ArgumentException($"Data must be of type {typeof(T).Name}.");
        }
    }
    public object? Deserialize(DeserializingState state)
    {
        return DeserializeValue(state);
    }
    protected abstract void SerializeValue(T value, SerializingState state);
    protected abstract T DeserializeValue(DeserializingState state);
}

public class Int8Serializer : ValueSerializer<sbyte>
{
    protected override sbyte DeserializeValue(DeserializingState state) => state.Reader.ReadSByte();

    protected override void SerializeValue(sbyte value, SerializingState state) => state.Writer.Write(value);
}
public class Int16Serializer : ValueSerializer<short>
{
    protected override short DeserializeValue(DeserializingState state) => state.Reader.ReadInt16();

    protected override void SerializeValue(short value, SerializingState state) => state.Writer.Write(value);
}
public class Int32Serializer : ValueSerializer<int>
{
    protected override int DeserializeValue(DeserializingState state) => state.Reader.ReadInt32();

    protected override void SerializeValue(int value, SerializingState state) => state.Writer.Write(value);
}
public class Int64Serializer : ValueSerializer<long>
{
    protected override long DeserializeValue(DeserializingState state) => state.Reader.ReadInt64();

    protected override void SerializeValue(long value, SerializingState state) => state.Writer.Write(value);
}

public class UInt8Serializer : ValueSerializer<byte>
{
    protected override byte DeserializeValue(DeserializingState state) => state.Reader.ReadByte();

    protected override void SerializeValue(byte value, SerializingState state) => state.Writer.Write(value);
}
public class UInt16Serializer : ValueSerializer<ushort>
{
    protected override ushort DeserializeValue(DeserializingState state) => state.Reader.ReadUInt16();

    protected override void SerializeValue(ushort value, SerializingState state) => state.Writer.Write(value);
}
public class UInt32Serializer : ValueSerializer<uint>
{
    protected override uint DeserializeValue(DeserializingState state) => state.Reader.ReadUInt32();

    protected override void SerializeValue(uint value, SerializingState state) => state.Writer.Write(value);
}
public class UInt64Serializer : ValueSerializer<ulong>
{
    protected override ulong DeserializeValue(DeserializingState state) => state.Reader.ReadUInt64();

    protected override void SerializeValue(ulong value, SerializingState state) => state.Writer.Write(value);
}

public class Float16Serializer : ValueSerializer<Half>
{
    protected override Half DeserializeValue(DeserializingState state) => state.Reader.ReadHalf();

    protected override void SerializeValue(Half value, SerializingState state) => state.Writer.Write(value);
}

public class Float32Serializer : ValueSerializer<float>
{
    protected override float DeserializeValue(DeserializingState state) => state.Reader.ReadSingle();

    protected override void SerializeValue(float value, SerializingState state) => state.Writer.Write(value);
}
public class Float64Serializer : ValueSerializer<double>
{
    protected override double DeserializeValue(DeserializingState state) => state.Reader.ReadDouble();

    protected override void SerializeValue(double value, SerializingState state) => state.Writer.Write(value);
}