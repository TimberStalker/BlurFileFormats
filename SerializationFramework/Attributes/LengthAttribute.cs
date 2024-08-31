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

        return Convert.ToInt32(GetValue(value, Path, tree));
    }

    private static object? GetValue(object value, string path, List<object> tree)
    {
        var pathItems = path.Split('.');
        var lengthObj = value;
        var lengthProperty = value.GetType().GetProperty(pathItems[0]);
        int pathIndex = 1;
        if (lengthProperty is null)
        {
            if (pathItems.Length <= 1)
            {
                throw new Exception("Path must contain a direct property or a qualified property of a parent type.");
            }
            int treeIndex;
            for (treeIndex = tree.Count - 1; treeIndex >= 0; treeIndex--)
            {
                var item = tree[treeIndex];
                if (item.GetType().Name == pathItems[0])
                {
                    lengthProperty = item.GetType().GetProperty(pathItems[1]);
                    if (lengthProperty is not null)
                    {
                        lengthObj = item;
                        pathIndex = 2;
                        break;
                    }
                }
            }
            if (lengthProperty is null)
            {
                throw new Exception("Path is not valid.");
            }
        }
        for (; pathIndex < pathItems.Length; pathIndex++)
        {
            lengthObj = lengthProperty.GetValue(lengthObj);

            if (lengthObj is null)
            {
                throw new Exception("Path is not valid.");
            }

            lengthProperty = lengthObj.GetType().GetProperty(pathItems[pathIndex]);
            if (lengthProperty is null)
            {
                throw new Exception("Path is not valid.");
            }
        }
        return lengthProperty.GetValue(lengthObj);
    }
}
