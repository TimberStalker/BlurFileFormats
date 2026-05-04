using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

namespace BlurFileFormats.SF2.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReadAttribute : Attribute;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ParameterAttribute : Attribute
    {
        public ParameterSource Source { get; set; }
    }
    public enum ParameterSource
    {
        None,
        ArrayIndex
    }
}
