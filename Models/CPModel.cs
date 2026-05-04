using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BlurFileFormats.Shared;

namespace BlurFileFormats.Models;

public class CPModel
{
    public Model[] Models { get; } = [];
    public Element[] Elements { get; } = [];
    public Scene Scene { get; }
    public Shape[] Shapes { get; }

    public CPModel(Model[] models, Element[] elements, Scene scene, Shape[] shapes)
    {
        Models = models;
        Elements = elements;
        Scene = scene;
        Shapes = shapes;
    }
}
struct OwnedVertexElement
{
    public ushort stream;
    public ushort offset;
    public VertexElementType type;
    public VertexElementMethod method;
    public VertexElementUsage usage;
    public sbyte usageIndex;
}
public struct VertexElement
{
    public VertexElementType type;
    public VertexElementMethod method;
    public VertexElementUsage usage;
    public int usageIndex;
};
public class VertexDeclaration
{
    public VertexElement[] elements = [];
};
public enum ColliderType
{
    TriangleMesh_Shape = 0,
    KDTriangleMesh_Shape = 1,
    Box_Shape = 2,
    ConvexHull_Shape = 3,
    Sphere_Shape = 4,
    Capsule_Shape = 5,
    Cylinder_Shape = 6,
    Composite_Shape = 7,
    CharacterController_Shape = 8,
    Particle_Shape = 9,
    InstancedTriangleMesh_Shape = 0xa,
    TriangleMeshBase_Shape = 0xb,
    Cloth_Shape = 0xc,
    Fluid_Shape = 0xd,
    Plane_Shape = 0xe,
    Cone_Shape = 0xf,
    Rope_Shape = 0x10,
    FluidMesh_Shape = 0x11,
    Continuous_Shape = 0x12,
    NoCollisionRope_Shape = 0x13,
    NoCollisionCloth_Shape = 0x14,
    Force_DWord = 0x7fffffff,
    End = 0x0fffffff
}
public struct ShapeData
{
    public float margin;
    public float epsilon;
    public uint physicsMaterialIndex;
    public uint effectMaterialIndex;
    public uint collisionFlags;
    public uint nodeFlags;
    public float mass;
    public int originalExportId;
    public Vector3 renderOffset;
    public uint remapped;
    public uint userDataFlag;
}
public abstract class ShapeDesc
{
    public abstract ColliderType ColliderType { get; }
    public ShapeData data;
}
public class TriangleMeshShapeDesc : ShapeDesc
{
    public override ColliderType ColliderType => ColliderType.TriangleMesh_Shape;
    public TriangleMesh mesh = new();
}
public class ConvexHullShapeDesc : ShapeDesc
{
    public override ColliderType ColliderType => ColliderType.ConvexHull_Shape;
    public ConvexHull hull = new();
}
public class ConvexHull
{
    public Vector3[] vertices = [];
    public int[] triangles = [];
}
public class CompositeShapeDesc : ShapeDesc
{
    public override ColliderType ColliderType => ColliderType.Composite_Shape;
    public uint constraintBodyId;
    public Shape[] shapes = [];

    public class Shape
    {
        public ShapeDesc shapeDesc;
        public float mass;
        public Matrix4x4 relativeTransform;
        public Matrix4x4 inertiaTensor;
    }
}
public class Shape
{
    public ShapeDesc shapeDesc;
    public Matrix4x4 relativeTransform;
    public Matrix4x4 inertiaTensor;
}
public class Scene
{
    public PlatformType platformType;
    public string name;
    public CullNode rootNode;
    public string[] effects;
    public DXTTexture[] textures;
    public MaterialList?[] dynamicMaterialLists;
    public ResourceBlock[] resourceBlocks;

