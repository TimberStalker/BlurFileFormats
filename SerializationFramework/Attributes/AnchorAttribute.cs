using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Attributes;
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class AnchorAttribute : Attribute
{
}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EndianSwitchAttribute : Attribute
{
}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class CStringAttribute : Attribute
{
}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class PositionAttribute : Attribute
{
}
