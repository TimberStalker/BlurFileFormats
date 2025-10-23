using System.Reflection;
using BlurFileFormats.SF2.DataSerializers;

namespace BlurFileFormats.SF2.SerializerCommands;

public class PropertySerializerCommand : ISerializerCommand
{
    public PropertyInfo Property { get; }
    public IDataSerializer DataSerializer { get; }
    public PropertySerializerCommand(PropertyInfo property, IDataSerializer dataSerializer)
    {
        Property = property;
        DataSerializer = dataSerializer;
    }
    public void OnRead(DeserializingState state)
    {
        Property.SetValue(state.CurrentObject, DataSerializer.Deserialize(state));
    }
    public void OnWrite(SerializingState state)
    {
        DataSerializer.Serialize(Property.GetValue(state.CurrentObject), state);
    }
}
