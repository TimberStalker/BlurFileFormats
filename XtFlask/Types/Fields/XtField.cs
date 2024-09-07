using System.Diagnostics;
using BlurFileFormats.XtFlask.Types.Fields.Behaviors;
using BlurFileFormats.XtFlask.Values;

namespace BlurFileFormats.XtFlask.Types.Fields;

[DebuggerDisplay("{DebuggerDisplay()}")]
public class XtField
{
    public XtStructType ParentType { get; }
    public IXtType Type { get; }
    public string Name { get; }
    public IFieldBehavior Behavior { get; }
    public XtField(XtStructType parentType, IXtType type, string name, IFieldBehavior behavior)
    {
        ParentType = parentType;
        Type = type;
        Name = name;
        Behavior = behavior;
    }
    public string DebuggerDisplay() => $"{Type.Name} {Name}";

    public IXtValue CreateDefault()
    {
        return Behavior.CreateDefault(Type);
    }

    internal IXtValue ReadValue(XtStructValue value, BinaryReader reader, ValueResolver resolver)
    {
        return Behavior.ReadValue(value, this, reader, resolver);
    }
}
