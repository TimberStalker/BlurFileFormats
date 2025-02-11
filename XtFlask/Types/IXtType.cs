using BlurFileFormats.XtFlask.Entities;
using BlurFileFormats.XtFlask.Values;

namespace BlurFileFormats.XtFlask.Types;

public interface IXtType
{
    string Name { get; }
    IXtValue CreateDefault();
    IXtValue ReadValue(BinaryReader reader, ValueResolver resolver);
    void Emit(TypeTableBuilder types, List<FlaskBaseEntity> bases, List<FlaskFieldEntity> fields, StringTableBuilder stringTable);
}