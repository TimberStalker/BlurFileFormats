using BlurFileFormats.FlaskReflection.Entities;
using BlurFileFormats.FlaskReflection.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.FlaskReflection.Values;
public class ArrayValue : IDataValue
{
    public ArrayPointerType Type { get; }
    IDataType IDataValue.Type => Type;
    public ObservableCollection<ArrayItemEntity> Values { get; } = [];
    object IDataValue.Value { get => Values; set => throw new NotImplementedException(); }

    public ArrayValue(ArrayPointerType type)
    {
        Type = type;
    }

}
public class ArrayItemEntity : IEntity
{
    public ArrayValue Parent { get; }
    public IEntity Entity { get; set; }

    public IDataType DataType => Entity.DataType;
    IDataValue IEntity.Value { get => Entity.Value; set => Entity.Value = value; }

    public ArrayItemEntity(ArrayValue parent, IEntity entity)
    {
        Parent = parent;
        Entity = entity;
    }
}