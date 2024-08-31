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
        if (values.Length <= 1) throw new ArgumentException();
        var type = values.GetType().GetElementType()!;

        var equatableType = typeof(IEquatable<>).MakeGenericType(type);
        var isEqual = true;
        List<Comparison> children = [];
        if (type.IsAssignableTo(equatableType))
        {
            var method = type.GetMethod(nameof(Equals), [type])!;

            var compareValue = values.GetValue(0);
            for (int i = 1; i < values.Length; i++)
            {
                isEqual = (bool)method.Invoke(compareValue, [values.GetValue(i)])!;
                if (!isEqual)
                {
                    break;
                }
            }
        } else if(type.IsArray)
        {
            int length = ((Array)values.GetValue(0)!).Length;
            for(int i = 1; i < values.Length; i++)
            {
                if(((Array)values.GetValue(i)!).Length != length)
                {
                    goto Skip;
                }
            }
            goto End;



        Skip:;

            isEqual = false;

        End:;
            values = values.Cast<Array>().Select(a => a.Length).ToArray();
        }
        else
        {
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var propValues = Array.CreateInstance(property.PropertyType, values.Length);
                for (int i = 0; i < values.Length; i++)
                {
                    propValues.SetValue(property.GetValue(values.GetValue(i))!, i);
                }
                var comparison = Compare(propValues);
                comparison.Name = property.Name;
                children.Add(comparison);
            }
        }
        return new Comparison(isEqual, values.Cast<object>().ToArray(), children);
    }
}