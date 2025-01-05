using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework;
public interface ISerializerCommand
{
    void Read(BinaryReader reader, ReadInfo readInfo, object value);
    void Write(BinaryWriter writer, WriteInfo writeInfo, object value);
}

public class PropertySerializerCommand : ISerializerCommand
{
    public PropertyInfo TargetProperty { get; set; }
    public Func<BinaryReader, ReadInfo, object?>? ReadAction { get; set; }
    public Action<BinaryWriter, WriteInfo, object?>? WriteAction { get; set; }
    public PropertySerializerCommand(PropertyInfo targetProperty)
    {
        TargetProperty = targetProperty;
    }
    public void Read(BinaryReader reader, ReadInfo readInfo, object value)
    {
        if (ReadAction is null) return;
        TargetProperty.SetValue(value, ReadAction(reader, readInfo));
    }

    public void Write(BinaryWriter writer, WriteInfo writeInfo, object value)
    {
        if (WriteAction is null) return;
        WriteAction(writer, writeInfo, TargetProperty.GetValue(value));
    }
}