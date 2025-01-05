namespace BlurFileFormats.SerializationFrameworkOld.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class TargetAttribute : Attribute
{
    public object Target { get; }
    public TargetAttribute(object target)
    {
        Target = target;
    }
}
