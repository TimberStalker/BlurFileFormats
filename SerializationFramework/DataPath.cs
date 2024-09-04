using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework;
public static class DataPath
{
    public static object GetValue(string path, object value, List<object> tree)
    {
        var pathItems = path.Split('.');
        var lengthObj = value;
        var desitnationProperty = value.GetType().GetProperty(pathItems[0]);
        int pathIndex = 1;
        if (desitnationProperty is null)
        {
            if (pathItems.Length <= 1)
            {
                throw new Exception("Path must contain a direct property or a qualified property of a parent type.");
            }
            int treeIndex;
            for (treeIndex = tree.Count - 1; treeIndex >= 0; treeIndex--)
            {
                var item = tree[treeIndex];
                if (item.GetType().Name == pathItems[0])
                {
                    desitnationProperty = item.GetType().GetProperty(pathItems[1]);
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
        for (; pathIndex < pathItems.Length; pathIndex++)
        {
            lengthObj = desitnationProperty.GetValue(lengthObj);

            if (lengthObj is null)
            {
                throw new Exception("Path is not valid.");
            }

            desitnationProperty = lengthObj.GetType().GetProperty(pathItems[pathIndex]);
            if (desitnationProperty is null)
            {
                throw new Exception("Path is not valid.");
            }
        }
        var result = desitnationProperty.GetValue(lengthObj);
        if (result is null) throw new Exception("Value has not been read.");
        return result;
    }
}
