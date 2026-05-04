using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using BlurFileFormats.Shared;
using System.Linq;

namespace BlurFileFormats.Animations;

public static class CycleSerializer
{
    public static Cycle DeserializeCycle(BinaryReader reader)
    {
        var header = reader.DeserializeCycleHeader();
        var data = reader.ReadCycleData(header);
        return new Cycle
        {
            header = header,
            data = data,
        };
    }
    /// <summary>
    /// Responsible for deserializing .cycle files.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="cycle"></param>
    /// <exception cref="NotSupportedException"></exception>
    public static CycleHeader DeserializeCycleHeader(this BinaryReader reader)
    {
        int version = reader.ReadInt32();
        // Blur does not support versions ouside of 12-15
        if(version < 12 || version > 15)
        {
            throw new NotSupportedException();
        }
        var cycleHeader = new CycleHeader();
        cycleHeader.version = version;
        // Version 14 added reference transforms. Although there is a reference transform count, you can only have one or zero reference transforma.
        if(version >= 14)
        {
            cycleHeader.referenceTransformCount = reader.ReadInt32();

            // Version 15 added support for reading scale for animations.
            if(version >= 15)
            {
                for(int i = 0; i < cycleHeader.referenceTransformCount; i++)
                {
                    cycleHeader.referenceTransform = reader.DeserializeJointTransform();
                }
            }
            else
            {

                for (int i = 0; i < cycleHeader.referenceTransformCount; i++)
                {
                    cycleHeader.referenceTransform = reader.DeserializeJointTransformWithoutScale();
                }
            }
        }
        else
        {
            cycleHeader.referenceTransformCount = 0;
        }

        if(version >= 13)
        {
            if(version >= 15)
            {
                cycleHeader.trajectoryFirst = reader.DeserializeJointTransform();
                cycleHeader.trajectoryFinal = reader.DeserializeJointTransform();
            }
            else
            {
                cycleHeader.trajectoryFirst = reader.DeserializeJointTransformWithoutScale();
                cycleHeader.trajectoryFinal = reader.DeserializeJointTransformWithoutScale();
            }

            cycleHeader.trajectoryDelta = cycleHeader.trajectoryFirst.Inverse() * cycleHeader.trajectoryFinal;
        }
        else
        {
            cycleHeader.trajectoryDelta = reader.DeserializeJointTransformWithoutScale();
            cycleHeader.trajectoryFirst = JointTransform.Identity;
            cycleHeader.trajectoryFinal = cycleHeader.trajectoryDelta;
        }

        cycleHeader.trajectoryRootFieldIndex = reader.ReadInt32();

        cycleHeader.frameCount = reader.ReadInt32();
        cycleHeader.playbackDuration = reader.ReadSingle();
        cycleHeader.frameRate = reader.ReadSingle();

        cycleHeader.fieldCount = reader.ReadInt32();
        if(cycleHeader.fieldCount > 0)
        {
            cycleHeader.fieldTypes = new PoseFieldType[cycleHeader.fieldCount];
            for (int i = 0; i < cycleHeader.fieldCount; i++)
            {
                cycleHeader.fieldTypes[i] = reader.ReadPoseFieldType(version);
            }

            // This is superfluous as it will always be the same as cycle.fieldCount. This is how blur saves string arrays however.
            int fieldNameCount = reader.ReadInt32();
            cycleHeader.fieldNames = new string[fieldNameCount];
            for(int i = 0; i < fieldNameCount; i++)
            {
                int stringLength = reader.ReadInt32();
                var name = Encoding.ASCII.GetString(reader.ReadBytes(stringLength));
                cycleHeader.fieldNames[i] = name;
            }
        }

        return cycleHeader;
    }
    const int simdPadding = 15;
    private static CycleData ReadCycleData(this BinaryReader reader, CycleHeader header)
    {
        CycleData cycleData = new CycleData();
        cycleData.totalConstantCurveCount = reader.ReadInt16();
        cycleData.totalVaryingCurveCount = reader.ReadInt16();

        for(int i = 0; i < (int)ConstantCurveType.Count; i++)
        {
            cycleData.constantCurveCountsOfType[i] = reader.ReadInt16();
        }

        for(int i = 0; i < (int)VaryingCurveType.Count; i++)
        {
            cycleData.varyingCurveCountsOfType[i] = reader.ReadInt16();
        }

        var totalCurves = cycleData.totalConstantCurveCount + cycleData.totalVaryingCurveCount;
        if(totalCurves > 0)
        {
            cycleData.channelInfo = new ChannelInfo[totalCurves];
            for(int i = 0; i < totalCurves ; i++)
            {
                cycleData.channelInfo[i] = reader.ReadChannelInfo(header.version);
            }
        }

        cycleData.constantCurveDataSize = reader.ReadInt32();

        var packedCurveCount = cycleData.varyingCurveCountsOfType[(int)VaryingCurveType.Vector3U16]
            + cycleData.varyingCurveCountsOfType[(int)VaryingCurveType.Vector3U8];
        if (packedCurveCount > 0)
        {
            cycleData.packedCurveInfo = new PackedCurveInfo[packedCurveCount];
            for(int i = 0;i < packedCurveCount ; i++)
            {
                cycleData.packedCurveInfo[i] = reader.ReadPackedCurveInfo();
            }
        }
        {
            var constantCurveCount = cycleData.constantCurveCountsOfType[(int)ConstantCurveType.Vector3F32];
            if (constantCurveCount > 0)
            {
                cycleData.constantVector3Curves = new Vector3[constantCurveCount];
                for (int i = 0; i < constantCurveCount; i++)
                {
                    cycleData.constantVector3Curves[i] = reader.ReadVector3();
                }
            }
        }
        {
            var constantCurveCount = cycleData.constantCurveCountsOfType[(int)ConstantCurveType.ScalarF32];
            if (constantCurveCount > 0)
            {
                cycleData.constantScalarCurves = new float[constantCurveCount];
                for(int i = 0; i < constantCurveCount; i++)
                {
                    cycleData.constantScalarCurves[i] = reader.ReadSingle();
                }
            }
        }
        {
            var constantCurveCount = cycleData.constantCurveCountsOfType[(int)ConstantCurveType.Quaternion48];
            if (constantCurveCount > 0)
            {
                cycleData.constantQuaternionCurves = new Quaternion[constantCurveCount];
                for(int i = 0; i < constantCurveCount; i++)
                {
                    cycleData.constantQuaternionCurves[i] = reader.ReadPackedQuaternion48();
                }
            }
        }

        // Blur stores keyframe data in segments. Each segment stores the key time and key values
        // key times are stored as a byte, which means each segment can store up to 256 keyframes.
        // The segments start frame places the keyframe at its final location.
        cycleData.segmentCount = reader.ReadInt32();
        cycleData.totalSegmentDataSize = reader.ReadInt32();

        if(cycleData.segmentCount > 0)
        {
            cycleData.segments = new Cycle.Segment[cycleData.segmentCount];

            for(int i = 0; i < cycleData.segmentCount; i++)
            {
                cycleData.segments[i] = new Cycle.Segment
                {
                    startFrame = reader.ReadInt32(),
                    frameCount = reader.ReadInt16(),
                    byteSize = reader.ReadInt16(),
                };
            }

            for (int i = 0; i < cycleData.segmentCount; i++)
            {
                var segment = cycleData.segments[i];
                // Initial frame will always have a key for each curve
                segment.initialFrame = reader.ReadCycleSegmentFrame(cycleData.varyingCurveCountsOfType, cycleData.packedCurveInfo);
                if(i != cycleData.segmentCount - 1)
                {
                    short[] keyCountPerCurveType = new short[(int)VaryingCurveType.Count];
                    for(int j = 0; j < (int)VaryingCurveType.Count; j++)
                    {
                        keyCountPerCurveType[j] = reader.ReadInt16();
                    }
                    byte[][][] keyTimes = new byte[keyCountPerCurveType.Length][][];
                    for (int j = 0; j < keyCountPerCurveType.Length; j++)
                    {
                        byte[][] curveKeyTimes = new byte[cycleData.varyingCurveCountsOfType[j]][];
                        for(int k = 0; k < cycleData.varyingCurveCountsOfType[j]; k++)
                        {
                            byte keyCount = reader.ReadByte();
                            curveKeyTimes[k] = reader.ReadBytes(keyCount);
                        }
                        keyTimes[j] = curveKeyTimes;
                    }
                    segment.processFrame = reader.ReadCycleSegmentFrame(keyCountPerCurveType, cycleData.packedCurveInfo);
                }
            }

            //cycleData.segmentData = reader.ReadBytes(cycleData.totalSegmentDataSize);
        }

        cycleData.initialFrameSize = 0;
        cycleData.initialFrameSize += cycleData.varyingCurveCountsOfType[(int)VaryingCurveType.Vector3F32] * 12;
        cycleData.initialFrameSize += cycleData.varyingCurveCountsOfType[(int)VaryingCurveType.Quaternion32] * 4;
        cycleData.initialFrameSize += cycleData.varyingCurveCountsOfType[(int)VaryingCurveType.ScalarF32] * 4;
        cycleData.initialFrameSize += cycleData.varyingCurveCountsOfType[(int)VaryingCurveType.Quaternion48] * 6;
        cycleData.initialFrameSize += cycleData.varyingCurveCountsOfType[(int)VaryingCurveType.Vector3U16] * 6;
        cycleData.initialFrameSize += cycleData.varyingCurveCountsOfType[(int)VaryingCurveType.Vector3U8] * 3;

        return cycleData;
    }
    private static int AlignUp(int offset, int align)
    {
        return ((offset + align - 1) & ~(align - 1));
    }
    private static PoseFieldType ReadPoseFieldType(this BinaryReader reader, int version)
    {
        var value = reader.ReadByte();
        if(version < 15)
        {

            value = (byte)Math.Max(0, value - 1);
        }
        return (PoseFieldType)value;
    }
    private static readonly PoseFieldElement[] poseFieldMapping =
    [
        PoseFieldElement.JointTransformRotation,		// was ObjectTransformRotation
        PoseFieldElement.JointTransformTranslation,	// was ObjectTransformTranslation
        PoseFieldElement.JointTransformScale,			// was ObjectTransformScale
        PoseFieldElement.JointTransformRotation,
        PoseFieldElement.JointTransformTranslation,
        PoseFieldElement.CustomScalar,
        PoseFieldElement.CustomVector3,
        PoseFieldElement.TrajectoryDeltaTransformRotation,
        PoseFieldElement.TrajectoryDeltaTransformTranslation,
        PoseFieldElement.TrajectoryTransformRotation,
        PoseFieldElement.TrajectoryTransformTranslation,
        PoseFieldElement.CustomVector4XYZ,
        PoseFieldElement.CustomVector4W
    ];
    private static ChannelInfo ReadChannelInfo(this BinaryReader reader, int version)
    {
        var value = reader.ReadUInt16();
        var elementType = value & 0x3f;
        var fieldIndex = value >> 6;
        if(version >= 15)
        {
            return new ChannelInfo
            {
                fieldIndex = fieldIndex,
                element = (PoseFieldElement)elementType
            };
        }

        return new ChannelInfo
        {
            fieldIndex = fieldIndex,
            element = poseFieldMapping[elementType]
        };
    }
    private static PackedCurveInfo ReadPackedCurveInfo(this BinaryReader reader)
    {
        var scale = reader.ReadVector3();
        var offset = reader.ReadVector3();

        return new PackedCurveInfo
        {
            scale = scale,
            offset = offset
        };
    }
    private static JointTransform DeserializeJointTransform(this BinaryReader reader)
    {
        var transform = reader.DeserializeJointTransformWithoutScale();
        transform.scale = reader.ReadVector3();
        return transform;
    }
    private static JointTransform DeserializeJointTransformWithoutScale(this BinaryReader reader)
    {
        var transform = JointTransform.Identity;
        transform.rotation = reader.ReadQuaternion();
        transform.translation = reader.ReadVector3();
        return transform;
    }

