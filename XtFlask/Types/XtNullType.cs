using BlurFileFormats.XtFlask.Entities;
using BlurFileFormats.XtFlask.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.XtFlask.Types;
public class XtNullType : IXtType
{
    public static XtNullType Instance { get; } = new XtNullType();

    public string Name => "null";

    public IXtValue CreateDefault() => XtNullValue.Instance;


    public IXtValue ReadValue(BinaryReader reader, ValueResolver resolver) => XtNullValue.Instance;
    public void Emit(TypeTableBuilder types, List<FlaskBaseEntity> bases, List<FlaskFieldEntity> fields, StringTableBuilder stringTable)
    {
    }
}
