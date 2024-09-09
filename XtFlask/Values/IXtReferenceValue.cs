using BlurFileFormats.XtFlask.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlurFileFormats.XtFlask.Values;
public interface IXtReferenceValue : IXtValue
{
}
public class XtPointerValue : IXtReferenceValue
{
    public IXtType Type => Reference.Type;
    public object Value => Reference.Value;
    public IXtValue Reference { get; set; }

    public XtPointerValue(IXtValue reference)
    {
        Reference = reference;
    }
}
public class XtHandleValue : IXtReferenceValue
{
    public IXtType Type => Reference.Type;
    public object Value => Reference.Value;
    public IXtRef Reference { get; set; }

    public XtHandleValue(IXtRef reference)
    {
        Reference = reference;
    }
}