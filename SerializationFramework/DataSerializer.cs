using BlurFileFormats.SerializationFramework.Attributes;
using BlurFileFormats.XtFlask.Values;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace BlurFileFormats.SerializationFramework;

public class DataSerializer
{
    public List<ISerializerCommand> Commands { get; } = [];
    public Type Type { get; }
    public DataSerializer(Type type)
    {
        Type = type;
    }

    public object Read(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var value = Activator.CreateInstance(Type)!;

        var info = new ReadInfo();
        foreach (var command in Commands)
        {
            command.Read(reader, info, value);
        }
        return value;
    }

    public void Write(Stream stream, object value)
    {
        var writer = new BinaryWriter(stream);


        var info = new WriteInfo();
        foreach (var command in Commands)
        {
            command.Write(writer, info, value);
        }
    }

    public static DataSerializer Create(Type type)
    {
        var readProperties = type.GetProperties()
            .Select(p => (property: p, order: p.GetCustomAttribute<ReadAttribute>()))
            .Where(p => p.order is not null)
            .OrderBy(p => p.order!.Order)
            .Select(p => p.property)
            .ToArray();

        var serializer = new DataSerializer(type);
        foreach (var property in readProperties)
        {
            var command = CreateCommand(type, property);
            serializer.Commands.Add(command);
        }
        return serializer;
    }

    public static ISerializerCommand CreateCommand(Type type, PropertyInfo property, int depth = 0)
    {
        if (type.IsArray)
        {
            var arrayElement = type.GetElementType()!;
            return CreateCommand(arrayElement, property, depth + 1);
        }

        if (type == typeof(int))
            return CreateReadablePositional(property, reader => reader.ReadInt32(), (writer, v) => writer.Write(v), p => p);
        if (type == typeof(short))
            return CreateReadablePositional(property, reader => reader.ReadInt16(), (writer, v) => writer.Write(v), p => p);
        if (type == typeof(sbyte))
            return CreateReadablePositional(property, reader => reader.ReadSByte(), (writer, v) => writer.Write(v), p => p);
        if (type == typeof(long))
            return CreateReadablePositional(property, reader => reader.ReadInt64(), (writer, v) => writer.Write(v), p => p);

        if (type == typeof(uint))
            return CreateReadablePositional(property, reader => reader.ReadUInt32(), (writer, v) => writer.Write(v), p => p);
        if (type == typeof(ushort))
            return CreateReadablePositional(property, reader => reader.ReadUInt16(), (writer, v) => writer.Write(v), p => p);
        if (type == typeof(byte))
            return CreateReadablePositional(property, reader => reader.ReadByte(), (writer, v) => writer.Write(v), p => p);
        if (type == typeof(ulong))
            return CreateReadablePositional(property, reader => reader.ReadUInt64(), (writer, v) => writer.Write(v), p => (ulong)p);

        if (type == typeof(float))
            return CreateReadablePositional(property, reader => reader.ReadSingle(), (writer, v) => writer.Write(v), p => p);
        if (type == typeof(double))
            return CreateReadablePositional(property, reader => reader.ReadDouble(), (writer, v) => writer.Write(v), p => p);
        if (type == typeof(Half))
            return CreateReadablePositional(property, reader => reader.ReadHalf(), (writer, v) => writer.Write(v), p => (Half)p);

        if(type == typeof(string))
        {
            CreateReadable<string, LengthAttribute, StringLengthAttribute, CStringAttribute, EncodingAttribute>(property, (reader, info, lengthAttribute, stringLengthAttribute, cstringAttribute, encodingAttribute) =>
            {
                int? length = null;
                if(stringLengthAttribute is not null)
                {
                    if(stringLengthAttribute.Path is not null)
                    {
                        length = Convert.ToInt32(info.GetValue(stringLengthAttribute.Path));
                    }
                    else
                    {
                        length = stringLengthAttribute.Value;
                    }
                }
                else if(lengthAttribute is not null && depth == lengthAttribute.Depth)
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

                Encoding encoding = Encoding.ASCII;
                if(encodingAttribute is not null)
                    encoding = Encoding.GetEncoding(encodingAttribute.Encoding);

                if(cstringAttribute is not null)
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
                    int count = length ?? reader.ReadInt32();
                    var bytes = reader.ReadBytes(count);

                    return encoding.GetString(bytes);
                }
            },
            (writer, info, text, length, stringLength, cString, encodingAttribute) =>
            {
                throw new NotImplementedException();
            });
        }

