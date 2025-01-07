using BlurFileFormats.SerializationFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.Models.Entities;
public enum VertexElementType
{
    S16_2,
    S16_4,
    S16_2N,
    S16_4N,
    F32_1,
    F32_2,
    F32_3,
    F32_4,
    F16_2,
    F16_4,
    U8_4,
    U8_4N,
    SHHD_3N,
    SDDD_3N,
    Max,
}
public enum VertexElementUsage
{
    Position,
    BlendWeight,
    BlendIndecies,
    Normal,
    PSize,
    TextureCoordinate,
    Tangent,
    BiNormal,
    TesselationFactor,
    Color,
    Sample,
    Max,
}