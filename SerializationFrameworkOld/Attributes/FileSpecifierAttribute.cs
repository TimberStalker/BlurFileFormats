namespace BlurFileFormats.SerializationFrameworkOld.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class FileSpecifierAttribute : Attribute
{
    public string Specifier { get; }
    public FileSpecifierAttribute(string specifier)
    {
        Specifier = specifier;
    }

}