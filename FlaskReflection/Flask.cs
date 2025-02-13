using BlurFileFormats.FlaskReflection.Entities;
using BlurFileFormats.SerializationFramework;
using BlurFileFormats.Utils;
using BlurFileFormats.XtFlask.Types;
using BlurFileFormats.XtFlask.Values;
using Microsoft.VisualBasic;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks.Dataflow;
using static System.Reflection.Metadata.BlobBuilder;

namespace BlurFileFormats.FlaskReflection;
public static class Flask
{
    static DataSerializer FlaskSerializer { get; } = DataSerializer.Create(typeof(FlaskEntity));
    static Flask()
    {
        DataSerializer.TryAddEncoding("flask", new FlaskEncoding());
    }
    public static XtDatabase Import(string file)
    {
        using var fileStream = File.OpenRead(file);
        return Import(fileStream);
    }
    public static XtDatabase Import(Stream source)
    {
        var flaskEntity = (FlaskEntity)FlaskSerializer.Read(source);
        if (source.Length != source.Position) throw new Exception("Not all read");
        
        List<IXtType> types = [];
        //Get initial types.
        for(int i = 0; i < flaskEntity.Types.Length; i++)
        {
            var type = flaskEntity.Types[i];
            switch (type.Behavior)
            {
                case TypeBehavior.Atom:
                    types.Add(new XtAtomType(flaskEntity.Strings.CStr(type.Name), type.Atom));
                    break;
                case TypeBehavior.Enum:
                    types.Add(new XtEnumType(flaskEntity.Strings.CStr(type.Name)) { IsFlags = false });
                    break;
                case TypeBehavior.Flags:
                    types.Add(new XtEnumType(flaskEntity.Strings.CStr(type.Name)) { IsFlags = true });
                    break;
                case TypeBehavior.Struct:
                    types.Add(new XtStructType(flaskEntity.Strings.CStr(type.Name), type.Size));
                    break;
                default:
                    throw new NotSupportedException();
            };
        }

        //Get struct base types.
        for (int i = 0, baseIndex = 0; i < flaskEntity.Types.Length; i++)
        {
            var type = flaskEntity.Types[i];
            for(int j = 0; j < type.BaseCount; j++, baseIndex++)
            {
                if (types[i] is not XtStructType structType) throw new NotSupportedException("Types that are not structs cannot have base types.");
                var baseType = flaskEntity.Bases[baseIndex];
                if (types[baseType.Type] is not XtStructType baseStructType) throw new NotSupportedException("Types that are not structs cannot be base types.");
                structType.Bases.Add(baseStructType);
            }
        }

        //Get struct,enum, flags fields.
        for(int i = 0, fieldIndex = 0; i < flaskEntity.Types.Length; i++)
        {
            var typeEntity = flaskEntity.Types[i];
            for(int j = 0; j < typeEntity.FieldCount; j++, fieldIndex++)
            {
                var fieldEntity = flaskEntity.Fields[fieldIndex];
                switch (types[i])
                {
                    case XtStructType structType:
                        var fieldType = types[fieldEntity.Type];
                        fieldType = fieldEntity.Behavior switch
                        {
                            FieldBehavior.Pointer => XtPointerType.Get(fieldType),
                            FieldBehavior.Handle => XtHandleType.Get(fieldType),
                            _ => fieldType
                        };
                        if (fieldEntity.IsArray)
                        {
                            fieldType = XtArrayType.Get(fieldType);
                        }
                        structType.Fields.Add(new XtStructField(flaskEntity.Strings.CStr(fieldEntity.Name), structType, fieldType));
                        break;
                    case XtEnumType enumType:
                        enumType.Labels.Add(flaskEntity.Strings.CStr(fieldEntity.Name));
                        break;
                    case var c:
                        throw new NotSupportedException($"{c.GetType().Name} cannot contain fields.");
                }
            }
        }
        var textEncoding = new FlaskEncoding();

        using var data_stream = new MemoryStream(flaskEntity.Data);
        using var reader = new BinaryReader(data_stream);
        //Parse Flask Blocks
        List<IXtRef> refs = [];
        List<List<XtBlock>> refBlocks = [];
        List<string> texts = [];
        for(int i = 0, componentIndex = 0; i < flaskEntity.Refs.Length; i++)
        {
            var refEntity = flaskEntity.Refs[i];
            var type = types[refEntity.Type];
            if (refEntity.Record == ushort.MaxValue)
            {
                refs.Add(new XtExternalRef(refEntity.Id, type));
                refBlocks.Add([]);
                texts.Add("");
                continue;
            }
            var recordEntity = flaskEntity.Records[refEntity.Record];
            List<XtBlock> blocks = [];

            long recordBytesStart = reader.BaseStream.Position;
            for (int j = 0; j < recordEntity.ComponentCount; j++, componentIndex++)
            {
                var componentEntity = flaskEntity.Components[componentIndex];
                var blockType = types[componentEntity.Type];
                blockType = componentEntity.Behavior switch
                {
                    ComponentBehavior.Pointer => XtPointerType.Get(blockType),
                    ComponentBehavior.Handle => XtHandleType.Get(blockType),
                    _ => blockType
                };
                XtBlock block = new XtBlock(blockType);
                
                for(int k = 0; k < componentEntity.Count; k++)
                {
                    switch (block.BlockType)
                    {
                        case XtPointerType pointerType:
                            block.Add(new XtBlock.PointerValue(pointerType, reader.ReadUInt16(), reader.ReadUInt16()));
                            break;
                        case XtHandleType handleType:
                            block.Add(new XtBlock.HandleValue(handleType, reader.ReadUInt32()));
                            break;
                        case var c:
                            long position = reader.BaseStream.Position;
                            block.Add(Parse(c, reader));
                            long readCount = reader.BaseStream.Position - position;
                            Debug.Assert(readCount == c.Size, $"Did not read the right amount of characters. Expected: {c.Size} - Read: {readCount}");
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }

                blocks.Add(block);
            }
            long recordBytesCount = reader.BaseStream.Position - recordBytesStart;
            Debug.Assert(recordBytesCount == recordEntity.DataBytes, $"Did not read the right amount of characters. Expected: {recordEntity.DataBytes} - Read: {recordBytesCount}");
            
            string text = textEncoding.GetString(reader.ReadBytes((int)recordEntity.StringBytes));
            refBlocks.Add(blocks);
            texts.Add(text);
            refs.Add(new XtRef(refEntity.Id, blocks[0].Values[0]));
        }
        for(int i = 0; i < refs.Count; i++)
        {
            var blocks = refBlocks[i];
            Dictionary<(int, int), XtArray> arrays = [];
            for(int j = 0; j < blocks.Count; j++)
            {
                var block = blocks[j];
                for(int k = 0; k < block.Values.Count; k++)
                {
                    if(block.Values[k] is IXtValueContainer container)
                    {
                        foreach (var value in container)
                        {
                            Reference(value, refs, blocks, arrays, texts[i]);
                        }
                    }
                    if(block.BlockType is not XtPointerType or XtHandleType)
                    {
                        (refs[i] as XtRef)!.RefHeap.Add(block.Values[k]);
                    }
                }
            }
        }

        XtDatabase xtDatabase = new XtDatabase();
        xtDatabase.Types.AddRange(types);
        xtDatabase.Refs.AddRange(refs);
        return xtDatabase;
    }
    static void Reference(IXtValueItem value, List<IXtRef> refs, List<XtBlock> blocks, Dictionary<(int, int), XtArray> arrays, string text)
    {
        switch (value.Value)
        {
            case IXtValueContainer container:
                foreach (var item in container)
                {
                    Reference(item, refs, blocks, arrays, text);
                }
                break;
            case XtBlock.PointerValue pointerValue:
                if(pointerValue.Block == ushort.MaxValue || pointerValue.Offset == ushort.MaxValue)
                {
                    value.Value = pointerValue.Type.CreateValue();
                }
                else
                {
                    value.Value = pointerValue.Type.CreateValue(blocks[pointerValue.Block].Values[pointerValue.Offset]);
                }
                break;
            case XtBlock.HandleValue handleValue:
                if (handleValue.Handle == uint.MaxValue)
                {
                    value.Value = handleValue.Type.CreateValue();
                }
                else
                {
                    value.Value = handleValue.Type.CreateValue(refs[(int)handleValue.Handle]);
                }
                break;
            case XtBlock.ArrayPointerValue arrayPointerValue:
                if (arrayPointerValue.Block == ushort.MaxValue || arrayPointerValue.Offset == ushort.MaxValue || arrayPointerValue.Length == 0)
                {
                    value.Value = new XtArrayValue(arrayPointerValue.Type);
                }
                else
                {
                    var block = blocks[arrayPointerValue.Block];
                    if(arrays.TryGetValue((arrayPointerValue.Block, arrayPointerValue.Offset), out var arrValue))
                    {
                        value.Value = new XtArrayValue(arrayPointerValue.Type, arrValue);
                    }
                    else
                    {
                        var val = new XtArray(arrayPointerValue.Type);
                        arrays[(arrayPointerValue.Block, arrayPointerValue.Offset)] = val;
                        for (int i = 0; i < arrayPointerValue.Length; i++)
                        {
                            val.Add(block.Values[arrayPointerValue.Offset + i]);
                        }
                        value.Value = new XtArrayValue(arrayPointerValue.Type, val);
                        for (int i = 0; i < val.Values.Count; i++)
                        {
                            Reference(val.Values[i], refs, blocks, arrays, text);
                        }
                    }
                }
                break;
            case XtBlock.StringIndexValue stringIndexValue:
                value.Value = stringIndexValue.Type.CreateValue(text.CStr(stringIndexValue.Index));
                break;
        }
    }
    public static IXtValue Parse(IXtType type, BinaryReader reader)
    {
        return type switch
        {
            XtStructType structType => ParseS(structType, reader),
            XtAtomType atomType => ParseA(atomType, reader),
            XtEnumType enumType => enumType.CreateValue(reader.ReadUInt32()),
            XtPointerType pointerType => new XtBlock.PointerValue(pointerType, reader.ReadUInt16(), reader.ReadUInt16()),
            XtHandleType handleType => new XtBlock.HandleValue(handleType, reader.ReadUInt32()),
            XtArrayType arrType => new XtBlock.ArrayPointerValue(arrType, reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt32()),
            _ => throw new NotSupportedException(),
        };
    }
    public static IXtValue ParseS(XtStructType type, BinaryReader reader)
    {
        List<IXtValue> xtValues = new List<IXtValue>();
        foreach (var field in type.FullFields())
        {
            xtValues.Add(Parse(field.TargetType, reader));
        }
        return type.CreateValue(xtValues);
    }
    public static IXtValue ParseA(XtAtomType type, BinaryReader reader)
    {
        return type.AtomType switch
        {
            AtomType.Bool => type.CreateValue(reader.ReadUInt32() > 0),
            AtomType.Int8 => type.CreateValue((sbyte)reader.ReadInt32()),
            AtomType.Int16 => type.CreateValue((short)reader.ReadInt32()),
            AtomType.Int32 => type.CreateValue(reader.ReadInt32()),
            AtomType.Int64 => type.CreateValue(reader.ReadInt64()),
            AtomType.Unsigned8 => type.CreateValue((byte)reader.ReadUInt32()),
            AtomType.Unsigned16 => type.CreateValue((ushort)reader.ReadUInt32()),
            AtomType.Unsigned32 => type.CreateValue(reader.ReadUInt32()),
            AtomType.Unsigned64 => type.CreateValue(reader.ReadUInt64()),
            AtomType.Float32 => type.CreateValue(reader.ReadSingle()),
            AtomType.Float64 => type.CreateValue(reader.ReadDouble()),
            AtomType.StringSz8 => new XtBlock.StringIndexValue(type, reader.ReadUInt32()),
            AtomType.StringSz16 => new XtBlock.StringIndexValue(type, (uint)reader.ReadUInt64()),
            AtomType.LocId => type.CreateValue((LocId)reader.ReadUInt32()),
            _ => throw new UnreachableException()
        };
    }

    public static void Export(XtDatabase flask, string path)
    {
        using (Stream fileStream = File.Open(path, FileMode.Create))
        Export(flask, fileStream);
    }
    public static void Export(XtDatabase db, Stream destination)
    {
        FlaskEntity entity = new();

        List<IXtRef> refs = db.Refs.OrderBy(r => r.Id).ToList();

        List<FlaskRefEntity> flaskRefs = [];
        List<FlaskRecordEntity> flaskRecords = [];
        List<FlaskComponentEntity> flaskComponents = [];

        List<List<XtBlock>> blocks = [];
        List<Dictionary<IXtValue, (XtBlock block, ushort offset)>> refPointers = [];
        foreach(var item in refs)
        {
            List<XtBlock> components = [];
            blocks.Add(components);
            Dictionary<IXtValue, (XtBlock block, ushort offset)> flattenedPointers = [];
            refPointers.Add(flattenedPointers);

            if (item is not XtRef xtref) continue;

            components.Add(new XtBlock(xtref.Type));
            components[0].Add(xtref.Value);
            FlattenValue_NonPointerAarray(xtref.Value, refs, db.Types, components, flattenedPointers, []);
            FlattenValue(xtref.Value, refs, db.Types, components, flattenedPointers);
        }
        var textEncoding = new FlaskEncoding();
        using var memoryStream = new MemoryStream();
        using var writer = new BinaryWriter(memoryStream);
        for (int i = 0; i < refs.Count; i++)
        {
            List<XtBlock> components = blocks[i];
            if(components.Count == 0)
            {
                flaskRefs.Add(new FlaskRefEntity
                {
                    Id = refs[i].Id,
                    Record = ushort.MaxValue,
                    Type = (ushort)db.Types.IndexOf(refs[i].Type is IXtCurryType c ? c.BaseType : refs[i].Type)
                });
                continue;
            }
            long startPos = memoryStream.Position;
            StringTableBuilder stringsBuilder = new();
            //var firstComponent = components[0];
            //components = components.Skip(1).OrderBy(b => db.Types.IndexOf(b.BlockType is IXtCurryType c ? c.BaseType : b.BlockType)).ToList();
            //components.Insert(0, firstComponent);
            foreach (var component in components)
            {
                foreach (var value in component.Values)
                {
                    WriteValue(value, writer, refs, refPointers[i], components, db.Types, stringsBuilder);
                }
                flaskComponents.Add(new FlaskComponentEntity
                {
                    Behavior = component.BlockType switch
                    {
                        XtPointerType => ComponentBehavior.Pointer,
                        XtHandleType => ComponentBehavior.Handle,
                        _ => ComponentBehavior.Object
                    },
                    Count = (ushort)component.Values.Count,
                    Type = (ushort)db.Types.IndexOf(component.BlockType switch
                    {
                        XtPointerType t => t.BaseType,
                        XtHandleType t => t.BaseType,
                        var c => c
                    })
                });
            }
            long writeCount = memoryStream.Position - startPos;
            var text = stringsBuilder.ToString();
            if (text.Length % 4 != 0) 
            {
                text += new string('\0', 4 - text.Length % 4);
            }
            var bytes = textEncoding.GetBytes(text);
            writer.Write(bytes);
            flaskRefs.Add(new FlaskRefEntity
            {
                Id = refs[i].Id,
                Record = (ushort)flaskRecords.Count,
                Type = (ushort)db.Types.IndexOf(refs[i].Type)
            });
            flaskRecords.Add(new FlaskRecordEntity
            {
                ComponentCount = (uint)components.Count,
                DataBytes = (uint)writeCount,
                Ref = (uint)i,
                StringBytes = (uint)bytes.Length
            });
        }

        List<FlaskTypeEntity> flaskTypes = [];
        List<FlaskBaseEntity> flaskBases = [];
        List<FlaskFieldEntity> flaskFields = [];
        StringTableBuilder typeStringsBuilder = new();
        foreach (var item in db.Types)
        {
            switch (item)
            {
                case XtStructType structType:
                    flaskTypes.Add(new FlaskTypeEntity()
                    {
                        Atom = AtomType.NotAtom,
                        BaseCount = (ushort)structType.Bases.Count,
                        Behavior = TypeBehavior.Struct,
                        FieldCount = (ushort)structType.Fields.Count,
                        Size = (ushort)structType.Size,
                        Name = (ushort)typeStringsBuilder.GetIndex(structType.Name)
                    });
                    int offset = 0;
                    foreach(var baseType in structType.Bases)
                    {
                        flaskBases.Add(new FlaskBaseEntity
                        {
                            Offset = (ushort)offset,
                            Type = (ushort)db.Types.IndexOf(baseType)
                        });
                        offset += baseType.Size;
                    }
                    foreach(var field in structType.Fields)
                    {
                        var fieldType = field.TargetType;
                        bool isArray = false;
                        if(fieldType is XtArrayType arrType)
                        {
                            isArray = true;
                            fieldType = arrType.BaseType;
                        }
                        FieldBehavior behavior;
                        if(fieldType is XtPointerType pointerType)
                        {
                            behavior = FieldBehavior.Pointer;
                            fieldType = pointerType.BaseType;
                        } else if(fieldType is XtHandleType handleType)
                        {
                            behavior = FieldBehavior.Handle;
                            fieldType = handleType.BaseType;
                        }
                        else
                        {
                            behavior = fieldType switch
                            {
                                XtStructType => FieldBehavior.Struct,
                                XtAtomType => FieldBehavior.Atom,
                                XtEnumType enumType => enumType.IsFlags ? FieldBehavior.Flags : FieldBehavior.Enum,
                                _ => throw new NotImplementedException()
                            };
                        }
                        flaskFields.Add(new FlaskFieldEntity
                        {
                            Behavior = behavior,
                            IsArray = isArray,
                            Name = (ushort)typeStringsBuilder.GetIndex(field.Name),
                            Offset = (ushort)offset,
                            Type = (ushort)db.Types.IndexOf(fieldType),
                        });
                        offset += field.Size;
                    }
                    break;
                case XtEnumType enumType:
                    flaskTypes.Add(new FlaskTypeEntity()
                    {
                        Atom = AtomType.NotAtom,
                        BaseCount = 0,
                        Behavior = enumType.IsFlags ? TypeBehavior.Flags : TypeBehavior.Enum,
                        FieldCount = (ushort)enumType.Labels.Count,
                        Size = (ushort)enumType.Size,
                        Name = (ushort)typeStringsBuilder.GetIndex(enumType.Name)
                    });
                    foreach (var label in enumType.Labels)
                    {
                        flaskFields.Add(new FlaskFieldEntity
                        {
                            Behavior = FieldBehavior.Label,
                            IsArray = false,
                            Name = (ushort)typeStringsBuilder.GetIndex(label),
                            Offset = 0,
                            Type = ushort.MaxValue,
                        });
                    }
                    break;
                case XtAtomType atomType:
                    flaskTypes.Add(new FlaskTypeEntity()
                    {
                        Atom = atomType.AtomType,
                        BaseCount = 0,
                        Behavior = TypeBehavior.Atom,
                        FieldCount = 0,
                        Name = (ushort)typeStringsBuilder.GetIndex(atomType.Name),
                        Size = (ushort)atomType.Size,
                    });
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        int fileOffset = 72;
        entity.TypesBlob = new (){ Count = flaskTypes.Count, Offset = fileOffset };
        fileOffset += flaskTypes.Count * 12;//sizeof(FlaskTypeEntity);

        entity.BasesBlob = new() { Count = flaskBases.Count, Offset = fileOffset };
        fileOffset += flaskBases.Count * 4;//sizeof(FlaskBaseEntity);

        entity.FieldsBlob = new() { Count = flaskFields.Count, Offset = fileOffset };
        fileOffset += flaskFields.Count * 8;//sizeof(FlaskFieldEntity);

        entity.Strings = typeStringsBuilder.ToString();
        if(entity.Strings.Length % 4 != 0)
        {
            entity.Strings += new string('\0', 4 - (entity.Strings.Length % 4));
        }
        entity.StringsBlob = new() { Count = entity.Strings.Length, Offset = fileOffset };
        fileOffset += entity.Strings.Length;


        entity.RefsBlob = new() { Count = flaskRefs.Count, Offset = fileOffset };
        fileOffset += flaskRefs.Count * 8;//sizeof(FlaskRefEntity);

        entity.RecordsBlob = new() { Count = flaskRecords.Count, Offset = fileOffset };
        fileOffset += flaskRecords.Count * 16;//sizeof(FlaskRecordEntity);

        entity.ComponentsBlob = new() { Count = flaskComponents.Count, Offset = fileOffset };
        fileOffset += flaskComponents.Count * 6;//sizeof(FlaskComponentEntity);
        if (fileOffset % 4 != 0)
        {
            fileOffset += 4 - (fileOffset % 4);
        }
        entity.Types = flaskTypes.ToArray();
        entity.Bases = flaskBases.ToArray();
        entity.Fields = flaskFields.ToArray();

        entity.Refs = flaskRefs.ToArray();
        entity.Records = flaskRecords.ToArray();
        entity.Components = flaskComponents.ToArray();

        memoryStream.Seek(0, SeekOrigin.Begin);
        entity.Data = memoryStream.ToArray();
        entity.DataBlob = new() { Count = entity.Data.Length, Offset = fileOffset };
        FlaskSerializer.Write(destination, entity);
    }
    static void FlattenValue_NonPointerAarray(IXtValue value, List<IXtRef> refs, List<IXtType> exportedTypes, List<XtBlock> blocks, Dictionary<IXtValue, (XtBlock block, ushort offset)> flattenedPointers, HashSet<IXtValue> searched)
    {
        if(value is IXtValueContainer container)
        {
            foreach (var item in container)
            {
                FlattenItem_NonPointerAarray(item, refs, exportedTypes, blocks, flattenedPointers, searched);
            }
        } else if(value is XtArrayValue arrayValue && arrayValue.Array is not null)
        {
            foreach (var item in arrayValue.Array)
            {
                FlattenItem_NonPointerAarray(item, refs, exportedTypes, blocks, flattenedPointers, searched);
            }
        }
    }
    static void FlattenItem_NonPointerAarray(IXtValueItem item, List<IXtRef> refs, List<IXtType> exportedTypes, List<XtBlock> blocks, Dictionary<IXtValue, (XtBlock block, ushort offset)> flattenedPointers, HashSet<IXtValue> searched)
    {
        switch(item.Value)
        {
            case XtArrayValue arrayValue:
                if (arrayValue.Array is not null && arrayValue.Array.Count > 0)
                {
                    if (!searched.Add(arrayValue.Array))
                    {
                        break;
                    }
                    var block = GetOrAddBlock(blocks, arrayValue.Type.BaseType);
                    switch (arrayValue.Type.BaseType)
                    {
                        case XtHandleType baseHandleType: break;
                        case XtPointerType basePointerType:
                            for (int i = 0; i < arrayValue.Array.Count; i++)
                            {
                                if (arrayValue.Array.Values[i].Value is not XtPointerValue pvalue) throw new UnreachableException();
                                if (pvalue.Value is null) continue;
                                if (!searched.Add(pvalue.Value)) continue;
                                FlattenValue_NonPointerAarray(pvalue.Value, refs, exportedTypes, blocks, flattenedPointers, searched);
                            }
                            break;
                        case var c:
                            Debug.Assert(c is XtStructType or XtEnumType or XtAtomType);
                            for (int i = 0; i < arrayValue.Array.Count; i++)
                            {
                                searched.Add(arrayValue.Array.Values[i].Value);
                                flattenedPointers.Add(arrayValue.Array.Values[i].Value, (block, (ushort)block.Values.Count));
                                block.Add(arrayValue.Array.Values[i].Value);
                            }
                            for (int i = 0; i < arrayValue.Array.Count; i++)
                            {
                                FlattenValue_NonPointerAarray(arrayValue.Array.Values[i].Value, refs, exportedTypes, blocks, flattenedPointers, searched);
                            }
                            break;
                    }
                    break;
                }
                break;
            case XtPointerValue pointerValue:
                if (pointerValue.Value is null) break;
                if (!searched.Add(pointerValue.Value))
                {
                    break;
                }
                FlattenValue_NonPointerAarray(pointerValue.Value, refs, exportedTypes, blocks, flattenedPointers, searched);
                break;
            case var c:
                FlattenValue_NonPointerAarray(c, refs, exportedTypes, blocks, flattenedPointers, searched);
                break;
        }
        return;
    }
    static void FlattenValue(IXtValue value, List<IXtRef> refs, List<IXtType> exportedTypes, List<XtBlock> blocks, Dictionary<IXtValue, (XtBlock block, ushort offset)> flattenedPointers)
    {
        if (value is IXtValueContainer container)
        {
            foreach (var item in container)
            {
                FlattenItem(item, refs, exportedTypes, blocks, flattenedPointers);
            }
        }
        else if (value is XtArrayValue arrayValue && arrayValue.Array is not null)
        {
            foreach (var item in arrayValue.Array)
            {
                FlattenItem(item, refs, exportedTypes, blocks, flattenedPointers);
            }
        }
    }
    static void FlattenItem(IXtValueItem item, List<IXtRef> refs, List<IXtType> exportedTypes, List<XtBlock> blocks, Dictionary<IXtValue, (XtBlock block, ushort offset)> flattenedPointers)
    {
        switch(item.Value)
        {
            case XtArrayValue arrayValue:
                if (arrayValue.Array is null || arrayValue.Array.Count <= 0)
                    break;
                if (flattenedPointers.ContainsKey(arrayValue.Array)) 
                    break;

                var block = GetOrAddBlock(blocks, arrayValue.Type.BaseType);
                flattenedPointers.Add(arrayValue.Array, (block, (ushort)block.Values.Count));
                switch (arrayValue.Type.BaseType)
                {
                    case XtPointerType basePointerType:
                        for (int i = 0; i < arrayValue.Array.Count; i++)
                        {
                            if (arrayValue.Array.Values[i].Value is not XtPointerValue pvalue) throw new UnreachableException();
                            block.Add(pvalue);

                            if (pvalue.Value is null || flattenedPointers.ContainsKey(pvalue.Value)) continue;

                            var targetBlock = GetOrAddBlock(blocks, pvalue.Value.Type);

                            flattenedPointers.Add(pvalue.Value, (targetBlock, (ushort)targetBlock.Values.Count));
                            Debug.Assert(!targetBlock.Values.Contains(pvalue.Value));
                            targetBlock.Add(pvalue.Value);
                        }
                        for (int i = 0; i < arrayValue.Array.Count; i++)
                        {
                            if (arrayValue.Array.Values[i].Value is not XtPointerValue pvalue) throw new UnreachableException();
                            if (pvalue.Value is null) continue;

                            FlattenValue(pvalue.Value, refs, exportedTypes, blocks, flattenedPointers);
                        }
                        break;
                    case XtHandleType baseHandleType:
                        for (int i = 0; i < arrayValue.Array.Count; i++)
                        {
                            if (arrayValue.Array.Values[i].Value is not XtHandleValue hvalue) throw new UnreachableException();
                            block.Add(hvalue);
                        }
                        break;
                    case var c:
                        //for (int i = 0; i < arrayValue.Array.Count; i++)
                        //{
                        //    flattenedPointers.Add(arrayValue.Array.Values[i].Value, (block, (ushort)block.Values.Count));
                        //    block.Add(arrayValue.Array.Values[i].Value);
                        //}
                        for (int i = 0; i < arrayValue.Array.Count; i++)
                        {
                            FlattenValue(arrayValue.Array.Values[i].Value, refs, exportedTypes, blocks, flattenedPointers);
                        }
                        break;
                }
                break;
            case XtPointerValue pointerValue:
                if (pointerValue.Value is null) break;
                if (flattenedPointers.ContainsKey(pointerValue.Value)) break;

                var valueBlock = GetOrAddBlock(blocks, pointerValue.Value.Type);

                flattenedPointers.Add(pointerValue.Value, (valueBlock, (ushort)valueBlock.Values.Count));
                valueBlock.Add(pointerValue.Value);
                FlattenValue(pointerValue.Value, refs, exportedTypes, blocks, flattenedPointers);
                break;
            case var c:
                FlattenValue(c, refs, exportedTypes, blocks, flattenedPointers);
                break;
        }
        return;
    }

    private static XtBlock GetOrAddBlock(List<XtBlock> blocks, IXtType type)
    {
        var block = blocks.FirstOrDefault(b => b.BlockType == type);
        if (block is null)
        {
            block = new XtBlock(type);
            blocks.Add(block);
        }

        return block;
    }

    private static void WriteValue(IXtValue value, BinaryWriter writer, IList<IXtRef> refs, Dictionary<IXtValue, (XtBlock block, ushort offset)> pointers, IList<XtBlock> blocks, List<IXtType> exportedTypes, StringTableBuilder stringBuilder)
    {
        switch (value)
        {
            case XtAtomValue<bool> v: writer.Write(v.Value ? 1 : 0); break;
            case XtAtomValue<sbyte> v: writer.Write((int)v.Value); break;
            case XtAtomValue<short> v: writer.Write((int)v.Value); break;
            case XtAtomValue<int> v: writer.Write(v.Value); break;
            case XtAtomValue<long> v: writer.Write(v.Value); break;
            case XtAtomValue<byte> v: writer.Write((uint)v.Value); break;
            case XtAtomValue<ushort> v: writer.Write((uint)v.Value); break;
            case XtAtomValue<uint> v: writer.Write(v.Value); break;
            case XtAtomValue<ulong> v: writer.Write(v.Value); break;
            case XtAtomValue<float> v: writer.Write(v.Value); break;
            case XtAtomValue<double> v: writer.Write(v.Value); break;
            case XtAtomValue<string> v: writer.Write((uint)stringBuilder.GetIndex(v.Value)); break;
            case XtAtomValue<LocId> v: writer.Write(v.Value); break;
            case XtEnumValue v: writer.Write(v.Value); break;
            case XtStructValue v:
                foreach (var item in v)
                {
                    WriteValue(item.Value, writer, refs, pointers, blocks, exportedTypes, stringBuilder); 
                }
                break;
            case XtPointerValue v: WritePointer(v, pointers, blocks, writer); break;
            case XtArrayValue v: WriteArray(v, pointers, blocks, writer); break;
            case XtHandleValue v: WriteHandle(v, refs, writer); break;
            default: throw new UnreachableException();
        };
    }

    static void WriteArray(XtArrayValue v, Dictionary<IXtValue, (XtBlock block, ushort offset)> pointers, IList<XtBlock> blocks, BinaryWriter writer)
    {
        if (v.Array is null || v.Array.Count == 0)
        {
            writer.Write(ushort.MaxValue);
            writer.Write(ushort.MaxValue);
            writer.Write(0);
        }
        else
        {
            var (block, offset) = pointers[v.Array];
            writer.Write((ushort)blocks.IndexOf(block));
            writer.Write(offset);
            writer.Write((uint)v.Array.Count);
        }
    }
    static void WriteHandle(XtHandleValue v, IList<IXtRef> refs, BinaryWriter writer)
    {
        writer.Write(v.XtRef is null ? uint.MaxValue : (uint)refs.IndexOf(v.XtRef));
    }
    static void WritePointer(XtPointerValue v, Dictionary<IXtValue, (XtBlock block, ushort offset)> pointers, IList<XtBlock> blocks, BinaryWriter writer)
    {
        if (v.Value is not null)
        {
            var (block, offset) = pointers[v.Value];
            writer.Write((ushort)blocks.IndexOf(block));
            writer.Write(offset);
        }
        else
        {
            writer.Write(ushort.MaxValue);
            writer.Write(ushort.MaxValue);
        }
    }
}

public struct LocId
{
    uint id;
    public static implicit operator uint(LocId x) => x.id;
    public static implicit operator LocId(uint id) => new LocId { id = id };
}
public class XtDatabase
{
    public List<IXtType> Types { get; } = new();
    public List<IXtRef> Refs { get; } = new();
}
public interface IXtRef
{
    public uint Id { get; set; }
    public IXtType Type { get; }
}
public class XtExternalRef : IXtRef
{
    public uint Id { get; set; }
    public IXtType Type { get; }
    public XtExternalRef(uint id, IXtType type)
    {
        Id = id;
        Type = type;
    }
}
public class XtRef : IXtRef, IXtValueItem
{
    public uint Id { get; set; }
    object IXtValueItem.Key => Id;
    public IXtType Type => Value.Type;
    public IXtValue Value { get; set; }
    public List<IXtValue> RefHeap { get; } = new();
    public XtRef(uint id, IXtValue value)
    {
        Value = value;
        Id = id;
    }

    public override string ToString() => $"[{Id}]({Type}) {Value}";
}
class XtBlock
{
    public IXtType BlockType { get; }
    public List<IXtValue> Values { get; } = new();

    public XtBlock(IXtType blockType)
    {
        BlockType = blockType;
    }

    public void Add(IXtValue value)
    {
        Debug.Assert(value.Type == BlockType, "Value does not fit in block.");
        Values.Add(value);
    }

    public override string ToString() => $"{{{Values.Count}}} {BlockType}";

    public class PointerValue : IXtValue
    {
        public XtPointerType Type { get; }
        IXtType IXtValue.Type => Type;
        public ushort Block { get; }
        public ushort Offset { get; }
        public PointerValue(XtPointerType type, ushort block, ushort offset)
        {
            Type = type;
            Block = block;
            Offset = offset;
        }
    }
    public class ArrayPointerValue : IXtValue
    {
        public XtArrayType Type { get; }
        IXtType IXtValue.Type => Type;
        public ushort Block { get; }
        public ushort Offset { get; }
        public uint Length { get; }
        public ArrayPointerValue(XtArrayType type, ushort block, ushort offset, uint length)
        {
            Type = type;
            Block = block;
            Offset = offset;
            Length = length;
        }
    }
    public class HandleValue : IXtValue
    {
        public XtHandleType Type { get; }
        IXtType IXtValue.Type => Type;
        public uint Handle { get; }

        public HandleValue(XtHandleType type, uint handle)
        {
            Type = type;
            Handle = handle;
        }
    }
    public class StringIndexValue : IXtValue
    {
        public XtAtomType Type { get; }
        IXtType IXtValue.Type => Type;
        public uint Index { get; }

        public StringIndexValue(XtAtomType stringType, uint index)
        {
            Type = stringType;
            Index = index;
        }
    }
}
public interface IXtType
{
    public string Name { get; }
    public int Size { get; }
    public IXtValue CreateValue();
}
public interface IXtCurryType : IXtType
{
    public IXtType BaseType { get; }
}
public interface IXtValue
{
    public IXtType Type { get; }
}
public interface IXtValueItem
{
    public IXtType Type { get; }
    public object Key { get; }
    public IXtValue Value { get; set; }
}
public interface IXtValueContainer : IXtValue, IEnumerable<IXtValueItem>
{

}
public class XtPointerType : IXtCurryType
{
    static Dictionary<IXtType, XtPointerType> pointerTypes = [];
    public string Name => BaseType.Name;
    public int Size => 4;
    public IXtType BaseType { get; }
    XtPointerType(IXtType baseType)
    {
        BaseType = baseType;
    }
    static public XtPointerType Get(IXtType baseType)
    {
        if(!pointerTypes.TryGetValue(baseType, out var result))
        {
            result = new XtPointerType(baseType);
            pointerTypes[baseType] = result;
        }
        return result;
    }
    public IXtValue CreateValue() => new XtPointerValue(this);
    public XtPointerValue CreateValue(IXtValue value) => new XtPointerValue(this, value);
    public override string ToString() => $"{BaseType}*";
}
public class XtPointerValue : IXtValue
{
    public IXtValue? Value { get; set; }
    public XtPointerType Type { get; }
    IXtType IXtValue.Type => Type;
    public XtPointerValue(XtPointerType handleType, IXtValue value)
    {
        Type = handleType;
        Value = value;
    }
    public XtPointerValue(XtPointerType handleType)
    {
        Type = handleType;
    }
    public override string ToString() => $"({Type}){Value}";
}
public class XtHandleType : IXtCurryType
{
    static Dictionary<IXtType, XtHandleType> handleTypes = [];
    public string Name => BaseType.Name;
    public int Size => 4;
    public IXtType BaseType { get; }
    XtHandleType(IXtType baseType)
    {
        BaseType = baseType;
    }
    public static XtHandleType Get(IXtType baseType)
    {
        if (!handleTypes.TryGetValue(baseType, out var result))
        {
            result = new XtHandleType(baseType);
            handleTypes[baseType] = result;
        }
        return result;
    }
    public IXtValue CreateValue() => new XtHandleValue(this);
    public XtHandleValue CreateValue(IXtRef reference) => new XtHandleValue(this, reference);
    public override string ToString() => $"{BaseType}^";
}
public class XtHandleValue : IXtValue
{
    public IXtRef? XtRef { get; set; }

    public XtHandleType Type { get; }
    IXtType IXtValue.Type => Type;

    public XtHandleValue(XtHandleType handleType, IXtRef xtRef)
    {
        Type = handleType;
        XtRef = xtRef;
    }
    public XtHandleValue(XtHandleType handleType)
    {
        Type = handleType;
    }
    public override string ToString() => $"[{XtRef?.Id ?? uint.MaxValue}]({Type}) {XtRef}";
}
public class XtArrayType : IXtCurryType
{
    static Dictionary<IXtType, XtArrayType> arrayTypes = [];
    public string Name => BaseType.Name;
    public int Size => 8;
    public IXtType BaseType { get; }
    XtArrayType(IXtType baseType)
    {
        BaseType = baseType;
    }
    static public XtArrayType Get(IXtType baseType)
    {
        if (!arrayTypes.TryGetValue(baseType, out var result))
        {
            result = new XtArrayType(baseType);
            arrayTypes[baseType] = result;
        }
        return result;
    }
    public IXtValue CreateValue() => new XtArrayValue(this);
    public override string ToString() => $"{BaseType}[]";
}
public class XtArrayValue : IXtValue
{
    public XtArray? Array { get; set; }
    IXtType IXtValue.Type => Type;
    public XtArrayType Type { get; }

    public XtArrayValue(XtArrayType type)
    {
        Type = type;
    }
    public XtArrayValue(XtArrayType type, XtArray array)
    {
        Type = type;
        Array = array;
    }

    public override string ToString() => $"{Type} = [{Array?.Values.Count.ToString() ?? "null"}]";
}
public class XtArray : IXtValueContainer
{
    public XtArrayType Type { get; }
    IXtType IXtValue.Type => Type;
    public List<XtArrayItem> Values { get; } = new();
    public int Count => Values.Count;
    public XtArray(XtArrayType type)
    {
        Type = type;
    }
    public void Add(IXtValue value) => Values.Add(new XtArrayItem(this, value));
    public override string ToString() => $"{Type} = [{Values.Count}]";
    public IEnumerator<IXtValueItem> GetEnumerator() => Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
public class XtArrayItem : IXtValueItem
{
    public IXtType Type => Container.Type;
    public int Index => Container.Values.IndexOf(this);
    object IXtValueItem.Key => Index;
    public IXtValue Value { get; set; }
    public XtArray Container { get; }
    public XtArrayItem(XtArray container, IXtValue value)
    {
        Container = container;
        Value = value;
    }
    public override string ToString() => $"[{Index}] = ({Type}){Value}";
}
public class XtStructType : IXtType
{
    public string Name { get; }
    public List<XtStructType> Bases { get; } = new();
    public List<XtStructField> Fields { get; } = new();
    public int Size
    {
        get
        {
            var fields = FullFields().ToArray();
            if (fields.Length == 0) return externalSize;
            return fields.Sum(f => f.Size);
        }
    }
    ushort externalSize;
    public XtStructType(string name, ushort size)
    {
        Name = name;
        externalSize = size;
    }
    public bool IsOfType(XtStructType type) => type == this ? true : Bases.Any(b => b.IsOfType(type));
    public IEnumerable<XtStructField> FullFields() => Bases.SelectMany(b => b.FullFields()).Concat(Fields);

    public IXtValue CreateValue()
    {
        var fields = FullFields().ToArray();
        var value = new XtStructValue(this, fields, fields.Select(f => f.InitValue()));
        return value;
    }
    public XtStructValue CreateValue(List<IXtValue> initializerList)
    {
        Debug.Assert(initializerList.Select(v => v.Type).SequenceEqual(FullFields().Select(f => f.TargetType)));
        var value = new XtStructValue(this, FullFields(), initializerList);
        return value;
    }
    public override string ToString() => Name;
}
public class XtStructField
{
    public string Name { get; }
    public IXtType ParentType { get; }
    public IXtType TargetType { get; }
    public int Size => TargetType.Size;
    public XtStructField(string name, IXtType parentType, IXtType targetType)
    {
        Name = name;
        ParentType = parentType;
        TargetType = targetType;
    }
    public IXtValue InitValue() => TargetType.CreateValue();
    public override string ToString() => $"{TargetType} {Name}";
}
public class XtStructValue : IXtValueContainer
{
    public XtStructType Type { get; }
    IXtType IXtValue.Type => Type;

    public List<XtFieldValueItem> Values { get; } = new();
    public XtStructValue(XtStructType type, IEnumerable<XtStructField> fields, IEnumerable<IXtValue> values)
    {
        Type = type;
        Values.AddRange(fields.Zip(values, (f, v) => new XtFieldValueItem(f, v)));
    }
    public override string ToString() => $"{{{Values.Count}}}";

    public IEnumerator<IXtValueItem> GetEnumerator() => Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
public class XtFieldValueItem : IXtValueItem
{
    public XtFieldValueItem(XtStructField field, IXtValue value)
    {
        Field = field;
        Value = value;
    }

    public IXtType Type => Field.TargetType;
    object IXtValueItem.Key => Field;
    public XtStructField Field { get; }
    public IXtValue Value { get; set; }
    public override string ToString() => $"{Field} = ({Type}){Value}";
}
public class XtEnumType : IXtType
{
    public string Name { get; set; }
    public int Size => 4;
    public bool IsFlags { get; set; }
    public List<string> Labels { get; } = new();
    public XtEnumType(string name)
    {
        Name = name;
    }
    public IXtValue CreateValue()
    {
        return new XtEnumValue(this);
    }
    public XtEnumValue CreateValue(uint value)
    {
        return new XtEnumValue(this) { Value = value };
    }

    public override string ToString() => Name;
}
public class XtEnumValue : IXtValue
{
    public XtEnumType Type { get; }
    IXtType IXtValue.Type => Type;
    public uint Value { get; set; } = 0;

    public XtEnumValue(XtEnumType type)
    {
        Type = type;
    }
    public void SetBit(int index, bool value)
    {
        uint mask = (uint)1 << index;
        if (value)
        {
            Value |= mask;
        }
        else
        {
            Value &= ~mask;
        }
    }
    public void ToggleBit(int index, bool value)
    {
        uint mask = (uint)1 << index;
        Value ^= mask;
    }
    public bool GetBit(int index)
    {
        uint mask = (uint)1 << index;
        return (mask & Value) > 0;
    }
    public override string ToString()
    {
        if (Type.IsFlags)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < Type.Labels.Count; i++)
            {
                if (GetBit(i))
                {
                    if (builder.Length != 0) builder.Append(" | ");
                    builder.Append(Type.Labels[i]);
                }
            }
            return builder.ToString();
        }
        else
        {
            return Type.Labels[(int)Value];
        }
    }
}
public class XtAtomType : IXtType
{
    public string Name { get; set; }
    public AtomType AtomType { get; }
    public int Size => AtomType switch
    {
        AtomType.Bool => 4,
        AtomType.Int8 => 4,
        AtomType.Int16 => 4,
        AtomType.Int32 => 4,
        AtomType.Int64 => 8,
        AtomType.Unsigned8 => 4,
        AtomType.Unsigned16 => 4,
        AtomType.Unsigned32 => 4,
        AtomType.Unsigned64 => 8,
        AtomType.Float32 => 4,
        AtomType.Float64 => 8,
        AtomType.StringSz8 => 4,
        AtomType.StringSz16 => 4,
        AtomType.LocId => 4,
        _ => throw new UnreachableException()
    };
    public XtAtomType(string name, AtomType atomType)
    {
        Name = name;
        AtomType = atomType;
    }
    public IXtValue CreateValue()
    {
        return AtomType switch
        {
            AtomType.Bool => new XtAtomValue<bool>(this, false),
            AtomType.Int8 => new XtAtomValue<sbyte>(this, 0),
            AtomType.Int16 => new XtAtomValue<short>(this, 0),
            AtomType.Int32 => new XtAtomValue<int>(this, 0),
            AtomType.Int64 => new XtAtomValue<long>(this, 0),
            AtomType.Unsigned8 => new XtAtomValue<byte>(this, 0),
            AtomType.Unsigned16 => new XtAtomValue<ushort>(this, 0),
            AtomType.Unsigned32 => new XtAtomValue<uint>(this, 0),
            AtomType.Unsigned64 => new XtAtomValue<ulong>(this, 0),
            AtomType.Float32 => new XtAtomValue<float>(this, 0),
            AtomType.Float64 => new XtAtomValue<double>(this, 0),
            AtomType.StringSz8 => new XtAtomValue<string>(this, ""),
            AtomType.StringSz16 => new XtAtomValue<string>(this, ""),
            AtomType.LocId => new XtAtomValue<LocId>(this, 0),
            _ => throw new NotSupportedException()
        };
    }
    public IXtValue CreateValue(object initial)
    {
        return AtomType switch
        {
            AtomType.Bool => CreateValue<bool>(initial),
            AtomType.Int8 => CreateValue<sbyte>(initial),
            AtomType.Int16 => CreateValue<short>(initial),
            AtomType.Int32 => CreateValue<int>(initial),
            AtomType.Int64 => CreateValue<long>(initial),
            AtomType.Unsigned8 => CreateValue<byte>(initial),
            AtomType.Unsigned16 => CreateValue<ushort>(initial),
            AtomType.Unsigned32 => CreateValue<uint>(initial),
            AtomType.Unsigned64 => CreateValue<ulong>(initial),
            AtomType.Float32 => CreateValue<float>(initial),
            AtomType.Float64 => CreateValue<double>(initial),
            AtomType.StringSz8 => CreateValue<string>(initial),
            AtomType.StringSz16 => CreateValue<string>(initial),
            AtomType.LocId => CreateValue<LocId>(initial),
            _ => throw new NotSupportedException()
        };
    }
    public XtAtomValue<T> CreateValue<T>(object initial) where T :notnull
    {
        Debug.Assert(initial is T);
        return new XtAtomValue<T>(this, (T)initial);
    }

    public override string ToString() => Name;
}
public class XtAtomValue<T> : IXtValue where T : notnull
{
    public XtAtomType Type { get; }
    IXtType IXtValue.Type => Type;
    public T Value { get; set; }

    public XtAtomValue(XtAtomType type, T value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString() => Value.ToString() ?? "";
}
public static class StringExtensions
{
    public static string CStr(this string stringList, int start)
    {
        int end = start;
        while(end < stringList.Length && stringList[end] != '\0')
        {
            end++;
        }
        return stringList[start..end];
    }
    public static string CStr(this string stringList, uint start)
    {
        uint end = start;
        while(end < stringList.Length && stringList[(int)end] != '\0')
        {
            end++;
        }
        return stringList[(int)start..(int)end];
    }
}
public class StringTableBuilder
{
    StringBuilder builder = new();
    Dictionary<string, int> locationMappings = new();
    public int GetIndex(string text)
    {
        if (locationMappings.TryGetValue(text, out var index)) return index;
        int i = builder.Length;
        builder.Append(text);
        builder.Append('\0');
        locationMappings[text] = i;
        return i;
    }
    public override string ToString() => builder.ToString();
}