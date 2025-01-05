using System.Runtime.CompilerServices;

namespace BlurFileFormats.SerializationFramework.Attributes;

public class StringLengthAttribute : ValueAttribute<int>
{
    public StringLengthAttribute(int value) : base(value)
    {
    }

    public StringLengthAttribute(string path, [CallerArgumentExpression(nameof(path))] string expression = "") : base(path, expression)
    {
    }
}