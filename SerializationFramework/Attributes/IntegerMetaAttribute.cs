using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Attributes;
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public abstract class IntegerMetaAttribute : Attribute
{
}
