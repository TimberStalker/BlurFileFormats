using BlurFileFormats.XtFlask.Components;
using BlurFileFormats.XtFlask.Types.Fields;
using BlurFileFormats.XtFlask.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.XtFlask.Types;
public class XtArrayType : IXtType
{
    public IXtType ElementType { get; }
    public string Name => $"{ElementType.Name}[]";

    public XtArrayType(IXtType baseType)
    {
        ElementType = baseType;
    }

    IXtValue IXtType.CreateDefault() => CreateDefault();
    public XtArrayValue CreateDefault() => new XtArrayValue(this);

    public IXtValue ReadValue(BinaryReader reader, ValueResolver resolver)
    {
        var array = CreateDefault();
        resolver.AddResolver(new ArrayItem(array, reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt32()));
        return array;
    }

    public record ArrayItem(XtArrayValue Value, ushort Component, ushort Offset, uint Length) : IResolverItem
    {
        public void Resolve(IReadOnlyList<IXtRef> references, List<List<IRecordComponent>> refRecords)
        {
            for (int i = Offset; i < Length; i++)
            {
                Value.Values.Add(new XtArrayValueItem(refRecords[Component][i].GetValue(references, refRecords)));
            }
        }
    }
}
public class XtPointerType : IXtType
{
    public IXtType BaseType { get; }
    public string Name => $"{BaseType.Name}*";

    public XtPointerType(IXtType baseType)
    {
        BaseType = baseType;
    }

    IXtValue IXtType.CreateDefault() => CreateDefault();
    public XtPointerValue CreateDefault() => new XtPointerValue(XtNullValue.Instance);

    public IXtValue ReadValue(BinaryReader reader, ValueResolver resolver)
    {
        var pointer = CreateDefault();
        resolver.AddResolver(new PointerItem(pointer, reader.ReadUInt16(), reader.ReadUInt16()));
        return pointer;
    }
    record PointerItem(XtPointerValue Value, ushort Component, ushort Offset) : IResolverItem
    {
        public void Resolve(IReadOnlyList<IXtRef> references, List<List<IRecordComponent>> refRecords)
        {
            if (Component == ushort.MaxValue || Offset == ushort.MaxValue) return;
            Value.Reference = refRecords[Component][Offset].GetValue(references, refRecords);
        }
    }
}
public class XtHandleType : IXtType
{
    public IXtType BaseType { get; }
    public string Name => $"{BaseType.Name}^";

    public XtHandleType(IXtType baseType)
    {
        BaseType = baseType;
    }

    IXtValue IXtType.CreateDefault() => CreateDefault();
    public XtHandleValue CreateDefault() => new XtHandleValue(XtRefNull.Instance);

    public IXtValue ReadValue(BinaryReader reader, ValueResolver resolver)
    {
        var handle = CreateDefault();
        resolver.AddResolver(new HandleItem(handle, reader.ReadUInt32()));
        return handle;
    }
    record HandleItem(XtHandleValue Value, uint Id) : IResolverItem
    {
        public void Resolve(IReadOnlyList<IXtRef> References, List<List<IRecordComponent>> RefRecords)
        {
            if (Id == uint.MaxValue) return;
            Value.Reference = References[(int)Id];
        }
    }
}
