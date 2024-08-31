using BlurFileFormats.FlaskReflection.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.FlaskReflection.Types;
public class PrimitiveType : IDataType
{
    public string Name { get; set; } = "";
    public Type InternalType { get; }

    public PrimitiveType(Type internalType)
    {
        InternalType = internalType;
    }

    public IDataValue CreateValue()
    {
        if(InternalType == typeof(bool))
        {
            return new BoolValue(this);
        } else if(InternalType == typeof(byte))
        {
            return new ByteValue(this);
        } else if(InternalType == typeof(short))
        {
            return new ShortValue(this);
        } else if(InternalType == typeof(int))
        {
            return new IntValue(this);
        } else if(InternalType == typeof(long))
        {
            return new LongValue(this);
        }
        else
        {
            throw new Exception();
        }
    }
}
