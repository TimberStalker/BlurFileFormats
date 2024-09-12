using System.Runtime.CompilerServices;

namespace BlurFileFormats.SerializationFramework.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class SwitchAttribute : Attribute
{
    public DataPath Path { get; }
    public SwitchAttribute(string path, [CallerArgumentExpression(nameof(path))] string expression = "")
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
