using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Api.ModKit;

namespace SolastaUnfinishedBusiness.DataViewer;

internal abstract class ResultNode
{
    internal virtual string Name { get; }

    internal virtual Type Type { get; }

    internal virtual string NodeTypePrefix { get; }

    internal virtual string ValueText { get; }
}

internal class ResultNode<TNode> : ResultNode where TNode : class
{
    internal delegate bool TraversalCallback(ResultNode<TNode> node, int depth);
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
    protected const BindingFlags ALL_FLAGS =
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

    internal TNode Node { get; set; }

    internal HashSet<TNode> matches => Children.Select(c => c.Node).ToHashSet();

    internal List<ResultNode<TNode>> Children { get; } = new();

    internal ToggleState ToggleState { get; set; } = ToggleState.Off;

    internal ToggleState ShowSiblings { get; set; } = ToggleState.Off;

    internal int Count { get; set; }

    internal bool IsMatch { get; set; }

    internal void Traverse(TraversalCallback callback, int depth = 0)
    {
        if (!callback(this, depth))
        {
            return;
        }

        foreach (var child in Children)
        {
            child.Traverse(callback, depth + 1);
        }
    }

    internal ResultNode<TNode> FindChild(TNode node)
    {
        return Children.Find(rn => rn.Node == node);
    }

    [NotNull]
    private ResultNode<TNode> FindOrAddChild(TNode node)
    {
        var rNode = Children.Find(rn => rn.Node == node);

        if (rNode != null)
        {
            return rNode;
        }

        rNode = Activator.CreateInstance(GetType(), ALL_FLAGS, null, null, null) as ResultNode<TNode>;
        rNode.Node = node;
        Children.Add(rNode);

        return rNode;
    }

    internal void AddSearchResult(IEnumerable<TNode> path)
    {
        var rnode = this;
        Count++;
        foreach (var node in path)
        {
            rnode = rnode.FindOrAddChild(node);
            rnode.Count++;
        }
    }

    internal void Clear()
    {
        Node = null;
        Count = 0;
        Children.Clear();
    }

    private StringBuilder BuildString(StringBuilder builder, int depth)
    {
        builder.Append($"{NodeTypePrefix} {Name}:{Type} - {ValueText}\n".Indent(depth));

        return Children.Aggregate(builder, (current, child) => child.BuildString(current, depth + 1));
    }

    public override string ToString()
    {
        return BuildString(new StringBuilder().Append('\n'), 0).ToString();
    }
}

internal class ReflectionSearchResult : ResultNode<Node>
{
    internal override string Name => Node.Name;
    internal override Type Type => Node.Type;
    internal override string NodeTypePrefix => Node.NodeTypePrefix;
    internal override string ValueText => Node.ValueText;

    internal void AddSearchResult(Node node)
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
