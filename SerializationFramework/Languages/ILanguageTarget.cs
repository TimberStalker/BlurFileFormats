using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.SerializationFramework.Languages;
public interface ILanguageTarget
{
    string CreateClasses(Type t);
}

public interface ILanguageReadTarget : ILanguageTarget
{
    public string Assign(string target, string source);
    public string Test(string source, string value);
    public string ReadInt8();
    public string ReadInt16();
    public string ReadInt32();
    public string ReadInt64();
    public string ReadUInt8();
    public string ReadUInt16();
    public string ReadUInt32();
    public string ReadUInt64();
    public string ReadBool(int size);
    public string ReadString(Encoding encoding, string length);
    public string ReadCString(Encoding encoding, string? length);
    public string ReadLoopStart(int length, string variable);
    public string ReadLoopEnd();

    public string Reader();
}
public interface ILanguageWriteTarget : ILanguageTarget
{
    public string WriteInt8();
    public string WriteInt16();
    public string WriteInt32();
    public string WriteInt64();
    public string WriteUInt8();
    public string WriteUInt16();
    public string WriteUInt32();
    public string WriteUInt64();

    public string Writer();
}
