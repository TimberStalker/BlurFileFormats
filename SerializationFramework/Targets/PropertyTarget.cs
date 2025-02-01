using BlurFileFormats.SerializationFramework.Commands;
using System.Diagnostics;
using System.Reflection;

namespace BlurFileFormats.SerializationFramework.Targets;

public class PropertyTarget : ISerializerTarget
{
    public PropertyInfo TargetProperty { get; }
    public ISerializerCommand Command { get; }

    public PropertyTarget(PropertyInfo targetProperty, ISerializerCommand command)
    {
        TargetProperty = targetProperty;
        Command = command;
    }

    public void Deserialize(BinaryReader reader, SerializerInfo readInfo, object target)
    {
        //try
        //{
        //Debug.Write($"{TargetProperty.Name}: ");
        object? targetValue = Command.Read(reader, readInfo);
        // {targetValue}");
        if (TargetProperty.SetMethod is null)
        {
            var expectedValue = TargetProperty.GetValue(target);
            if (expectedValue?.Equals(targetValue) != true)
            {
                throw new Exception($"Values do not match for {TargetProperty.DeclaringType!.Name}.{TargetProperty.Name} Expected: {expectedValue} | Read: {targetValue}");
            }
        }
        else
        {
            TargetProperty.SetValue(target, targetValue);
        }
        //} catch(Exception ex)
        //{
        //    throw new Exception($"There was an error while reading {TargetProperty.DeclaringType!.Name}.{TargetProperty.Name}.", ex);
        //}
    }
    public void Serialize(BinaryWriter writer, SerializerInfo writeInfo, object target)
    {
        Command.Write(writer, writeInfo, TargetProperty.GetValue(target)!);
    }
}
