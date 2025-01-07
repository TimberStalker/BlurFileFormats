using BlurFileFormats.SerializationFramework.Attributes;
using BlurFileFormats.XtFlask.Values;
using System.Reflection;
using System.Reflection.PortableExecutable;

namespace BlurFileFormats.SerializationFramework;
public interface ISerializerCommand
{
    object? Read(BinaryReader reader, SerializerInfo readInfo);
    void Write(BinaryWriter writer, SerializerInfo writeInfo, object? value);
}
public abstract class AtomicValueCommand<T> : ISerializerCommand
{
    object? ISerializerCommand.Read(BinaryReader reader, SerializerInfo readInfo) => Read(reader, readInfo);
    void ISerializerCommand.Write(BinaryWriter writer, SerializerInfo writeInfo, object? value) => Write(writer, writeInfo, (T)value!);
    public abstract T Read(BinaryReader reader, SerializerInfo readInfo);
    public abstract void Write(BinaryWriter writer, SerializerInfo writeInfo, T value);
}
public class PositionCommand : ISerializerCommand
{
    public Func<long, object> MapPosition { get; }

    public PositionCommand(Func<long, object> mapPosition)
    {
        MapPosition = mapPosition;
    }

    public object? Read(BinaryReader reader, SerializerInfo readInfo) => MapPosition(reader.BaseStream.Position - readInfo.CurrentAnchor);

    public void Write(BinaryWriter writer, SerializerInfo writeInfo, object? value) { }
}
public class ValueCommand : ISerializerCommand
{
    public Func<BinaryReader, SerializerInfo, object?>? ReadAction { get; set; }
    public Action<BinaryWriter, SerializerInfo, object?>? WriteAction { get; set; }

    public object? Read(BinaryReader reader, SerializerInfo readInfo) => ReadAction?.Invoke(reader, readInfo);

    public void Write(BinaryWriter writer, SerializerInfo writeInfo, object? value) => WriteAction?.Invoke(writer, writeInfo, value);
}
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
            array.SetValue(ChildCommand.Read(reader, readInfo), i);
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
        foreach (var command in DataSerializer.CreateStructureSerializationTargets(targetType))
        {
            command.Deserialize(reader, readInfo, targetValue);
        }
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
public interface ISerializerTarget
{
    public void Deserialize(BinaryReader reader, SerializerInfo readInfo, object target);
    public void Serialize(BinaryWriter writer, SerializerInfo writeInfo, object target);
}

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
            object? targetValue = Command.Read(reader, readInfo);
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

public class AnchorTarget : ISerializerTarget
{
    public ISerializerTarget InternalTarget { get; }

    public AnchorTarget(ISerializerTarget internalTarget)
    {
        InternalTarget = internalTarget;
    }

    public void Deserialize(BinaryReader reader, SerializerInfo readInfo, object target)
    {
        readInfo.AnchorStack.Push((int)reader.BaseStream.Position);
        InternalTarget.Deserialize(reader, readInfo, target);
        readInfo.AnchorStack.Pop();
    }

    public void Serialize(BinaryWriter writer, SerializerInfo writeInfo, object target)
    {
        writeInfo.AnchorStack.Push((int)writer.BaseStream.Position);
        InternalTarget.Serialize(writer, writeInfo, target);
        writeInfo.AnchorStack.Pop();
    }
}
public class AlignTarget : ISerializerTarget
{
    public ISerializerTarget InternalTarget { get; }
    public ISerializerSource AlignSource { get; }

    public AlignTarget(ISerializerTarget internalTarget, ISerializerSource alignSource)
    {
        InternalTarget = internalTarget;
        AlignSource = alignSource;
    }

    public void Deserialize(BinaryReader reader, SerializerInfo readInfo, object target)
    {
        var position = reader.BaseStream.Position - readInfo.CurrentAnchor;
        var align = Convert.ToInt32(AlignSource.Read(reader, readInfo));
        if (position % align != 0)
        {

            int paddingCount = align - ((int)position % align); 
            for (int i = 0; i < paddingCount; i++)
            {
                var temp = reader.ReadByte();
            }
        }
        InternalTarget.Deserialize(reader, readInfo, target);
    }

    public void Serialize(BinaryWriter writer, SerializerInfo writeInfo, object target)
    {
        var position = writer.BaseStream.Position - writeInfo.CurrentAnchor;
        var align = Convert.ToInt32(AlignSource.Write(writer, writeInfo, null));
        if (position % align != 0)
        {
            int paddingCount = align - ((int)position % align);
            for (int i = 0; i < paddingCount; i++)
            {
                writer.Write((byte)0);
            }
        }
        InternalTarget.Serialize(writer, writeInfo, target);
    }
}
public interface ISerializerSource
{
    object? Read(BinaryReader reader, SerializerInfo readInfo);
    object? Write(BinaryWriter writer, SerializerInfo writeInfo, object? value);
}
public class SerializerCommandSource : ISerializerSource
{
    ISerializerCommand Command { get; }

    public SerializerCommandSource(ISerializerCommand command)
    {
        Command = command;
    }

    public object? Read(BinaryReader reader, SerializerInfo readInfo)
    {
        return Command.Read(reader, readInfo);
    }

    public object? Write(BinaryWriter writer, SerializerInfo writeInfo, object? value)
    {
        Command.Write(writer, writeInfo, value);
        return value;
    }
}
public class SerilaizerConstantSource<T> : ISerializerSource
{
    public T Value { get; }
    public SerilaizerConstantSource(T value)
    {
        Value = value;
    }


    public object? Read(BinaryReader reader, SerializerInfo readInfo) => Value;

    public object? Write(BinaryWriter writer, SerializerInfo writeInfo, object? value) => Value;

}
public class SerializerDynamicSource : ISerializerSource
{
    public string Path { get; }
    public SerializerDynamicSource(string path)
    {
        Path = path;
    }

    public object? Read(BinaryReader reader, SerializerInfo readInfo) => readInfo.GetValue(Path);

    public object? Write(BinaryWriter writer, SerializerInfo writeInfo, object? value)
    {
        if(value is not null)
        {
            writeInfo.SetValue(Path, value);
            return value;
        }
        return writeInfo.GetValue(Path);
    }
}