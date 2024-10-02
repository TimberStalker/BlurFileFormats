using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace BlurFileFormats.SerializationFramework.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EncodingAttribute : Attribute
{
    public DataPath Path { get; }
    public EncodingAttribute(string path, [CallerArgumentExpression(nameof(path))] string expression = "")
    {
        expression = expression.Trim();
        if (expression.StartsWith("nameof("))
        {
            Path = new DataPath(expression[7..^1].Replace(" ", ""));
        }
        else
        {
            Path = new DataPath(path);
        }
    }
}
