﻿using BlurFileFormats.SerializationFramework.Read;

namespace BlurFileFormats.SerializationFramework.Command.Numeric;

public class UInt64Command : ISerializationValueCommand
{
    public void Read(BinaryReader reader, ReadTree tree, IReadTarget target)
    {
        target.SetValue(reader.ReadUInt64());
    }

    public void Write(BinaryWriter writer, ReadTree tree, object value)
    {
        writer.Write((ulong)value);
    }
}