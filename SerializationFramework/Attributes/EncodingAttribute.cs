using System.Text;

namespace BlurFileFormats.SerializationFramework.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EncodingAttribute : Attribute
{
    public Encoding Encoding { get; }
    public EncodingAttribute(string encoding)
    {
        Encoding = Encoding.GetEncoding(encoding);
    }
}
