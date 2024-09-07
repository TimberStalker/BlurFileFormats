namespace BlurFileFormats.XtFlask.Types;

public interface IXtEnumType : IXtType
{
    IList<string> Labels { get; }
}
