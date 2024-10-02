using BlurFileFormats.SerializationFramework.Command;
using BlurFileFormats.SerializationFramework.Commands.Structures;
using System.Collections;
using System.Reflection;
using System.Text;

namespace BlurFileFormats.SerializationFramework;

class CommandKey
{
    public PropertyInfo Property { get; }
    public string Path { get; }

    public CommandKey(PropertyInfo property, string path)
    {
        Property = property;
        Path = path;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Property, Path);
    }
    public override bool Equals(object? obj)
    {
        return obj is CommandKey k && k.Property == Property;
    }
    public override string ToString() => $"{Path}->{Property.Name}";
}
public class TypeTree : IEnumerable<object>
{
    Type type;

    public TypeTree(Type type)
    {
        this.type = type;
    }

    List<PropertyInfo> tree = [];
    Dictionary<CommandKey, ISerializerCommand> commandTree = [];
    public PropertyInfo? Current => tree.Count > 0 ? tree[^1] : null;
    public void Push(PropertyInfo value)
    {
        tree.Add(value);
    }
    public void Add(PropertyInfo property, ISerializerCommand command)
    {
        commandTree.Add(new CommandKey(property, string.Join('.',tree.Select(i => i.Name))), command);
    }
    public void Pop()
    {
        tree.RemoveAt(tree.Count - 1);
    }
    public ISerializerCommand<T> GetCommand<T>(DataPath path) => (ISerializerCommand<T>)GetCommand(path);
    public ISerializerCommand GetCommand(DataPath path)
    {
        Type currentType = type;
        if (Current is PropertyInfo p)
        {
            currentType = p.DeclaringType!;
        }
        var desitnationProperty = currentType.GetProperty(path[0]);
        int pathIndex = 1; 
        StringBuilder builder = new StringBuilder();
        if (desitnationProperty is null)
        {
            if (path.Length <= 1)
            {
                throw new Exception("Path must contain a direct property or a qualified property of a parent type.");
            }
            int treeIndex;
            for (treeIndex = tree.Count - 1; treeIndex >= 0; treeIndex--)
            {
                var item = tree[treeIndex].DeclaringType!;
                if (item.Name == path[0])
                {
                    desitnationProperty = item.GetProperty(path[1]);
                    if (desitnationProperty is not null)
                    {
                        currentType = item;
                        pathIndex = 2;
                        break;
                    }
                }
            }
            if (desitnationProperty is null)
            {
                throw new Exception("Path is not valid.");
            }
            if (treeIndex >= 0)
            {
                for (int i = 0; i <= treeIndex; i++)
                {
                    builder.Append(tree[i].Name);
                    builder.Append('.');
                }
            }
        }
        else
        {
            builder.Append(desitnationProperty.Name);
            builder.Append('.');
        }
        for (; pathIndex < path.Length; pathIndex++)
        {
            desitnationProperty = desitnationProperty.PropertyType.GetProperty(path[pathIndex]);
            if (desitnationProperty is null)
            {
                throw new Exception("Path is not valid.");
            }
            if(pathIndex != path.Length-1)
            {
                builder.Append(desitnationProperty.Name);
                builder.Append('.');
            }
        }
        if(builder.Length > 0)
        {
            builder.Remove(builder.Length - 1, 1);
        }
        return commandTree[new CommandKey(desitnationProperty, builder.ToString())];
    }

    public IEnumerator<object> GetEnumerator() => tree.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