        throw new NotImplementedException();
    }
    public static ISerializerCommand CreateReadablePositional<T>(PropertyInfo property, Func<BinaryReader, T>? reader, Action<BinaryWriter, T>? writer, Func<long, T> convertPosition)
    {
        var position = property.GetCustomAttribute<PositionAttribute>();
        if (position is not null)
        {
            return CreateReadable(property, (r, info) => convertPosition(r.BaseStream.Position), null);
        }
        return CreateReadable<T>(property, (reader is null ? null : (r, info) => reader(r)), (writer is null ? null : (w, info, v) => writer(w, v)));
    }
    public static ISerializerCommand CreateReadable<T>(PropertyInfo property, Func<BinaryReader, ReadInfo, T>? reader, Action<BinaryWriter, WriteInfo, T>? writer)
    {
        if (writer is null)
        {
            return new PropertySerializerCommand(property)
            {
                ReadAction = reader is null ? null : (r, info) => reader(r, info),
                WriteAction = null,
            };
        }
        else
        {
            return new PropertySerializerCommand(property)
            {
                ReadAction = reader is null ? null : (r, info) => reader(r, info),
                WriteAction = (w, info, o) => writer(w, info, (T)o!),
            };
        }
    }
    public static ISerializerCommand CreateReadable<T, U1>(PropertyInfo property, Func<BinaryReader, ReadInfo, U1?, T> reader, Action<BinaryWriter, WriteInfo, T, U1?> writer) where U1 : Attribute
    {
        var attribute1 = property.GetCustomAttribute<U1>();
        return new PropertySerializerCommand(property)
        {
            ReadAction = (r, info) => reader(r, info, attribute1),
            WriteAction = (w, info, o) => writer(w, info, (T)o!, attribute1),
        };
    }
    public static ISerializerCommand CreateReadable<T, U1, U2>(PropertyInfo property, Func<BinaryReader, ReadInfo, U1?, U2?, T> reader, Action<BinaryWriter, WriteInfo, T, U1?, U2?> writer) where U1 : Attribute where U2 : Attribute
    {
        var attribute1 = property.GetCustomAttribute<U1>();
        var attribute2 = property.GetCustomAttribute<U2>();
        return new PropertySerializerCommand(property)
        {
            ReadAction = (r, info) => reader(r, info, attribute1, attribute2),
            WriteAction = (w, info, o) => writer(w, info, (T)o!, attribute1, attribute2),
        };
    }
    public static ISerializerCommand CreateReadable<T, U1, U2, U3>(PropertyInfo property, Func<BinaryReader, ReadInfo, U1?, U2?, U3?, T> reader, Action<BinaryWriter, WriteInfo, T, U1?, U2?, U3?> writer) where U1 : Attribute where U2 : Attribute where U3 : Attribute
    {
        var attribute1 = property.GetCustomAttribute<U1>();
        var attribute2 = property.GetCustomAttribute<U2>();
        var attribute3 = property.GetCustomAttribute<U3>();
        return new PropertySerializerCommand(property)
        {
            ReadAction = (r, info) => reader(r, info, attribute1, attribute2, attribute3),
            WriteAction = (w, info, o) => writer(w, info, (T)o!, attribute1, attribute2, attribute3),
        };
    }
    public static ISerializerCommand CreateReadable<T, U1, U2, U3, U4>(PropertyInfo property, Func<BinaryReader, ReadInfo, U1?, U2?, U3?, U4?, T> reader, Action<BinaryWriter, WriteInfo, T, U1?, U2?, U3?, U4?> writer)
        where U1 : Attribute
        where U2 : Attribute
        where U3 : Attribute
        where U4 : Attribute
    {
        var attribute1 = property.GetCustomAttribute<U1>();
        var attribute2 = property.GetCustomAttribute<U2>();
        var attribute3 = property.GetCustomAttribute<U3>();
        var attribute4 = property.GetCustomAttribute<U4>();
        return new PropertySerializerCommand(property)
        {
            ReadAction = (r, info) => reader(r, info, attribute1, attribute2, attribute3, attribute4),
            WriteAction = (w, info, o) => writer(w, info, (T)o!, attribute1, attribute2, attribute3, attribute4),
        };
    }
}
public class ReadInfo
{
    [return: MaybeNull]
    public object GetValue(string path)
    {
        throw new NotImplementedException();
    }
}
public class WriteInfo
{
    [return: MaybeNull]
    public object SetValue(string path, object value)
    {
        throw new NotImplementedException();
    }
}