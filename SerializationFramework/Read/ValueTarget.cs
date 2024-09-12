using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Read;
public class ValueTarget : IReadTarget
{
    [AllowNull]
    public object Value { get; set; }
    public void SetValue(object value)
    {
        Value = value;
    }
}
