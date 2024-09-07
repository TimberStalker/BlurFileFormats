using BlurFileFormats.XtFlask.Values;

namespace BlurFileFormats.XtFlask.Components;

public interface IRecordComponent
{
    IXtValue GetValue(IReadOnlyList<IXtRef> references, List<List<IRecordComponent>> refRecords);
}
