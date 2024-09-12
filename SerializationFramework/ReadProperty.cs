using System;
using System.Data;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using BlurFileFormats.SerializationFramework.Attributes;
using BlurFileFormats.SerializationFramework.Command;
using BlurFileFormats.SerializationFramework.Command.Meta;
using BlurFileFormats.SerializationFramework.Command.Meta.Position;
using BlurFileFormats.SerializationFramework.Command.Numeric;
using BlurFileFormats.SerializationFramework.Command.Sequence.Strings;
using BlurFileFormats.SerializationFramework.Commands;
using BlurFileFormats.SerializationFramework.Commands.Meta.Position;
using BlurFileFormats.SerializationFramework.Commands.Sequence;
using BlurFileFormats.SerializationFramework.Commands.Structures;

namespace BlurFileFormats.SerializationFramework;

public class ReadProperty : IRead
{
    PropertyInfo Property { get; }

    public ReadProperty(PropertyInfo property)
    {
        Property = property;
    }
    public void Build(List<ISerializationCommand> commands)
    {
        if (Property.GetCustomAttribute<AlignAttribute>() is AlignAttribute a)
        {
            commands.Add(new AlignCommand(a));
        }

        var getAttr = Property.GetCustomAttribute<GetAttribute>();

        //if (getAttr is not null)
        //{
        //    var resultValue = tree.GetValue(getAttr);
        //    Property.SetValue(tree.CurrentObject, resultValue);
        //    return;
        //}


        var command = Build(Property.PropertyType, new PropertyAttributeProvider(Property));

        if (Property.SetMethod != null)
        {
            commands.Add(new PropertyCommand(Property, command));
        }
        else
        {
            commands.Add(new VerifyPropertyCommand(Property, (ISerializationReadCommand)command));
        }
    }
    public ISerializationCommand Build(Type type, IAttributeProvider attributeProvider)
    {
        if (type.IsArray)
        {
            var length = attributeProvider.GetAttribute<LengthAttribute>();
            Type elementType = type.GetElementType()!;
            var readItemCommand = Build(elementType, attributeProvider);

            ISerializationValueCommand<int> lengthCommand;
            if (length is not null)
            {
                if (length.Path is null)
                {
                    lengthCommand = new ConstantValueCommand<int>(length.Length);
                }
                else
                {
                    lengthCommand = new DataPathCommand<int>(length.Path);
                }
            }
            else
            {
                lengthCommand = new Int32Command();
            }
            return new ArrayCommand(elementType, lengthCommand, (ISerializationValueCommand)readItemCommand);
        }
        else if (type.IsEnum)
        {
            return Build(type.GetEnumUnderlyingType()!, attributeProvider);
        }
        if (type == typeof(string))
        {
            var encoding = attributeProvider.GetAttribute<EncodingAttribute>();
            var cstring = attributeProvider.GetAttribute<CStringAttribute>();
            var length = attributeProvider.GetAttribute<LengthAttribute>();

            ISerializationValueCommand<Encoding> encodingCommand = encoding is null ? new ConstantValueCommand<Encoding>(Encoding.ASCII) : new DataPathCommand<Encoding>(encoding.Path);

            if (cstring is null)
            {
                ISerializationValueCommand<int> lengthCommand;
                if (length is not null)
                {
                    if (length.Path is null)
                    {
                        lengthCommand = new ConstantValueCommand<int>(length.Length);
                    }
                    else
                    {
                        lengthCommand = new DataPathCommand<int>(length.Path);
                    }
                }
                else
                {
                    lengthCommand = new Int32Command();
                }
                return new StringCommand(encodingCommand, lengthCommand);
            }
            else
            {
                if (length is not null)
                {
                    ISerializationValueCommand<int> lengthCommand;
                    if (length.Path is null)
                    {
                        lengthCommand = new ConstantValueCommand<int>(length.Length);
                    }
                    else
                    {
                        lengthCommand = new DataPathCommand<int>(length.Path);
                    }
                    return new CStringLengthCommmand(encodingCommand, lengthCommand);
                }
                else
                {
                    return new CStringCommmand(encodingCommand);
                }
            }
        }
        else if (type == typeof(bool))
        {
            var length = attributeProvider.GetAttribute<LengthAttribute>();
            if (length is not null)
            {

                ISerializationValueCommand<int> lengthCommand;
                if (length is not null)
                {
                    if (length.Path is null)
                    {
                        lengthCommand = new ConstantValueCommand<int>(length.Length);
                    }
                    else
                    {
                        throw new NotSupportedException();
                        //lengthCommand = new DataPathCommand<int>(length.Path);
                    }
                }
                else
                {
                    throw new NotSupportedException();
                    //lengthCommand = new Int32Command();
                }
                return new LargeBoolCommand(lengthCommand);
            }
            else
            {
                return new BoolCommand();
            }
        }
        else if (type == typeof(byte))
        {
            var position = attributeProvider.GetAttribute<PositionAttribute>();
            return position == null ? new UInt8Command() : new UInt8PositionCommand();
        }
        else if (type == typeof(int))
        {
            var position = attributeProvider.GetAttribute<PositionAttribute>();
            return position == null ? new Int32Command() : new Int32PositionCommand();
        }
        else if (type == typeof(short))
        {
            var position = attributeProvider.GetAttribute<PositionAttribute>();
            return position == null ? new Int16Command() : new Int16PositionCommand();
        }
        else if (type == typeof(long))
        {
            var position = attributeProvider.GetAttribute<PositionAttribute>();
            return position == null ? new Int64Command() : new Int64PositionCommand();
        }
        else if (type == typeof(sbyte))
        {
            var position = attributeProvider.GetAttribute<PositionAttribute>();
            return position == null ? new Int8Command() : new Int8PositionCommand();
        }
        else if (type == typeof(uint))
        {
            var position = attributeProvider.GetAttribute<PositionAttribute>();
            return position == null ? new UInt32Command() : new UInt32PositionCommand();
        }
        else if (type == typeof(ushort))
        {
            var position = attributeProvider.GetAttribute<PositionAttribute>();
            return position == null ? new UInt16Command() : new UInt16PositionCommand();
        }
        else if (type == typeof(ulong))
        {
            var position = attributeProvider.GetAttribute<PositionAttribute>();
            return position == null ? new UInt64Command() : new UInt64PositionCommand();
        }
        else if (type == typeof(float))
        {
            return new Float32Command();
        }
        else if (type == typeof(double))
        {
            return new Float64Command();
        }
        else
        {
            var switchAttr = Property.GetCustomAttribute<SwitchAttribute>();

            if(switchAttr is null)
            {
                return DataSerializer.BuildCommand(type);
            }
            else
            {
                throw new NotImplementedException();
                //var target = tree.GetValue(switchAttr.Path);
                //
                //var objType =
                //    propertyType.Assembly.GetTypes()
                //    .Where(t => t.IsAssignableTo(propertyType) && !t.IsInterface && !t.IsAbstract)
                //    .Select(t => new { Type = t, Attr = t.GetCustomAttribute<TargetAttribute>() })
                //    .Where(t => t.Attr is not null)
                //    .FirstOrDefault(t => t.Attr!.Target.Equals(target))?.Type;
                //if (objType is null)
                //{
                //    objType = propertyType.Assembly.GetTypes().Where(t => t.IsAssignableTo(propertyType) && !t.IsInterface && !t.IsAbstract).FirstOrDefault(t => t.GetCustomAttribute<DefaultAttribute>() != null);
                //    if (objType is null)
                //    {
                //        throw new Exception($"No Matching type with target '{target}'");
                //    }
                //}
            }
        }
    }
    public interface IAttributeProvider
    {
        T? GetAttribute<T>() where T : Attribute;
    }
    public class PropertyAttributeProvider : IAttributeProvider
    {
        public PropertyInfo PropertyInfo { get; }
        public HashSet<Type> UsedTypes { get; } = [];
        public PropertyAttributeProvider(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
        }
        public T? GetAttribute<T>() where T : Attribute
        {
            if (!UsedTypes.Add(typeof(T))) return null;
            return PropertyInfo.GetCustomAttribute<T>();
        }
    }
}
