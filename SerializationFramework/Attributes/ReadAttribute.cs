using System.Runtime.CompilerServices;

namespace BlurFileFormats.SerializationFramework.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ReadAttribute : Attribute
{
    public int Order { get; }
	public ReadAttribute([CallerLineNumber] int order = 0)
	{
        Order = order;
    }
}
