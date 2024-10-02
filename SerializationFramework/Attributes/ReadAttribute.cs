using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
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
    public static IRead[] GetReads(Type t) => t.GetProperties()
            .Select(o => (IRead)new ReadProperty(o))
            .Concat(t.GetMethods().Select(m => new ReadMethod(m)))
            .OrderBy(p => p.Order)
            .ToArray();
}
