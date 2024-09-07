using BlurFileFormats.XtFlask.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.XtFlask.Values;
public class XtRefValue : IXtValue
{
    public IXtType Type => Ref.Type;
    public object Value => Ref.Value;

    public IXtRef Ref { get; }

    public XtRefValue(IXtRef xtRef)
    {
        Ref = xtRef;
    }
}
