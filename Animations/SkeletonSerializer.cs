using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using BlurFileFormats.Animations.Models;
using BlurFileFormats.Shared;

namespace BlurFileFormats.Animations;

public static class SkeletonSerializer
{
    public static Skeleton Deserialize(BinaryReader reader)
    {
        Skeleton skeleton = new Skeleton();

        int version = reader.ReadInt32();
        if(version is < 5 or > 7)
        {
            throw new NotSupportedException("Unsupported Skeleton Version");
        }

        if(version == 6)
        {
            // This appears to be unused.
            byte supportsJointScaling = reader.ReadByte();
        }

        int jointCount = reader.ReadInt32();
        skeleton.bones = new Bone[jointCount];
        for(int i = 0; i < jointCount; i++)
        {
            skeleton.bones[i].parentIndex = reader.ReadInt16();
        }

        for(int i = 0; i < jointCount; i++)
        {
            if(version == 7)
            {
                skeleton.bones[i].jointTransform = reader.DeserializeJointTransform();
            }
            else
            {
                skeleton.bones[i].jointTransform = reader.DeserializeJointTransformWithoutScale();
            }
        }
        for(int i = 0; i < jointCount; i++)
        {
            //skeleton.bones[i].inverseWorld = reader.ReadMat4x3();
        }
        //skeleton.constraints = reader.ReadConstraints();

        var names = reader.ReadStringArray();
        for(int i = 0; i < jointCount; i++)
        {
            skeleton.bones[i].name = names[i];
        }

        return skeleton;
    }
    //public static Constraint[] ReadConstraints(this BinaryReader reader)
    //{
    //    int version = reader.ReadInt32();
    //    if (version < 1) throw new NotSupportedException("Constraint version is not supported.");

    //    int constraintCount = reader.ReadInt32();
    //    int drivenKeyCount = reader.ReadInt32();

    //    byte[] constraintCountsOfType = new byte[(int)ConstraintType.Count];
    //    for (int i = 0; i < (int)ConstraintType.Count; i++)
    //    {
    //        constraintCountsOfType[i] = reader.ReadByte();
    //    }
    //}

    public static ConstraintInfo ReadConstraintInfo(this BinaryReader reader) => new ConstraintInfo
    {
        type = reader.ReadByte(),
        index = reader.ReadByte(),
    };

    public static DrivenKey ReadDrivenKey(this BinaryReader reader) => new DrivenKey
    {
        inputType = reader.ReadInt16(),
        inputJointIndex = reader.ReadInt16(),
        scaleFactor = reader.ReadSingle(),
        curve = reader.ReadCurve()
    };
    public static DrivenKey.Curve ReadCurve(this BinaryReader reader) => new DrivenKey.Curve
    {
        minX = reader.ReadSingle(),
        maxX = reader.ReadSingle(),
        minY = reader.ReadSingle(),
        maxY = reader.ReadSingle(),
        keys = reader.ReadBytes(DrivenKey.Curve.keyCount)
    };
    public static AimConstraint ReadAimConstraint(this BinaryReader reader) => new AimConstraint
    {
        localRotationOffset = reader.ReadQuaternion(),
        localAimTargetOffset = reader.ReadVector3(),
        localAimVector = reader.ReadVector3(),
        localUpVector = reader.ReadVector3(),
        referenceUpVector = reader.ReadVector3(),
        localReferenceJointOffset = reader.ReadVector3(),
        minLimits = reader.ReadVector3(),
        maxLimits = reader.ReadVector3(),
        jointIndex = reader.ReadInt16(),
        aimTargetJointIndex = reader.ReadInt16(),
        upVectorJointIndex = reader.ReadInt16(),
        upVectorType = reader.ReadByte(),
        axisFlags = reader.ReadByte(),
    };
}
public class Skeleton
{
    public Bone[] bones = [];
    public ConstraintCollection constraints = new ConstraintCollection();
}
public class ConstraintCollection
{
    public ConstraintInfo[] constraintInfo = [];
    public DrivenKey[] drivenKeys = [];
    public AimConstraint[] aimConstraints = [];
}
public class AimConstraint
{
    public Quaternion localRotationOffset;
    public Vector3 localAimTargetOffset;
    public Vector3 localAimVector;
    public Vector3 localUpVector;
    public Vector3 referenceUpVector;
    public Vector3 localReferenceJointOffset;
    public Vector3 minLimits;
    public Vector3 maxLimits;
    public short jointIndex;
    public short aimTargetJointIndex;
    public short upVectorJointIndex;
    public byte upVectorType;
    public byte axisFlags;
}
public class OrientationConstraint
{

}
public struct Bone
{
    public short parentIndex;
    public string name;
    public JointTransform jointTransform;
    public Matrix4x4 inverseWorld;
}
public enum ConstraintType
{
    Aim = 0,
    Orientation,
    Position,
    DrivenKeyRotation,
    DrivenKeyTranslation,
    Custom,
    DynamicAimJoint,
    DynamicSliderJoint,
    Count
}
public struct ConstraintInfo
{
    public byte type;
    public byte index;
}
public class DrivenKey
{
    public short inputType;
    public short inputJointIndex;
    public float scaleFactor;
    public Curve curve = new();
    public class Curve
    {
        public const int keyCount = 24;
        public float minX, maxX, minY, maxY;
        public byte[] keys = new byte[keyCount];

        public float this[int index]
        {
            set
            {
                float clampedValue = Math.Clamp(value, minY, maxY);
                uint packedValue = (uint)(255.0f * (clampedValue - minY) / (maxY - minY));
                Debug.Assert(packedValue < 256);
                keys[index] = (byte)(packedValue);
            }
        }

        public float Evaluate(float x)
    	{
            float clampedX = Math.Clamp(x, minX, maxX);

            float fKeyIndex = (clampedX - minX) * (keyCount - 1.0f) / (maxX - minX);

            int currKeyIndex = (int)fKeyIndex;
            int nextKeyIndex = Math.Min(currKeyIndex + 1, keyCount - 1);

            Debug.Assert(currKeyIndex >= 0 && currKeyIndex< keyCount );

            float scale = (maxY - minY) / 255.0f;

            float prevValue = minY + (keys[currKeyIndex] * scale);
            float nextValue = minY + (keys[nextKeyIndex] * scale);
		    		
		    return prevValue + (nextValue - prevValue ) * (fKeyIndex - MathF.Floor(fKeyIndex ) );
	    }
    }
}