    const float kRootTwo = 1.414213562373f;
    const float kOneOverRootTwo = 1f / kRootTwo;
    const float kOneOverRootTwoApprox = 0.707107f;

    public static Quaternion ReadPackedQuaternion48(this BinaryReader reader)
    {
        Span<ushort> packed = [reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16()];
        return DecompressQuaternion48(packed);
    }

    public static Quaternion ReadPackedQuaternion32(this BinaryReader reader)
    {
        var packed = reader.ReadUInt32();
        return DecompressQuaternion32(packed);
    }

    public static Vector3 ReadPackedVector3U16(this BinaryReader reader, Vector3 offset, Vector3 scale)
    {
        Span<ushort> packed = [reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16()];
        return DecompressVector3U16(packed, offset, scale);
    }

    public static Vector3 ReadPackedVector3U8(this BinaryReader reader, Vector3 offset, Vector3 scale)
    {
        Span<byte> packed = [reader.ReadByte(), reader.ReadByte(), reader.ReadByte()];
        return DecompressVector3U8(packed, offset, scale);
    }

    public static Quaternion DecompressQuaternion48(ReadOnlySpan<ushort> packed)
    {
        if (packed.Length != 3) throw new NotSupportedException();
        uint src = (uint)packed[2]
            | ((uint)(packed[1]) << 16)
            | ((uint)(packed[0]) << 32);

        //static const U64 mask = kMask15;
        const uint kMask15 = (1 << 15) - 1;

        uint largestSign = (uint)(src & 1);
        int largestIndex = (int)(uint)((src >> 1) & 3);



        int iC0 = (int)(uint)((src >> 3) & kMask15);
        int iC1 = (int)(uint)((src >> 18) & kMask15);
        int iC2 = (int)(uint)((src >> 33) & kMask15);

        const float fRange = 32766;

        float fC0 = kOneOverRootTwo * (2f * (iC0 / fRange) - 1f);
        float fC1 = kOneOverRootTwo * (2f * (iC1 / fRange) - 1f);
        float fC2 = kOneOverRootTwo * (2f * (iC2 / fRange) - 1f);

        Vector4 q = new Vector4();

        int[][] permutes =
            [
                [ 1, 2, 3 ],
                [ 0, 2, 3 ],
                [ 0, 1, 3 ],
                [ 0, 1, 2 ]
            ];

            q[permutes[largestIndex][0]] = fC0;
            q[permutes[largestIndex][1]] = fC1;
            q[permutes[largestIndex][2]] = fC2;

            float largestSquared = 1f - (fC0 * fC0 + fC1 * fC1 + fC2 * fC2);


        float largestValue = MathF.Sqrt(largestSquared);
        q[largestIndex] = largestSign != 0 ? -largestValue : largestValue;

        return q.AsQuaternion();
    }

