using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Attributes;
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class AlignAttribute : ValueAttribute<int>
{
    public AlignAttribute(int value) : base(value)
    {
    }

    public AlignAttribute(string path, [CallerArgumentExpression(nameof(path))] string expression = "") : base(path, expression)
    {
    }
}
