using BlurFileFormats.SerializationFramework.Attributes;
using BlurFileFormats.SerializationFramework.Commands;
using BlurFileFormats.SerializationFramework.Sources;
using BlurFileFormats.SerializationFramework.Targets;
using BlurFileFormats.XtFlask.Values;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using SwitchAttribute = BlurFileFormats.SerializationFramework.Attributes.SwitchAttribute;

namespace BlurFileFormats.SerializationFramework;
public interface ITargetBuffer
{
    void Emit(ISerializerTarget command);
}
public class DataSerializer : ITargetBuffer
{
    public static readonly Dictionary<string, Encoding> Encodings;
    public List<ISerializerTarget> Targets { get; } = [];
    public Type Type { get; }
    public DataSerializer(Type type)
    {
        Type = type;
    }
    static DataSerializer()
    {
        Encodings = Encoding.GetEncodings().ToDictionary(e => e.Name, e => e.GetEncoding());
    }
    public static void TryAddEncoding(string name, Encoding encoding) => Encodings.TryAdd(name, encoding);
    public void Emit(ISerializerTarget command)
    {
        Targets.Add(command);
    }

    public object Read(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var value = Activator.CreateInstance(Type)!;

        var info = new SerializerInfo(value);
        foreach (var command in Targets)
        {
            command.Deserialize(reader, info, value);
        }
        return value;
    }

    public void Write(Stream stream, object value)
    {
        var writer = new BinaryWriter(stream);


        var info = new SerializerInfo(value);
        foreach (var command in Targets)
        {
            command.Serialize(writer, info, value);
        }
    }

    public static DataSerializer Create(Type type)
    {
        var serializer = new DataSerializer(type);
        serializer.Targets.AddRange(CreateStructureSerializationTargets(type));
        return serializer;
    }

