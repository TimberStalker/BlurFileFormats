using System.Numerics;

namespace BlurFileFormats.Animations.Models;

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
