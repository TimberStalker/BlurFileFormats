using BlurFileFormats.SerializationFramework.Command;
using System.Collections;
using System.Reflection;

namespace BlurFileFormats.SerializationFramework;

public class ReadTree
{
    public ReadTree? Parent { get; }
    List<CommandValue> tree = [];
    public ReadTree()
    {
        
    }
    public ReadTree(ReadTree parent) : this()
    {
        Parent = parent;
    }
    public void Add(ISerializerCommand command, object value)
    {
        tree.Add(new CommandValue(command, value));
    }

    public T GetValue<T>(ISerializeCommand<T> command) => (T)GetValue((ISerializerCommand)command);
    public T GetValue<T>(ISerializerCommand<T> command) => (T)GetValue((ISerializerCommand)command);
    public object GetValue(ISerializerCommand command)
    {
        var result = TryGetValue(command);
        if (result is null) throw new Exception("Could not find command in tree.");
        return result;
    }
    object? TryGetValue(ISerializerCommand command, ReadTree? limit = null, bool includeParent = false)
    {
        for (int i = 0; i < tree.Count; i++)
        {
            if (tree[i].Command == command)
            {
                return tree[i].Value;
            }
            else if (tree[i].Value is ReadTree subTree)
            {
                if (subTree == limit) break;
                var subResult = subTree.TryGetValue(command, includeParent: true);
                if (subResult is not null) return subResult;
            }
        }
        if (!includeParent) return null;
        return Parent?.TryGetValue(command, this);
    }

    record CommandValue(ISerializerCommand Command, object Value);
}