    public static IEnumerable<ISerializerTarget> CreateStructureSerializationTargets(Type type)
    {
        var readProperties = type.GetProperties()
                    .Select(p => (property: p, order: p.GetCustomAttribute<ReadAttribute>()))
                    .Where(p => p.order is not null)
                    .OrderBy(p => p.order!.Order)
                    .Select(p => p.property)
                    .ToArray();

        foreach (var property in readProperties)
        {
            ISerializerTarget target = new PropertyTarget(property, CreateCommand(property.PropertyType, property));
            if(property.GetCustomAttribute<AnchorAttribute>() is not null)
            {
                target = new AnchorTarget(target);
            }
            var alignAttribute = property.GetCustomAttribute<AlignAttribute>();
            if (alignAttribute is not null)
            {
                ISerializerSource alignSource;
                if (alignAttribute.IsValue)
                {
                    alignSource = new SerilaizerConstantSource<int>(alignAttribute.Value);
                }
                else
                {
                    alignSource = new SerializerDynamicSource(alignAttribute.Path);
                }
                target = new AlignTarget(target, alignSource);
            }
            yield return target;
        }
    }
    static readonly Dictionary<Type, PositionCommand> positionCommands = new()
    {
        {typeof(int), new PositionCommand(p => (int)p) },
        {typeof(short), new PositionCommand(p => (short)p) },
        {typeof(sbyte), new PositionCommand(p => (sbyte)p) },
        {typeof(long), new PositionCommand(p => (long)p) },

        {typeof(uint), new PositionCommand(p => (uint)p) },
        {typeof(ushort), new PositionCommand(p => (ushort)p) },
        {typeof(byte), new PositionCommand(p => (byte)p) },
        {typeof(ulong), new PositionCommand(p => (ulong)p) },

        {typeof(float), new PositionCommand(p => (float)p) },
        {typeof(double), new PositionCommand(p => (double)p) },
        {typeof(Half), new PositionCommand(p => (Half)p) },
    };
    static readonly ISerializerCommand Int32Command = CreateAtomic(reader => reader.ReadInt32(), (writer, v) => writer.Write(v));
    static readonly Dictionary<Type, ISerializerCommand> atomicCommands = new()
    {
        {typeof(int), Int32Command },
        {typeof(short), CreateAtomic(reader => reader.ReadInt16(), (writer, v) => writer.Write(v))},
        {typeof(sbyte), CreateAtomic(reader => reader.ReadSByte(), (writer, v) => writer.Write(v))},
        {typeof(long), CreateAtomic(reader => reader.ReadInt64(), (writer, v) => writer.Write(v))},

        {typeof(bool), CreateAtomic(reader => reader.ReadByte() > 0, (writer, v) => writer.Write((byte)(v ? 1 : 0))) },

        {typeof(uint), CreateAtomic(reader => reader.ReadUInt32(), (writer, v) => writer.Write(v)) },
        {typeof(ushort), CreateAtomic(reader => reader.ReadUInt16(), (writer, v) => writer.Write(v)) },
        {typeof(byte), CreateAtomic(reader => reader.ReadByte(), (writer, v) => writer.Write(v)) },
        {typeof(ulong), CreateAtomic(reader => reader.ReadUInt64(), (writer, v) => writer.Write(v)) },

        {typeof(float), CreateAtomic(reader => reader.ReadSingle(), (writer, v) => writer.Write(v)) },
        {typeof(double), CreateAtomic(reader => reader.ReadDouble(), (writer, v) => writer.Write(v))},
        {typeof(Half), CreateAtomic(reader => reader.ReadHalf(), (writer, v) => writer.Write(v)) },
    };
    private static ISerializerCommand CreateCommand(Type type, PropertyInfo property, int depth = 0)
    {
        if (type.IsArray)
        {
            var arrayElement = type.GetElementType()!;
            ISerializerSource lengthSource = new SerializerCommandSource(Int32Command);

            var lengthAttribute = property.GetCustomAttributes<LengthAttribute>().SingleOrDefault(d => d.Depth == depth);
            if (lengthAttribute is not null)
            {
                if (lengthAttribute.IsValue)
                {
                    lengthSource = new SerilaizerConstantSource<int>(lengthAttribute.Value);
                }
                else
                {
                    lengthSource = new SerializerDynamicSource(lengthAttribute.Path);
                }
            }
            if(arrayElement == typeof(byte))
            {
                return new ByteArrayCommand(lengthSource);
            }
            else
            {
                return new ArrayCommand(arrayElement, lengthSource, CreateCommand(arrayElement, property, depth + 1));
            }
        }
        else
        {
            var positionAttribute = property.GetCustomAttribute<PositionAttribute>();
            if(positionAttribute is not null)
            {
                if (positionCommands.TryGetValue(type, out var positionCommand))
                    return positionCommand;
                else
                    throw new NotSupportedException($"Cannot store position as type {type.Name}");
            }

            if (atomicCommands.TryGetValue(type, out var atomicCommand))
            {
                return atomicCommand;
            }
            else if(type.IsEnum)
            {
                return CreateCommand(type.GetEnumUnderlyingType(), property, depth);
            }
            else if(type == typeof(string))
            {
                return (CreateStringCommand(property, depth));
            }
            else
            {
                var switchAttribute = property.GetCustomAttribute<SwitchAttribute>();
                return new StructureCommand(type, switchAttribute);
            }
        }
    }

