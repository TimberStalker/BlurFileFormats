using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

namespace BlurFileFormats.SerializationFramework.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class LengthAttribute : Attribute
{
    public int Length { get; }
    public DataPath? Path { get; }
    public LengthAttribute(int length)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(length, 0);
        Length = length;
    }
    public LengthAttribute(string path, [CallerArgumentExpression(nameof(path))] string expression = "")
    {
        expression = expression.Trim();
        if (expression.StartsWith("nameof("))
        {
            Path = new DataPath(expression[7..^1].Replace(" ", ""));
        } else
        {
            Path = new DataPath(path);
        }
    }
}
