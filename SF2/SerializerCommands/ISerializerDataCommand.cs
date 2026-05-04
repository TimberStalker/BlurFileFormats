namespace BlurFileFormats.SF2.SerializerCommands;

public interface ISerializerCommand
{
    void OnRead(DeserializingState state);
    void OnWrite(SerializingState state);
}