    public static Cycle.Segment.Frame ReadCycleSegmentFrame(this BinaryReader reader, short[] curveCountsOfType, PackedCurveInfo[] packedCurveInfos)
    {
        var frame = new Cycle.Segment.Frame();

        {
            var constantCurveCount = curveCountsOfType[(int)VaryingCurveType.Vector3F32];
            if (constantCurveCount > 0)
            {
                frame.vector3F32Keys = new Vector3[constantCurveCount];
                for (int i = 0; i < constantCurveCount; i++)
                {
                    frame.vector3F32Keys[i] = reader.ReadVector3();
                }
            }
        }
        {
            var constantCurveCount = curveCountsOfType[(int)VaryingCurveType.Quaternion32];
            if (constantCurveCount > 0)
            {
                frame.quaternion32Keys = new Quaternion[constantCurveCount];
                for (int i = 0; i < constantCurveCount; i++)
                {
                    frame.quaternion32Keys[i] = reader.ReadPackedQuaternion32();
                }
            }
        }
        {
            var constantCurveCount = curveCountsOfType[(int)VaryingCurveType.ScalarF32];
            if (constantCurveCount > 0)
            {
                frame.scalarF32Keys = new float[constantCurveCount];
                for (int i = 0; i < constantCurveCount; i++)
                {
                    frame.scalarF32Keys[i] = reader.ReadSingle();
                }
            }
        }
        {
            var constantCurveCount = curveCountsOfType[(int)VaryingCurveType.Quaternion48];
            if (constantCurveCount > 0)
            {
                frame.quaternion48Keys = new Quaternion[constantCurveCount];
                for (int i = 0; i < constantCurveCount; i++)
                {
                    frame.quaternion48Keys[i] = reader.ReadPackedQuaternion48();
                }
            }
        }
        int packedCurveIndex = 0;

        {
            var constantCurveCount = curveCountsOfType[(int)VaryingCurveType.Vector3U16];
            if (constantCurveCount > 0)
            {
                frame.vector3U16Keys = new Vector3[constantCurveCount];
                for (int i = 0; i < constantCurveCount; i++)
                {
                    var info = packedCurveInfos[packedCurveIndex];
                    frame.vector3U16Keys[i] = reader.ReadPackedVector3U16(info.offset, info.scale);
                    packedCurveIndex++;
                }
            }
        }
        {
            var constantCurveCount = curveCountsOfType[(int)VaryingCurveType.Vector3U8];
            if (constantCurveCount > 0)
            {
                frame.vector3U8Keys = new Vector3[constantCurveCount];
                for (int i = 0; i < constantCurveCount; i++)
                {
                    var info = packedCurveInfos[packedCurveIndex];
                    frame.vector3U8Keys[i] = reader.ReadPackedVector3U8(info.offset, info.scale);
                    packedCurveIndex++;
                }
            }
        }
        return frame;
    }
    public static Quaternion DecompressQuaternion32(uint quat32)
    {
        const uint kMask10 = (1 << 10) - 1;
        const uint kMask9 = (1 << 9) - 1;


        uint largestSign = quat32 & 1;
        int largestIndex = (int)((quat32 >> 1) & 0x3);
        int iC0 = (int)((quat32 >> 3) & kMask9);
        int iC1 = (int)((quat32 >> 12) & kMask10);
        int iC2 = (int)((quat32 >> 22) & kMask10);

        const float fRange0 = 510f;
        const float fRange1 = 1022f;
        const float fRange2 = 1022f;

        float fC0 = kOneOverRootTwo * (2f * (iC0 / fRange0) - 1f);
        float fC1 = kOneOverRootTwo * (2f * (iC1 / fRange1) - 1f);
        float fC2 = kOneOverRootTwo * (2f * (iC2 / fRange2) - 1f);

        Vector4 q = new Vector4();

        int[][] permutes =

        [
            [ 1, 2, 3 ],
			[ 0, 2, 3 ],
			[ 0, 1, 3 ],
			[ 0, 1, 2 ]
        ]
        ;

        q[permutes[largestIndex][0]] = fC0;
        q[permutes[largestIndex][1]] = fC1;
        q[permutes[largestIndex][2]] = fC2;

        float largestSquared = 1f - (fC0 * fC0 + fC1 * fC1 + fC2 * fC2);

        float largestValue = MathF.Sqrt(largestSquared);
        q[largestIndex] = largestSign != 0 ? -largestValue : largestValue;

        return q.AsQuaternion();
    }

