using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using BlurFileFormats.Shaders;
using BlurFileFormats.Shared;

namespace BlurFileFormats.Models;

public static partial class CPModelSerializer
{
    public static CPModel Import(string filePath)
    {
        var stream = File.OpenRead(filePath);
        return Import(stream);
    }
    public static CPModel Import(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream, Encoding.ASCII);
        var magic = reader.ReadInt32(); // Magic
        if (magic != 0x50432020) throw new Exception("Not a CPModel file");

        var modelBlockHeader = BinaryReaderExtensions.ReadBlockHeader(reader);

        var modelHeaderBlockHeader = BinaryReaderExtensions.ReadBlockHeader(reader);
        reader.ReadUInt32();

        var modelDataBlockHeader = BinaryReaderExtensions.ReadBlockHeader(reader);

        var modelDataHeaderBlockHeader = BinaryReaderExtensions.ReadBlockHeader(reader);
        int modelVersion = reader.ReadInt32();
        uint modelCount = reader.ReadUInt32();
        uint elementCount = reader.ReadUInt32();
        uint constraintCount = reader.ReadUInt32();
        BoundingBox boundingBox = reader.ReadBoundingBox();

        string[] strings = reader.ReadStringTable();
        Model[] models = reader.ReadModels(modelCount, strings);
        Element[] elements = reader.ReadElements(elementCount, strings);
        reader.ReadConstraints(constraintCount, strings);

        var renderBlockHeader = BinaryReaderExtensions.ReadBlockHeader(reader);
        var renderRenderBlockHeader = BinaryReaderExtensions.ReadBlockHeader(reader);
        var renderRenderHeaderBlockHeader = BinaryReaderExtensions.ReadBlockHeader(reader);
        reader.ReadUInt32();
        var scene = reader.ReadScene();


