using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class VertexStreamEntity
{
    [Read] public byte Id { get; set; }

    [Read] public short Unknown1_0 { get; set; }
    [Read] public short Unknown1_1 { get; set; }

    [Read] public short Unknown2_0 { get; set; }
    [Read] public short Unknown2_1 { get; set; }

    [Read] public int ByteLength { get; set; }

    [Read] public short Unknown3_0 { get; set; }
    [Read] public short Unknown3_1 { get; set; }
    [Read] public VertexStreamDefinitionEntity[] VertexStreamDefinitions { get; set; }

    [Read] public short Unknown4_0 { get; set; }
    [Read] public short Unknown4_1 { get; set; }

    [Read] public short[] VertexOffsets { get; set; }

    [Read] public int VertexCount { get; set; }
    [Read] public int Unknown5 { get; set; }

    [Read] public int VertexStreamLength { get; set; }

    [Read] public int Unknown6 { get; set; }
    [Position]
    [Read] public int Position { get; set; }
    public int Align => Position % 16;
    [Read]
    public void FixOffset(BinaryReader reader)
    {
        var position = reader.BaseStream.Position;
        reader.BaseStream.Position += VertexStreamLength;
        while(true)
        {
            if(reader.ReadByte() == 0x02)
            {
                if(reader.ReadByte() == 0x52)
                {
                    if(reader.ReadByte() == 0x41)
                    {
                        reader.BaseStream.Position -= VertexStreamLength;
                        reader.BaseStream.Position -= 3;
                        TruePosition = (int)reader.BaseStream.Position;
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
                    reader.BaseStream.Position -= VertexStreamLength;
                    reader.BaseStream.Position -= 2;
                    TruePosition = (int)reader.BaseStream.Position;
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
    public int TruePosition { get; set; }
    public int TrueAlign => TruePosition % 16;
    public int ZeroLength => TruePosition - Position;

    [Length(nameof(ZeroLength))]
    [Read] public byte[] MissedBytes { get; set; }
    [Length(nameof(VertexStreamLength))]
    [Read] public byte[] VertexStream { get; set; }
}
