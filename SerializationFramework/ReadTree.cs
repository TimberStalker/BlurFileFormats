using System.Collections;
using System.Reflection;

namespace BlurFileFormats.SerializationFramework;

public class ReadTree : IEnumerable<object>
{
    List<object> tree = [];
    public object CurrentObject => tree.Last();
    public void Push(object value)
    {
        tree.Add(value);
    }
    public void Pop()
    {
        tree.RemoveAt(tree.Count - 1);
    }

    public F? GetValue<T, F>(PropertyInfo property) where T : Attribute, IPathSource<F>
    {
        var attribute = property.GetCustomAttribute<T>();
        if (attribute is null) return default;
        return GetValue(attribute);
    }
    public T GetValue<T>(IPathSource<T> source) => GetValue<T>(source.Path);

    public T GetValue<T>(DataPath path) => (T)GetValue(path);
    public object GetValue(DataPath path)
    {
        var lengthObj = CurrentObject;
        var desitnationProperty = lengthObj.GetType().GetProperty(path[0]);
        int pathIndex = 1;
        if (desitnationProperty is null)
        {
            if (path.Length <= 1)
            {
                throw new Exception("Path must contain a direct property or a qualified property of a parent type.");
            }
            int treeIndex;
            for (treeIndex = tree.Count - 1; treeIndex >= 0; treeIndex--)
            {
                var item = tree[treeIndex];
                if (item.GetType().Name == path[0])
                {
                    desitnationProperty = item.GetType().GetProperty(path[1]);
                    if (desitnationProperty is not null)
                    {
                        lengthObj = item;
                        pathIndex = 2;
                        break;
                    }
                }
            }
            if (desitnationProperty is null)
            {
                throw new Exception("Path is not valid.");
            }
        }
        for (; pathIndex < path.Length; pathIndex++)
        {
            lengthObj = desitnationProperty.GetValue(lengthObj);

            if (lengthObj is null)
            {
                throw new Exception("Path is not valid.");
            }

            desitnationProperty = lengthObj.GetType().GetProperty(path[pathIndex]);
            if (desitnationProperty is null)
            {
                throw new Exception("Path is not valid.");
            }
        }
        var result = desitnationProperty.GetValue(lengthObj);
        if (result is null) throw new Exception("Value has not been read.");
        return result;
    }

    public IEnumerator<object> GetEnumerator() => tree.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
public class WriteTree : IEnumerable<object>
{
    List<object> tree = [];
    public object CurrentObject => tree.Last();
    public void Push(object value)
    {
        tree.Add(value);
    }
    public void Pop()
    {
        tree.RemoveAt(tree.Count - 1);
    }
    public T SetValue<T>(IPathSource<T> source) => SetValue<T>(source.Path);

    public T SetValue<T>(DataPath path) => (T)SetValue(path);
    public object SetValue(DataPath path)
    {
        var lengthObj = CurrentObject;
        var desitnationProperty = lengthObj.GetType().GetProperty(path[0]);
        int pathIndex = 1;
        if (desitnationProperty is null)
        {
            if (path.Length <= 1)
            {
                throw new Exception("Path must contain a direct property or a qualified property of a parent type.");
            }
            int treeIndex;
            for (treeIndex = tree.Count - 1; treeIndex >= 0; treeIndex--)
            {
                var item = tree[treeIndex];
                if (item.GetType().Name == path[0])
                {
                    desitnationProperty = item.GetType().GetProperty(path[1]);
                    if (desitnationProperty is not null)
                    {
                        lengthObj = item;
                        pathIndex = 2;
                        break;
                    }
                }
            }
            if (desitnationProperty is null)
            {
                throw new Exception("Path is not valid.");
            }
        }
        for (; pathIndex < path.Length; pathIndex++)
        {
            lengthObj = desitnationProperty.GetValue(lengthObj);

            if (lengthObj is null)
            {
                throw new Exception("Path is not valid.");
            }

            desitnationProperty = lengthObj.GetType().GetProperty(path[pathIndex]);
            if (desitnationProperty is null)
            {
                throw new Exception("Path is not valid.");
            }
        }
        var result = desitnationProperty.GetValue(lengthObj);
        if (result is null) throw new Exception("Value has not been read.");
        return result;
    }

    public IEnumerator<object> GetEnumerator() => tree.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
