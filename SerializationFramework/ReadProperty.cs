using System.Reflection;
using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.SerializationFramework;

public class ReadProperty : IRead
{
    PropertyInfo Property { get; }

    public ReadProperty(PropertyInfo property)
    {
        Property = property;
    }

    public void Read(BinaryReader reader, ReadTree tree)
    {
        Func<object> action;
        Type propertyType = Property.PropertyType;


        if (Property.GetCustomAttribute<AlignAttribute>() is AlignAttribute a)
        {
            a.AlignStream(tree, reader);
        }

        var getAttr = Property.GetCustomAttribute<GetAttribute>();

        if (getAttr is not null)
        {
            var resultValue = tree.GetValue(getAttr);
            Property.SetValue(tree.CurrentObject, resultValue);
            return;
        }

        if (propertyType.IsArray)
        {
            propertyType = propertyType.GetElementType()!;
        }
        else if (propertyType.IsEnum)
        {
            propertyType = propertyType.GetEnumUnderlyingType()!;
        }
        if (propertyType == typeof(string))
        {
            action = () =>
            {
                var encoding = EncodingAttribute.GetEncoding(Property, tree);
                var cstring = Property.GetCustomAttribute<CStringAttribute>();

                var length = Property.PropertyType.IsArray ? null : LengthAttribute.GetLength(Property, tree);

                if (length is int l)
                {
                    if (l == 0)
                    {
                        return "";
                    }
                    else
                    {

                        if (cstring is null)
                        {
                            var bytes = reader.ReadBytes(l);
                            return encoding.GetString(bytes.ToArray());
                        }
                        else
                        {
                            var bytes = reader.ReadBytes(l);
                            int byteCount = bytes.TakeWhile(b => b != 0).Count();
                            return encoding.GetString(bytes.ToArray(), 0, byteCount);
                        }
                    }
                }
                else
                {
                    if (cstring is null)
                    {
                        l = reader.ReadInt32();
                        return encoding.GetString(reader.ReadBytes(l));
                    }
                    else
                    {
                        List<byte> bytes = new List<byte>(10);
                        byte b = reader.ReadByte();
                        while (b != 0)
                        {
                            bytes.Add(b);
                            b = reader.ReadByte();
                        }
                        return encoding.GetString(bytes.ToArray());
                    }
                }
            };
        }
        else if (propertyType == typeof(bool))
        {
            var length = Property.PropertyType.IsArray ? null : LengthAttribute.GetLength(Property, tree);
            if (length is int l)
            {
                action = () =>
                {
                    bool isTrue = false;
                    for (int i = 0; i < l; i++)
                    {
                        if (reader.ReadByte() > 0)
                        {
                            isTrue = true;
                        }
                    }
                    return isTrue;
                };
            }
            else
            {
                action = () => reader.ReadByte() > 0;
            }
        }
        else if (propertyType == typeof(byte))
        {
            var position = Property.GetCustomAttribute<PositionAttribute>();
            if (position == null)
            {
                action = () => reader.ReadByte();
            }
            else
            {
                action = () => (byte)reader.BaseStream.Position;
            }
        }
        else if (propertyType == typeof(int))
        {
            var position = Property.GetCustomAttribute<PositionAttribute>();
            if (position == null)
            {
                action = () => reader.ReadInt32();
            }
            else
            {
                action = () => (int)reader.BaseStream.Position;
            }
        }
        else if (propertyType == typeof(short))
        {
            var position = Property.GetCustomAttribute<PositionAttribute>();
            if (position == null)
            {
                action = () => reader.ReadInt16();
            }
            else
            {
                action = () => (short)reader.BaseStream.Position;
            }
        }
        else if (propertyType == typeof(long))
        {
            var position = Property.GetCustomAttribute<PositionAttribute>();
            if (position == null)
            {
                action = () => reader.ReadInt64();
            }
            else
            {
                action = () => reader.BaseStream.Position;
            }
        }
        else if (propertyType == typeof(sbyte))
        {
            var position = Property.GetCustomAttribute<PositionAttribute>();
            if (position == null)
            {
                action = () => reader.ReadSByte();
            }
            else
            {
                action = () => (sbyte)reader.BaseStream.Position;
            }
        }
        else if (propertyType == typeof(uint))
        {
            var position = Property.GetCustomAttribute<PositionAttribute>();
            if (position == null)
            {
                action = () => reader.ReadUInt32();
            }
            else
            {
                action = () => (uint)reader.BaseStream.Position;
            }
        }
        else if (propertyType == typeof(ushort))
        {
            var position = Property.GetCustomAttribute<PositionAttribute>();
            if (position == null)
            {
                action = () => reader.ReadUInt16();
            }
            else
            {
                action = () => (ushort)reader.BaseStream.Position;
            }
        }
        else if (propertyType == typeof(ulong))
        {
            action = () => reader.ReadUInt64();
            var position = Property.GetCustomAttribute<PositionAttribute>();
            if (position == null)
            {
                action = () => reader.ReadUInt64();
            }
            else
            {
                action = () => (ulong)reader.BaseStream.Position;
            }
        }
        else if (propertyType == typeof(float))
        {
            action = () => reader.ReadSingle();
        }
        else if (propertyType == typeof(double))
        {
            action = () => reader.ReadDouble();
        }
        else
        {
            var switchType = Property.GetCustomAttribute<SwitchAttribute>();

            var objType = propertyType;
            if (switchType is not null)
            {
                var target = tree.GetValue(switchType.Path);

                objType =
                    propertyType.Assembly.GetTypes()
                    .Where(t => t.IsAssignableTo(propertyType) && !t.IsInterface && !t.IsAbstract)
                    .Select(t => new { Type = t, Attr = t.GetCustomAttribute<TargetAttribute>() })
                    .Where(t => t.Attr is not null)
                    .FirstOrDefault(t => t.Attr!.Target.Equals(target))?.Type;
                if (objType is null)
                {
                    objType = propertyType.Assembly.GetTypes().Where(t => t.IsAssignableTo(propertyType) && !t.IsInterface && !t.IsAbstract).FirstOrDefault(t => t.GetCustomAttribute<DefaultAttribute>() != null);
                    if (objType is null)
                    {
                        throw new Exception($"No Matching type with target '{target}'");
                    }
                }
            }

            action = () => DataSerializer.Deserialize(objType, reader, o =>
            {
                if (!Property.PropertyType.IsArray)
                {
                    Property.SetValue(tree.CurrentObject, o);
                }
            }, tree);
        }

        if (Property.PropertyType.IsArray)
        {
            int length = LengthAttribute.GetLength(Property, tree) ?? reader.ReadInt32();
            var arr = Array.CreateInstance(propertyType, length);
            Property.SetValue(tree.CurrentObject, arr);
            for (int i = 0; i < length; i++)
            {
                arr.SetValue(action(), i);
            }
        }
        else
        {
            var actionResult = action();
            if (Property.SetMethod != null)
            {
                Property.SetValue(tree.CurrentObject, actionResult);
            }
            else
            {
                var test = Property.GetValue(tree.CurrentObject);
                if (test?.Equals(actionResult) != true)
                {
                    throw new Exception($"Expected to read '{test}' but instead read '{actionResult}'");
                }
            }
        }
    }
}
