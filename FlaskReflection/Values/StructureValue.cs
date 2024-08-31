using BlurFileFormats.FlaskReflection.Entities;
using BlurFileFormats.FlaskReflection.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.FlaskReflection.Values;
public class StructureValue : IDataValue
{
    public StructureType Type { get; }
    IDataType IDataValue.Type => Type;
    public ObservableCollection<StructureFieldEntity> Fields { get; } = [];
    public object Value { get => Fields; set => throw new NotImplementedException(); }


    public StructureValue(StructureType type)
    {
        Type = type;
    }
}
public class StructureFieldEntity : IEntity
{
    public StructureField Field { get; }
    public IDataType DataType => Field.DataType;
    public IDataValue Value { get; set; } = NullDataValue.NullValue;

    public StructureFieldEntity(StructureField field)
    {
        Field = field;
    }

}