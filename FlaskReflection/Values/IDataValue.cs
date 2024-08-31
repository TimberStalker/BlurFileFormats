using BlurFileFormats.FlaskReflection.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.FlaskReflection.Values;
public interface IDataValue
{
    IDataType Type { get; }
    object Value { get; set; }
}
public class NullDataValue : IDataValue
{
    public static NullDataValue NullValue { get; } = new NullDataValue();
    public object Value { get => 0; set { } }

    public IDataType Type { get => throw new NotImplementedException(); }
}