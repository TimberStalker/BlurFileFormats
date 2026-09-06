using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using BlurFileFormats.Animations;
using BlurFileFormats.Models;
using BlurFileFormats.Shared;

namespace BlurFileFormats.UI;

public class IPF
{
    public uint MajorVersion { get; set; }
    public uint MinorVersion { get; set; }
    public List<IPFGroup> Layers { get; set; } = [];
}
public interface IPFElement
{
    string Name { get; }
}
public class IPFGroup : IPFElement
{
    public required string Name { get; set; }
    public List<IPFElement> Elements { get; set; } = [];
}
public record class IPFPolygon(
    IPFShape Shape,
    IPFFillVertex[][] FillVertexLists,
    int[][] FillIndexLists,
    IPFStrokeVertex[][] StrokeVertexLists) : IPFElement
{
    public string Name => Shape.Name;
}

public class IPFShape
{
    public required string Name { get; set; }
    public required IPFFillStyle? FillStyle { get; set; }
    public required IPFStrokeStyle StrokeStyle { get; set; }
    public required IPFBlendStyle BlendStyle { get; set; }
    public required IPFBoundingBox BoundingBox { get; set; }
    public required Vector2 PositionOffset { get; set; }
    public required Vector2 ScaleOffset { get; set; }
}
public class IPFBoundingBox
{
    public required Vector3 Min {  get; set; }
    public required Vector3 Max {  get; set; }
}
public record struct IPFFillVertex(Vector2 Position, Vector4 Normal);
public record struct IPFStrokeVertex(Vector2 Position, Vector4 Normal, Vector2 Uv);
public record class IPFColor(float Red, float Green, float Blue);
public enum IPFElementType
{
    None = 0,
    Polygon,
    Shape,
    Group,
    Path
};
public static class IPFSerializer
{
    public static IPF Read(string filePath) => Read(File.OpenRead(filePath));
    public static IPF Read(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        uint version = reader.ReadUInt32();

        uint majorVersion = (version >> 16);
        uint minorVersion = version & 0xffff;

        var ipf = new IPF();

        int layerCount = reader.ReadInt32();
        for (int i = 0; i < layerCount; i++)
        {
            var layerType = reader.ReadElementType();
            if (layerType == IPFElementType.Group)
            {
                ipf.Layers.Add(reader.ReadGroup());
            }
        }
        return ipf;
    }

    public static IPFGroup ReadGroup(this BinaryReader reader)
    {
        uint level = reader.ReadUInt32();
        string name = reader.ReadAscii();

        var group = new IPFGroup { Name = name };

        int size = reader.ReadInt32();
        for (int i = 0; i < size; i++)
        {
            var layerType = reader.ReadElementType();
            if (layerType == IPFElementType.Group)
            {
                group.Elements.Add(reader.ReadGroup());
            } else if (layerType == IPFElementType.Polygon)
            {
                group.Elements.Add(reader.ReadPolygon());
            }
        }
        return group;
    }
    public static IPFPolygon ReadPolygon(this BinaryReader reader)
    {
        IPFShape shape = reader.ReadShape();
        var fillVertexLists = reader.ReadLists(static r => r.ReadFillVertex());
        var fillIndexLists = reader.ReadLists(static r => r.ReadInt32());
        var strokeVertexLists = reader.ReadLists(static r => r.ReadStrokeVertex());

        return new(shape, fillVertexLists, fillIndexLists, strokeVertexLists);
    }

    public static T[][] ReadLists<T>(this BinaryReader reader, Func<BinaryReader, T> readElement)
    {
        int listCount = reader.ReadInt32();
        T[][] lists = new T[listCount][];
        for (int i = 0; i < listCount; i++)
        {
            int size = reader.ReadInt32();
            T[] elements = new T[size];
            for (int j = 0; j < size; j++)
            {
                elements[j] = readElement(reader);
            }
            lists[i] = elements;
        }
        return lists;
    }
    public static IPFFillVertex ReadFillVertex(this BinaryReader reader) => new (reader.ReadVector2(), reader.ReadVector4());
    public static IPFStrokeVertex ReadStrokeVertex(this BinaryReader reader) => new (reader.ReadVector2(), reader.ReadVector4(), reader.ReadVector2());
    public static IPFShape ReadShape(this BinaryReader reader)
    {
        int level = reader.ReadInt32();
        var name = reader.ReadAscii();
        var fillStyle = reader.ReadFillStyle();
        var strokeStyle = reader.ReadStrokeStyle();
        var blendStyle = reader.ReadBlendStyle();
        var boundingBox = reader.ReadBoundingBox();
        var positionOffset = reader.ReadVector2();
        var scaleOffset = reader.ReadVector2();
        return new IPFShape
        {
            Name = name,
            FillStyle = fillStyle,
            StrokeStyle = strokeStyle,
            BlendStyle = blendStyle,
            BoundingBox = boundingBox,
            PositionOffset = positionOffset,
            ScaleOffset = scaleOffset,
        };
    }
    public static IPFBlendStyle ReadBlendStyle(this BinaryReader reader) => new(reader.ReadByte() / 255f, reader.ReadBlendMode());
    public static IPFBlendMode ReadBlendMode(this BinaryReader reader) => (IPFBlendMode)reader.ReadInt32();
    public static IPFStrokeStyle ReadStrokeStyle(this BinaryReader reader) => new(reader.ReadRGB(), reader.ReadSingle());
    public static IPFFillStyle? ReadFillStyle(this BinaryReader reader)
    {
        int kind = reader.ReadInt32();
        switch (kind)
        {
            case 1:
                return new IPFSolidColorFillStyle(reader.ReadRGB());
            case 2:
                return new IPFGradientFillStyle(
                    reader.ReadInt32(),
                    reader.ReadSingle(),
                    reader.ReadVector2(),
                    reader.ReadSingle());
        }
        return null;
    }
    public static IPFColor ReadRGB(this BinaryReader reader) => new (reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    public static IPFBoundingBox ReadBoundingBox(this BinaryReader reader)
    {
        return new IPFBoundingBox
        {
            Min = reader.ReadVector3(),
            Max = reader.ReadVector3(),
        };
    }

    public static IPFElementType ReadElementType(this BinaryReader reader) => (IPFElementType)reader.ReadInt32();
}