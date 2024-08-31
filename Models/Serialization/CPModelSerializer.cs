using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlurFileFormats.Models.Serialization;
public static partial class CPModelSerializer
{
    public static CPModel DeserializeModel(string fileName)
    {
        var bytes = File.ReadAllBytes(fileName);
        return DeserializeModel(bytes);
    }
    public static CPModel DeserializeModel(byte[] bytes)
    {
        using var headerStream = new MemoryStream(bytes);
        using var headerReader = new BinaryReader(headerStream);

        var formatBytes = headerReader.ReadChars(4);
        if (formatBytes is not [' ', ' ', 'C', 'P'])
        {
            throw new Exception($"{string.Concat(formatBytes)} is not supported for cpmodel files.");
        }

        var modelSection = headerReader.ReadSection();
        Debug.WriteLine(modelSection);
        var modelheaderSection = headerReader.ReadSection();
        Debug.WriteLine(modelheaderSection);

        // Skip
        headerReader.ReadInt32();

        var mdlDataSection = headerReader.ReadSection();
        Debug.WriteLine(mdlDataSection);
        var mdlDataHeaderSection = headerReader.ReadSection();
        Debug.WriteLine(mdlDataHeaderSection);

        // Skip
        headerReader.ReadInt32();

        int modelCount = headerReader.ReadInt32();
        Debug.WriteLine($"Models: {modelCount}");
        int elementCount = headerReader.ReadInt32();
        Debug.WriteLine($"Elements: {elementCount}");

        // Skip
        headerReader.ReadInt32();

        var modelBoundingBox = headerReader.ReadBoundingBox();
        Debug.WriteLine($"Model BB: {modelBoundingBox}");


        var stringTableSection = headerReader.ReadSection();
        Debug.WriteLine(stringTableSection);

        int stringCount = headerReader.ReadInt32();
        Debug.WriteLine($"StringCount: {stringCount}");

        int[] stringOffsets = new int[stringCount];
        for(int i = 0; i < stringCount; i++)
        {
            stringOffsets[i] = headerReader.ReadInt32();
        }

        int stringCharCount = headerReader.ReadInt32();
        var stringCharacters = headerReader.ReadChars(stringCharCount);

        string[] strings = new string[stringCount];
        for(int i = 0; i < stringCount; i++)
        {
            strings[i] = stringCharacters.GetTerminatedString(stringOffsets[i]);
            Debug.WriteLine(strings[i]);
        }

        var modelsSection = headerReader.ReadSection();
        Debug.WriteLine(modelsSection);

        for (int i = 0; i < modelCount; i++)
        {
            var modelMatrix = headerReader.ReadMatrix();
            var modelBB = headerReader.ReadBoundingBox();

            var nameIndex = headerReader.ReadInt32();
            var modelIndex = headerReader.ReadInt32();
            var childElementCount = headerReader.ReadInt32();
            var hierarchyIndex = headerReader.ReadInt32();
            var modelParent = headerReader.ReadInt32();
            
            var unknown2 = headerReader.ReadInt32();
            var unknown3 = headerReader.ReadInt32();

            Debug.WriteLine($"Model {i}");
            Debug.IndentLevel++;
            Debug.WriteLine(modelMatrix.ToString().Replace("\n", "\n\t"));
            Debug.WriteLine(modelBB);
            Debug.WriteLine($"Name: {strings[nameIndex]}");
            Debug.WriteLine($"Model Index: {modelIndex}");
            Debug.WriteLine($"Child Element Count: {childElementCount}");
            Debug.WriteLine($"Hierarchy Index: {hierarchyIndex}");
            Debug.WriteLine($"Model Parent: {modelParent}");

            Debug.WriteLine($"Unknown 2: {unknown2}");
            Debug.WriteLine($"Unknown 3: {unknown3}");
            Debug.IndentLevel--;
        }

        var elementsSection = headerReader.ReadSection();
        Debug.WriteLine(elementsSection);

        for (int i = 0; i < elementCount; i++)
        {
            int modelIndex = headerReader.ReadInt32();

            var elementMatrix = headerReader.ReadMatrix();
            var elementBB = headerReader.ReadBoundingBox();

            var nameIndex = headerReader.ReadInt32();

            var elementIndex = headerReader.ReadInt32();
            var parentIndex = headerReader.ReadInt32();

            var unknown1 = headerReader.ReadInt32();
            var unknown2 = headerReader.ReadInt32();
            var unknown3 = headerReader.ReadInt32();

            var unknown4_0 = headerReader.ReadInt16();
            var unknown4_1 = headerReader.ReadInt16();

            Debug.WriteLine($"Element {i}");
            Debug.IndentLevel++;
            Debug.WriteLine($"Model Index: {modelIndex}");
            Debug.WriteLine(elementMatrix.ToString().Replace("\n", "\n\t"));
            Debug.WriteLine(elementBB);
            Debug.WriteLine($"Name: {strings[nameIndex]}");
            Debug.WriteLine($"Element Index: {elementIndex}");
            Debug.WriteLine($"Parent Index: {parentIndex}");

            Debug.WriteLine($"Unknown 1: {unknown1}");
            Debug.WriteLine($"Unknown 2: {unknown2}");
            Debug.WriteLine($"Unknown 3: {unknown3}");
            Debug.WriteLine($"Unknown 4_0: {unknown4_0}");
            Debug.WriteLine($"Unknown 4_1: {unknown4_1}");
            Debug.IndentLevel--;
        }

        var constrSection = headerReader.ReadSection();
        Debug.WriteLine(constrSection);

        var render1Section = headerReader.ReadSection();
        Debug.WriteLine(render1Section);

        var render2Section = headerReader.ReadSection();
        Debug.WriteLine(render2Section);

        var renderHeader = headerReader.ReadSection();
        Debug.WriteLine(renderHeader);

        //headerStream.Position = render2Section.Start + render2Section.Length;

        //var collHeader = headerReader.ReadSection();
        //Debug.WriteLine(collHeader);
        //var headHeader = headerReader.ReadSection();
        //Debug.WriteLine(headHeader);
        //return null;
        
        // Skip
        headerReader.ReadInt32();

        var sceneHeader = headerReader.ReadSection();
        Debug.WriteLine(sceneHeader);


        headerReader.ReadInt32(); // ARCH
        headerReader.ReadInt32(); // 1
        headerReader.ReadInt32(); // 0

        headerReader.ReadInt32(); // ARCH
        headerReader.ReadInt32(); // 0
        headerReader.ReadInt32(); // 1

        headerReader.ReadInt32(); // 52410100 // 1
        headerReader.ReadInt32(); // 52410000 // 2
        headerReader.ReadInt32(); // 02000000 // 3

        headerReader.ReadInt32(); // 02000000 // 4
        headerReader.ReadInt32(); // 00000000 // 5

        headerReader.ReadInt32(); // 52410000 // 6
        headerReader.ReadInt32(); // 52410200 // 7

        headerReader.ReadInt32(); // 52410000 // 8
        headerReader.ReadInt32(); // 00000000 // 9

        headerReader.ReadInt32(); // 52410000 // 10

        int archDataCount = headerReader.ReadInt32();
        for(int i = 0; i < archDataCount; i++)
        {
            int unknown1 = headerReader.ReadInt32();
            int unknown2 = headerReader.ReadInt32();

            Debug.WriteLine($"Arch ({i}): {unknown1}-{unknown2}");
        }

        headerReader.ReadInt32(); // FFFFFFFFFFFF // 11
        headerReader.ReadInt32(); // FFFFFFFFFFFF // 12
        headerReader.ReadInt32(); // FFFFFFFFFFFF // 13

        headerReader.ReadInt32(); // 00000000 // 14
        headerReader.ReadInt32(); // 52410000 // 15

        headerReader.ReadInt32(); // Unknown // 16
        headerReader.ReadInt32(); // 17
        headerReader.ReadInt32(); // 18
        headerReader.ReadInt32(); // 19
        headerReader.ReadInt32(); // 20
        headerReader.ReadInt32(); // 21
        headerReader.ReadInt32(); // 22
        headerReader.ReadInt32(); // 23

        headerReader.ReadInt32(); //52410000 // 24
        headerReader.ReadInt32(); //Unknown // 25

        headerReader.ReadInt32(); //52410000 // 26

        int ffCount = headerReader.ReadInt32();
        headerReader.ReadBytes(ffCount + 4);

        headerReader.ReadInt32(); //52410000 // 28
        headerReader.ReadInt32(); //00000000 // 29

        headerReader.ReadInt32(); //52410000 // 30
        headerReader.ReadInt32(); //00000000 // 31

        headerReader.ReadInt32(); //52410000 // 32

        int vertexDefinitionCount = headerReader.ReadInt32();
        for(int i = 0; i < vertexDefinitionCount; i++)
        {
            int skip = headerReader.ReadInt32(); //Skip

            int definitionCount = headerReader.ReadInt32();
            Debug.WriteLine($"Vertex Definition({i}) : {skip:X}");
            Debug.IndentLevel++;
            for (int j = 0; j < definitionCount; j++)
            {
                int unknown1 = headerReader.ReadInt32(); //Skip
                int typePrefix = headerReader.ReadInt16();
                int offset = headerReader.ReadInt16();
                int dataType = headerReader.ReadInt32();
                int unknown2 = headerReader.ReadInt32(); //Skip
                int channel = headerReader.ReadInt32();
                int subChannel = headerReader.ReadByte();
                
                Debug.WriteLine($"{dataType:X}   {typePrefix}-{offset,-2}   {channel}-{subChannel} | {unknown1:X}, {unknown2:X}");
            }
            Debug.IndentLevel--;
        }

        headerReader.ReadInt32(); //Skip // 33

        int fxFileCount = headerReader.ReadInt32();
        for(int i = 0; i < fxFileCount; i++)
        {
            int unknown1 = headerReader.ReadInt32();
            int unknown2 = headerReader.ReadInt32();

            int nameLength = headerReader.ReadInt32();
            string fileName = Encoding.UTF8.GetString(headerReader.ReadBytes(nameLength));
            Debug.WriteLine($"{unknown1:X}:{unknown2:X} {fileName}");
        }

        headerReader.ReadInt32(); //Skip // 34
        headerReader.ReadInt32(); //52410000 // 35
        headerReader.ReadInt32(); //52410000 // 36
        headerReader.ReadInt32(); //02000000 // 37

        int textureCount = headerReader.ReadInt32();
        for(int i = 0; i < textureCount; i++)
        {
            int nameLength = headerReader.ReadInt32();
            string name = Encoding.UTF8.GetString(headerReader.ReadBytes(nameLength));

            int unknown1 = headerReader.ReadInt32(); // 1

            headerReader.ReadInt32(); //52410000 // 2
            headerReader.ReadInt32(); //52410000 // 3
            headerReader.ReadInt32(); //02000000 // 4

            int name2Length = headerReader.ReadInt32();
            string name2 = Encoding.UTF8.GetString(headerReader.ReadBytes(nameLength));
            int unknown2 = headerReader.ReadInt32(); // 5

            int tu1 = headerReader.ReadInt32(); // 6
            int tu2 = headerReader.ReadInt32(); // 7
            int tu3 = headerReader.ReadInt32(); // 8
            int tu4 = headerReader.ReadInt32(); // 9
            int tu5 = headerReader.ReadInt32(); // 10
            int tu6 = headerReader.ReadInt32(); // 11
            int tu7 = headerReader.ReadInt32(); // 12
            int tu8 = headerReader.ReadInt32(); // 13
            int tu9 = headerReader.ReadInt32(); // 14
            int tu10 = headerReader.ReadInt32(); // 15
            int tu11 = headerReader.ReadInt32(); // 16

            int length = headerReader.ReadInt32();
            int height = headerReader.ReadInt32();
            int width = headerReader.ReadInt32();

            int tu12 = headerReader.ReadInt32(); // 17

            int mipmaps = headerReader.ReadInt32();
            int dxtVer = headerReader.ReadInt32();

            int tu13 = headerReader.ReadInt32(); // 18
            int tu14 = headerReader.ReadInt32(); // 19

            int pitch = ((width * 1024 + 7) / 8);
            byte[] textData = headerReader.ReadBytes(length - 0x1C);

            Debug.WriteLine($"Texture: {name},{name2}");
            Debug.WriteLine($"\t{dxtVer} {width}x{height} {mipmaps}m");

        }

        headerReader.ReadInt32(); //52410000 // 38
        headerReader.ReadInt32(); //52410300 // 39

        headerReader.ReadInt32(); //52410000 // 40
        headerReader.ReadInt32(); //00000000 // 41

        headerReader.ReadInt32(); //52410000 // 42
        headerReader.ReadInt32(); //00000000 // 43

        headerReader.ReadInt32(); //52410000 // 44
        headerReader.ReadInt32(); //00000000 // 45

        headerReader.ReadInt32(); //52410000 // 46
        headerReader.ReadInt32(); //00000000 // 47

        headerReader.ReadInt32(); //52410000 // 48
        headerReader.ReadInt32(); //00000000 // 49

        headerReader.ReadInt32(); //01000000 // 50


        headerReader.ReadInt32(); //52410000 // 51
        headerReader.ReadInt32(); //03000000 // 52
        headerReader.ReadBytes(7); // 0000002 // 53

        headerReader.ReadInt32(); //52410000 // 54
        headerReader.ReadInt32(); //52410000 // 55


        headerReader.ReadInt32(); //52410000 // 56
        headerReader.ReadInt32(); //00000000 // 57
        headerReader.ReadInt32(); //00000000 // 58
        headerReader.ReadInt32(); //00000000 // 59

        headerReader.ReadInt32(); //Unknown // 60
        
        headerReader.ReadInt32(); //52410000 // 61
        headerReader.ReadInt32(); //52410000 // 62
        
        headerReader.ReadInt32(); //02000000 // 63
        headerReader.ReadInt32(); //0A000000 // 64

        headerReader.ReadInt32(); //52410000 // 65
        headerReader.ReadInt32(); //52410000 // 66

        headerReader.ReadInt32(); //02000000 // 67
        headerReader.ReadInt32(); //00000000 // 68

        headerReader.ReadInt32(); //52410000 // 69

        int vertextStreamCount = headerReader.ReadInt32();
        for(int i = 0; i < vertextStreamCount; i ++)
        {
            int id = headerReader.ReadByte();
            headerReader.ReadInt32(); //52410100 // 1
            headerReader.ReadInt32(); //52410000 // 2

            int byteLength = headerReader.ReadInt32();

            headerReader.ReadInt32(); //52410000 // 3

            Debug.WriteLine($"Vertex Stream({i}): {id}");
            Debug.WriteLine("Definitions");
            Debug.IndentLevel++;
            int vertexStreamDefinitionCount = headerReader.ReadInt32();
            for (int j = 0; j < vertextStreamCount; j++)
            {
                headerReader.ReadInt32(); //52410000

                int dataType = headerReader.ReadInt32();
                int unknown = headerReader.ReadInt32();
                int channel = headerReader.ReadInt32();
                int subChannel = headerReader.ReadInt32();

                Debug.WriteLine($"{dataType} {unknown} {channel}-{subChannel}");
            }
            Debug.IndentLevel--;
            headerReader.ReadInt32(); //52410000

            int vertexOffsetCount = headerReader.ReadInt32();
            int[] vertexOffsets = new int[vertexOffsetCount];
            for(int j = 0; j < vertexOffsetCount; j++)
            {
                vertexOffsets[j] = headerReader.ReadInt16();
            }

            int vertexCount = headerReader.ReadInt32();


            headerReader.ReadInt32(); //52410000
            int vertexStreamLength = headerReader.ReadInt32();
            headerReader.ReadInt32(); //52410000
            Debug.WriteLine($"Position: {headerStream.Position:X}");
            headerStream.Position += vertexStreamLength;
            if(i == vertextStreamCount - 1)
            {
                //headerStream.Seek()
            }
            else
            {

            }
        }

        return new CPModel();
    }

    static Section ReadSection(this BinaryReader reader)
    {
        int start = (int)reader.BaseStream.Position;
        var charName = reader.ReadChars(8);
        int length = reader.ReadInt32();
        short marker1 = reader.ReadInt16();
        short marker2 = reader.ReadInt16();
        return new Section(charName.GetTerminatedString(0), start, length, marker1, marker2);
    }
    static BoundingBox ReadBoundingBox(this BinaryReader reader)
    {
        return new BoundingBox(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }
    static Matrix ReadMatrix(this BinaryReader reader)
    {
        return new Matrix()
        {
            M00 = reader.ReadSingle(), M01 = reader.ReadSingle(), M02 = reader.ReadSingle(),
            M10 = reader.ReadSingle(), M11 = reader.ReadSingle(), M12 = reader.ReadSingle(),
            M20 = reader.ReadSingle(), M21 = reader.ReadSingle(), M22 = reader.ReadSingle(),
            M30 = reader.ReadSingle(), M31 = reader.ReadSingle(), M32 = reader.ReadSingle(),
        };
    }
    
    public static string GetTerminatedString(this char[] chars, int offset = 0)
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
