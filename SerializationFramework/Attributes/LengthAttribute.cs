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
    public static int? GetLength(PropertyInfo property, ReadTree tree)
    {
        var attribute = property.GetCustomAttribute<LengthAttribute>();
        if (attribute is null) return null;
        if (attribute.Path is null)
        {
            return attribute.Length;
        }

        return Convert.ToInt32(tree.GetValue(attribute.Path));
    }
}
