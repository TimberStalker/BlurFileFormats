using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework;
public static partial class DataSerializer
{
    public static void Serialize(object t, Stream stream)
    {
        using var writer = new BinaryWriter(stream);
        Serialize(t, writer);
    }
    static void Serialize(object t, BinaryWriter writer, Action<object>? setValue = null, List<object>? tree = null)
    {

    }

}
