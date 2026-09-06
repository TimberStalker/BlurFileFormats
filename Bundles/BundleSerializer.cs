using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using BlurFileFormats.Utils.Extensions;

namespace BlurFileFormats.Bundles;


public class BundleResourceHeader
{
    /// <summary>
    /// Offset to next resource, or Zero for last one
    /// </summary>
    public required ushort OffsetNext { get; set; }
    /// <summary>
    /// Offset to string entry from start of this header
    /// </summary>
    public required ushort OffsetString {get; set; }
    /// <summary>
    /// Offset to platform specific resource data from start of platform specific data
    /// </summary>
    public required uint OffsetData {get; set; }
    public required BundleResourceType Type {get; set; }
    public required ushort Flags {get; set; }
    public required uint UncompressedDataSize {get; set; }
    public required uint CompressedDataSize {get; set; }
}

public enum BundleResourceType
{
    Unknown = 0,
    Texture2D = 1,
    TextureArray2D = 2,
    Texture3D = 3,
    TextureCubeMap = 4
};

public enum HeaderCompression
{
    Uncompressed,
    Compressed,
    CompressedIndividually,
    Unknown = 0xf
}

public static class BundleSerializer
{
    public static void Read(string path) => Read(File.OpenRead(path));
    public static void Read(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        var magic = reader.ReadBytes(4);


        switch (magic)
        {
            case [(byte)'2', (byte)'R', (byte)'Z', (byte)'B']:
                reader.ReadBizzare2();
                break;
            default:
                break;
        }
    }

    public static void ReadBizzare2(this BinaryReader reader)
    {
        int version = reader.ReadByte();
        switch(version)
        {
            case 2:
                reader.ReadBizzare2Reader2();
                break;
            case 3:
                reader.ReadBizzare2Reader3();
                break;
        };
    }

    public static void ReadBizzare2Reader2(this BinaryReader reader)
    {
        byte platformCode = reader.ReadByte();
        ushort resourceInfoSize = reader.ReadUInt16();
        ushort stringTableOffset = reader.ReadUInt16();
        ushort flags = reader.ReadUInt16();
        uint uncompressedDataSize = reader.ReadUInt32();
        uint compressedDataSize = reader.ReadUInt32();
    }

    public static void ReadBizzare2Reader3(this BinaryReader reader)
    {
        byte platformCode = reader.ReadByte();
        ushort resourceInfoSize = reader.ReadUInt16();
        ushort stringTableOffset = reader.ReadUInt16();
        ushort flags = reader.ReadUInt16();
        uint uncompressedDataSize = reader.ReadUInt32();
        uint compressedDataSize = reader.ReadUInt32();

        const ushort headerCompressionMask = 0xf;

        using MemoryStream resourceInfoStream = new MemoryStream(reader.ReadBytes(resourceInfoSize));
        using BinaryReader resourceInfoReader = new BinaryReader(resourceInfoStream);


        HeaderCompression headerCompresion = (HeaderCompression)(flags & headerCompressionMask);
        switch (headerCompresion)
        {
            case HeaderCompression.Uncompressed:
                break;
            case HeaderCompression.Compressed:
                break;
            case HeaderCompression.CompressedIndividually:
                break;
            case HeaderCompression.Unknown:
                break;
            default:
                break;
        }
        List<BundleResourceHeader> headers = new();
        BundleResourceHeader? resourceHeader = null;
        do
        {
            if(resourceHeader is not null && resourceHeader.OffsetNext != 20)
            {
                resourceInfoReader.BaseStream.Seek(resourceHeader.OffsetNext - 20, SeekOrigin.Current);
            }
            resourceHeader = resourceInfoReader.ReadResourceHeader();
            headers.Add(resourceHeader);
        } while (resourceHeader.OffsetNext > 0);

        List<string> names = new(headers.Count);
        foreach (var header in headers)
        {
            resourceInfoReader.BaseStream.Seek(stringTableOffset + header.OffsetString, SeekOrigin.Begin);
            names.Add(resourceInfoReader.ReadCString());
        }
    }

    public static BundleResourceHeader ReadResourceHeader(this BinaryReader reader)
    {
        ushort offsetNext = reader.ReadUInt16();
        ushort offsetString = reader.ReadUInt16();
        uint offsetData = reader.ReadUInt32();
        BundleResourceType type = (BundleResourceType)reader.ReadUInt16();
        ushort flags = reader.ReadUInt16();
        uint uncompressedDataSize = reader.ReadUInt32();
        uint compressedDataSize = reader.ReadUInt32();

        return new BundleResourceHeader
        {
            OffsetNext = offsetNext,
            OffsetString = offsetString,
            OffsetData = offsetData,
            Type = type,
            Flags = flags,
            UncompressedDataSize = uncompressedDataSize,
            CompressedDataSize = compressedDataSize,
        };
    }
}
