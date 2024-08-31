using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.Localizations;
public class Text
{
    public uint Id { get; set; }
    public string Header { get; set; } = "";
    public ObservableCollection<TextItem> TextItems { get; } = [];
}
public class TextItem
{
    public Language Language { get; set; }
    public string Value { get; set;  }

    public TextItem(Language language, string value)
    {
        Language = language;
        Value = value;
    }
}