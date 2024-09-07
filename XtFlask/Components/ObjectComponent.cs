using BlurFileFormats.XtFlask.Values;

namespace BlurFileFormats.XtFlask.Components;

public class ObjectComponent : IRecordComponent
{
    public IXtValue Value { get; }

    public ObjectComponent(IXtValue value)
    {
        Value = value;
    }

    public IXtValue GetValue(IReadOnlyList<IXtRef> references, List<List<IRecordComponent>> refRecords) => Value;
}
