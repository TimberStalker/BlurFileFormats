using BlurFileFormats.SerializationFramework.Attributes;
using System.Reflection;

namespace BlurFileFormats.SerializationFramework.Commands;

public class StructureCommand : ISerializerCommand
{
    public Type TargetType { get; }
    public SwitchAttribute? SwitchAttribute { get; }

    public Type? DefaultSwitchType { get; set; }
    public (object, Type)[] SwitchTargets { get; set; } = [];
    public StructureCommand(Type targetType, SwitchAttribute? switchAttribute)
    {
        TargetType = targetType;
        SwitchAttribute = switchAttribute;
        if (switchAttribute != null)
        {
            var subclasses = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.DefinedTypes).Where(t => t.IsAssignableTo(TargetType)).ToArray();
            List<(object, Type)> switchTargets = new(subclasses.Length);
            foreach (var subclass in subclasses)
            {
                if (subclass.GetCustomAttribute<DefaultAttribute>() is not null)
                {
                    System.Diagnostics.Debug.Assert(DefaultSwitchType is null, "");
                    DefaultSwitchType = subclass;
                }
                foreach (var targetAttribute in subclass.GetCustomAttributes<TargetAttribute>())
                {
                    switchTargets.Add((targetAttribute.Target, subclass));
                }
            }
            SwitchTargets = switchTargets.ToArray();
        }
    }


    public object? Read(BinaryReader reader, SerializerInfo readInfo)
    {
        Type targetType = GetTargetType(readInfo);
        object targetValue = Activator.CreateInstance(targetType)!;
        readInfo.ParentStack.Push(targetValue);
        System.Diagnostics.Debug.Indent();
        foreach (var command in DataSerializer.CreateStructureSerializationTargets(targetType))
        {
            command.Deserialize(reader, readInfo, targetValue);
        }
        System.Diagnostics.Debug.Unindent();
        readInfo.ParentStack.Pop();
        return targetValue;
    }

    public void Write(BinaryWriter writer, SerializerInfo writeInfo, object? value)
    {
        if (value is null) throw new Exception();
        foreach (var command in DataSerializer.CreateStructureSerializationTargets(value.GetType()))
        {
            command.Serialize(writer, writeInfo, value);
        }
    }

    private Type GetTargetType(SerializerInfo readInfo)
    {
        if (SwitchAttribute is not null)
        {
            var switchConditionValue = readInfo.GetValue(SwitchAttribute.Path);
            foreach (var (expected, type) in SwitchTargets)
            {
                if (expected.Equals(switchConditionValue))
                {
                    return type;
                }
            }
            if (DefaultSwitchType is null) throw new Exception($"{switchConditionValue} does not match any of the targets expected for {TargetType.Name}, and no default class is provided.");
            return DefaultSwitchType;
        }
        return TargetType;
    }
}
