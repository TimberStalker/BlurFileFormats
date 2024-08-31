namespace BlurFileFormats.Models.Serialization;
public static partial class CPModelSerializer
{
    class BoundingBox
    {
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float Z1 { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }
        public float Z2 { get; set; }
        public BoundingBox(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            X1 = x1;
            Y1 = y1;
            Z1 = z1;
            X2 = x2;
            Y2 = y2;
            Z2 = z2;
        }
        public override string ToString()
        {
            return $"({X1:E2}, {Y1:E2}, {Z1:E2}) - ({X2:E2}, {Y2:E2}, {Z2:E2})";
        }
    }
}
