﻿using BlurFileFormats.SerializationFramework.Attributes;

namespace BlurFileFormats.Models.Serialization.Entities;

public class VectorEntity
{
    [Read] public float X { get; set; }
    [Read] public float Y { get; set; }
    [Read] public float Z { get; set; }
    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }
}
