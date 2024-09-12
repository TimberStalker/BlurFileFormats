using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework;
public interface IPathSource<T>
{
    DataPath Path { get; }
}
public class DataPath : IEnumerable<string>
{
    string[] items;
    public string this[int i] => items[i];
    public int Length => items.Length;
    public DataPath(string path)
    {
        items = path.Split('.', StringSplitOptions.TrimEntries);
    }


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<string> GetEnumerator()
    {
        foreach (var item in items) yield return item;
    }
}
