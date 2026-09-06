namespace BlurFileFormats.UI;

public record class IPFBlendStyle(float Transparency, IPFBlendMode BlendMode);

public enum IPFBlendMode
{
    None,
    Normal,
    Multiply
}