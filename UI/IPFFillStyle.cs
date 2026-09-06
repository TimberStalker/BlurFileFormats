using System.Drawing;
using System.Numerics;

namespace BlurFileFormats.UI;

public interface IPFFillStyle;

public sealed record class IPFSolidColorFillStyle(IPFColor Color) : IPFFillStyle;

public sealed record class IPFGradientFillStyle(int DictionaryIndex, float Angle, Vector2 Offset, float Length) : IPFFillStyle;
