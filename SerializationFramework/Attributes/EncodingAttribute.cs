using System.Runtime.CompilerServices;
using System.Text;

namespace BlurFileFormats.SerializationFramework.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EncodingAttribute : Attribute
{
    public string Path { get; }
    public EncodingAttribute(string path, [CallerArgumentExpression(nameof(path))] string expression = "")
    {
        expression = expression.Trim();
        if (expression.StartsWith("nameof("))
        {
            Path = expression[7..^1].Replace(" ", "");
        }
        else
        {
            Path = path;
        }
    }
    public Encoding GetEncoding(object value, List<object> tree)
    {
        return (Encoding)DataPath.GetValue(Path, value, tree)!;
    }
}
