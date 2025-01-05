namespace BlurFileFormats.SerializationFramework.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EncodingAttribute : Attribute
{
    public string Encoding { get; }
    public EncodingAttribute(string encoding)
    {
        Encoding = encoding;
    }
}