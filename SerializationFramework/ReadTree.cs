using BlurFileFormats.SerializationFramework.Command;
using System.Collections;
using System.Reflection;

namespace BlurFileFormats.SerializationFramework;

public class ReadTree
{
    Dictionary<ISerializerCommand, object> tree = [];
    public void Add(ISerializerCommand command, object value)
    {
        tree.Add(command, value);
    }

    public virtual T GetValue<T>(ISerializeCommand<T> command) => (T)GetValue((ISerializerCommand)command);
    public virtual T GetValue<T>(ISerializerCommand<T> command) => (T)GetValue((ISerializerCommand)command);
    public virtual object GetValue(ISerializerCommand command) => tree[command];
}

public class ChildTree : ReadTree
{
    public ReadTree Tree { get; }

    public ChildTree(ReadTree tree)
    {
        this.Tree = tree;
    }
}