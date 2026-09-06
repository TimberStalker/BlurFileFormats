using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using BlurFileFormats.Shared;
using System.Linq;
using BlurFileFormats.Animations.Models;

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

            cycleHeader.fieldNames = reader.ReadStringArray();
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