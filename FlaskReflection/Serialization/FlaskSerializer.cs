using BlurFormats.Utils;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.FlaskReflection.Serialization;
public static class FlaskSerializer
{
    public static Flask DeserializeFlask(string file)
    {
        var filestream = File.ReadAllBytes(file);
        return DeserializeFlask(filestream);
    }
    public static Flask DeserializeFlask(byte[] bytes)
    {
        using var headerStream = new MemoryStream(bytes);
        using BinaryReader headerReader = new BinaryReader(headerStream, Encoding.ASCII, true);

        var formatBytes = headerReader.ReadChars(4);
        if (formatBytes is not ['K', 'S', 'L', 'F'])
        {
            throw new Exception($"{string.Concat(formatBytes)} is not supported for flask files.");
        }
        var version = headerReader.ReadInt32();
        if (version != 2)
        {
            throw new Exception($"Version {version} is not supported for localization files.");
        }

        var dataTypesHeader = new Header(headerReader.ReadUInt32(), headerReader.ReadUInt32());
        var inheritenceHeader = new Header(headerReader.ReadUInt32(), headerReader.ReadUInt32());
        var fieldsHeader = new Header(headerReader.ReadUInt32(), headerReader.ReadUInt32());
        var typeNamesHeader = new Header(headerReader.ReadUInt32(), headerReader.ReadUInt32());
        var recordsHeader = new Header(headerReader.ReadUInt32(), headerReader.ReadUInt32());
        var entitiesHeader = new Header(headerReader.ReadUInt32(), headerReader.ReadUInt32());
        var blocksHeader = new Header(headerReader.ReadUInt32(), headerReader.ReadUInt32());
        var dataHeader = new Header(headerReader.ReadUInt32(), headerReader.ReadUInt32());


        using var typeStream = new MemoryStream(bytes, (int)dataTypesHeader.Start, (int)dataTypesHeader.Length * 12);
        using var typeReader = new BinaryReader(typeStream);

        using var inheritenceStream = new MemoryStream(bytes, (int)inheritenceHeader.Start, (int)inheritenceHeader.Length * 4);
        using var inheritenceReader = new BinaryReader(inheritenceStream);

        using var fieldsStream = new MemoryStream(bytes, (int)fieldsHeader.Start, (int)fieldsHeader.Length * 8);
        using var fieldsReader = new BinaryReader(fieldsStream);

        using var typeNamesStream = new MemoryStream(bytes, (int)typeNamesHeader.Start, (int)typeNamesHeader.Length);
        using var typeNamesReader = new BinaryReader(typeNamesStream);

        var typeNames = new FlaskEncoding().GetChars(typeNamesReader.ReadBytes((int)typeNamesHeader.Length));

        for (int i = 0; i < dataTypesHeader.Length; i++)
        {
            var stringOffset = typeReader.ReadInt16();
            var fieldCount = typeReader.ReadInt16();
            var hasBase = typeReader.ReadInt16();
            var structureType = typeReader.ReadInt16();
            var primitiveType = typeReader.ReadInt16();
            var size = typeReader.ReadInt16();

            for (int j = 0; j < fieldCount; j++)
            {
                var nameOffset = fieldsReader.ReadInt16();
                var baseType = fieldsReader.ReadInt16();
                var offset = fieldsReader.ReadInt16();
                var fieldType = fieldsReader.ReadByte();
                var isArray = fieldsReader.ReadByte();
            }
            if (hasBase == 1)
            {
                var parent = inheritenceReader.ReadInt32();
            }
        }


        using var recordStream = new MemoryStream(bytes, (int)dataTypesHeader.Start, (int)dataTypesHeader.Length * 12);
        using var recordReader = new BinaryReader(recordStream);

        using var entityStream = new MemoryStream(bytes, (int)dataTypesHeader.Start, (int)dataTypesHeader.Length * 12);
        using var entityReader = new BinaryReader(entityStream);
        
        using var blockStream = new MemoryStream(bytes, (int)dataTypesHeader.Start, (int)dataTypesHeader.Length * 12);
        using var blockReader = new BinaryReader(blockStream);
        
        using var dataStream = new MemoryStream(bytes, (int)dataTypesHeader.Start, (int)dataTypesHeader.Length * 12);
        using var dataReader = new BinaryReader(dataStream);


        for (int i = 0; i < recordsHeader.Length; i++)
        {
            uint id = headerReader.ReadUInt32();
            short baseType = headerReader.ReadInt16();
            short entity = headerReader.ReadInt16();
            if(entity == -1)
            {

            }
            else
            {
                int record = headerReader.ReadInt32();
                int length = headerReader.ReadInt32();
                int size = headerReader.ReadInt32();
                int namesLength = headerReader.ReadInt32();
            }
        }
        return new Flask();
    }
    public static string GetTerminatedString(this char[] chars, int offset)
    {
        if (offset < 0 || offset >= chars.Length || chars[offset] == (char)0) return "";
        var builder = new StringBuilder();

        char nextchar;
        while ((nextchar = chars[offset + builder.Length]) != (char)0)
        {
            builder.Append(nextchar);
        }
        return builder.ToString();
    }
}