using System;
using System.Collections.Generic;
using System.Text;

namespace BlurFileFormats.Shaders;

public class FXB
{
    public string name = "";
    public List<Technique> techniques = new List<Technique>();
    public List<FXParameter> parameters = new List<FXParameter>();
    public byte[] parameterData = [];
    public List<InputDeclaration> inputDeclarations = new List<InputDeclaration>();
    public List<FXStruct> structs = new List<FXStruct>();

}
public class Technique
{
    public string name = "";
    public List<Pass> passes = new List<Pass>();
    public List<Annotation> annotations = new List<Annotation>();
}
public class Pass
{
    public string name = "";
    public RenderState renderState;
    public Shader vertexShader = new();
    public Shader pixelShader = new();
    public List<Annotation> annotations = new List<Annotation>();
}
public class Shader
{
    public readonly FunctionBinding[] bindings = [];
    public readonly byte[] bytecode = [];
    public Shader()
    {
        
    }
    public Shader(FunctionBinding[] bindings, byte[] bytecode)
    {
        this.bindings = bindings;
        this.bytecode = bytecode;
    }
}
public class FXParameter
{
    public readonly string name;
    public readonly string semantic;
    public readonly FXParameterType parameterType;
    public readonly int count;
    public readonly int offset;
    public readonly byte unknownByte;
    public readonly List<Annotation> annotations = new List<Annotation>();

    public FXParameter(string name, string semantic, FXParameterType parameterType, int count, int offset, byte unknownByte, List<Annotation> annotations)
    {
        this.name = name;
        this.semantic = semantic;
        this.parameterType = parameterType;
        this.count = count;
        this.offset = offset;
        this.unknownByte = unknownByte;
        this.annotations = annotations;
    }
}
public class FXStruct
{
    public string name = "";
    public List<FXStructMember> members = new List<FXStructMember>();
}
public class FXStructMember
{
    public string name = "";
    public int size;
    public List<Annotation> annotations = new List<Annotation>();
}
public class InputDeclaration
{
    public FXSemantic usage;
    public int usageIndex;
    public List<Annotation> annotations = new List<Annotation>();

    public InputDeclaration()
    {
    }
}
public enum FXParameterType
{
    FLOAT = 4,
    SAMPLER = 14,
}
public struct FunctionBinding
{
    public int paramaterIndex;
    public int handle;
    public int type;
    public int unknown;
    public uint offset;
    public uint count;
    public string name;
}
public enum Blend
{
    Unknown,
    Zero,
    One,
    SrcColour,
    InvSrcColour,
    SrcAlpha,
    InvSrcAlpha,
    DstAlpha,
    InvDstAlpha,
    DstColour,
    InvDstColour
};
public enum BlendOp
{
    Unknown,
    Add,
    Subtract,
    RevSubtract,
    Min,
    Max
};
public enum CullMode
{
    Unknown,
    Default,
    None,
    Cw,
    Ccw,
    Reversed,
    Front,
    Back
}
public enum FillMode
{
    Unknown,
    Default,
    Point,
    Wireframe,
    Solid
}
public enum CmpFunc
{
    Unknown,
    Default,
    Never,
    Less,
    Equal,
    LessEqual,
    Greater,
    NotEqual,
    GreaterEqual,
    Always
}
public enum StencilOp
{
    Keep,
    Zero,
    Replace,
    IncrSat,
    DecrSat,
    Invert,
    Incr,
    Decr,
    Count
}
public enum FXSemantic
{
    Invalid = -1,
    Position = 0,
    BlendWeight = 1,
    BlendIndices = 2,
    Normal = 3,
    PSize = 4,
    TexCoord = 5,
    Tangent = 6,
    Binormal = 7,
    TessFactor = 8,
    Color = 9,
    Fog = 10,
    Depth = 11,
    Sample = 12,
}
public struct RenderState
{
    public uint alphaBlendEnable;
    public Blend srcBlend;
    public Blend destBlend;
    public BlendOp blendOp;
    public uint textureFactor;
    public uint alphaTestEnable;
    public CmpFunc alphaFunc;
    public uint alphaRef;
    public float depthBias;
    public float slopeScaleDepthBias;
    public uint ditherEnable;
    public CullMode cullMode;
    public FillMode fillMode;
    public uint shadeMode;
    public uint colorWriteEnable;
    public uint zTestEnable;
    public CmpFunc zFunc;
    public uint zWriteEnable;
    public uint stencilEnable;
    public StencilOp stencilPass;
    public StencilOp stencilFail;
    public StencilOp stencilZFail;
    public CmpFunc stencilFunc;
    public uint stencilMask;
    public uint stencilRef;
    public uint stencilWriteMask;
    public uint pointSpriteEnable;
    public float pointSize;
    public float pointSize_Min;
    public float pointSize_Max;
    public uint tessellate;
    public float tessellationLevel;
    public uint alphaToMaskEnable;
    public uint alphaToMaskOffsets;
    public uint hiZEnable;
}
public class Annotation
{
    public readonly string name;
    public readonly AnnotationValue value;

    public Annotation(string name, AnnotationValue value)
    {
        this.name = name;
        this.value = value;
    }

    public static Annotation CreateNull(string name) => new Annotation(name, NullAnnotation.instance);
    public static Annotation Create(string name, bool b) => new Annotation(name, b ? BoolAnnotation.trueAnnotation : BoolAnnotation.falseAnnotation);
    public static Annotation Create(string name, int v) => new Annotation(name, new IntAnnotation(v));
    public static Annotation Create(string name, string v) => new Annotation(name, new StringAnnotation(v));
}
public enum AnnotationType
{
    Null = 0,
    Bool = 1,
    Int = 2,
    String = 6
}
public interface AnnotationValue{
    public AnnotationType Type { get; }
}
public class NullAnnotation : AnnotationValue
{
    public static readonly NullAnnotation instance = new();
    public AnnotationType Type => AnnotationType.Null;
    private NullAnnotation()
    {
        
    }
}
public class BoolAnnotation : AnnotationValue
{
    public static readonly BoolAnnotation trueAnnotation = new(true);
    public static readonly BoolAnnotation falseAnnotation = new(false);
    public AnnotationType Type => AnnotationType.Bool;
    public readonly bool value;
    private BoolAnnotation(bool v)
    {
        value = v;
    }
}
public class IntAnnotation : AnnotationValue
{
    public AnnotationType Type => AnnotationType.Int;
    public readonly int value;
    public IntAnnotation(int v)
    {
        value = v;
    }
}
public class StringAnnotation : AnnotationValue
{
    public AnnotationType Type => AnnotationType.String;
    public readonly string value;
    public StringAnnotation(string v)
    {
        value = v;
    }
}