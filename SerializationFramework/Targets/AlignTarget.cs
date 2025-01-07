using BlurFileFormats.SerializationFramework.Sources;

namespace BlurFileFormats.SerializationFramework.Targets;

public class AlignTarget : ISerializerTarget
{
    public ISerializerTarget InternalTarget { get; }
    public ISerializerSource AlignSource { get; }

    public AlignTarget(ISerializerTarget internalTarget, ISerializerSource alignSource)
    {
        InternalTarget = internalTarget;
        AlignSource = alignSource;
    }

    public void Deserialize(BinaryReader reader, SerializerInfo readInfo, object target)
    {
        var position = reader.BaseStream.Position - readInfo.CurrentAnchor;
        var align = Convert.ToInt32(AlignSource.Read(reader, readInfo));
        if (position % align != 0)
        {

            int paddingCount = align - (int)position % align;
            for (int i = 0; i < paddingCount; i++)
            {
                var temp = reader.ReadByte();
            }
        }
        InternalTarget.Deserialize(reader, readInfo, target);
    }

    public void Serialize(BinaryWriter writer, SerializerInfo writeInfo, object target)
    {
        var position = writer.BaseStream.Position - writeInfo.CurrentAnchor;
        var align = Convert.ToInt32(AlignSource.Write(writer, writeInfo, null));
        if (position % align != 0)
        {
            int paddingCount = align - (int)position % align;
            for (int i = 0; i < paddingCount; i++)
            {
                writer.Write((byte)0);
            }
        }
        InternalTarget.Serialize(writer, writeInfo, target);
    }
}
