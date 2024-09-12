using System.Reflection;

namespace BlurFileFormats.SerializationFramework.Read;

public class PropertyReadTarget : IReadTarget
{

    object Target { get; }
    PropertyInfo Property { get; }
    public PropertyReadTarget(object target, PropertyInfo property)
    {
        Target = target;
        Property = property;
    }
    public void SetValue(object value)
    {
        Property.SetValue(Target, value);
    }
}