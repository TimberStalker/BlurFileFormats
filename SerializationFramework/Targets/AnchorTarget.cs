namespace BlurFileFormats.SerializationFramework.Targets;

public class AnchorTarget : ISerializerTarget
{
    public ISerializerTarget InternalTarget { get; }

    public AnchorTarget(ISerializerTarget internalTarget)
    {
        InternalTarget = internalTarget;
    }

    public void Deserialize(BinaryReader reader, SerializerInfo readInfo, object target)
    {
        readInfo.AnchorStack.Push((int)reader.BaseStream.Position);
        InternalTarget.Deserialize(reader, readInfo, target);
        readInfo.AnchorStack.Pop();
    }

    public void Serialize(BinaryWriter writer, SerializerInfo writeInfo, object target)
    {
        writeInfo.AnchorStack.Push((int)writer.BaseStream.Position);
        InternalTarget.Serialize(writer, writeInfo, target);
        writeInfo.AnchorStack.Pop();
    }
}
