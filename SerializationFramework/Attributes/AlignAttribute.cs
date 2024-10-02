using System.Reflection;
using System.Runtime.CompilerServices;

namespace BlurFileFormats.SerializationFramework.Attributes;

public class AlignAttribute : IntegerMetaAttribute
{
    public int Align { get; }
    public DataPath? Path { get; }
    public AlignAttribute(int align)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(align, 0);
        Align = align;
    }
    public AlignAttribute(string path, [CallerArgumentExpression(nameof(path))] string expression = "")
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

    public static long GetAlignOffset(long position, long align) => ((position + align - 1) & -align) - position;

    public static void AlignStream(int align, BinaryReader reader)
    {
        long alignOffset = GetAlignOffset(reader.BaseStream.Position, align);
        reader.BaseStream.Seek(alignOffset, SeekOrigin.Current);
    }
    public static void AlignStream(int align, BinaryWriter writer)
    {
        long alignOffset = GetAlignOffset(writer.BaseStream.Position, align);
        for (long i = 0; i < alignOffset; i++)
        {
            writer.Write((byte)0);
        }
    }
}
