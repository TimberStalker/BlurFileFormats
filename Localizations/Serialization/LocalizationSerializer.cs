using BlurFileFormats.Utils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlurFileFormats.Localizations.Serialization;
public static class LocalizationSerializer
{
    public static Localization DeserializeLocalization(string fileName)
    {
        using FileStream stream = File.OpenRead(fileName);
        return DeserializeLocalization(stream);
    }
    public static void SerializeLocalization(Localization localization, string fileName)
    {
        using FileStream stream = File.OpenWrite(fileName);
        SerializeLocalization(localization, stream);
    }
    public static Localization DeserializeLocalization(Stream stream)
    {
        using BinaryReader reader = new BinaryReader(stream, Encoding.ASCII, true);

        var formatBytes = reader.ReadChars(4);
        if (formatBytes is not ['O', 'L', 'T', 'X'])
        {
            throw new Exception($"{string.Concat(formatBytes)} is not supported for localization files.");
        }
        var version = reader.ReadInt32();
        if (version != 1)
        {
            throw new Exception($"Version {version} is not supported for localization files.");
        }

        var localization = new Localization();

        int headerSize = reader.ReadInt32();
        int langStarts = reader.ReadInt32();

        int unknownC = reader.ReadInt32();
        int languageCount = reader.ReadInt32();
        int unknown84 = reader.ReadInt32();

        for (int i = 0; i < languageCount; i++)
        {
            var name = reader.ReadChars(4)[0..2];
            var unknown1 = reader.ReadUInt32();
            var unknown2 = reader.ReadUInt32();
            var unknown3 = reader.ReadUInt32();
            var start = reader.ReadUInt32();
            var end = reader.ReadUInt32();

            localization.Languages.Add(new Language() { Name = string.Concat(name) });
        }

        int stringCount = reader.ReadInt32();
        int byteSize = reader.ReadInt32();

        var stringHeaders = new (uint id, uint pos)[stringCount];

        for (int i = 0; i < stringCount; i++)
        {
            uint stringID = reader.ReadUInt32();
            uint stringPos = reader.ReadUInt32();
            stringHeaders[i] = (stringID, stringPos);
        }
        Dictionary<uint, Text> locDictionary = new Dictionary<uint, Text>();
        for (int i = 0; i < stringCount; i++)
        {
            var textString = reader.ReadCString();
            var text = new Text()
            {
                Id = stringHeaders[i].id,
                Header = textString
            };
            localization.Texts.Add(text);
            locDictionary[text.Id] = text;
        }

        for (int i = 0; i < languageCount; i++)
        {
            var language = localization.Languages[i];

            int langStringCount = reader.ReadInt32();
            int langByteSize = reader.ReadInt32();
            var langStringHeaders = new (uint id, uint pos)[langStringCount];

            for (int j = 0; j < langStringCount; j++)
            {
                uint stringID = reader.ReadUInt32();
                uint stringPos = reader.ReadUInt32();
                langStringHeaders[j] = (stringID, stringPos);
            }

            for (int j = 0; j < langStringCount; j++)
            {
                var text = reader.ReadCStringUnicode();
                locDictionary[langStringHeaders[j].id].TextItems.Add(new TextItem(language, text));
            }
        }
        return localization;
    }
    public static void SerializeLocalization(Localization localization, Stream stream)
    {
        int asciiOffset = localization.Texts.Count * 8;
        byte[] asciiBytes;
        using var asciiStream = new MemoryStream();
        using (var asciiWriter = new BinaryWriter(asciiStream))
        {
            asciiWriter.Write(localization.Texts.Count);
            asciiWriter.Write(8);
            using var asciiTextStream = new MemoryStream();
            using (var writer = new BinaryWriter(asciiTextStream))
            {
                foreach (var item in localization.Texts)
                {
                    asciiWriter.Write(item.Id);
                    asciiWriter.Write((int)asciiTextStream.Position + asciiOffset + 8);
                    writer.Write(Encoding.ASCII.GetBytes(item.Header));
                    writer.Write((byte)0);
                }
            }
            asciiWriter.Write(asciiTextStream.ToArray());
            asciiBytes = asciiStream.ToArray();
        }

        var unicodeStreams = GetUnicodeLanguageStreams(localization);

        using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
        {
            writer.Write('O');
            writer.Write('L');
            writer.Write('T');
            writer.Write('X');
            writer.Write(1);
            writer.Write(16);

            using var headerStream = new MemoryStream();
            using (var headerWriter = new BinaryWriter(headerStream))
            {
                headerWriter.Write(0xC);
                headerWriter.Write(unicodeStreams.Count);
                headerWriter.Write(0x84);
                int startOffset = 28 + localization.Languages.Count * 24 + asciiBytes.Length;
                foreach (var lang in localization.Languages)
                {
                    headerWriter.Write(Encoding.ASCII.GetBytes(lang.Name));
                    headerWriter.Write((short)0);
                    headerWriter.Write(0);
                    headerWriter.Write(0);
                    headerWriter.Write(0);
                    headerWriter.Write(startOffset);
                    headerWriter.Write(unicodeStreams[lang].Length);
                    startOffset += unicodeStreams[lang].Length;
                }
                headerWriter.Write(asciiBytes);
                writer.Write((int)headerStream.Length);
                writer.Write(headerStream.ToArray());
            }
            foreach (var lang in localization.Languages)
            {
                writer.Write(unicodeStreams[lang]);
            }
        }
    }

    static Dictionary<Language, byte[]> GetUnicodeLanguageStreams(Localization localization)
    {
        Dictionary<Language, byte[]> languageStreams = new Dictionary<Language, byte[]>();
        int langOffset = localization.Texts.Count() * 8;
        foreach (var language in localization.Languages)
        {
            var langStream = new MemoryStream();
            using (var langWriter = new BinaryWriter(langStream))
            {
                langWriter.Write(langOffset / 8);
                langWriter.Write(8);
                using var langTextStream = new MemoryStream();
                using (var writer = new BinaryWriter(langTextStream))
                {
                    foreach (var item in localization.Texts.OrderBy(l => l.Id))
                    {
                        var languageText = item.TextItems.FirstOrDefault(t => t.Language == language);
                        if (languageText != null)
                        {
                            langWriter.Write(item.Id);
                            langWriter.Write((int)langTextStream.Position + langOffset + 8);
                            writer.Write(Encoding.Unicode.GetBytes(languageText.Value));
                            writer.Write((short)0);
                        }
                    }
                }
                langWriter.Write(langTextStream.ToArray());
            }
            languageStreams[language] = langStream.ToArray();
        }
        return languageStreams;
    }
}
