using BlurFileFormats.FlaskReflection.Entities;
using BlurFileFormats.FlaskReflection.Values;
using System.Collections.ObjectModel;

namespace BlurFileFormats.FlaskReflection.Types;

public class EnumType : IDataType
{
    public string Name { get; set; }
    public bool IsFlags { get; set; }
    public ObservableCollection<string> Values { get; } = [];

    public EnumType(string name)
    {
        Name = name;
    }

    public IDataValue CreateValue()
    {
        return new EnumValue(this);
    }
}
