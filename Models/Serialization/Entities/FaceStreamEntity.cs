using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class FaceStreamEntity
{
    [Read] public byte ID { get; set; }
    [Read] public int Unknown1 { get; set; }
    [Read] public int FaceCount { get; set; }
    [Read] public int Unknown2 { get; set; }
    [Read] public int Unknown3 { get; set; }
    [Read] public int FaceStreamLength { get; set; }

    [Read] public int Unknown4 { get; set; }
    [Read]
    public void FixOffset(BinaryReader reader)
    {
        var position = reader.BaseStream.Position;
        reader.BaseStream.Position += FaceStreamLength;
        while (true)
        {
            if (reader.ReadByte() == 0x02)
            {
                if (reader.ReadByte() == 0x52)
                {
                    if (reader.ReadByte() == 0x41)
                    {
                        reader.BaseStream.Position -= FaceStreamLength;
                        reader.BaseStream.Position -= 3;
                        Offset = (int)(reader.BaseStream.Position - position);
                        reader.BaseStream.Position = position;
                        return;
                    }
                    else
                    {
                        reader.BaseStream.Position -= 2;
                    }
                }
                else
                {
                    reader.BaseStream.Position -= 2;
                }
            }
            else
            {
                reader.BaseStream.Position -= 1;
            }
            if (reader.ReadByte() == 0x52)
            {
                if (reader.ReadByte() == 0x41)
                {
                    reader.BaseStream.Position -= FaceStreamLength;
                    reader.BaseStream.Position -= 2;
                    Offset = (int)(reader.BaseStream.Position - position);
                    reader.BaseStream.Position = position;
                    return;
                }
                else
                {
                    reader.BaseStream.Position -= 1;
                }
            }
        }
    }
    public int Offset { get; set; }

    [Length(nameof(Offset))]
    [Read] public byte[] MissedBytes { get; set; }
    [Length(nameof(FaceStreamLength))]
    [Read] public byte[] VertexStream { get; set; }
}