using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlurFileFormats.FlaskReflection.Entities;
using BlurFileFormats.FlaskReflection.Types;

namespace BlurFileFormats.FlaskReflection;
public class Flask
{
    public ObservableCollection<IRecord> Records { get; } = [];
    public ObservableCollection<IDataType> DataTypes { get; } = [];
}
public interface IRecord
{
    uint Id { get; set; }
    IEntity Entity { get; }
}
public class Record : IRecord
{
    public uint Id { get; set; }
    public IEntity Entity { get; set; }
    public ObservableCollection<IEntity> Heap { get; } = [];
    public Record(uint id, IEntity entity)
    {
        Id = id;
        Entity = entity;
    }
}
public class ExternalRecord : IRecord
{
    public uint Id { get; set; }

    public IEntity Entity { get; }
    public ExternalRecord(IDataType dataType)
    {
        Entity = new NullEntity(dataType);
    }
}