    public static Vector3 DecompressVector3U8(ReadOnlySpan<byte> vec3U8, Vector3 offset, Vector3 scale)
    {
        if (vec3U8.Length != 3) throw new NotSupportedException();
        const float fRange = 254f;
        float x = offset.X + scale.X * (vec3U8[0] / fRange);
        float y = offset.Y + scale.Y * (vec3U8[1] / fRange);
        float z = offset.Z + scale.Z * (vec3U8[2] / fRange);
        return new Vector3(x, y, z);
    }
    
    public static Vector3 DecompressVector3U16(ReadOnlySpan<ushort> vec3U16, Vector3 offset, Vector3 scale)
    {
        if (vec3U16.Length != 3) throw new NotSupportedException();
        const float fRange = 65534f;
        float x = offset.X + scale.X * (vec3U16[0] / fRange);
        float y = offset.Y + scale.Y * (vec3U16[1] / fRange);
        float z = offset.Z + scale.Z * (vec3U16[2] / fRange);
        return new Vector3(x, y, z);
    }
}

public class CycleHeader
{
    public int version;

    public JointTransform trajectoryDelta = new JointTransform();
    public JointTransform trajectoryFirst = new JointTransform();
    public JointTransform trajectoryFinal = new JointTransform();
    public JointTransform referenceTransform = new JointTransform();

