using System;
using System.ComponentModel;
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
    
    public int Order => Property.GetCustomAttribute<ReadAttribute>() is ReadAttribute r ? r.Order : -1;

    public ReadProperty(PropertyInfo property)
    {
        Property = property;
    }
    public void Build(List<ISerializerPropertyCommand> commands, TypeTree tree)
    {
        if(Property.GetCustomAttribute<ReadAttribute>() is null)
        {
            commands.Add(new MetaPropertyCommand(Property, new ConstantExpressionCommand(v => Property.GetValue(v)!)));
            return;
        }
        var getAttr = Property.GetCustomAttribute<GetAttribute>();

        //if (getAttr is not null)
        //{
        //    var resultValue = tree.GetValue(getAttr);
        //    Property.SetValue(tree.CurrentObject, resultValue);
        //    return;
        //}


        var command = Build(Property.PropertyType, tree, new PropertyAttributeProvider(Property));
        tree.Add(Property, command);
        if (Property.SetMethod != null)
        {
            if(command is ISerializeCommand s)
            {
                commands.Add(new PropertyCommand(Property, s));
            }
            else if(command is IGetCommand g)
            {
                commands.Add(new MetaPropertyCommand(Property, g));
            }
        }
        else
        {
            if (command is ISerializeCommand s)
            {
                commands.Add(new PropertyCommand(Property, s));
            }
            else if (command is IGetCommand g)
            {
                commands.Add(new MetaPropertyCommand(Property, g));
            }
            //commands.Add(new ConstantPropertyCommand(Property, command));
        }
    }
    public ISerializerCommand Build(Type type, TypeTree tree, IAttributeProvider attributeProvider)
    {
        if (type.IsArray)
        {
            var length = attributeProvider.GetAttribute<LengthAttribute>();
            Type elementType = type.GetElementType()!;
            var readItemCommand = Build(elementType, tree, attributeProvider);

            ISerializerCommand<int> lengthCommand;
            if (length is not null)
            {
                if (length.Path is null)
                {
                    lengthCommand = new ConstantValueCommand<int>(length.Length);
                }
                else
                {
                    lengthCommand = DataPathCommand<int>.Create(length.Path, tree);
                }
            }
            else
            {
                lengthCommand = new Int32Command();
            }
            return new ArrayCommand(elementType, new SerializerCommandReference<int>(lengthCommand), (ISerializeCommand)readItemCommand);
        }
        else if (type.IsEnum)
        {
            return Build(type.GetEnumUnderlyingType()!, tree, attributeProvider);
        }
        if (type == typeof(string))
        {
            var encoding = attributeProvider.GetAttribute<EncodingAttribute>();
            var cstring = attributeProvider.GetAttribute<CStringAttribute>();
            var length = attributeProvider.GetAttribute<LengthAttribute>();

            ISerializerCommand<Encoding> encodingCommand = encoding is null ? new ConstantValueCommand<Encoding>(Encoding.ASCII) : DataPathCommand<Encoding>.Create(encoding.Path, tree);
            if (cstring is null)
            {
                ISerializerCommand<int> lengthCommand;
                if (length is not null)
                {
                    if (length.Path is null)
                    {
                        lengthCommand = new ConstantValueCommand<int>(length.Length);
                    }
                    else
                    {
                        lengthCommand = DataPathCommand<int>.Create(length.Path, tree);
                    }
                }
                else
                {
                    lengthCommand = new Int32Command();
                }
                return new StringCommand(new SerializerCommandReference<Encoding>(encodingCommand), new SerializerCommandReference<int>(lengthCommand));
            }
            else
            {
                if (length is not null)
                {
                    ISerializerCommand<int> lengthCommand;
                    if (length.Path is null)
                    {
                        lengthCommand = new ConstantValueCommand<int>(length.Length);
                    }
                    else
                    {
                        lengthCommand = DataPathCommand<int>.Create(length.Path, tree);
                    }
                    return new CStringLengthCommmand(new SerializerCommandReference<Encoding>(encodingCommand), new SerializerCommandReference<int>(lengthCommand));
                }
                else
                {
                    return new CStringCommmand(new SerializerCommandReference<Encoding>(encodingCommand));
                }
            }
        }
        else if (type == typeof(bool))
        {
            var length = attributeProvider.GetAttribute<LengthAttribute>();
            if (length is not null)
            {

                ISerializerCommand<int> lengthCommand;
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
                return new LargeBoolCommand(new SerializerCommandReference<int>(lengthCommand));
            }
            else
            {
                return new BoolCommand();
            }
        }
        else if (type == typeof(byte))
        {
            var position = attributeProvider.GetAttribute<IntegerMetaAttribute>();

            return position switch
            {
                PositionAttribute p => new UInt8PositionCommand(),
                AlignAttribute a => new AlignCommand(
                    a.Path is null ? 
                        new ConstantValueCommand<int>(a.Align) : 
                        (IGetCommand<int>)DataPathCommand<int>.Create(a.Path, tree)),
                _ => new UInt8Command(),
            };
        }
        else if (type == typeof(int))
        {
            var position = attributeProvider.GetAttribute<IntegerMetaAttribute>();

            return position switch
            {
                PositionAttribute p => new Int32PositionCommand(),
                AlignAttribute a => new AlignCommand(
                    a.Path is null ?
                        new ConstantValueCommand<int>(a.Align) :
                        (IGetCommand<int>)DataPathCommand<int>.Create(a.Path, tree)),
                _ => new Int32Command(),
            };
        }
        else if (type == typeof(short))
        {
            var position = attributeProvider.GetAttribute<IntegerMetaAttribute>();

            return position switch
            {
                PositionAttribute p => new Int16PositionCommand(),
                AlignAttribute a => new AlignCommand(
                    a.Path is null ?
                        new ConstantValueCommand<int>(a.Align) :
                        (IGetCommand<int>)DataPathCommand<int>.Create(a.Path, tree)),
                _ => new Int16Command(),
            };
        }
        else if (type == typeof(long))
        {
            var position = attributeProvider.GetAttribute<IntegerMetaAttribute>();

            return position switch
            {
                PositionAttribute p => new Int64PositionCommand(),
                AlignAttribute a => new AlignCommand(
                    a.Path is null ?
                        new ConstantValueCommand<int>(a.Align) :
                        (IGetCommand<int>)DataPathCommand<int>.Create(a.Path, tree)),
                _ => new Int64Command(),
            };
        }
        else if (type == typeof(sbyte))
        {
            var position = attributeProvider.GetAttribute<IntegerMetaAttribute>();

            return position switch
            {
                PositionAttribute p => new Int8PositionCommand(),
                AlignAttribute a => new AlignCommand(
                    a.Path is null ?
                        new ConstantValueCommand<int>(a.Align) :
                        (IGetCommand<int>)DataPathCommand<int>.Create(a.Path, tree)),
                _ => new Int8Command(),
            };
        }
        else if (type == typeof(uint))
        {
            var position = attributeProvider.GetAttribute<IntegerMetaAttribute>();

            return position switch
            {
                PositionAttribute p => new UInt32PositionCommand(),
                AlignAttribute a => new AlignCommand(
                    a.Path is null ?
                        new ConstantValueCommand<int>(a.Align) :
                        (IGetCommand<int>)DataPathCommand<int>.Create(a.Path, tree)),
                _ => new UInt32Command(),
            };
        }
        else if (type == typeof(ushort))
        {
            var position = attributeProvider.GetAttribute<IntegerMetaAttribute>();

            return position switch
            {
                PositionAttribute p => new UInt16PositionCommand(),
                AlignAttribute a => new AlignCommand(
                    a.Path is null ?
                        new ConstantValueCommand<int>(a.Align) :
                        (IGetCommand<int>)DataPathCommand<int>.Create(a.Path, tree)),
                _ => new UInt16Command(),
            };
        }
        else if (type == typeof(ulong))
        {
            var position = attributeProvider.GetAttribute<IntegerMetaAttribute>();

            return position switch
            {
                PositionAttribute p => new UInt64PositionCommand(),
                AlignAttribute a => new AlignCommand(
                    a.Path is null ?
                        new ConstantValueCommand<int>(a.Align) :
                        (IGetCommand<int>)DataPathCommand<int>.Create(a.Path, tree)),
                _ => new UInt64Command(),
            };
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
                tree.Push(Property);
                ISerializeCommand serializeCommand = DataSerializer.BuildCommand(type, tree);
                tree.Pop();
                return serializeCommand;
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
