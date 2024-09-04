using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Languages;
public class SerializerWriter
{
    public string Indent { get; set; } = "    ";
    public int IndentLevel { get; set; }
    public StringBuilder Builder { get; } = new();
    public void Write(string s)
    {
        foreach(var text in s.Split('\n'));
    }
    public void WriteLine(string s)
    {

    }
    public override string ToString()
    {
        return base.ToString();
    }
}