    public Vector3 angularVelocity = new Vector3();
    public Vector3 linearVelocity = new Vector3();

    public int trajectoryRootFieldIndex = -1;

    public int frameCount = 0;
    /// <summary>
    /// Duration in frames
    /// </summary>
    public float playbackDuration = 0;
    public float frameRate = 0;
    public int referenceTransformCount = 0;

    public int fieldCount = 0;
    public PoseFieldType[] fieldTypes = [];
    public string[] fieldNames = [];
}
public class CycleData
{
    public short totalConstantCurveCount = 0;
    public short totalVaryingCurveCount = 0;

    public short[] constantCurveCountsOfType = new short[(int)ConstantCurveType.Count];
    public short[] varyingCurveCountsOfType = new short[(int)VaryingCurveType.Count];

    public ChannelInfo[] channelInfo = [];
    public int constantCurveDataSize = 0;

    public PackedCurveInfo[] packedCurveInfo = [];
    public Vector3[] constantVector3Curves = [];
    public float[] constantScalarCurves = [];
    public Quaternion[] constantQuaternionCurves = [];

    public int segmentCount = 0;
    public int totalSegmentDataSize = 0;
    public byte[] segmentData = [];
    public Cycle.Segment[] segments = [];

    public int initialFrameSize = 0;
}
public struct PackedCurveInfo
{
    public static PackedCurveInfo Identity { get; } = new PackedCurveInfo
    {
        scale = Vector3.One,
        offset = Vector3.Zero,
    };
    public Vector3 scale;
    public Vector3 offset;
}
public struct ChannelInfo
{
    public int fieldIndex;
    public PoseFieldElement element;
}
public class Cycle
{
    public CycleHeader header = new CycleHeader();
    public CycleData data = new CycleData();
    public class Segment
    {
        public int startFrame = 0;
        public short frameCount;
        public short byteSize;
        public int keyCount;

