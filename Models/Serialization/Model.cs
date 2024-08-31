namespace BlurFileFormats.Models.Serialization;
public static partial class CPModelSerializer
{
    class Model
    {
        public Matrix Matrix { get; set; } = new Matrix();
        public BoundingBox BoundingBox { get; set; } = new BoundingBox(0, 0, 0, 0, 0, 0);
        public int NameIndex { get; set; }
        public int ModelIndex { get; set; }
        public int ElementCount { get; set; }
        public int HierarchyIndex { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
    }
    class Matrix
    {
        public float M00 { get; set; }
        public float M01 { get; set; }
        public float M02 { get; set; }
        public float M03 { get; set; }

        public float M10 { get; set; }
        public float M11 { get; set; }
        public float M12 { get; set; }
        public float M13 { get; set; }

        public float M20 { get; set; }
        public float M21 { get; set; }
        public float M22 { get; set; }
        public float M23 { get; set; }

        public float M30 { get; set; }
        public float M31 { get; set; }
        public float M32 { get; set; }
        public float M33 { get; set; }

        public float ScaleX => M00;
        public float ScaleY => M11;
        public float ScaleZ => M22;

        public float RotationX => MathF.Atan2(M21, M11);
        public float RotationY => MathF.Atan2(-M20, MathF.Sqrt(M21 * M21 + M11 * M11));
        public float RotationZ => MathF.Atan2(M10, M00);

        public override string ToString()
        {
            return $"{M00,6:0.00} {M01,6:0.00} {M02,6:0.00} {M03,6:0.00}\n{M10,6:0.00} {M11,6:0.00} {M12,6:0.00} {M13,6:0.00}\n{M20,6:0.00} {M21,6:0.00} {M22,6:0.00} {M23,6:0.00}\n{M30,6:0.00} {M31,6:0.00} {M32,6:0.00} {M33,6:0.00}";
        }
    }
}
