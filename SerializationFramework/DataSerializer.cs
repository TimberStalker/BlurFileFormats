using BlurFileFormats.SerializationFramework.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SwitchAttribute = BlurFileFormats.SerializationFramework.Attributes.SwitchAttribute;

namespace BlurFileFormats.SerializationFramework;
public static class DataSerializer
{
    public static T Deserialize<T>(Stream stream) where T : new()
    {
        using var reader = new BinaryReader(stream, Encoding.ASCII, true);
        return (T)Deserialize(typeof(T), reader);
    }
    public static object Deserialize(Type t, Stream stream)
    {
        using var reader = new BinaryReader(stream);
        return Deserialize(t, reader);
    }
    static object Deserialize(Type t, BinaryReader reader, Action<object>? setValue = null, List<object>? tree = null)
    {
        tree ??= [];

        var properties = t.GetProperties()
            .Select(p => new { Prop = (object)p, Attr = p.GetCustomAttribute<ReadAttribute>() })
            .Concat(t.GetMethods()
                .Select(p => new { Prop = (object)p, Attr = p.GetCustomAttribute<ReadAttribute>() }))
            .Where(p => p.Attr is not null)
            .OrderBy(p => p.Attr!.Order);

        object value = Activator.CreateInstance(t)!;
        setValue?.Invoke(value);
        tree.Add(value);
        foreach (var property in properties)
        {
            if(property.Prop is PropertyInfo p)
                ReadProperty(reader, tree, value, p);
            else if(property.Prop is MethodInfo m)
            {
                var parameters = m.GetParameters();
                object[] paramValues = new object[parameters.Length];
                for(int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    if(parameter.ParameterType == typeof(List<object>))
                    {
                        paramValues[i] = tree;
                    } else if(parameter.ParameterType == typeof(BinaryReader))
                    {
                        paramValues[i] = reader;
                    }
                }
                m.Invoke(value, paramValues);
            }
        }

