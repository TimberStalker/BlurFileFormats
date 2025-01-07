namespace BlurFileFormats.SerializationFramework.Targets;

public interface ISerializerTarget
{
    public void Deserialize(BinaryReader reader, SerializerInfo readInfo, object target);
    public void Serialize(BinaryWriter writer, SerializerInfo writeInfo, object target);
}