    public Scene(PlatformType platformType,
        string name,
        CullNode rootNode,
        string[] effects,
        DXTTexture[] textures,
        MaterialList?[] dynamicMaterialLists,
        ResourceBlock[] resourceBlocks)
    {
        this.platformType = platformType;
        this.name = name;
        this.rootNode = rootNode;
        this.effects = effects;
        this.textures = textures;
        this.dynamicMaterialLists = dynamicMaterialLists;
        this.resourceBlocks = resourceBlocks;
    }
}
public class VertexBuffer
{
    public VertexDeclaration declaration = new VertexDeclaration();
    public int vertexCount;
    public byte[] data = [];
    public short[] offsets = [];
    public int vertexSize;
}
public class IndexBuffer
{
    public IndexType indexType;
    public byte[] data = [];
    public uint IndexSize => indexType switch
    {
        IndexType.U16 => 2,
        IndexType.U32 => 4,
        _ => throw new InvalidOperationException($"Unknown index type: {indexType}")
    };
    public uint IndexCount => (uint)data.LongLength / IndexSize;
};
public enum PlatformType
{
    Unknown = -1,
    X360 = 0,
    PS3 = 1,
    PC = 2
}
public enum VertexElementType
{
    Vec2S16,
    Vec4S16,
    Vec2NS16,
    Vec4NS16,
    F32,
    Vec2F32,
    Vec3F32,
    Vec4F32,
    Vec2F16,
    Vec4F16,
    Vec4U8,
    Vec4NU8,
    Vec3NSHHD,
    Vec3NSDDD
};
public enum VertexElementMethod
{
    Default
};

public enum VertexElementUsage
{
    Position,
    BlendWeights,
    BlendIndeces,
    Normal,
    PSize,
    Texcoord,
    Tangent,
    Binormal,
    TesselationFactor,
    Color,
    Sample
};
public enum IndexType
{
    U16,
    U32
};

public enum CompressionType
{
    None,
    ZLib,
    XMem
}
public abstract class RenderingNode
{
    public string name = "";
    public ResourceIndex[] subNodes = [];
    public ResourceIndex parentIndex;
    public ResourceIndex location;
 
    
    public abstract string Type { get; }
}
public class CullNode : RenderingNode
{

    public RangedBoundingBox bounds = new();
    public int groupIndex;
    public override string Type => "RenderingData::CullNode";
    public BitArray pvsBits;
    public BitArray portalBits;
}

public class CommonRenderListNode : CullNode
{
    public override string Type => "RenderingData::RenderListNode_Common";
    public MaterialList constantMaterialTypes = new MaterialList();
    public Primitive[] primitives = [];
    public Instance[] instances = [];
    public InstanceBatch[] instanceBatches = [];
    public Primitive[] basePrimitives = [];
    public Material[] materials = [];
    public RenderPass[] renderPasses = [];

    public RangedBoundingBox[] primitveBounds = [];
    public RangedBoundingBox[] instanceBounds = [];
    public RangedBoundingBox[] instanceBatchBounds = [];

    public PVSCellData primitvePvs = new();
    public PVSCellData instancePvs = new();
    public PVSCellData instanceBatchPvs = new();

    public int[] roomIndecies = [];
    public RoomObjectMapper[] roomObjectMappers = [];
    public MaterialList[] dynamicMaterialLsts = [];
    public ResourceIndex[] indexBufferSources = [];
    public ResourceIndex[] vertexBufferSources = [];
    public ResourceIndex[] textureBufferSources = [];
    public ChunkData[] chunkData = [];
}
public class ChunkData;
public class RoomObjectMapper
{
    public BitArray primitves = new(0);
    public BitArray instances = new(0);
    public BitArray instanceBatches = new(0);
}
public class PVSCellData
{
    public int cellCount;
    public short[] offsets = [];
    public int cellSize;
    public byte[] data = [];
}
public class RenderPass
{
    public int primitveCount;
    public int primitveOffset;
    public int instanceCount;
    public int instanceOffset;
    public int instanceBatchCount;
    public int instanceBatchOffset;
}
public class Instance
{
    public int primitveIndex;
    public int vertexBuffer;
}
public class InstanceBatch
{

}
public class BlendState
{
    public BlendFactor sourceBlend;
    public BlendFactor destinationBlend;
    public BlendOperation blendOperation;

    public bool alphaBlendEnable;
    public bool alphaTestEnable;
    public byte alphaRef;
    public bool alphaToMaskEnable;

