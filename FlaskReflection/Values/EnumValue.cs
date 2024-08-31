using BlurFileFormats.FlaskReflection.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.FlaskReflection.Values;
public class EnumValue : IDataValue
{
    public EnumType Type { get; }
    IDataType IDataValue.Type => Type;
    public int Value { get; set; }
    object IDataValue.Value { get => Value; set => Value = (int)value; }


    public EnumValue(EnumType type)
    {
        Type = type;
    }

}
