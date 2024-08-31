using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.Utils.Extensions;
public static class BinaryReaderExtensions
{
    public static string ReadCStringUnicode(this BinaryReader reader)
    {
        var originalPosition = reader.BaseStream.Position;
        while(reader.ReadInt16() != 0) { }

        Span<byte> bytes = stackalloc byte[(int)(reader.BaseStream.Position - originalPosition - 2)];
        reader.BaseStream.Position = originalPosition;
        reader.Read(bytes);
        reader.Read();
        reader.Read();
        return Encoding.Unicode.GetString(bytes);
    }
    public static string ReadCString(this BinaryReader reader)
    {
        var originalPosition = reader.BaseStream.Position;
        while (reader.Read() != 0) { }
        Span<byte> bytes = stackalloc byte[(int)(reader.BaseStream.Position - originalPosition - 1)];
        reader.BaseStream.Position = originalPosition;
        reader.Read(bytes);
        reader.Read();
        return Encoding.ASCII.GetString(bytes);
    }

}
