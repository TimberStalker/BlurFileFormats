using BlurFileFormats.FlaskReflection.Values;
using System.Collections.ObjectModel;

namespace BlurFileFormats.FlaskReflection.Types;

public class StructureType : IDataType
{
    public string Name { get; set; }
    public StructureType? Base { get; set; }
    public ObservableCollection<StructureField> Fields { get; } = [];
    public StructureType(string name)
    {
        Name = name;
    }

    public IDataValue CreateValue()
    {
        var value = new StructureValue(this);
        foreach(var item in Fields)
        {
            value.Fields.Add(new StructureFieldEntity(item));
        }
        return value;
    }
}
public class StructureField
{
    public StructureType Parent { get; }
    public string Name { get; set; }
    public IDataType DataType { get; set; }
    public StructureField(StructureType parent, string name, IDataType dataType)
    {
        Parent = parent;
        Name = name;
        DataType = dataType;
    }
}