using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace BlurFileFormats.SerializationFramework.Attributes;

public abstract class ValueAttribute<T> : Attribute
{
    public string? Path { get; }
    public T? Value { get; init; }
    [MemberNotNullWhen(false, nameof(Path))]
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsValue { get; }
    public ValueAttribute(T value)
    {
        Value = value;
        IsValue = true;
    }
    public ValueAttribute(string path, [CallerArgumentExpression(nameof(path))] string expression = "")
    {
        expression = expression.Trim();
        if(expression.StartsWith("nameof(") && expression.EndsWith(')'))
        {
            path = expression[7..^1];
        }
        path = path.Replace(" ", "");
        Path = path;
        IsValue = false;
    }
}
public abstract class ValueAttribute : Attribute
{
    public string Path { get; }
    public ValueAttribute(string path, [CallerArgumentExpression(nameof(path))] string expression = "")
    {
        expression = expression.Trim();
        if(expression.StartsWith("nameof(") && expression.EndsWith(')'))
        {
            path = expression[7..^1];
        }
        path = path.Replace(" ", "");
        Path = path;
    }
}
