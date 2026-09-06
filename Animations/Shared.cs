using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using BlurFileFormats.Animations.Models;
using BlurFileFormats.Shared;

namespace BlurFileFormats.Animations;

public static class Shared
{

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


    public static JointTransform DeserializeJointTransform(this BinaryReader reader)
    {
        var transform = reader.DeserializeJointTransformWithoutScale();
        transform.scale = reader.ReadVector3();
        return transform;
    }
    public static JointTransform DeserializeJointTransformWithoutScale(this BinaryReader reader)
    {
        var transform = JointTransform.Identity;
        transform.rotation = reader.ReadQuaternion();
        transform.translation = reader.ReadVector3();
        return transform;
    }

    public static string[] ReadStringArray(this BinaryReader reader)
    {
        int nameCount = reader.ReadInt32();
        var names = new string[nameCount];
        for (int i = 0; i < nameCount; i++)
        {
            int stringLength = reader.ReadInt32();
            var name = Encoding.ASCII.GetString(reader.ReadBytes(stringLength));
            names[i] = name;
        }
        return names;
    }
}
