using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ModKit;
using SolastaCommunityExpansion.Api.Infrastructure;

namespace SolastaCommunityExpansion.DataViewer;

public abstract class ResultNode
{
    public virtual string Name { get; }

    public virtual Type Type { get; }

    public virtual string NodeTypePrefix { get; }

    public virtual string ValueText { get; }
}

public class ResultNode<TNode> : ResultNode where TNode : class
{
    public delegate bool TraversalCallback(ResultNode<TNode> node, int depth);
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
    protected const BindingFlags ALL_FLAGS =
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

    public TNode Node { get; set; }

    public HashSet<TNode> matches => Children.Select(c => c.Node).ToHashSet();

    public List<ResultNode<TNode>> Children { get; } = new();

    public ToggleState ToggleState { get; set; } = ToggleState.Off;

    public ToggleState ShowSiblings { get; set; } = ToggleState.Off;

    public int Count { get; set; }

    public bool IsMatch { get; set; }

    public void Traverse(TraversalCallback callback, int depth = 0)
    {
        if (callback(this, depth))
        {
            foreach (var child in Children)
            {
                child.Traverse(callback, depth + 1);
            }
        }
    }

    public ResultNode<TNode> FindChild(TNode node)
    {
        return Children.Find(rn => rn.Node == node);
    }

    public ResultNode<TNode> FindOrAddChild(TNode node)
    {
        var rnode = Children.Find(rn => rn.Node == node);
        if (rnode == null)
        {
            rnode = Activator.CreateInstance(GetType(), ALL_FLAGS, null, null, null) as ResultNode<TNode>;
            rnode.Node = node;
            Children.Add(rnode);
        }

        return rnode;
    }

    public void AddSearchResult(IEnumerable<TNode> path)
    {
        var rnode = this;
        Count++;
        foreach (var node in path)
        {
            rnode = rnode.FindOrAddChild(node);
            rnode.Count++;
        }
    }

    public void Clear()
    {
        Node = null;
        Count = 0;
        Children.Clear();
    }

    private StringBuilder BuildString(StringBuilder builder, int depth)
    {
        builder.Append($"{NodeTypePrefix} {Name}:{Type} - {ValueText}\n".Indent(depth));
        foreach (var child in Children)
        {
            builder = child.BuildString(builder, depth + 1);
        }

        return builder;
    }

    public override string ToString()
    {
        return BuildString(new StringBuilder().Append('\n'), 0).ToString();
    }
}

public class ReflectionSearchResult : ResultNode<Node>
{
    public override string Name => Node.Name;
    public override Type Type => Node.Type;
    public override string NodeTypePrefix => Node.NodeTypePrefix;
    public override string ValueText => Node.ValueText;

    public void AddSearchResult(Node node)
    {
        if (node == null)
        {
            return;
        }

        var path = new List<Node>();
        for (; node != null && node != Node; node = node.GetParent())
        {
            path.Add(node);
        }

        AddSearchResult(path.Reverse<Node>());
    }
}
