using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Attributes;
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public sealed class ReadAttribute : Attribute
{
    public int Order { get; }
    public ReadAttribute([CallerLineNumber] int order = 0)
    {
        Order = order;
    }
}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class CStringAttribute : Attribute
{
}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class EndianSwitchAttribute : Attribute
{
}
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class DefaultAttribute : Attribute
{
}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class SwitchAttribute : Attribute
{
    public string Path { get; }
    public SwitchAttribute(string path, [CallerArgumentExpression(nameof(path))] string expression = "")
    {
        expression = expression.Trim();
        if (expression.StartsWith("nameof("))
        {
            Path = expression[7..^1].Replace(" ", "");
        }
        else
        {
            Path = path;
        }
    }
    public object GetTarget(object value, List<object> tree)
    {
        return DataPath.GetValue(Path, value, tree)!;
    }
}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class GetAttribute : Attribute
{
    public string Path { get; }
    public GetAttribute(string path, [CallerArgumentExpression(nameof(path))] string expression = "")
    {
        expression = expression.Trim();
        if (expression.StartsWith("nameof("))
        {
            Path = expression[7..^1].Replace(" ", "");
        }
        else
        {
            Path = path;
        }
    }
    public object GetTarget(object value, List<object> tree)
    {
        return DataPath.GetValue(Path, value, tree)!;
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