        public Frame initialFrame = new Frame();
        public Frame processFrame = new Frame();

        public class Frame
        {
            public Vector3[] vector3F32Keys = [];
            public Quaternion[] quaternion32Keys = [];
            public float[] scalarF32Keys = [];
            public Quaternion[] quaternion48Keys = [];
            public Vector3[] vector3U16Keys = [];
            public Vector3[] vector3U8Keys = [];
        }
    }
}
public struct JointTransform
{
    public static JointTransform Identity { get; } = new JointTransform
    {
        rotation = Quaternion.Identity,
        translation = Vector3.Zero,
        scale = Vector3.One,
    };
    public Quaternion rotation;
    public Vector3 translation;
    public Vector3 scale;

    public JointTransform Inverse()
    {
        var newRotation = Quaternion.Inverse(rotation);
        return new JointTransform
        {
            rotation = newRotation,
            translation = Vector3.Transform(-translation, newRotation),
            scale = new Vector3(1 / scale.X, 1 / scale.Y, 1 / scale.Z)
        };
    }
    public static JointTransform operator *(JointTransform left, JointTransform right)
    {
        return new JointTransform
        {
            rotation = left.rotation * right.rotation,
            translation = Vector3.Transform(left.translation, right.rotation) + right.translation,
            scale = left.scale * right.scale
        };
    }
}
public enum ConstantCurveType
{
    Vector3F32,
    ScalarF32,
    Quaternion48,
    Count,
}
public enum VaryingCurveType
{
    Vector3F32,
    Quaternion32,
    ScalarF32,
    Quaternion48,
    Vector3U16,
    Vector3U8,
    Count,
}
/// <summary>
/// Types of data fields that can be present in a pose.
/// <strong>Reordering this enum can break the cycle file format! Appending is generally OK.</strong>
/// </summary>
public enum PoseFieldType
{
    /// <summary>
    /// Skeleton joint transforms.
    /// </summary>
    JointTransform,
    /// <summary>
    /// Skeleton trajectory joint delta transform.
    /// </summary>
    TrajectoryDeltaTransform,
    /// <summary>
    /// Custom 3-d vectors.
    /// </summary>
    CustomVector3,
    /// <summary>
    /// Custom scalars.
    /// </summary>
    CustomScalar,
    /// <summary>
    /// Skeleton trajectory joint transform.
    /// </summary>
    TrajectoryTransform,
    /// <summary>
    /// Custom 4-d vectors.
    /// </summary>
    CustomVector4,
    /// <summary>
    /// Total number of pose field types.
    /// </summary>
    Count
}

