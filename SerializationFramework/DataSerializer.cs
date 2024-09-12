using BlurFileFormats.SerializationFramework.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework;
public class DataSerializaer<T>
{
    public T Deserialize(Stream stream)
    {

    }
    public void Serialize(T value, Stream stream)
    {

    }
}
public static partial class DataSerializer
{
    public static DataSerializaer<T> Create<T>()
    {

    }

    public static T Deserialize<T>(Stream stream) where T : new()
    {
        using var reader = new BinaryReader(stream, Encoding.ASCII, true);
        return (T)Deserialize(typeof(T), reader);
    }
    public static object Deserialize(Type t, Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.ASCII, true);
        return Deserialize(t, reader);
    }
    public static object Deserialize(Type t, BinaryReader reader, Action<object>? setValue = null, ReadTree? tree = null)
    {
        tree ??= [];

        var reads = ReadAttribute.GetReads(t);

        object value = Activator.CreateInstance(t)!;
        setValue?.Invoke(value);
        tree.Push(value);

        foreach (var read in reads)
        {
            read.Read(reader, tree);
        }
        tree.Pop();
        return value;
    }

    public static string CreateReaderProgram<T>(string language) where T : new() => CreateReader(typeof(T), language);
    static string CreateReader(Type t, string language)
    {
        return "";
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
