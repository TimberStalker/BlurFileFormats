using System.Numerics;
using System.Text;

namespace BlurFileFormats.Shared;

public static class BinaryReaderExtensions
{
    public static ArchiveHeader ReadArchHeader(this BinaryReader reader)
    {
        ushort type = reader.ReadUInt16();
        ushort version = reader.ReadUInt16();
        return new ArchiveHeader { type = type, version = version };
    }

    public static BlockHeader ReadBlockHeader(this BinaryReader reader)
    {
        Span<char> name = stackalloc char[8];
        reader.Read(name);
        uint size = reader.ReadUInt32();
        ushort count = reader.ReadUInt16();
        byte type = reader.ReadByte();
        byte compressionType = reader.ReadByte();
        return new BlockHeader
        {
            name = name.Trim().ToString(),
            size = size,
            count = count,
            type = type,
            compressionType = compressionType,
        };
    }
    public static ResourceIndex ReadResourceIndex(this BinaryReader reader)
    {
        var block = reader.ReadInt32();
        var index = reader.ReadInt32();
        return new ResourceIndex { block = block, index = index };
    }
    public static string ReadAscii(this BinaryReader reader)
    {
        int length = reader.ReadInt32();
        if (length <= 0)
        {
            return "";
        }
        return Encoding.ASCII.GetString(reader.ReadBytes(length));
    }
    public static List<T> ReadSequence<T>(this BinaryReader reader, Func<BinaryReader, T> operation)
    {
        int size = reader.ReadInt32();
        List<T> items = new List<T>(size);
        for (int i = 0; i < size; i++)
        {
            items.Add(operation(reader));
        }
        return items;
    }
    public static T[] ReadArray<T>(this BinaryReader reader, Func<BinaryReader, T> operation)
    {
        int size = reader.ReadInt32();
        T[] items = new T[size];
        for (int i = 0; i < size; i++)
        {
            items[i] = operation(reader);
        }
        return items;
    }
    public static Vector2 ReadVector2(this BinaryReader reader)
    {
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        return new Vector2(x, y);
    }
    public static Vector3 ReadVector3(this BinaryReader reader)
    {
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        float z = reader.ReadSingle();
        return new Vector3(x, y, z);
    }

    public static Vector4 ReadVector4(this BinaryReader reader)
    {
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        float z = reader.ReadSingle();
        float w = reader.ReadSingle();
        return new Vector4(x, y, z, w);
    }

    public static Quaternion ReadQuaternion(this BinaryReader reader)
    {
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        float z = reader.ReadSingle();
        float w = reader.ReadSingle();
        return new Quaternion(x, y, z, w);
    }
}