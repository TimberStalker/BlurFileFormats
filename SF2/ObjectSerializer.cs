using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using BlurFileFormats.SF2.SerializerCommands;

namespace BlurFileFormats.SF2;

public class ObjectSerializer
{
    public List<ISerializerCommand> Commands { get; } = new List<ISerializerCommand>();
    public Dictionary<Type, ObjectSerializer> SubSerializers { get; } = new Dictionary<Type, ObjectSerializer>();
    Type Type { get; }


    public ObjectSerializer(Type type)
    {
        Type = type;
    }

    public void Serialize(object data)
    {
    }
    public object Deserialize()
    {
        object t = Activator.CreateInstance(Type)!;
        return t;
    }
}

public abstract class SerializationState
{
    public object RootObject { get; }
    public object CurrentObject { get; set; }
    public SerializationState(object rootObject)
    {
        RootObject = rootObject;
        CurrentObject = rootObject;
    }
}

public class SerializingState : SerializationState
{
    public BinaryWriter Writer { get; }
    public SerializingState(object rootObject, BinaryWriter writer) : base(rootObject)
    {
        Writer = writer;
    }
}

public class DeserializingState : SerializationState
{
    public BinaryReader Reader { get; }
    public DeserializingState(object rootObject, BinaryReader reader) : base(rootObject)
    {
        Reader = reader;
    }
}