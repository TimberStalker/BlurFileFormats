using System.Runtime.CompilerServices;

namespace BlurFileFormats.SerializationFramework.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class LengthAttribute : Attribute
{
    public int Length { get; }
    public string? Path { get; }
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
            Path = expression[7..^1].Replace(" ", "");
        } else
        {
            Path = path;
        }
    }
    public int GetLength(object value, List<object> tree)
    {
        if (Path is null)
        {
            return Length;
        }

        return Convert.ToInt32(DataPath.GetValue(Path, value, tree));
    }
}
