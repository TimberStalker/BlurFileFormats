using BlurFileFormats.SerializationFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Languages;
public class CSharpTarget : ILanguageReadTarget
{
    public string Assign(string target, string source) => $"{target} = {source};";
    public string Test(string source, string value) => $"if({source} != {value}) throw new Exception();";

    public string ReadBool(int size) => $"ReadBool({size})";
    public string ReadInt8() => "ReadInt8()";
    public string ReadInt16() => "ReadInt16()";
    public string ReadInt32() => "ReadInt32()";
    public string ReadInt64() => "ReadInt64()";

    public string ReadUInt8() => "ReadUInt8()";
    public string ReadUInt16() => "ReadUInt16()";
    public string ReadUInt32() => "ReadUInt32()";
    public string ReadUInt64() => "ReadUInt64()";

    public string ReadString(Encoding encoding, string length) => $"ReadString({GetEncoding(encoding)}, {length})";

    public string ReadCString(Encoding encoding, string? length) => $"ReadCString({GetEncoding(encoding)}, {length})";
    string GetEncoding(Encoding encoding)
    {
        if(encoding == Encoding.ASCII)
        {
            return "System.Text.Encoding.ASCII";
        } else if(encoding == Encoding.UTF8)
        {
            return "System.Text.Encoding.UTF8";
        } else if(encoding == Encoding.Unicode)
        {
            return "System.Text.Encoding.Unicode";
        }
        else
        {
            throw new Exception("Unsupported encoding.");
        }
    }

    public string ReadLoopStart(int length, string variable) => $$"""
        for(int {{variable}} = 0; i < {{length}}; i++) {
        """;

    public string ReadLoopEnd() => "}";

    public string Reader() => """
        public class BinaryReader
        {
            
        }
        """;

    public string CreateClasses(Type t)
    {
        List<Type> createdClasses = new()
        {
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(int),
            typeof(uint),
            typeof(short),
            typeof(ushort),
            typeof(long),
            typeof(ulong),
            typeof(double),
            typeof(float),
        };
        List<string> types = new();

        CreateClasses(t, types, createdClasses);

        StringBuilder builder = new();
        foreach (var type in types)
        {
            builder.AppendLine(type);
        }
        return builder.ToString();
    }
    public void CreateClasses(Type t, List<string> types, List<Type> createdClasses)
    {
        createdClasses.Add(t);
        var properties = t.GetProperties()
            .Select(p => new {Prop = p, Attr = t.GetCustomAttribute<ReadAttribute>() })
            .Where(p => p.Attr != null && p.Prop.SetMethod != null)
            .OrderBy(p => p.Attr!.Order)
            .Select(p => p.Prop).ToArray();

        StringBuilder builder = new();
        if(t.IsEnum)
        {
            builder.AppendLine($"public enum {t.Name} {{");
            var names = Enum.GetNames(t);
            var values = Enum.GetValues(t);
            for (int i = 0; i < names.Length; i++)
            {
                builder.AppendLine($"    {names[i]} = {values.GetValue(i)},");
            }
            builder.AppendLine($"}}");
        }
        else
        {
            builder.AppendLine($"public class {t.Name} {{");
            foreach (var item in properties)
            {
                builder.AppendLine($"    public {item.PropertyType.Name}");
            }
            builder.AppendLine($"}}");
        }
    }

}
public class BinaryDeserializer
{
    BinaryReader Reader { get; }
    public BinaryDeserializer(Stream stream)
    {
        Reader = new BinaryReader(stream);
    }
    public sbyte ReadInt8() => Reader.ReadSByte();
    public short ReadInt16() => Reader.ReadInt16();
    public int ReadInt32() => Reader.ReadInt32();
    public long ReadInt64() => Reader.ReadInt64();

    public byte ReadUInt8() => Reader.ReadByte();
    public ushort ReadUInt16() => Reader.ReadUInt16();
    public uint ReadUInt32() => Reader.ReadUInt32();
    public ulong ReadUInt64() => Reader.ReadUInt64();

    public float ReadSingle() => Reader.ReadSingle();
    public double ReadDouble() => Reader.ReadDouble();
    
    public bool ReadBool(int size)
    {
        bool result = false;
        for(int i = 0; i < size; i++)
        {
            var next = Reader.ReadByte();
            if(next > 0)
            {
                result = true;
            }
        }
        return result;
    }
    public string ReadString(Encoding encoding, int length)
    {
        var bytes = Reader.ReadBytes(length);
        return encoding.GetString(bytes);
    }
    public string ReadCString(Encoding encoding, int? length)
    {
        if(length is int l)
        {
            var bytes = new byte[l];
            int size = 0;
            bool end = false;
            for(int i = 0; i < l; i++)
            {
                var next = Reader.ReadByte();
                bytes[i] = next;
                if(!end)
                {
                    if(next == 0)
                    {
                        end = true;
                    }
                    else
                    {
                        size++;
                    }
                }
            }
            return encoding.GetString(bytes, 0, size);
        }
        else
        {
            List<byte> bytes = new List<byte>(10);
            byte b = Reader.ReadByte();
            while (b != 0)
            {
                bytes.Add(b);
                b = Reader.ReadByte();
            }
            return encoding.GetString(bytes.ToArray());
        }
    }
}