    private static ISerializerCommand CreateStringCommand(PropertyInfo property, int depth)
    {
        return CreateReadable<string, LengthAttribute, StringLengthAttribute, CStringAttribute, EncodingAttribute>(property, (reader, info, lengthAttribute, stringLengthAttribute, cstringAttribute, encodingAttribute) =>
        {
            int length;
            if (stringLengthAttribute is not null)
            {
                if (stringLengthAttribute.Path is not null)
                {
                    length = Convert.ToInt32(info.GetValue(stringLengthAttribute.Path));
                }
                else
                {
                    length = stringLengthAttribute.Value;
                }
            }
            else if (lengthAttribute is not null && depth == lengthAttribute.Depth)
            {
                if (lengthAttribute.Path is not null)
                {
                    length = Convert.ToInt32(info.GetValue(lengthAttribute.Path));
                }
                else
                {
                    length = lengthAttribute.Value;
                }
            }
            else
            {
                length = reader.ReadInt32();
            }
            Encoding? encoding;
            if (encodingAttribute is not null)
            {
                if (!Encodings.TryGetValue(encodingAttribute.Encoding, out encoding))
                {
                    throw new Exception($"{encodingAttribute.Encoding} is not a valid encoding.");
                }
            }
            else encoding = Encoding.ASCII;

            if (cstringAttribute is not null)
            {
                byte[] bytes;
                if (length is int count)
                {
                    bytes = reader.ReadBytes(count);
                }
                else
                {
                    using var memoryStream = new MemoryStream();
                    var nextByte = reader.ReadByte();
                    while (nextByte != 0)
                    {
                        memoryStream.WriteByte(nextByte);
                        nextByte = reader.ReadByte();
                    }
                    return encoding.GetString(memoryStream.ToArray());
                }

                return encoding.GetString(bytes);
            }
            else
            {
                int count = length;
                var bytes = reader.ReadBytes(count);

                return encoding.GetString(bytes);
            }
        },
        (writer, info, text, lengthAttribute, stringLengthAttribute, cStringAttribute, encodingAttribute) =>
        {
            Encoding? encoding;
            if (encodingAttribute is not null)
            {
                if (!Encodings.TryGetValue(encodingAttribute.Encoding, out encoding))
                {
                    throw new Exception($"{encodingAttribute.Encoding} is not a valid encoding.");
                }
            }
            else encoding = Encoding.ASCII;

            var writeBytes = encoding.GetBytes(text);
            int length = writeBytes.Length;
            
            if (stringLengthAttribute is not null)
            {
                if (stringLengthAttribute.Path is not null)
                {
                    if (cStringAttribute is not null) length++;
                    info.SetValue(stringLengthAttribute.Path, length);
                }
                else
                {
                    if (length != stringLengthAttribute.Value)
                    {
                        System.Diagnostics.Debug.WriteLine("Length is truncated.");
                        length = stringLengthAttribute.Value;
                    }
                }
            }
            else if (lengthAttribute is not null && depth == lengthAttribute.Depth)
            {
                if (lengthAttribute.Path is not null)
                {
                    if (cStringAttribute is not null) length++;
                    info.SetValue(lengthAttribute.Path, length);
                }
                else
                {
                    if (length != lengthAttribute.Value)
                    {
                        System.Diagnostics.Debug.WriteLine("Length is truncated.");
                        length = lengthAttribute.Value;
                    }
                }
            }
            else
            {
                if (cStringAttribute is not null) length++;
                writer.Write(length);
            }

            writer.Write(writeBytes, 0, Math.Min(length, writeBytes.Length));
            for(int i = writeBytes.Length; i < length; i++)
            {
                writer.Write((byte)0);
            }
        });
    }
    private static ISerializerCommand CreateAtomic<T>(Func<BinaryReader, T> reader, Action<BinaryWriter, T> writer)
    {
        return new ValueCommand() { ReadAction = (r, info) =>
        {

            T value = reader(r);
            System.Diagnostics.Debug.WriteLine(value);
            return value;
        }, WriteAction = (w, info, v) => writer(w, (T)v!) };
    }
    private static ISerializerCommand CreateReadable<T>(Func<BinaryReader, SerializerInfo, T>? reader, Action<BinaryWriter, SerializerInfo, T>? writer)
    {
        if (writer is null)
        {
            return new ValueCommand
            {
                ReadAction = reader is null ? null : (r, info) => reader(r, info),
                WriteAction = null,
            };
        }
        else
        {
            return new ValueCommand
            {
                ReadAction = reader is null ? null : (r, info) => reader(r, info),
                WriteAction = (w, info, o) => writer(w, info, (T)o!),
            };
        }
    }
    private static ISerializerCommand CreateReadable<T, U1>(PropertyInfo property, Func<BinaryReader, SerializerInfo, U1?, T> reader, Action<BinaryWriter, SerializerInfo, T, U1?> writer) where U1 : Attribute
    {
        var attribute1 = property.GetCustomAttribute<U1>();
        return new ValueCommand
        {
            ReadAction = (r, info) => reader(r, info, attribute1),
            WriteAction = (w, info, o) => writer(w, info, (T)o!, attribute1),
        };
    }
    private static ISerializerCommand CreateReadable<T, U1, U2>(PropertyInfo property, Func<BinaryReader, SerializerInfo, U1?, U2?, T> reader, Action<BinaryWriter, SerializerInfo, T, U1?, U2?> writer) where U1 : Attribute where U2 : Attribute
    {
        var attribute1 = property.GetCustomAttribute<U1>();
        var attribute2 = property.GetCustomAttribute<U2>();
        return new ValueCommand
        {
            ReadAction = (r, info) => reader(r, info, attribute1, attribute2),
            WriteAction = (w, info, o) => writer(w, info, (T)o!, attribute1, attribute2),
        };
    }
    private static ISerializerCommand CreateReadable<T, U1, U2, U3>(PropertyInfo property, Func<BinaryReader, SerializerInfo, U1?, U2?, U3?, T> reader, Action<BinaryWriter, SerializerInfo, T, U1?, U2?, U3?> writer) where U1 : Attribute where U2 : Attribute where U3 : Attribute
    {
        var attribute1 = property.GetCustomAttribute<U1>();
        var attribute2 = property.GetCustomAttribute<U2>();
        var attribute3 = property.GetCustomAttribute<U3>();
        return new ValueCommand
        {
            ReadAction = (r, info) => reader(r, info, attribute1, attribute2, attribute3),
            WriteAction = (w, info, o) => writer(w, info, (T)o!, attribute1, attribute2, attribute3),
        };
    }
    private static ISerializerCommand CreateReadable<T, U1, U2, U3, U4>(PropertyInfo property, Func<BinaryReader, SerializerInfo, U1?, U2?, U3?, U4?, T> reader, Action<BinaryWriter, SerializerInfo, T, U1?, U2?, U3?, U4?> writer)
        where U1 : Attribute
        where U2 : Attribute
        where U3 : Attribute
        where U4 : Attribute
    {
        var attribute1 = property.GetCustomAttribute<U1>();
        var attribute2 = property.GetCustomAttribute<U2>();
        var attribute3 = property.GetCustomAttribute<U3>();
        var attribute4 = property.GetCustomAttribute<U4>();
        return new ValueCommand
        {
            ReadAction = (r, info) =>
            {
                T? value = reader(r, info, attribute1, attribute2, attribute3, attribute4);
                Debug.WriteLine(value);
                return value;
            },
            WriteAction = (w, info, o) => writer(w, info, (T)o!, attribute1, attribute2, attribute3, attribute4),
        };
    }
}
public class SerializerInfo
{
    object Root { get; }
    public int CurrentAnchor => AnchorStack.Peek();
    public object CurrentObject => ParentStack.Peek();
    public Stack<int> AnchorStack { get; }
    public Stack<object> ParentStack { get; }
    public SerializerInfo(object root)
    {
        Root = root;
        AnchorStack = new();
        AnchorStack.Push(0);
        ParentStack = new();
        ParentStack.Push(root);
    }
    [return: MaybeNull]
    public object GetValue(string path)
    {
        var segments = path.Split('.');
        if (segments.Length == 0) throw new Exception("Path must have at least one segment.");
        if(segments.Length == 1)
        {
            var property = CurrentObject.GetType().GetProperty(segments[0]);
            if(property is null) throw new Exception("Path was not found.");
            return property.GetValue(CurrentObject);
        }
        var returnItem = GetValueAtPath(segments, CurrentObject);
        if (returnItem is not null) return returnItem;
        foreach (var item in ParentStack.Skip(1).Where(i => i.GetType().Name == segments[0]))
        {
            returnItem = GetValueAtPath(segments.AsSpan(1..), item);
            if (returnItem is not null) return returnItem;
        }
        throw new Exception("Path was not found.");
    }
    object? GetValueAtPath(Span<string> segments, object root)
    {
        var localObject = root;
        for(int i = 0; i < segments.Length; i++)
        {
            var localProperty = localObject.GetType().GetProperty(segments[i]);
            if (localProperty is null) return null;
            localObject = localProperty.GetValue(localObject);
            if (localObject is null) return null;
        }
        return localObject;
    }
    public void SetValue(string path, object? value)
    {
    }
}