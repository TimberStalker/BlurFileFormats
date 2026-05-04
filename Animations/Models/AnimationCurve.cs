using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BlurFileFormats.Animations.Models;

public class AnimationCurve
{
    public AnimationProperty Property { get; set; }
    public List<int> KeyTimes { get; } = [];
    public AnimationCurveKeys Keys { get; }

    public AnimationCurve(AnimationProperty property)
    {
        Property = property;
        switch (property.PropertyField)
        {
            case PoseFieldElement.JointTransformRotation:
            case PoseFieldElement.TrajectoryDeltaTransformRotation:
            case PoseFieldElement.TrajectoryTransformRotation:
                Keys = new AnimationQuaternionCurveKeys();
                break;
            case PoseFieldElement.TrajectoryTransformTranslation:
            case PoseFieldElement.TrajectoryDeltaTransformTranslation:
            case PoseFieldElement.JointTransformTranslation:
            case PoseFieldElement.JointTransformScale:
            case PoseFieldElement.CustomVector3:
            case PoseFieldElement.CustomVector4XYZ:
                Keys = new AnimationVector3CurveKeys();
                break;
            case PoseFieldElement.CustomScalar:
            case PoseFieldElement.CustomVector4W:
                Keys = new AnimationFloatCurveKeys();
                break;
            default:
                throw new NotSupportedException();
        }
    }
}
public interface AnimationCurveKeys
{
    public Type KeyType { get; }
}
public class AnimationVector3CurveKeys : AnimationCurveKeys
{
    public Vector3CurveCompression Compression { get; }
    public List<Vector3> Keys { get; } = [];
    public Type KeyType => typeof(Vector3);
}
public class AnimationFloatCurveKeys : AnimationCurveKeys
{
    public List<float> Keys { get; } = [];
    public Type KeyType => typeof(float);
}
public class AnimationQuaternionCurveKeys : AnimationCurveKeys
{
    public QuaternionCurveCompression Compression { get; }
    public List<Quaternion> Keys { get; } = [];
    public Type KeyType => typeof(Quaternion);
}

public enum Vector3CurveCompression
{
    None,
    U16,
    U8,
}

public enum QuaternionCurveCompression
{
    U48,
    U32,
}