    public FaceCullMode cullMode;
    public int zBias;
}
public class Primitive
{
    public int effect;
    public int vertexDeclaration;
    public PrimitiveType primitiveType;
    public int indexBuffer;
    public int elementIndex;
    public int elementListIndex;

    public MaterialIndex baseMaterial;
    public MaterialIndex instanceMaterial;

    public BlendState blendState = new();
    public Lod[] lods = [];
    public PrimitiveStream[] primitiveStreams = [];
    public class Lod
    {
        public int indexOffset;
        public int indexCount;
        public int vertexOffset;
        public int vertexCount;
    }
}
public class PrimitiveStream
{
    public int vertexBuffer;
    public int streamIndex;
    public int vertexOffset;
    public int frequency;

    public MaterialIndex material;
};
public enum FaceCullMode
{
    None,
    Front,
    Back,
}
public enum PrimitiveType
{
    TriangleList,
    TriangleStrip,
    PointList,
    QuadList,
    QuadStrip
};
public class BoundingBox
{
    public Vector3 start;
    public Vector3 end;
}
public class RangedBoundingBox : BoundingBox
{
    public float distanceMin;
    public float distanceMax;
}
public class Model
{
    public string name = "";
    public Matrix4x4 transform;
    public BoundingBox bounds = new BoundingBox();
    public int modelIndex;
    public int elementCount;
    public int modelDataIndex;
    public int parentIndex;
    public int firstChild;
    public int nextSibling;
}
public class Element
{
    public string name = "";
    public Matrix4x4 transform;
    public BoundingBox bounds = new BoundingBox();
    public int renderMeshId;
    public int physicsShapeId;
    public int modelIndex;
    public int elementIndex;
    public int parentElement;
    public int firstChild;
    public int nextSibling;
}
public class TextureDescription<TFormat> where TFormat : Enum
{
    public int width;
    public int height;
    public int depth;
    public int mipmaps;
    public TFormat textureFormat;
    public int usage;
    public TextureType type;
}
public enum TextureType
{
    Texture2D = 3,
    Cubemap = 5,
}
public class DXTTexture : LodTexture
{
    public string name = "";
    public TextureDescription<DXTFormat> description = new();
    public SamplerState samplerState;
    public int BlockSize => description.textureFormat switch
    {
        DXTFormat.DXT1 => 8,
        _ => 16
    };

    public byte[] bytes = [];
}
public enum DXTFormat
{
    R8G8B8 = 0x14,
    A8R8G8B8 = 0x15,
    R5G6B5 = 0x17,
    A4R4G4B4 = 0x1A,
    A8 = 0x1C,
    L8 = 0x32,
    Uncompressed3 = 0x33,
    DXT1 = 0x31545844,
    DXT3 = 0x33545844,
    DXT5 = 0x35545844,
}
public enum TextureWrap
{
    Wrap,
    Mirror,
    Clamp,
    Border
};
public enum TextureFilter
{
    Point,
    Linear,
    Anisotropic
};
public struct SamplerState
{
    public TextureWrap u, v, w;
    public TextureFilter magFilter, minFilter, mipFilter;
    public int maxAnisotropy, maxMiplevel, mipMapLodBias;
    public uint rgb;
}

