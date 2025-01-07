using BlurFileFormats.SerializationFramework.Sources;
using System.Diagnostics;

namespace BlurFileFormats.SerializationFramework.Commands;

public class ArrayCommand : ISerializerCommand
{
    public Type TargetType { get; }
    public ISerializerSource LengthSource { get; }
    public ISerializerCommand ChildCommand { get; }

    public ArrayCommand(Type targetType, ISerializerSource lengthSource, ISerializerCommand childCommand)
    {
        TargetType = targetType;
        LengthSource = lengthSource;
        ChildCommand = childCommand;
    }

    public object? Read(BinaryReader reader, SerializerInfo readInfo)
    {
        int length = Convert.ToInt32(LengthSource.Read(reader, readInfo));


        var array = Array.CreateInstance(TargetType, length);
        for (int i = 0; i < length; i++)
        {
            Debug.WriteLine($"[{i}]:");
            Debug.Indent();
            array.SetValue(ChildCommand.Read(reader, readInfo), i);
            Debug.Unindent();
        }
        return array;
    }

    public void Write(BinaryWriter writer, SerializerInfo writeInfo, object? value)
    {
        if (value is not Array arr) throw new InvalidOperationException();
        int length = arr.Length;
        int trueLength = Convert.ToInt32(LengthSource.Write(writer, writeInfo, length));
        if (length != trueLength) throw new Exception("Array length does not match expected");
        for (int i = 0; i < length; i++)
        {
            ChildCommand.Write(writer, writeInfo, arr.GetValue(i));
        }
    }
}
