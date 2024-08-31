using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Attributes;
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public class ReadAttribute : Attribute
{
    public int Order { get; }
    public ReadAttribute([CallerLineNumber] int order = 0)
    {
        Order = order;
    }
}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class CStringAttribute : Attribute
{
}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class PositionAttribute : Attribute
{
}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EncodingAttribute : Attribute
{
    public Encoding Encoding { get; }
    public EncodingAttribute(string encoding)
    {
        Encoding = Encoding.GetEncoding(encoding);
    }
}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class FileSpecifierAttribute : Attribute
{
    public string Specifier { get; }
    public FileSpecifierAttribute(string specifier)
    {
        Specifier = specifier;
    }

}