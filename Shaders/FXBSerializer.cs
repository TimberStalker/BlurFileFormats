using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using BlurFileFormats.FlaskReflection;
using BlurFileFormats.Models;
using BlurFileFormats.Shared;

namespace BlurFileFormats.Shaders;

public static class FXBSerializer
{
    public static FXB Import(string filePath)
    {
        var stream = File.OpenRead(filePath);
        return Import(stream);
    }
    public static FXB Import(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.ASCII);

        var magic = reader.ReadUInt32();
        if (magic != 0x46584232) throw new Exception("Not a CPModel file");

        var fxbHeader = reader.ReadBlockHeader();
        var pcfxHeader = reader.ReadBlockHeader();

        int version = reader.ReadInt32();
        if (version != 9) throw new Exception($"Unsupported FXB Version: {version}");

        string name = reader.ReadAscii();
        List<Technique> techniques = reader.ReadSequence(ReadTechnique);
        List<FXParameter> parameters = reader.ReadSequence(ReadParameter);
        int parameterData = reader.ReadInt32();
        byte[] data = reader.ReadBytes(parameterData * 4);
        List<InputDeclaration> inputDeclaration = reader.ReadSequence(ReadInputDeclaration);
        List<FXStruct> structs = reader.ReadSequence(ReadStruct);
        return new FXB
        {
            name = name,
            techniques = techniques,
            parameters = parameters,
            parameterData = data,
            inputDeclarations = inputDeclaration,
            structs = structs
        };
    }
    static FXStruct ReadStruct(this BinaryReader reader)
    {
        string name = reader.ReadAscii();
        var members = reader.ReadSequence(ReadStructMember);
        return new FXStruct
        {
            name = name,
            members = members,
        };
    }
    static FXStructMember ReadStructMember(this BinaryReader reader)
    {
        string name = reader.ReadAscii();
        int size = reader.ReadInt32();
        List<Annotation> annotations = reader.ReadSequence(ReadAnnotation);
        return new FXStructMember
        {
            name = name,
            size = size,
            annotations = annotations
        };
    }
    static InputDeclaration ReadInputDeclaration(this BinaryReader reader)
    {
        FXSemantic usage = (FXSemantic)reader.ReadInt32();
        int usageIndex = reader.ReadInt32();
        List<Annotation> annotations = reader.ReadSequence(ReadAnnotation);
        return new InputDeclaration
        {
            usage = usage,
            usageIndex = usageIndex,
            annotations = annotations
        };
    }
    static FXParameter ReadParameter(this BinaryReader reader)
    {
        string name = reader.ReadAscii();
        string semantic = reader.ReadAscii();
        FXParameterType parameterType = (FXParameterType)reader.ReadInt32();
        int count = reader.ReadInt32();
        int offset = reader.ReadInt32();
        byte unknown = reader.ReadByte();
        List<Annotation> annotations = reader.ReadSequence(ReadAnnotation);
        return new FXParameter(name, semantic, parameterType, count, offset, unknown, annotations);
    }
    static Technique ReadTechnique(this BinaryReader reader)
    {
        string name = reader.ReadAscii();
        List<Pass> passes = reader.ReadSequence(ReadPass);
        List<Annotation> annotations = reader.ReadSequence(ReadAnnotation);
        return new Technique()
        {
            name = name,
            passes = passes,
            annotations = annotations
        };
    }

    static Pass ReadPass(this BinaryReader reader)
    {
        string name = reader.ReadAscii();
        var renderState = reader.ReadRenderState();
        var vertexShader = reader.ReadShader();
        var pixelShader = reader.ReadShader();
        var annotations = reader.ReadSequence(ReadAnnotation);
        return new Pass()
        {
            name = name,
            renderState = renderState,
            vertexShader = vertexShader,
            pixelShader = pixelShader,
            annotations = annotations,
        };
    }
    static Shader ReadShader(this BinaryReader reader)
    {
        int bytecodeSize = reader.ReadInt32();
        var functionBindings = reader.ReadArray(ReadFunctionBinding);
        byte[] bytecode = reader.ReadBytes(bytecodeSize);
        return new Shader(functionBindings, bytecode);
    }
    static Annotation ReadAnnotation(this BinaryReader reader)
    {
        string name = reader.ReadAscii();
        AnnotationType type = (AnnotationType)reader.ReadUInt32();
        return type switch
        {
            AnnotationType.Null => Annotation.CreateNull(name),
            AnnotationType.Bool => Annotation.Create(name, reader.ReadBoolean()),
            AnnotationType.Int => Annotation.Create(name, reader.ReadInt32()),
            AnnotationType.String => Annotation.Create(name, reader.ReadAscii()),
            _ => throw new Exception($"Annotation type not supported: {type}")
        };
    }
    static FunctionBinding ReadFunctionBinding(this BinaryReader reader)
    {
        return new FunctionBinding()
        {
            paramaterIndex = reader.ReadInt32(),
            handle = reader.ReadInt32(),
            type = reader.ReadInt32(),
            unknown = reader.ReadInt32(),
            offset = reader.ReadUInt32(),
            count = reader.ReadUInt32(),
            name = reader.ReadAscii(),
        };
    }
    static RenderState ReadRenderState(this BinaryReader reader)
    {
        return new RenderState
        {
            alphaBlendEnable = reader.ReadUInt32(),
            srcBlend = (Blend)reader.ReadUInt32(),
            destBlend = (Blend)reader.ReadUInt32(),
            blendOp = (BlendOp)reader.ReadUInt32(),
            textureFactor = reader.ReadUInt32(),
            alphaTestEnable = reader.ReadUInt32(),
            alphaFunc = (CmpFunc)reader.ReadUInt32(),
            alphaRef = reader.ReadUInt32(),
            depthBias = reader.ReadSingle(),
            slopeScaleDepthBias = reader.ReadSingle(),
            ditherEnable = reader.ReadUInt32(),
            cullMode = (CullMode)reader.ReadUInt32(),
            fillMode = (FillMode)reader.ReadUInt32(),
            shadeMode = reader.ReadUInt32(),
            colorWriteEnable = reader.ReadUInt32(),
            zTestEnable = reader.ReadUInt32(),
            zFunc = (CmpFunc)reader.ReadUInt32(),
            zWriteEnable = reader.ReadUInt32(),
            stencilEnable = reader.ReadUInt32(),
            stencilPass = (StencilOp)reader.ReadUInt32(),
            stencilFail = (StencilOp)reader.ReadUInt32(),
            stencilZFail = (StencilOp)reader.ReadUInt32(),
            stencilFunc = (CmpFunc)reader.ReadUInt32(),
            stencilMask = reader.ReadUInt32(),
            stencilRef = reader.ReadUInt32(),
            stencilWriteMask = reader.ReadUInt32(),
            pointSpriteEnable = reader.ReadUInt32(),
            pointSize = reader.ReadSingle(),
            pointSize_Min = reader.ReadSingle(),
            pointSize_Max = reader.ReadSingle(),
            tessellate = reader.ReadUInt32(),
            tessellationLevel = reader.ReadSingle(),
            alphaToMaskEnable = reader.ReadUInt32(),
            alphaToMaskOffsets = reader.ReadUInt32(),
            hiZEnable = reader.ReadUInt32(),
        };
    }
}
