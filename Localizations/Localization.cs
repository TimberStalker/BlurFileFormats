using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.Localizations;
public class Localization
{
    public ObservableCollection<Language> Languages { get; } = [];
    public ObservableCollection<Text> Texts { get; } = [];
}
