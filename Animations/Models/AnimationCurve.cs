using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public void InsertKey(int frame, object value)
    {
        int i;
        bool replace = false;
        for (i = 0; i < KeyTimes.Count; i++)
        {
            if (KeyTimes[i] >= frame)
            {
                replace = KeyTimes[i] == frame;
                break;
            }
        }
        switch (Keys)
        {
            case AnimationFloatCurveKeys floatCurve:
                var floatValue = (float)value;
                KeyTimes.Add(frame);
                if(replace)
                {
                    floatCurve.Keys[i] = floatValue;
                }
                else
                {
                    floatCurve.Keys.Insert(i, floatValue);
                }
                break;
            case AnimationVector3CurveKeys vec3Curve:
                var vecValue = (Vector3)value;
                KeyTimes.Add(frame);
                if (replace)
                {
                    vec3Curve.Keys[i] = vecValue;
                }
                else
                {
                    vec3Curve.Keys.Insert(i, vecValue);
                }
                break;
            case AnimationQuaternionCurveKeys quaternionCurve:
                var quatValue = (Quaternion)value;
                KeyTimes.Add(frame);
                if (replace)
                {
                    quaternionCurve.Keys[i] = quatValue;
                }
                else
                {
                    quaternionCurve.Keys.Insert(i, quatValue);
                }
                break;
        }
    }
    public void AddKey(int frame, object value)
    {
        Debug.Assert(KeyTimes[^1] < frame);
        switch (Keys)
        {
            case AnimationFloatCurveKeys floatCurve:
                var floatValue = (float)value;
                KeyTimes.Add(frame);
                floatCurve.Keys.Add(floatValue);
                break;
            case AnimationVector3CurveKeys vec3Curve:
                var vecValue = (Vector3)value;
                KeyTimes.Add(frame);
                vec3Curve.Keys.Add(vecValue);
                break;
            case AnimationQuaternionCurveKeys quaternionCurve:
                var quatValue = (Quaternion)value;
                KeyTimes.Add(frame);
                quaternionCurve.Keys.Add(quatValue);
                break;
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