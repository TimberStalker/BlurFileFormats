using System.Runtime.CompilerServices;

namespace BlurFileFormats.SerializationFramework.Attributes;

public class LengthAttribute : ValueAttribute<int>
{
    public int Depth { get; }
    public LengthAttribute(int value, int depth = 0) : base(value)
    {
        Depth = depth;
    }

    public LengthAttribute(string path, int depth = 0, [CallerArgumentExpression(nameof(path))] string expression = "") : base(path, expression)
    {
        Depth = depth;
    }
}