        tree.Remove(value);
        return value;
    }
    public static string CreateReaderProgram<T>(string language) where T : new() => CreateReader(typeof(T), language);
    static string CreateReader(Type t, string language)
    {
        return "";
    }
    private static void ReadProperty(BinaryReader reader, List<object> tree, object value, PropertyInfo property)
    {
        Func<object> action;
        Type propertyType = property.PropertyType;


        var getAttr = property.GetCustomAttribute<GetAttribute>();

        if(getAttr is not null)
        {
            var resultValue = getAttr.GetTarget(value, tree);
            property.SetValue(value, resultValue);
            return;
        }

        if (propertyType.IsArray)
        {
            propertyType = propertyType.GetElementType()!;
        } else if(propertyType.IsEnum)
        {
            propertyType = propertyType.GetEnumUnderlyingType()!;
        }
        if (propertyType == typeof(string))
        {
            action = () =>
            {
                var encoding = property.GetCustomAttribute<EncodingAttribute>()?.Encoding ?? Encoding.ASCII;
                var cstring = property.GetCustomAttribute<CStringAttribute>();
                var lengthAttr = property.PropertyType.IsArray ? null : property.GetCustomAttribute<LengthAttribute>();

                if (lengthAttr is null)
                {
                    if (cstring is null)
                    {
                        int length = reader.ReadInt32();
                        return encoding.GetString(reader.ReadBytes(length));
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
                else
                {
                    int length = lengthAttr.GetLength(value, tree);
                    if (length == 0)
                    {
                        return "";
                    }
                    else
                    {
                        var bytes = reader.ReadBytes(length);

                        if (cstring is null)
                        {
                            return encoding.GetString(bytes.ToArray());
                        }
                        else
                        {
                            int byteCount = bytes.TakeWhile(b => b != 0).Count();
                            return encoding.GetString(bytes.ToArray(), 0, byteCount);
                        }
                    }
                }
            };
        }
        else if (propertyType == typeof(bool))
        {
            var lengthAttr = property.PropertyType.IsArray ? null : property.GetCustomAttribute<LengthAttribute>();
            if(lengthAttr is null)
            {
                action = () => reader.ReadByte() > 0;
            }
            else
            {
                var length = lengthAttr.GetLength(value, tree);
                action = () =>
                {
                    bool isTrue = false;
                    for (int i = 0; i < length; i++)
                    {
                        if (reader.ReadByte() > 0)
                        {
                            isTrue = true;
                        }
                    }
                    return isTrue;
                };
            }
        }
        else if (propertyType == typeof(byte))
        {
            var position = property.GetCustomAttribute<PositionAttribute>();
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
            var position = property.GetCustomAttribute<PositionAttribute>();
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
            var position = property.GetCustomAttribute<PositionAttribute>();
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
            var position = property.GetCustomAttribute<PositionAttribute>();
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
            var position = property.GetCustomAttribute<PositionAttribute>();
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
            var position = property.GetCustomAttribute<PositionAttribute>();
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
            var position = property.GetCustomAttribute<PositionAttribute>();
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
            var position = property.GetCustomAttribute<PositionAttribute>();
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
            var switchType = property.GetCustomAttribute<SwitchAttribute>();

            var objType = propertyType;
            if(switchType is not null)
            {
                var target = switchType.GetTarget(value, tree);

                objType =
                    propertyType.Assembly.GetTypes()
                    .Where(t => t.IsAssignableTo(propertyType) && !t.IsInterface && !t.IsAbstract)
                    .Select(t => new { Type = t, Attr = t.GetCustomAttribute<TargetAttribute>() })
                    .Where(t => t.Attr is not null)
                    .FirstOrDefault(t => t.Attr!.Target.Equals(target))?.Type;
                if(objType is null)
                {
                    objType = propertyType.Assembly.GetTypes().Where(t => t.IsAssignableTo(propertyType) && !t.IsInterface && !t.IsAbstract).FirstOrDefault(t => t.GetCustomAttribute<DefaultAttribute>() != null);
                    if(objType is null)
                    {
                        throw new Exception($"No Matching type with target '{target}'");
                    }
                }
            }

            action = () => Deserialize(objType, reader, o =>
            {
                if (!property.PropertyType.IsArray)
                {
                    property.SetValue(value, o);
                }
            }, tree);
        }

        if (property.PropertyType.IsArray)
        {
            var lengthAttr = property.GetCustomAttribute<LengthAttribute>();

            int length = lengthAttr?.GetLength(value, tree) ?? reader.ReadInt32();
            var arr = Array.CreateInstance(propertyType, length);
            property.SetValue(value, arr);
            for (int i = 0; i < length; i++)
            {
                arr.SetValue(action(), i);
            }
        }
        else
        {
            var actionResult = action();
            if (property.SetMethod != null)
            {
                property.SetValue(value, actionResult);
            }
            else
            {
                var test = property.GetValue(value);
                if (test?.Equals(actionResult) != true)
                {
                    throw new Exception($"Expected to read '{test}' but instead read '{actionResult}'");
                }
            }
        }
    }

    public static void PrintEntity(this object? value, bool offset = false)
    {
        if(value is null)
        {
            Debug.WriteLine("null");
            offset = false;
        }
        else if(value is Enum)
        {
            Debug.WriteLine(value.ToString());
            offset = false;
        }
        else if (value is string)
        {
            Debug.Write("\"");
            Debug.Write(value);
            Debug.WriteLine("\"");
            offset = false;
        } else if(value is int ni)
        {
            Debug.WriteLine($"{ni:X8} : {ni}");
            offset = false;
        } else if(value is long nl)
        {
            Debug.WriteLine($"{nl:X16} : {nl}");
            offset = false;
        } else if(value is short ns)
        {
            Debug.WriteLine($"{ns:X4} : {ns}");
            offset = false;
        } else if(value is byte nb)
        {
            Debug.WriteLine($"{nb:X2} : {nb}");
            offset = false;
        }
        else if (value is Array a)
        {
            if(a is byte[] b)
            {
                Debug.WriteLine("");
                Debug.IndentLevel++;
                foreach (var item in b.Chunk(4).Select(b => string.Concat(b.Select(b => b.ToString("X2")))).Chunk(4).Take(8))
                {
                    Debug.WriteLine(string.Join(' ', item));
                }
                Debug.IndentLevel--;
                offset = false;
            }
            else
            {
                if (offset)
                {
                    offset = false;
                    Debug.WriteLine("");
                }
                for (int i = 0; i < Math.Min(a.Length, 30); i++)
                {
                    Debug.Write("[");
                    Debug.Write(i);
                    Debug.Write("]:");
                    Debug.IndentLevel++;
                    PrintEntity(a.GetValue(i));
                    Debug.IndentLevel--;
                }
            }
        } else if(value is IEnumerable e)
        {
            if (e is IEnumerable<byte> b)
            {
                Debug.WriteLine("");
                Debug.IndentLevel++;
                foreach (var item in b.Chunk(4).Select(b => string.Concat(b.Select(b => b.ToString("X2")))).Chunk(4).Take(8))
                {
                    Debug.WriteLine(string.Join(' ', item));
                }
                Debug.IndentLevel--;
                offset = false;
            }
            else
            {
                if (offset)
                {
                    offset = false;
                    Debug.WriteLine("");
                }
                int i = 0;
                foreach (var item in e)
                {
                    Debug.Write("[");
                    Debug.Write(i);
                    Debug.Write("]:");
                    Debug.IndentLevel++;
                    PrintEntity(item);
                    Debug.IndentLevel--;
                }
            }
        }
        else if (value.GetType().GetMethod("ToString", [])!.DeclaringType == value.GetType())
        {
            Debug.WriteLine(value);
            offset = false;
        } 
        else
        {
            if(offset)
            {
                offset = false;
                Debug.WriteLine("");
            }
            foreach (var property in value.GetType().GetProperties())
            {
                Debug.Write(property.Name);
                Debug.Write(": ");
                Debug.IndentLevel++;
                PrintEntity(property.GetValue(value), true);
                Debug.IndentLevel--;
            }
        }
    }
}
