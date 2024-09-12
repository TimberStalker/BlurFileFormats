using BlurFileFormats.SerializationFramework.Attributes;
using BlurFileFormats.SerializationFramework.Command;
using System.Reflection;

namespace BlurFileFormats.SerializationFramework;

public class ReadMethod : IRead
{
    MethodInfo Method { get; }

    public ReadMethod(MethodInfo method)
    {
        Method = method;
    }

    public void Read(BinaryReader reader, ReadTree tree)
    {
        if(Method.GetCustomAttribute<AlignAttribute>() is AlignAttribute a)
        {
            a.AlignStream(tree, reader);
        }
        var parameters = Method.GetParameters();
        object[] arguments = new object[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            if (parameter.ParameterType == typeof(ReadTree))
            {
                arguments[i] = tree;
            }
            else if (parameter.ParameterType == typeof(BinaryReader))
            {
                arguments[i] = reader;
            }
        }
        Method.Invoke(tree.CurrentObject, arguments);
    }

    public void Build(List<ISerializationCommand> commands)
    {
        throw new NotImplementedException();
    }
}