using BlurFileFormats.SerializationFramework.Attributes;
using BlurFileFormats.SerializationFramework.Command;
using BlurFileFormats.SerializationFramework.Commands.Structures;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace BlurFileFormats.SerializationFramework;
public class DataSerializer<T> : DataSerializer where T : notnull
{
    public DataSerializer(ISerializationValueCommand serializationCommand) : base(serializationCommand) { }

    new public T Deserialize(Stream stream)
    {
        return (T)base.Deserialize(stream);
    }
    public void Serialize(T value, Stream stream)
    {
        base.Serialize(value, stream);
    }
}
public class DataSerializer
{
    ISerializationValueCommand SerializationCommand { get; }

    public DataSerializer(ISerializationValueCommand serializationCommand)
    {
        SerializationCommand = serializationCommand;
    }

    public object Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var readValue = SerializationCommand.Read(reader, []);
        return readValue;
    }
    public void Serialize(object value, Stream stream)
    {
        using var writer = new BinaryWriter(stream);
        SerializationCommand.Write(writer, [], value);
    }
    public static DataSerializer<T> Build<T>() where T : notnull
    {
        return new DataSerializer<T>(BuildCommand(typeof(T)));
    }
    public static DataSerializer Build(Type type)
    {
        return new DataSerializer(BuildCommand(type));
    }

    internal static ISerializationValueCommand BuildCommand(Type type)
    {
        var reads = ReadAttribute.GetReads(type);

        List<ISerializationCommand> subCommands = [];
        foreach (var read in reads)
        {
            read.Build(subCommands);
        }
        return new StructureCommand(type, subCommands);
    }
}
public static class Foo
{
    public static void PrintEntity(this object? value, bool offset = false)
    {
        if (value is null)
        {
            Debug.WriteLine("null");
            offset = false;
        }
        else if (value is Enum)
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
        }
        else if (value is int ni)
        {
            Debug.WriteLine($"{ni:X8} : {ni}");
            offset = false;
        }
        else if (value is long nl)
        {
            Debug.WriteLine($"{nl:X16} : {nl}");
            offset = false;
        }
        else if (value is short ns)
        {
            Debug.WriteLine($"{ns:X4} : {ns}");
            offset = false;
        }
        else if (value is byte nb)
        {
            Debug.WriteLine($"{nb:X2} : {nb}");
            offset = false;
        }
        else if (value is Array a)
        {
            if (a is byte[] b)
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
        }
        else if (value is IEnumerable e)
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
            if (offset)
            {
                offset = false;
                Debug.WriteLine("");
            }
            foreach (var property in value.GetType().GetProperties())
            {
                if (property.GetCustomAttribute<IgnorePrintAttribute>() is not null) continue;
                Debug.Write(property.Name);
                Debug.Write(": ");
                Debug.IndentLevel++;
                PrintEntity(property.GetValue(value), true);
                Debug.IndentLevel--;
            }
        }
    }
}