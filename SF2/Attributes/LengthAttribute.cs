using System.Runtime.CompilerServices;

namespace BlurFileFormats.SF2.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LengthAttribute : Attribute
    {
        public enum Type
        {
            Length,
            Path
        }
        public Type LengthType { get; }
        public int? Length { get; }
        public string[]? Path { get; }
        public LengthAttribute(int length)
        {
            LengthType = Type.Length;
            Length = length;
        }
        public LengthAttribute(string path, [CallerArgumentExpression(nameof(path))] string? text = null)
        {
            LengthType = Type.Path;
            if (text?.StartsWith("nameof") == true)
            {
                path = text.Trim()["nameof(".Length..^1];
            }
            Path = path.Split('.');
        }
        public bool TryGetPath(out string[] path)
        {
            if (LengthType == Type.Path && Path != null)
            {
                path = Path;
                return true;
            }
            path = Array.Empty<string>();
            return false;
        }
        public bool TryGetLength(out int length)
        {
            if (LengthType == Type.Length && Length.HasValue)
            {
                length = Length.Value;
                return true;
            }
            length = 0;
            return false;
        }
    }
}
