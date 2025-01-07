using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Attributes;
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class FromAttribute : ValueAttribute
{
    public FromTarget From { get; set; } = FromTarget.Value;
    public FromAttribute(string path, [CallerArgumentExpression("path")] string expression = "") : base(path, expression)
    {
    }
}

public enum FromTarget
{
    Value,
    Position,
    Size
}
