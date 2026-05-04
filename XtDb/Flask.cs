using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlurFileFormats.FlaskReflection.Entities;
using BlurFileFormats.Utils;

namespace BlurFileFormats.XtDb;

//public static class Flask
//{
//    public static XtDatabase Import(Stream source)
//    {
//        var flaskEntity = (FlaskEntity)FlaskSerializer.Read(source);
//        if (source.Length != source.Position) throw new Exception("Not all read");
//
//        List<XtType> types = [];
//        //Get initial types.
//        for (int i = 0; i < flaskEntity.Types.Length; i++)
//        {
//            var type = flaskEntity.Types[i];
//            switch (type.Behavior)
//            {
//                case TypeBehavior.Atom:
//                    types.Add(new XtAtomType(flaskEntity.Strings.CStr(type.Name), type.Atom));
//                    break;
//                case TypeBehavior.Enum:
//                    types.Add(new XtEnumType(flaskEntity.Strings.CStr(type.Name)) { IsFlags = false });
//                    break;
//                case TypeBehavior.Flags:
//                    types.Add(new XtEnumType(flaskEntity.Strings.CStr(type.Name)) { IsFlags = true });
//                    break;
//                case TypeBehavior.Struct:
//                    types.Add(new XtStructType(flaskEntity.Strings.CStr(type.Name)));
//                    break;
//                default:
//                    throw new NotSupportedException();
//            }
//            ;
//        }
//
//        //Get struct base types.
//        for (int i = 0, baseIndex = 0; i < flaskEntity.Types.Length; i++)
//        {
//            var type = flaskEntity.Types[i];
//            for (int j = 0; j < type.BaseCount; j++, baseIndex++)
//            {
//                if (types[i] is not XtStructType structType) throw new NotSupportedException("Types that are not structs cannot have base types.");
//                var baseType = flaskEntity.Bases[baseIndex];
//                if (types[baseType.Type] is not XtStructType baseStructType) throw new NotSupportedException("Types that are not structs cannot be base types.");
//                structType.Bases.Add(baseStructType);
//            }
//        }
//
//        //Get struct,enum, flags fields.
//        for (int i = 0, fieldIndex = 0; i < flaskEntity.Types.Length; i++)
//        {
//            var typeEntity = flaskEntity.Types[i];
//            for (int j = 0; j < typeEntity.FieldCount; j++, fieldIndex++)
//            {
//                var fieldEntity = flaskEntity.Fields[fieldIndex];
//                switch (types[i])
//                {
//                    case XtStructType structType:
//                        var fieldType = types[fieldEntity.Type];
//                        fieldType = fieldEntity.Behavior switch
//                        {
//                            FieldBehavior.Pointer => XtPointerType.Get(fieldType),
//                            FieldBehavior.Handle => XtHandleType.Get(fieldType),
//                            _ => fieldType
//                        };
//                        if (fieldEntity.IsArray)
//                        {
//                            fieldType = XtArrayType.Get(fieldType);
//                        }
//                        structType.Fields.Add(new XtStructField(flaskEntity.Strings.CStr(fieldEntity.Name), structType, fieldType));
//                        break;
//                    case XtEnumType enumType:
//                        enumType.Labels.Add(flaskEntity.Strings.CStr(fieldEntity.Name));
//                        break;
//                    case var c:
//                        throw new NotSupportedException($"{c.GetType().Name} cannot contain fields.");
//                }
//            }
//        }
//        var textEncoding = new FlaskEncoding();
//
//        using var data_stream = new MemoryStream(flaskEntity.Data);
//        using var reader = new BinaryReader(data_stream);
//        //Parse Flask Blocks
//
//        List<XtRef> refs = [];
//        List<List<XtBlock>> refBlocks = [];
//        List<string> texts = [];
//        for (int i = 0, componentIndex = 0; i < flaskEntity.Refs.Length; i++)
//        {
//            var refEntity = flaskEntity.Refs[i];
//            var type = types[refEntity.Type];
//            if (refEntity.Record == ushort.MaxValue)
//            {
//                continue;
//            }
//            var recordEntity = flaskEntity.Records[refEntity.Record];
//            List<XtBlock> blocks = [];
//
//            long recordBytesStart = reader.BaseStream.Position;
//            for (int j = 0; j < recordEntity.ComponentCount; j++, componentIndex++)
//            {
//                var componentEntity = flaskEntity.Components[componentIndex];
//                var blockType = types[componentEntity.Type];
//                blockType = componentEntity.Behavior switch
//                {
//                    ComponentBehavior.Pointer => XtPointerType.Get(blockType),
//                    ComponentBehavior.Handle => XtHandleType.Get(blockType),
//                    _ => blockType
//                };
//                XtBlock block = new XtBlock(blockType);
//
//                for (int k = 0; k < componentEntity.Count; k++)
//                {
//                    switch (block.BlockType)
//                    {
//                        case XtPointerType pointerType:
//                            block.Add(new XtBlock.PointerValue(pointerType, reader.ReadUInt16(), reader.ReadUInt16()));
//                            break;
//                        case XtHandleType handleType:
//                            block.Add(new XtBlock.HandleValue(handleType, reader.ReadUInt32()));
//                            break;
//                        case var c:
//                            long position = reader.BaseStream.Position;
//                            block.Add(Parse(c, reader));
//                            long readCount = reader.BaseStream.Position - position;
//                            Debug.Assert(readCount == c.Size, $"Did not read the right amount of characters. Expected: {c.Size} - Read: {readCount}");
//                            break;
//                        default:
//                            throw new NotSupportedException();
//                    }
//                }
//
//                blocks.Add(block);
//            }
//            long recordBytesCount = reader.BaseStream.Position - recordBytesStart;
//            Debug.Assert(recordBytesCount == recordEntity.DataBytes, $"Did not read the right amount of characters. Expected: {recordEntity.DataBytes} - Read: {recordBytesCount}");
//
//            string text = textEncoding.GetString(reader.ReadBytes((int)recordEntity.StringBytes));
//            refBlocks.Add(blocks);
//            texts.Add(text);
//            refs.Add(new XtRef(refEntity.Id, blocks[0].Values[0]));
//        }
//
//        XtDatabase xtDatabase = new XtDatabase();
//        for (int j = 0, i = 0; j < flaskEntity.Refs.Length; j++)
//        {
//            if (flaskEntity.Refs[j].Record == ushort.MaxValue)
//            {
//                continue;
//            }
//            var blocks = refBlocks[i];
//            Dictionary<(int, int), XtArray> arrays = [];
//            for (int k = 0; k < blocks.Count; k++)
//            {
//                var block = blocks[k];
//                for (int l = 0; l < block.Values.Count; l++)
//                {
//                    if (block.Values[l] is XtStructValue container)
//                    {
//                        foreach (var value in container)
//                        {
//                            Reference(value, flaskEntity.Refs, blocks, arrays, texts[i]);
//                        }
//                    }
//                    if (block.BlockType is not XtPointerType or XtHandleType)
//                    {
//                        refs[i].RefHeap.Add(block.Values[l]);
//                    }
//                }
//            }
//            xtDatabase.Refs[flaskEntity.Refs[j].Id] = refs[i];
//            i++;
//        }
//
//        xtDatabase.Types.AddRange(types);
//        return xtDatabase;
//    }
//    static void Reference(IXtValueItem value, FlaskRefEntity[] refs, List<XtBlock> blocks, Dictionary<(int, int), XtArray> arrays, string text)
//    {
//        switch (value.Value)
//        {
//            case XtStructValue container:
//                foreach (var item in container)
//                {
//                    Reference(item, refs, blocks, arrays, text);
//                }
//                break;
//            case XtBlock.PointerValue pointerValue:
//                if (pointerValue.Block == ushort.MaxValue || pointerValue.Offset == ushort.MaxValue)
//                {
//                    value.Value = pointerValue.Type.CreateValue();
//                }
//                else
//                {
//                    value.Value = pointerValue.Type.CreateValue(blocks[pointerValue.Block].Values[pointerValue.Offset]);
//                }
//                break;
//            case XtBlock.HandleValue handleValue:
//                if (handleValue.Handle == uint.MaxValue)
//                {
//                    value.Value = handleValue.Type.CreateValue();
//                }
//                else
//                {
//                    value.Value = handleValue.Type.CreateValue(refs[handleValue.Handle].Id);
//                }
//                break;
//            case XtBlock.ArrayPointerValue arrayPointerValue:
//                if (arrayPointerValue.Block == ushort.MaxValue || arrayPointerValue.Offset == ushort.MaxValue || arrayPointerValue.Length == 0)
//                {
//                    value.Value = new XtArrayValue(arrayPointerValue.Type);
//                }
//                else
//                {
//                    var block = blocks[arrayPointerValue.Block];
//                    if (arrays.TryGetValue((arrayPointerValue.Block, arrayPointerValue.Offset), out var arrValue))
//                    {
//                        value.Value = new XtArrayValue(arrayPointerValue.Type, arrValue);
//                    }
//                    else
//                    {
//                        var val = new XtArray(arrayPointerValue.Type);
//                        arrays[(arrayPointerValue.Block, arrayPointerValue.Offset)] = val;
//                        for (int i = 0; i < arrayPointerValue.Length; i++)
//                        {
//                            val.Add(block.Values[arrayPointerValue.Offset + i]);
//                        }
//                        value.Value = new XtArrayValue(arrayPointerValue.Type, val);
//                        for (int i = 0; i < val.Values.Count; i++)
//                        {
//                            Reference(val.Values[i], refs, blocks, arrays, text);
//                        }
//                    }
//                }
//                break;
//            case XtBlock.StringIndexValue stringIndexValue:
//                value.Value = stringIndexValue.Type.CreateValue(text.CStr(stringIndexValue.Index));
//                break;
//        }
//    }
//}