        var collBlockHeader = BinaryReaderExtensions.ReadBlockHeader(reader);
        var shapes = reader.ReadCollision();
        return new CPModel(models, elements, scene, shapes);
    }
    static Shape[] ReadCollision(this BinaryReader reader)
    {
        Span<char> head = stackalloc char[4];
        reader.Read(head);
        int version = reader.ReadInt32();
        List<Shape> shapes = new List<Shape>();
        while (reader.TryReadShape(out var shape))
        {
            shapes.Add(shape);
        }
        return shapes.ToArray();
    }
    static bool TryReadShapeDesc(this BinaryReader reader, [NotNullWhen(true)]out ShapeDesc? shapeDesc)
    {

        ColliderType colliderType = (ColliderType)reader.ReadInt32();
        if (colliderType == ColliderType.End)
        {
            shapeDesc = null;
            return false;
        }
        float margin = reader.ReadSingle();
        float epsilon = reader.ReadSingle();
        uint physicsMaterialIndex = reader.ReadUInt32();
        uint effectMaterialIndex = reader.ReadUInt32();
        uint collisionFlags = reader.ReadUInt32();
        uint nodeFlags = reader.ReadUInt32();
        float mass = reader.ReadUInt32();
        int originalExportId = reader.ReadInt32();
        Vector3 renderOffset = reader.ReadVector3();
        uint remapped = reader.ReadUInt32();
        uint userDataFlag = reader.ReadUInt32();

        var shapeData = new ShapeData
        {
            margin = margin,
            epsilon = epsilon,
            physicsMaterialIndex = physicsMaterialIndex,
            effectMaterialIndex = effectMaterialIndex,
            collisionFlags = collisionFlags,
            nodeFlags = nodeFlags,
            mass = mass,
            originalExportId = originalExportId,
            renderOffset = renderOffset,
            remapped = remapped,
            userDataFlag = userDataFlag,
        };

        switch (colliderType)
        {
            case ColliderType.TriangleMesh_Shape:
                var mesh = reader.ReadTriangleMesh();
                shapeDesc = new TriangleMeshShapeDesc
                {
                    data = shapeData,
                    mesh = mesh
                };
                return true;
            case ColliderType.ConvexHull_Shape:
                var hull = reader.ReadConvexHull();
                shapeDesc = new ConvexHullShapeDesc
                {
                    data = shapeData,
                    hull = hull,
                };
                return true;
            case ColliderType.Composite_Shape:
                int shapeCount = reader.ReadInt32();
                uint constraintBodyId = reader.ReadUInt32();
                CompositeShapeDesc.Shape[] shapes = new CompositeShapeDesc.Shape[shapeCount];
                for (int i = 0; i < shapeCount; i++)
                {
                    if (!reader.TryReadCompositeShapeElement(out var shapeElement)) throw new NotSupportedException();
                    shapes[i] = shapeElement;
                }
                shapeDesc = new CompositeShapeDesc
                {
                    data = shapeData,
                    constraintBodyId = constraintBodyId,
                    shapes = shapes,
                };
                return true;
            case ColliderType.KDTriangleMesh_Shape:
            case ColliderType.Box_Shape:
            case ColliderType.Sphere_Shape:
            case ColliderType.Capsule_Shape:
            case ColliderType.Cylinder_Shape:
            case ColliderType.CharacterController_Shape:
            case ColliderType.Particle_Shape:
            case ColliderType.InstancedTriangleMesh_Shape:
            case ColliderType.TriangleMeshBase_Shape:
            case ColliderType.Cloth_Shape:
            case ColliderType.Fluid_Shape:
            case ColliderType.Plane_Shape:
            case ColliderType.Cone_Shape:
            case ColliderType.Rope_Shape:
            case ColliderType.FluidMesh_Shape:
            case ColliderType.Continuous_Shape:
            case ColliderType.NoCollisionRope_Shape:
            case ColliderType.NoCollisionCloth_Shape:
            case ColliderType.Force_DWord:
            case ColliderType.End:
            default:
                throw new NotImplementedException();
        }
    }
    static ConvexHull ReadConvexHull(this BinaryReader reader)
    {
        uint vertexCount = reader.ReadUInt32();
        uint triangleCount = reader.ReadUInt32();
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[triangleCount * 3];
        for (int i = 0; i < vertexCount; i++)
        {
            vertices[i] = reader.ReadVector3();
        }
        for (int i = 0; i < triangleCount*3; i++)
        {
            triangles[i] = reader.ReadInt32();
        }
        return new ConvexHull
        {
            vertices = vertices,
            triangles = triangles,
        };
    }
    static TriangleMesh ReadTriangleMesh(this BinaryReader reader)
    {
        uint vertexCount = reader.ReadUInt32();
        uint normalCount = reader.ReadUInt32();
        uint triangleCount = reader.ReadUInt32();
        uint nodeCount = reader.ReadUInt32();
        Vector3 low = reader.ReadVector3();
        Vector3 high = reader.ReadVector3();
        Vector3[] verticies = new Vector3[vertexCount];
        HenDN3[] normals = new HenDN3[normalCount];
        AABBNode[] nodes = new AABBNode[nodeCount];
        uint numActiveEdges = reader.ReadUInt32();
        return new TriangleMesh
        {
            low = low,
            high = high,
            verticies = verticies,
            normals = normals,
            nodes = nodes,
            numActiveEdges = numActiveEdges
        };
    }
    static bool TryReadShape(this BinaryReader reader, [NotNullWhen(true)] out Shape? shape)
    {
        if(!reader.TryReadShapeDesc(out var desc))
        {
            shape = null;
            return false;
        }
        Matrix4x4 relativeTransform = reader.ReadMat43();
        Matrix4x4 inertiaTensor = reader.ReadMat33();
        shape = new Shape
        {
            shapeDesc = desc,
            relativeTransform = relativeTransform,
            inertiaTensor = inertiaTensor,
        };
        return true;
    }
    static bool TryReadCompositeShapeElement(this BinaryReader reader, [NotNullWhen(true)] out CompositeShapeDesc.Shape? shape)
    {
        if (!reader.TryReadShapeDesc(out var desc))
        {
            shape = null;
            return false;
        }
        float mass = reader.ReadSingle();
        Matrix4x4 relativeTransform = reader.ReadMat43();
        Matrix4x4 inertiaTensor = reader.ReadMat33();
        shape = new CompositeShapeDesc.Shape
        {
            shapeDesc = desc,
            mass = mass,
            relativeTransform = relativeTransform,
            inertiaTensor = inertiaTensor,
        };
        return true;
    }
    static string[] ReadStringTable(this BinaryReader reader)
    {
        var header = BinaryReaderExtensions.ReadBlockHeader(reader);
        int stringCount = reader.ReadInt32();
        int[] offsets = new int[stringCount];
        for(int i = 0; i < offsets.Length; i++)
        {
            offsets[i] = reader.ReadInt32();
        }
        int characterCount = reader.ReadInt32();
        byte[] bytes = reader.ReadBytes(characterCount);

        string[] strings = new string[stringCount];

        for(int i = 0; i < strings.Length; i++)
        {
            int start = offsets[i];
            int end = start;
            while (end < characterCount && bytes[end] != '\0') end++;
            strings[i] = Encoding.ASCII.GetString(bytes.AsSpan()[start..end]);
        }

        return strings;
    }
    static Scene ReadScene(this BinaryReader reader)
    {
        var header = BinaryReaderExtensions.ReadBlockHeader(reader);
        
        var anchor = reader.BaseStream.Position;

        var arch1 = reader.ReadUInt32();
        var arch1x1 = reader.ReadUInt32();
        var arch1x0 = reader.ReadUInt32();
        var arch2 = reader.ReadUInt32();
        var arch2x1 = reader.ReadUInt32();
        var arch2x0 = reader.ReadUInt32();
        BinaryReaderExtensions.ReadArchHeader(reader);
        BinaryReaderExtensions.ReadArchHeader(reader);
        var platformType = reader.ReadPlatformType();

        BinaryReaderExtensions.ReadArchHeader(reader);
        string name = reader.ReadAscii();
        
        BinaryReaderExtensions.ReadArchHeader(reader);
        var rootNode = reader.ReadCullNode();
        var declarations = reader.ReadVertexDeclarationList();
        var effects = reader.ReadArchArray(ReadEffect);

        var resourceBlockCount = reader.ReadUInt32();
        BinaryReaderExtensions.ReadArchHeader(reader);
        BinaryReaderExtensions.ReadArchHeader(reader);
        var textures = reader.ReadTextureList();


        BinaryReaderExtensions.ReadArchHeader(reader);
        BinaryReaderExtensions.ReadArchHeader(reader);
        var chunks = reader.ReadArchArray(ReadChunk);

        var chunkPvs = reader.ReadArchArray(PrefixArch(ReadBitVector));
        var chunkBounds = reader.ReadArchArray(PrefixArch(ReadRangedBoundingBox));
        var groups = reader.ReadArchArray(PrefixArch(ReadBitVector));
        var chunkRooms = reader.ReadArchArray(PrefixArch(ReadBitVector));

        int chunkVersion = reader.ReadInt32();

        var dynamicMaterialLists = reader.ReadArchArray(ReadSceneMaterialList);
        var resourceBlocks = reader.ReadArray(r => r.ReadResourceBlock((int)anchor));

        return new Scene(platformType, name, rootNode, effects, textures, dynamicMaterialLists, resourceBlocks);
    }
    static DXTTexture[] ReadTextureList(this BinaryReader reader)
    {
        var platformType = reader.ReadPlatformType();
        var textures = reader.ReadArray(ReadTexture);
        return textures;
    }
    static ResourceBlock ReadResourceBlock(this BinaryReader reader, int anchor)
    {
        reader.ReadArchHeader();
        reader.ReadArchHeader();
        var platformType = reader.ReadPlatformType();
        
        var lods = reader.ReadArray(r => r.ReadBlockLod(anchor));
        return new ResourceBlock
        {
            platformType = platformType,
            lods = lods,
        };
    }
    static BlockLod ReadBlockLod(this BinaryReader reader, int anchor)
    {
        reader.ReadArchHeader();

        reader.ReadArchHeader();
        var platformType = reader.ReadPlatformType();
        var lod = reader.ReadInt32();
        VertexBuffer[]? vertexBuffers = null;
        IndexBuffer[]? indexBuffers = null;
        RenderingNode?[]? renderingNodes = null;
        if (lod == 0)
        {
            vertexBuffers = reader.ReadArchArray(r => r.ReadVertexBuffer(anchor));
            indexBuffers = reader.ReadArchArray(r => r.ReadIndexBuffer(anchor));
            renderingNodes = reader.ReadArchArray(r => r.ReadRenderingNode());
        }
        var textures = reader.ReadArray(r => r.ReadLodTexture(lod, anchor));
        return new BlockLod
        {
            platformType = platformType,
            lod = lod,
            vertexBuffers = vertexBuffers ?? [],
            indexBuffers = indexBuffers ?? [],
            renderingNodes = renderingNodes ?? [],
            textures = textures,
        };
    }
    static RenderingNode? ReadRenderingNode(this BinaryReader reader)
    {
        byte id = reader.ReadByte();
        if(id == 0) return null;
        if (id != 3) throw new NotImplementedException();
        var type = reader.ReadAscii();
        switch (type)
        {
            case "RenderingData::CullNode":
                return reader.ReadCullNode();
            case "RenderingData::RenderListNode_Common":
                return reader.ReadCommonRenderListNode();
            default:
                throw new NotSupportedException($"Unsupported rendering node type: {type}");
        }
    }
    static IndexBuffer ReadIndexBuffer(this BinaryReader reader, int anchor)
    {
        byte id = reader.ReadByte();
        reader.ReadArchHeader();

        int bufferSize = reader.ReadInt32();
        IndexType indexType = (IndexType)reader.ReadInt32();

        var data = reader.ReadByteStream(anchor);
        return new IndexBuffer
        {
            indexType = indexType,
            data = data,
        };
    }
    static VertexBuffer ReadVertexBuffer(this BinaryReader reader, int anchor)
    {
        byte id = reader.ReadByte();
        reader.ReadArchHeader();
        reader.ReadArchHeader();

        int vertexSize = reader.ReadInt32();
        var elements = reader.ReadArchArray(ReadStreamVertexElement);
        short[] offsets = reader.ReadArchArray(r => r.ReadInt16());

        int vertexCount = reader.ReadInt32();

        var data = reader.ReadByteStream(anchor);
        return new VertexBuffer
        {
            declaration = new VertexDeclaration
            {
                elements = elements,
            },
            offsets = offsets,
            vertexSize = vertexSize,
            vertexCount = vertexCount,
            data = data,
        };
    }
    static byte[] ReadByteStream(this BinaryReader reader, int anchor)
    {
        reader.ReadArchHeader();
        int size = reader.ReadInt32();
        int alignment = reader.ReadInt32();
        CompressionType compressionType = (CompressionType)reader.ReadUInt32();
        if(compressionType != CompressionType.None)
            throw new NotSupportedException("Compressed data is currently not supported");
        bool endianSwap = reader.ReadBoolean();

        var offset = reader.BaseStream.Position - anchor;
        offset %= alignment;
        if (offset != 0)
        {
            reader.ReadBytes(alignment - (int)offset);
        }
        byte[] data = reader.ReadBytes(size);
        return data;
    }
    static MaterialList? ReadSceneMaterialList(this BinaryReader reader)
    {
        byte bit = reader.ReadByte();
        if (bit == 0 || bit == 3) return null;
        reader.ReadArchHeader();
        return reader.ReadMaterialList();
    }
    static MaterialList ReadMaterialList(this BinaryReader reader)
    {
        reader.ReadArchHeader();
        var materialTypes = reader.ReadArchArray(ReadMaterialType);
        var materials = reader.ReadMaterials(materialTypes);
        return new MaterialList
        {
            materialTypes = materialTypes,
            materials = materials,
        };
    }
    static Material[] ReadMaterials(this BinaryReader reader, MaterialType[] types)
    {
        int materialCount = reader.ReadInt32();
        int dataSize = reader.ReadInt32();
        Material[] materials = new Material[materialCount];
        for(int i = 0; i < materialCount; i++)
        {
            Material material = ReadMaterial(reader, types);
            materials[i] = material;
        }
        return materials;
    }

    private static Material ReadMaterial(BinaryReader reader, MaterialType[] types)
    {
        int typeIndex = reader.ReadInt32();
        var type = types[typeIndex];
        MaterialElement[][] elements = new MaterialElement[type.elements.Length][];
        if (type.dataSize > 0)
        {
            for (int j = 0; j < type.elements.Length; j++)
            {
                var elementType = type.elements[j];
                MaterialElement[] elementData = new MaterialElement[elementType.count];
                for (int k = 0; k < elementType.count; k++)
                {
                    var element = reader.ReadMaterialElement(elementType.type);
                    elementData[k] = element;
                }
                elements[j] = elementData;
            }
        }
        Material material = new(typeIndex, elements);
        return material;
    }

    static MaterialElement ReadMaterialElement(this BinaryReader reader, MaterialElementType type)
    {
        switch (type)
        {
            case MaterialElementType.S8:
                return MaterialElement.Create(reader.ReadSByte());
            case MaterialElementType.U8:
                return MaterialElement.Create(reader.ReadByte());
            case MaterialElementType.S16:
                return MaterialElement.Create(reader.ReadInt16());
            case MaterialElementType.U16:
                return MaterialElement.Create(reader.ReadUInt16());
            case MaterialElementType.S32:
                return MaterialElement.Create(reader.ReadInt32());
            case MaterialElementType.U32:
                return MaterialElement.Create(reader.ReadUInt32());
            case MaterialElementType.F32:
                return MaterialElement.Create(reader.ReadSingle());
            case MaterialElementType.TextureIndex:
                return MaterialElement.CreateTextureIndex(reader.ReadInt32());
            case MaterialElementType.MaterialIndex:
                return MaterialElement.CreateMaterialIndex(reader.ReadMaterialIndex());
            case MaterialElementType.DataIndex:
                return MaterialElement.CreateDataIndex(reader.ReadMaterialIndex());
            case MaterialElementType.TexturePointer:
                return MaterialElement.CreateTexturePointer(reader.ReadInt32());
            default:
                throw new NotSupportedException();
        }
    }
    static MaterialIndex ReadMaterialIndex(this BinaryReader reader)
    {
        reader.ReadArchHeader();
        return new ()
        {
            index = reader.ReadInt32(),
            listIndex = reader.ReadInt32(),
        };
    }

    static MaterialType ReadMaterialType(this BinaryReader reader)
    {
        reader.ReadArchHeader();
        var name = reader.ReadAscii();
        var elements = reader.ReadArchArray(ReadMaterialTypeElement);
        int dataSize = reader.ReadInt32();
        int maxAlign = reader.ReadInt32();
        var offsets = reader.ReadArchArray(r => r.ReadInt16());
        int effect = reader.ReadInt32();
        int listIndex = reader.ReadInt32();

        return new MaterialType()
        {
            name = name,
            elements = elements,
            dataSize = dataSize,
            maxAlign = maxAlign,
            offsets = offsets,
            effect = effect,
            listIndex = listIndex,
        };
    }
    static MaterialTypeElement ReadMaterialTypeElement(this BinaryReader reader)
    {
        reader.ReadArchHeader();
        var name = reader.ReadAscii();
        var type = (MaterialElementType)reader.ReadInt32();
        var count = reader.ReadInt32();
        return new MaterialTypeElement
        {
            name = name,
            type = type,
            count = count,
        };
    }
    static Func<BinaryReader, T> PrefixArch<T>(Func<BinaryReader, T> operation) => r =>
        {
            r.ReadArchHeader();
            return operation(r);
        };
    static OwnedVertexElement[][] ReadVertexDeclarationList(this BinaryReader reader)
    {
        return reader.ReadArchArray(r =>
        {
            return r.ReadArchArray(ReadOwnedVertexElement);
        });
    }
    static Chunk ReadChunk(this BinaryReader reader)
    {
        throw new NotSupportedException("Chunks are not yet supported.");
    }
    static ChunkData ReadChunkData(this BinaryReader reader)
    {
        throw new NotSupportedException("Chunk Data is not yet supported.");
    }
    static LodTexture? ReadLodTexture(this BinaryReader reader, int lod, int anchor)
    {
        reader.ReadArchHeader();

        reader.ReadArchHeader();
        var platformType = reader.ReadPlatformType();
        string name = "";
        SamplerState samplerState = new SamplerState();
        int hash = 0;
        if (lod == 0)
        {
            name = reader.ReadAscii();
            samplerState = reader.ReadSamplerState();
            hash = reader.ReadInt32();
        }
        int present = reader.ReadInt32();

        bool isPresent = present != 0;
        if(!isPresent)
        {
            return null;
        }
        bool isPaired = present > 1;

        int headerSize = reader.ReadInt32();
        if (headerSize == 0) return null;
        byte[] headerData = reader.ReadBytes(headerSize);
        if(present >= 2)
        {
            var index = reader.ReadResourceIndex();
            return new LodReferenceTexture
            {
                name = name,
                samplerState = samplerState,
                header = headerData,
                index = index,
            };
        }
        byte[] textureBytes = reader.ReadByteStream(anchor);

        using var headerReader = new BinaryReader(new MemoryStream(headerData));
        var description = headerReader.ReadTextureDescription<DXTFormat>();
        return new DXTTexture
        {
            name = name,
            samplerState = samplerState,
            description = description,
            bytes = textureBytes,
        };
    }
    static DXTTexture ReadTexture(this BinaryReader reader)
    {
        var name = reader.ReadAscii();
        var hash = reader.ReadUInt32();
        BinaryReaderExtensions.ReadArchHeader(reader);
        BinaryReaderExtensions.ReadArchHeader(reader);
        return reader.ReadPlatformTexture();
    }
    static DXTTexture ReadPlatformTexture(this BinaryReader reader)
    {
        var platformType = reader.ReadPlatformType();

        var name = reader.ReadAscii();
        var samplerState = reader.ReadSamplerState();

        var hash = reader.ReadUInt32();
        var size = reader.ReadUInt32();
        switch (platformType)
        {
            case PlatformType.PC:
                var description = reader.ReadTextureDescription<DXTFormat>();
                var bytes = reader.ReadBytes((int)size - 28);
                return new DXTTexture
                {
                    name = name,
                    samplerState = samplerState,
                    description = description,
                    bytes = bytes,
                };
            default:
                throw new NotSupportedException();
        }
    }
    public static TextureDescription<TFormat> ReadTextureDescription<TFormat>(this BinaryReader reader) where TFormat : Enum
    {
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        int depth = reader.ReadInt32();
        int mipmaps = reader.ReadInt32();
        TFormat textureFormat = (TFormat)Enum.ToObject(typeof(TFormat), reader.ReadInt32());
        int usage = reader.ReadInt32();
        TextureType type = (TextureType)reader.ReadInt32();
        return new TextureDescription<TFormat>
        {
            width = width,
            height = height,
            depth = depth,
            mipmaps = mipmaps,
            textureFormat = textureFormat,
            usage = usage,
            type = type,
        };
    }    
    static SamplerState ReadSamplerState(this BinaryReader reader)
    {
        reader.ReadArchHeader();
        TextureWrap u = (TextureWrap)reader.ReadInt32();
        TextureWrap v = (TextureWrap)reader.ReadInt32();
        TextureWrap w = (TextureWrap)reader.ReadInt32();
        TextureFilter magFilter = (TextureFilter)reader.ReadInt32();
        TextureFilter minFilter = (TextureFilter)reader.ReadInt32();
        TextureFilter mipFilter = (TextureFilter)reader.ReadInt32();
        int maxAnisotropy = reader.ReadInt32();
        int maxMiplevel = reader.ReadInt32();
        int mipMapLodBias = reader.ReadInt32();
        uint rgb = reader.ReadUInt32();
        return new SamplerState
        {
            u = u,
            v = v,
            w = w,
            magFilter = magFilter,
            minFilter = minFilter,
            mipFilter = mipFilter,
            maxAnisotropy = maxAnisotropy,
            maxMiplevel = maxMiplevel,
            mipMapLodBias = mipMapLodBias,
            rgb = rgb,
        };
    }
    static string ReadEffect(this BinaryReader reader)
    {
        BinaryReaderExtensions.ReadArchHeader(reader);
        BinaryReaderExtensions.ReadArchHeader(reader);
        return reader.ReadAscii();
    }
    static OwnedVertexElement ReadOwnedVertexElement(this BinaryReader reader)
    {
        var header = BinaryReaderExtensions.ReadArchHeader(reader);
        ushort stream = reader.ReadUInt16();
        ushort offset = reader.ReadUInt16();
        VertexElementType type = (VertexElementType)reader.ReadInt32();
        VertexElementMethod method = (VertexElementMethod)reader.ReadInt32();
        VertexElementUsage usage = (VertexElementUsage)reader.ReadInt32();
        sbyte usageIndex = reader.ReadSByte();
        return new OwnedVertexElement
        {
            stream = stream,
            offset = offset,
            type = type,
            method = method,
            usage = usage,
            usageIndex = usageIndex
        };
    }
    static VertexElement ReadStreamVertexElement(this BinaryReader reader)
    {
        var header = BinaryReaderExtensions.ReadArchHeader(reader);
        VertexElementType type = (VertexElementType)reader.ReadInt32();
        VertexElementMethod method = (VertexElementMethod)reader.ReadInt32();
        VertexElementUsage usage = (VertexElementUsage)reader.ReadInt32();
        int usageIndex = reader.ReadInt32();
        return new VertexElement
        {
            type = type,
            method = method,
            usage = usage,
            usageIndex = usageIndex
        };
    }
    static CullNode ReadCullNode(this BinaryReader reader)
    {
        BinaryReaderExtensions.ReadArchHeader(reader);
        BinaryReaderExtensions.ReadArchHeader(reader);
        string name = reader.ReadAscii();

        var subNodes = reader.ReadArchArray(BinaryReaderExtensions.ReadResourceIndex);
        var parentIndex = BinaryReaderExtensions.ReadResourceIndex(reader);
        
        var location = BinaryReaderExtensions.ReadResourceIndex(reader);
        BinaryReaderExtensions.ReadArchHeader(reader);
        var boundingBox = reader.ReadRangedBoundingBox();

        var pvsBits = reader.ReadBitVector();
        int groupIndex = reader.ReadInt32();

        var portalBits = reader.ReadBitVector();
        var node = new CullNode
        {
            name = name,
            bounds = boundingBox,
            subNodes = subNodes,
            parentIndex = parentIndex,
            location = location,
            pvsBits = pvsBits,
            groupIndex = groupIndex,
            portalBits = portalBits
        };
        return node;
    }
    static CommonRenderListNode ReadCommonRenderListNode(this BinaryReader reader)
    {
        BinaryReaderExtensions.ReadArchHeader(reader);
        BinaryReaderExtensions.ReadArchHeader(reader);

        // Cull Node Start ---------------
        BinaryReaderExtensions.ReadArchHeader(reader);
        BinaryReaderExtensions.ReadArchHeader(reader);
        string name = reader.ReadAscii();

        var subNodes = reader.ReadArchArray(BinaryReaderExtensions.ReadResourceIndex);
        var parentIndex = BinaryReaderExtensions.ReadResourceIndex(reader);
        var location = BinaryReaderExtensions.ReadResourceIndex(reader);
        BinaryReaderExtensions.ReadArchHeader(reader);
        var boundingBox = reader.ReadRangedBoundingBox();

        var pvsBits = reader.ReadBitVector();
        int groupIndex = reader.ReadInt32();
        var portalBits = reader.ReadBitVector();
        // Cull Node End ---------------

        var constantMaterialTypes = reader.ReadMaterialList();
        var primitives = reader.ReadArchArray(ReadPrimitive);
        var instances = reader.ReadArchArray(ReadInstance);
        var instanceBatches = reader.ReadArchArray(ReadInstanceBatch);
        var basePrimitives = reader.ReadArchArray(ReadPrimitive);

        var materials = reader.ReadMaterials(constantMaterialTypes.materialTypes);
        var renderPasses = reader.ReadArchArray(ReadRenderPass);

        var primitveBounds = reader.ReadArchArray(PrefixArch(ReadRangedBoundingBox));
        var instanceBounds = reader.ReadArchArray(PrefixArch(ReadRangedBoundingBox));
        var instanceBatchBounds = reader.ReadArchArray(PrefixArch(ReadRangedBoundingBox));

        var primitvePvs = reader.ReadPVSCellData();
        var instancePvs = reader.ReadPVSCellData();
        var instanceBatchPvs = reader.ReadPVSCellData();

        var roomIndecies = reader.ReadArchArray(r => r.ReadInt32());
        var roomObjectMappers = reader.ReadArchArray(ReadRoomObjectMapper);

        var dynamixMaterialList = reader.ReadArchArray(ReadMaterialList);
        var indexBufferSources = reader.ReadArchArray(BinaryReaderExtensions.ReadResourceIndex);
        var vertexBufferSources = reader.ReadArchArray(BinaryReaderExtensions.ReadResourceIndex);
        var textureSources = reader.ReadArchArray(BinaryReaderExtensions.ReadResourceIndex);

        var chunkData = reader.ReadArchArray(ReadChunkData);
        var node = new CommonRenderListNode
        {
            name = name,
            bounds = boundingBox,
            pvsBits = pvsBits,
            groupIndex = groupIndex,
            portalBits = portalBits,
            location = location,
            parentIndex = parentIndex,
            subNodes = subNodes,
            constantMaterialTypes = constantMaterialTypes,
            primitives = primitives,
            instances = instances,
            instanceBatches = instanceBatches,
            basePrimitives = basePrimitives,
            materials = materials,
            renderPasses = renderPasses,
            primitveBounds = primitveBounds,
            instanceBounds = instanceBounds,
            instanceBatchBounds = instanceBatchBounds,
            primitvePvs = primitvePvs,
            instancePvs = instancePvs,
            instanceBatchPvs = instanceBatchPvs,
            roomIndecies = roomIndecies,
            roomObjectMappers = roomObjectMappers,
            dynamicMaterialLsts = dynamixMaterialList,
            indexBufferSources = indexBufferSources,
            vertexBufferSources = vertexBufferSources,
            textureBufferSources = textureSources,
            chunkData = chunkData,
        };
        return node;
    }
    static PVSCellData ReadPVSCellData(this BinaryReader reader)
    {
        reader.ReadArchHeader();
        int cellCount = reader.ReadInt32();
        var offsets = reader.ReadArchArray(r => r.ReadInt16());
        int cellSize = reader.ReadInt32();
        reader.ReadArchHeader();
        int dataSize = reader.ReadInt32();
        var data = reader.ReadBytes(dataSize);
        return new PVSCellData
        {
            cellCount = cellCount,
            offsets = offsets,
            cellSize = cellSize,
            data = data,
        };
    }
    static RenderPass ReadRenderPass(this BinaryReader reader)
    {
        reader.ReadArchHeader();
        int primitveCount = reader.ReadInt32();
        int primitveOffset = reader.ReadInt32();
        int instanceCount = reader.ReadInt32();
        int instanceOffset = reader.ReadInt32();
        int instanceBatchCount = reader.ReadInt32();
        int instanceBatchOffset = reader.ReadInt32();
        return new RenderPass
        {
            primitveCount = primitveCount,
            primitveOffset = primitveOffset,
            instanceCount = instanceCount,
            instanceOffset = instanceOffset,
            instanceBatchCount = instanceBatchCount,
            instanceBatchOffset = instanceBatchOffset,
        };
    }
    static InstanceBatch ReadInstanceBatch(this BinaryReader reader)
    {
        throw new NotSupportedException("Instance Batches are not yet supported.");
    }
    static RoomObjectMapper ReadRoomObjectMapper(this BinaryReader reader)
    {
        throw new NotSupportedException("Room Object Mappers are not yet supported.");
    }
    static Instance ReadInstance(this BinaryReader reader)
    {
        reader.ReadArchHeader();
        int primitveIndex = reader.ReadInt32();
        reader.ReadArchHeader();
        int vertexBuffer = reader.ReadInt32();
        return new Instance
        {
            primitveIndex = primitveIndex,
            vertexBuffer = vertexBuffer,
        };
    }
    static Primitive ReadPrimitive(this BinaryReader reader)
    {
        reader.ReadArchHeader();
        int effect = reader.ReadInt32();
        int vertexDeclaration = reader.ReadInt32();
        PrimitiveType primitiveType = (PrimitiveType)reader.ReadInt32();
        int indexBuffer = reader.ReadInt32();
        uint flags = reader.ReadUInt32();
        var baseMaterial = reader.ReadMaterialIndex();
        var instanceMaterial = reader.ReadMaterialIndex();

        reader.ReadArchHeader();
        var blendState = reader.ReadBlendState();

        var lods = reader.ReadArchArray(ReadLod);
        var primitveStreams = reader.ReadArchArray(ReadPrimitiveStream);

        return new Primitive
        {
            effect = effect,
            vertexDeclaration = vertexDeclaration,
            primitiveType = primitiveType,
            indexBuffer = indexBuffer,
            elementIndex = (int)(flags & (0b1111111111)),
            elementListIndex = (int)(flags >> 10),
            baseMaterial = baseMaterial,
            instanceMaterial = instanceMaterial,
            blendState = blendState,
            lods = lods,
            primitiveStreams = primitveStreams,
        };
    }
    static BlendState ReadBlendState(this BinaryReader reader)
    {
        BlendFactor sourceBlend = (BlendFactor)reader.ReadInt32();
        BlendFactor destinationBlend = (BlendFactor)reader.ReadInt32();
        BlendOperation blendOperation = (BlendOperation)reader.ReadInt32();

        bool alphaBlendEnable = reader.ReadBoolean();
        bool alphaTestEnable = reader.ReadBoolean();
        byte alphaRef = reader.ReadByte();
        bool alphaToMaskEnable = reader.ReadBoolean();

        FaceCullMode cullMode = (FaceCullMode)reader.ReadInt32();
        int zBias = reader.ReadInt32();
        return new BlendState
        {
            sourceBlend = sourceBlend,
            destinationBlend = destinationBlend,
            blendOperation = blendOperation,
            alphaBlendEnable = alphaBlendEnable,
            alphaTestEnable = alphaTestEnable,
            alphaRef = alphaRef,
            alphaToMaskEnable = alphaToMaskEnable,
            cullMode = cullMode,
            zBias = zBias,
        };
    }
    static Primitive.Lod ReadLod(this BinaryReader reader)
    {
        reader.ReadArchHeader();
        int indexOffset = reader.ReadInt32();
        int indexCount = reader.ReadInt32();
        int vertexOffset = reader.ReadInt32();
        int vertexCount = reader.ReadInt32();
        return new Primitive.Lod
        {
            indexOffset = indexOffset,
            indexCount = indexCount,
            vertexOffset = vertexOffset,
            vertexCount = vertexCount,
        };
    }
    static PrimitiveStream ReadPrimitiveStream(this BinaryReader reader)
    {
        reader.ReadArchHeader();
        int vertexBuffer = reader.ReadInt32();
        int streamIndex = reader.ReadInt32();
        int vertexOffset = reader.ReadInt32();
        int frequency = reader.ReadInt32();
        var material = reader.ReadMaterialIndex();
        return new PrimitiveStream
        {
            vertexBuffer = vertexBuffer,
            streamIndex = streamIndex,
            vertexOffset = vertexOffset,
            frequency = frequency,
            material = material,
        };
    }
    static BitArray ReadBitVector(this BinaryReader reader)
    {
        BinaryReaderExtensions.ReadArchHeader(reader);

        int bitCount = reader.ReadInt32();
        BinaryReaderExtensions.ReadArchHeader(reader);
        int byteCount = reader.ReadInt32();


        byte[] bytes = reader.ReadBytes(byteCount);
        return new BitArray(bytes);
    }
    static T[] ReadArchArray<T>(this BinaryReader reader, Func<BinaryReader, T> operation)
    {
        BinaryReaderExtensions.ReadArchHeader(reader);
        int size = reader.ReadInt32();
        T[] items = new T[size];
        for(int i = 0; i < size; i++)
        {
            items[i] = operation(reader);
        }
        return items;
    }
    static Model[] ReadModels(this BinaryReader reader, uint modelCount, string[] strings)
    {
        Model[] models = new Model[modelCount];
        var header = BinaryReaderExtensions.ReadBlockHeader(reader);
        for(int i = 0; i < modelCount; i++)
        {
            var transform = reader.ReadMat43();
            var bounds = reader.ReadBoundingBox();
            var name = reader.ReadUInt32();
            var modelIndex = reader.ReadInt32();
            var elementCount = reader.ReadInt32();
            var modelDataIndex = reader.ReadInt32();
            var parentIndex = reader.ReadInt32();
            var firstChild = reader.ReadInt32();
            var nextSibling = reader.ReadInt32();
            var model = new Model
            {
                name = strings[name],
                transform = transform,
                bounds = bounds,
                modelIndex = modelIndex,
                elementCount = elementCount,
                modelDataIndex = modelDataIndex,
                parentIndex = parentIndex,
                firstChild = firstChild,
                nextSibling = nextSibling,
            };
            models[i] = model;
        }
        return models;
    }
    static Element[] ReadElements(this BinaryReader reader, uint elementCount, string[] strings)
    {
        Element[] elements = new Element[elementCount];
        var header = BinaryReaderExtensions.ReadBlockHeader(reader);
        for(int i = 0; i < elementCount; i++)
        {
            var modelIndex = reader.ReadInt32();
            var transform = reader.ReadMat43();
            var bounds = reader.ReadBoundingBox();
            var name = reader.ReadInt32();
            var elementIndex = reader.ReadInt32();
            var parentElement = reader.ReadInt32();
            var firstChild = reader.ReadInt32();
            var nextSibling = reader.ReadInt32();
            var renderMeshId = reader.ReadInt32();
            var physicsShapeId = reader.ReadInt32();

            var element = new Element
            {
                name = strings[name],

                modelIndex = modelIndex,
                transform = transform,
                bounds = bounds,
                elementIndex = elementIndex,
                parentElement = parentElement,
                firstChild = firstChild,
                nextSibling = nextSibling,
                renderMeshId = renderMeshId,
                physicsShapeId = physicsShapeId,
            };
            elements[i] = element;
        }
        return elements;
    }
    static void ReadConstraints(this BinaryReader reader, uint constraintCount, string[] strings)
    {
        var header = BinaryReaderExtensions.ReadBlockHeader(reader);
    }
    static Matrix4x4 ReadMat43(this BinaryReader reader)
    {
        var col1 = reader.ReadVector3();
        var col2 = reader.ReadVector3();
        var col3 = reader.ReadVector3();
        var col4 = reader.ReadVector3();
        return new Matrix4x4(col1.X, col1.Y, col1.Z, 0,
                             col2.X, col2.Y, col2.Z, 0,
                             col3.X, col3.Y, col3.Z, 0,
                             col4.X, col4.Y, col4.Z, 1);
    }
    static Matrix4x4 ReadMat33(this BinaryReader reader)
    {
        var col1 = reader.ReadVector3();
        var col2 = reader.ReadVector3();
        var col3 = reader.ReadVector3();
        return new Matrix4x4(col1.X, col1.Y, col1.Z, 0,
                             col2.X, col2.Y, col2.Z, 0,
                             col3.X, col3.Y, col3.Z, 0,
                                  0,      0,      0, 1);
    }
    static BoundingBox ReadBoundingBox(this BinaryReader reader)
    {
        Vector3 start = reader.ReadVector3();
        Vector3 end = reader.ReadVector3();

        return new BoundingBox()
        {
            start = start,
            end = end,
        };
    }
    static RangedBoundingBox ReadRangedBoundingBox(this BinaryReader reader)
    {
        Vector3 start = reader.ReadVector3();
        Vector3 end = reader.ReadVector3();
        var min = reader.ReadSingle();
        var max = reader.ReadSingle();

        return new RangedBoundingBox()
        {
            start = start,
            end = end,
            distanceMin = min,
            distanceMax = max
        };
    }

    private static PlatformType ReadPlatformType(this BinaryReader reader)
    {
        return (PlatformType)reader.ReadUInt32();
    }
}
class AnchoredStream : Stream
{
    private readonly Stream baseStream;
    private readonly int anchor;
    public AnchoredStream(Stream baseStream, int anchor)
    {
        this.baseStream = baseStream;
        this.anchor = anchor;
    }
    public override bool CanRead => baseStream.CanRead;
    public override bool CanSeek => baseStream.CanSeek;
    public override bool CanWrite => baseStream.CanWrite;
    public override long Length => baseStream.Length - anchor;
    public override long Position
    {
        get => baseStream.Position - anchor;
        set => baseStream.Position = value + anchor;
    }
    public override void Flush() => baseStream.Flush();
    public override int Read(byte[] buffer, int offset, int count) => baseStream.Read(buffer, offset, count);
    public override long Seek(long offset, SeekOrigin origin)
    {
        long newPosition = origin switch
        {
            SeekOrigin.Begin => offset + anchor,
            SeekOrigin.Current => baseStream.Position + offset,
            SeekOrigin.End => baseStream.Length + offset,
            _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null),
        };
        return baseStream.Seek(newPosition, SeekOrigin.Begin) - anchor;
    }
    public override void SetLength(long value) => baseStream.SetLength(value + anchor);
    public override void Write(byte[] buffer, int offset, int count) => baseStream.Write(buffer, offset, count);
}