/// <summary>
/// Individual elements of pose fields.
///
/// Custom 4D vectors are currently stored as two separate elements to leverage existing code.
/// This wastes around 12 bytes of storage for each 4D vector field in a pose.Gains made in reduced
/// code size and improved key frame compression.
/// 
/// <strong>Reordering this enum will break the cycle file format! Appending is generally OK.</strong>
/// </summary>

public enum PoseFieldElement
{
    /// <summary>
    /// Joint transform rotation quaternion.
    /// </summary>
    JointTransformRotation,
    /// <summary>
    /// Joint transform translation 3-d vector.
    /// </summary>
    JointTransformTranslation,
    /// <summary>
    /// Joint transform scale 3-d vector.
    /// </summary>
    JointTransformScale,
    /// <summary>
    /// Custom scalar value.
    /// </summary>
    CustomScalar,
    /// <summary>
    /// Custom 3-d vector.
    /// </summary>
    CustomVector3,
    /// <summary>
    /// Skeleton trajectory joint delta transform rotation quaternion.
    /// </summary>
    TrajectoryDeltaTransformRotation,
    /// <summary>
    /// Skeleton trajectory joint delta transform translation 3-d vector.
    /// </summary>
    TrajectoryDeltaTransformTranslation,
    /*
    /// <summary>
    /// Skeleton trajectory joint delta transform scale 3-d vector.
    /// </summary>
    TrajectoryDeltaTransformScale,
    */
    /// <summary>
    /// Skeleton trajectory joint transform rotation quaternion.
    /// </summary>
    TrajectoryTransformRotation,
    /// <summary>
    /// Skeleton trajectory joint transform translation 3-d vector.
    /// </summary>
    TrajectoryTransformTranslation,

    /*
    /// <summary>
    /// Skeleton trajectory joint transform rotation quaternion.
    /// </summary>
    TrajectoryTransformScale,
    */
    /// <summary>
    /// Custom 4-d vector, XYZ part.
    /// </summary>
    CustomVector4XYZ,
    /// <summary>
    /// Custom 4-d vector, W part.
    /// </summary>
    CustomVector4W,
    /// <summary>
    /// Total number of pose field element types.
    /// </summary>
    Count
};