using System.Diagnostics;

namespace BlurFileFormats.XtFlask.Types.Fields;

[DebuggerDisplay("{DebuggerDisplay()}")]
public class XtField
{
    public XtStructType ParentType { get; }
    public IXtType Type { get; }
    public string Name { get; }
    public int Size { get; internal set; }

    public XtField(XtStructType parentType, IXtType type, string name)
    {
        ParentType = parentType;
        Type = type;
        Name = name;
    }
    public string DebuggerDisplay() => $"{Type.Name} {Name}";
}
