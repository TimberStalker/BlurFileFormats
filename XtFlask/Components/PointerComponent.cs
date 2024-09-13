using BlurFileFormats.XtFlask.Values;

namespace BlurFileFormats.XtFlask.Components;

public class PointerComponent : IRecordComponent
{
    public List<List<IRecordComponent>> Components { get; }
    public ushort Component { get; }
    public ushort Offset { get; }
    public PointerComponent(List<List<IRecordComponent>> components, ushort component, ushort offset)
    {
        Components = components;
        Component = component;
        Offset = offset;
    }

    public IXtValue GetValue(IReadOnlyList<IXtRef> references, List<List<IRecordComponent>> refRecords)
    {
        if(Component == ushort.MaxValue || Offset == ushort.MaxValue)
        {
            return XtNullValue.Instance;
        }
        return refRecords[Component][Offset].GetValue(references, refRecords);
    }
}
