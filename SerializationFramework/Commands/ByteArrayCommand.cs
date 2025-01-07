using BlurFileFormats.SerializationFramework.Sources;

namespace BlurFileFormats.SerializationFramework.Commands;

public class ByteArrayCommand : ISerializerCommand
{
    public ISerializerSource LengthSource { get; }

    public ByteArrayCommand(ISerializerSource lengthSource)
    {
        LengthSource = lengthSource;
    }

    public object? Read(BinaryReader reader, SerializerInfo readInfo)
    {
        int length = Convert.ToInt32(LengthSource.Read(reader, readInfo));
        return reader.ReadBytes(length);
    }

    public void Write(BinaryWriter writer, SerializerInfo writeInfo, object? value)
    {
        if (value is not byte[] arr) throw new InvalidOperationException();
        int length = arr.Length;
        int trueLength = Convert.ToInt32(LengthSource.Write(writer, writeInfo, length));
        if (length != trueLength) throw new Exception("Array length does not match expected");

        writer.Write(arr);
    }
}
