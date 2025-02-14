using BlurFileFormats.Packing.Entities;
using BlurFileFormats.SerializationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.Packing;
public class PackSerializer
{
    static DataSerializer Serializer = DataSerializer.Create(typeof(PakEntity));
    public static void Deserialize(Stream stream)
    {
        var pack = (PakEntity)Serializer.Read(stream);
        ;
    }
}
