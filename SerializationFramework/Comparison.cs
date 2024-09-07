using BlurFileFormats.SerializationFramework.Attributes;
using System;
using System.Reflection;

namespace BlurFileFormats.SerializationFramework;

public class Comparison
{
    public bool IsEqual { get; }
    public object[] Values { get; }
    public IEnumerable<Comparison> Children { get; }
    public string Name { get; set; } = "";
    public Comparison(bool isEqual, object[] values, IEnumerable<Comparison> children)
    {
        IsEqual = isEqual;
        Values = values;
        Children = children;
    }
    public static Comparison Compare<T>(params T[] values) => Compare((Array)values);
    public static Comparison Compare(Array values)
    {
        if (values.Length < 1)
        {
            return new Comparison(true, values.Cast<object>().ToArray(), []);
        }
        var type = values.GetType().GetElementType()!;

        var equatableType = typeof(IEquatable<>).MakeGenericType(type);
        var isEqual = true;
        List<Comparison> children = [];
        if (type.IsAssignableTo(equatableType))
        {
            var method = type.GetMethod(nameof(Equals), [type])!;

            var compareValue = values.GetValue(0);
            for (int i = 0; i < values.Length; i++)
            {
                isEqual = (bool)method.Invoke(compareValue, [values.GetValue(i)])!;
                if (!isEqual)
                {
                    break;
                }
            }
        } else if(type.IsArray)
        {
            var arrayType = type.GetElementType()!;
            if(arrayType != typeof(byte))
            {
                int totalCount = 0;
                for(int i = 0; i <  values.Length; i++)
                {
                    var arr = (Array)values.GetValue(i)!;
                    for (int j = 0; j < arr.Length; j++)
                    {
                        totalCount++;
                    }
                }
                var arrayValues = Array.CreateInstance(arrayType, totalCount);
                int x = 0;
                for (int i = 0; i < values.Length; i++)
                {
                    var arr = (Array)values.GetValue(i)!;
                    for (int j = 0; j < arr.Length; j++)
                    {
                        arrayValues.SetValue(arr.GetValue(j), x);
                        x++;
                    }
                }
                var comparison = Compare(arrayValues);
                comparison.Name = "arrayType";
                children.Add(comparison);
            }
        }
        else
        {
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<IgnorePrintAttribute>() is not null) continue;
                Dictionary<Type, int> types = [];
                for (int i = 0; i < values.Length; i++)
                {
                    object value = property.GetValue(values.GetValue(i))!;
                    if(!types.TryGetValue(value.GetType(), out int len))
                    {
                        len = 0;
                    }
                    types[value.GetType()] = len + 1;
                }
                List<Comparison> diffChildren = [];
                foreach (var (elementType, len) in types)
                {
                    var propValues = Array.CreateInstance(elementType, len);
                    int x = 0;
                    for (int i = 0; i < values.Length; i++)
                    {
                        var value = property.GetValue(values.GetValue(i))!;
                        if (value.GetType() == elementType)
                        {
                            propValues.SetValue(value, x);
                            x++;
                        }
                    }
                    var comparison = Compare(propValues);
                    comparison.Name = elementType.Name;
                    diffChildren.Add(comparison);
                }
                if(diffChildren.Count == 1)
                {
                    diffChildren[0].Name = property.Name;
                    children.Add(diffChildren[0]);
                }
                else
                {
                    var c = new Comparison(true, values.Cast<object>().ToArray(), diffChildren);
                    c.Name = property.Name;
                    children.Add(c);
                }
            }
        }
        return new Comparison(isEqual, values.Cast<object>().ToArray(), children);
    }
}