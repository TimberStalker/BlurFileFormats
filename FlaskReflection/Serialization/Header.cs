using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.FlaskReflection.Serialization;
public struct Header
{
    public uint Start { get; private set; }
    public uint Length { get; private set; }
    public Header(uint start, uint length)
    {
        Start = start;
        Length = length;
    }
}
