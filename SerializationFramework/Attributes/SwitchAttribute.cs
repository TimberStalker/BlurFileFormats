using System.Runtime.CompilerServices;

namespace BlurFileFormats.SerializationFramework.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class SwitchAttribute : ValueAttribute
{
    public SwitchAttribute(string path, [CallerArgumentExpression(nameof(path))] string expression = "") : base(path, expression)
    {
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class DefaultAttribute : Attribute
{
    public DefaultAttribute()
    {
    }
}
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class TargetAttribute : Attribute
{
    public object Target { get; }
    public TargetAttribute(object target)
    {
        Target = target;
    }
}