public class Chunk;
public class ResourceBlock
{
    public PlatformType platformType;
    public BlockLod[] lods = [];
}
public class BlockLod
{
    public PlatformType platformType;
    public int lod;
    public VertexBuffer[] vertexBuffers = [];
    public IndexBuffer[] indexBuffers = [];
    public RenderingNode?[] renderingNodes = [];
    public LodTexture?[] textures = [];

}
public class MaterialList
{
    public MaterialType[] materialTypes = [];
    public Material[] materials = [];
}
public class MaterialType
{
    public string name = "";
    public MaterialTypeElement[] elements = [];
    public int dataSize;
    public int maxAlign;
    public short[] offsets = [];
    public int effect;
    public int listIndex;
}
public class MaterialTypeElement
{
    public string name = "";
    public MaterialElementType type;
    public int count;
}
public enum MaterialElementType
{
    S8,
    U8,
    S16,
    U16,
    S32,
    U32,
    F32,
    TextureIndex,
    MaterialIndex,
    DataIndex,
    TexturePointer
}
public enum BlendFactor
{
    Zero,
    One,
    SourceColor,
    InverseSourceColor,
    SourceAlpha,
    InverseSourceAlpha,
    DestinationAlpha,
    InverseDestinationAlpha,
    DestinationColor,
    InverseDestinationColor,
};
public enum BlendOperation
{
    Add,
    Subtract,
    Min,
    Max,
    ReverseSubtract,
};
public class Material
{
    public int typeIndex;
    public MaterialElement[][] elements;
    public Material(int typeIndex, MaterialElement[][] elements)
    {
        this.typeIndex = typeIndex;
        this.elements = elements;
    }
}
public class MaterialElement
{
    public readonly MaterialElementType type;
    public readonly object value = 0;
    public MaterialElement(MaterialElementType type, object value)
    {
        this.type = type;
        this.value = value;
    }
    public static MaterialElement Create(sbyte value) => new(MaterialElementType.S8, value);
    public static MaterialElement Create(byte value) => new(MaterialElementType.U8, value);
    public static MaterialElement Create(short value) => new(MaterialElementType.S16, value);
    public static MaterialElement Create(ushort value) => new(MaterialElementType.U16, value);
    public static MaterialElement Create(int value) => new(MaterialElementType.S32, value);
    public static MaterialElement Create(uint value) => new(MaterialElementType.U32, value);
    public static MaterialElement Create(float value) => new(MaterialElementType.F32, value);
    public static MaterialElement CreateTextureIndex(int value) => new(MaterialElementType.TextureIndex, value);
    public static MaterialElement CreateMaterialIndex(MaterialIndex value) => new(MaterialElementType.MaterialIndex, value);
    public static MaterialElement CreateDataIndex(MaterialIndex value) => new(MaterialElementType.DataIndex, value);
    public static MaterialElement CreateTexturePointer(int value) => new(MaterialElementType.TexturePointer, value);
}
public struct MaterialIndex
{
    public int index;
    public int listIndex;
    public override string ToString() => $"{index}:{listIndex}";
}
public interface LodTexture;
public class LodReferenceTexture : LodTexture
{
    public string name = "";
    public SamplerState? samplerState;
    public byte[] header = [];
    public ResourceIndex index;
}
public enum SerializationType
{
    Unknown,
    Primitive,
    Version,
    Polymorphic,
}
public static class CPModelExtensions
{
    public static int Size(this VertexElementType type) => type switch
    {
        VertexElementType.Vec2S16 => 4,
        VertexElementType.Vec4S16 => 8,
        VertexElementType.Vec2NS16 => 4,
        VertexElementType.Vec4NS16 => 8,
        VertexElementType.F32 => 4,
        VertexElementType.Vec2F32 => 8,
        VertexElementType.Vec3F32 => 12,
        VertexElementType.Vec4F32 => 16,
        VertexElementType.Vec2F16 => 4,
        VertexElementType.Vec4F16 => 8,
        VertexElementType.Vec4U8 => 4,
        VertexElementType.Vec4NU8 => 4,
        VertexElementType.Vec3NSHHD => 6,
        VertexElementType.Vec3NSDDD => 12,
        _ => throw new InvalidOperationException($"Unknown vertex element type: {type}")
    };
}

public struct AABBTriangle
{
    public byte indexA;
    public byte indexB;
    public byte indexC;
    public HenDN3 surfaceNormal;
    public int activeEdgeDataIndex;
};
public struct Vector3S
{
    short x;
    short y;
    short z;
};
public struct AABBNode
{
    public Vector3S bbmin;
    public Vector3S bbmax;
    public int triangleSizeIndices;
    public int triStartIndexORrightChildOffset;
    public int verticeStartIndex;
    public int numVertices;
};
public class TriangleMesh
{
    public Vector3 low;
    public Vector3 high;
    public Vector3[] verticies = [];
    public HenDN3[] normals = [];
    public AABBNode[] nodes = [];
    public uint numActiveEdges;
};

public struct HenDN3
{
    public uint data;
    public HenDN3(uint data)
    {
    }
}