using BlurFileFormats.XtFlask.Values;

namespace BlurFileFormats.XtFlask.Components;

public class HandleComponent : IRecordComponent
{
    public uint Id { get; }

    public HandleComponent(uint id)
    {
        Id = id;
    }

    public IXtValue GetValue(IReadOnlyList<IXtRef> references, List<List<IRecordComponent>> refRecords)
    {
        if (Id == uint.MaxValue) return XtNullValue.Instance;
        return new XtRefValue(references[(int)Id]);
    }
}