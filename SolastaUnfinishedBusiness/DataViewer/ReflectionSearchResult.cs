using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.ModKit;

namespace SolastaUnfinishedBusiness.DataViewer;

internal abstract class ResultNode
{
    [UsedImplicitly] internal virtual string Name { get; }

    [UsedImplicitly] internal virtual Type Type { get; }

    [UsedImplicitly] internal virtual string NodeTypePrefix { get; }

    [UsedImplicitly] internal virtual string ValueText { get; }
}

internal class ResultNode<TNode> : ResultNode where TNode : class
{
    private const BindingFlags AllFlags =
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    internal TNode Node { get; set; }

    internal HashSet<TNode> Matches => Children.Select(c => c.Node).ToHashSet();

    private List<ResultNode<TNode>> Children { get; } = [];

    internal ToggleState ToggleState { get; set; } = ToggleState.Off;

    internal int Count { get; private set; }

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

    [CanBeNull]
    private ResultNode<TNode> FindOrAddChild(TNode node)
    {
        var rNode = Children.Find(rn => rn.Node == node);

        if (rNode != null)
        {
            return rNode;
        }

        rNode = Activator.CreateInstance(GetType(), AllFlags, null, null, null) as ResultNode<TNode>;

        if (rNode == null)
        {
            return null;
        }

        rNode.Node = node;
        Children.Add(rNode);

        return rNode;
    }

    internal void AddSearchResult(IEnumerable<TNode> path)
    {
        var rNode = this;

        Count++;

        foreach (var node in path)
        {
            if (rNode == null)
            {
                break;
            }

            rNode = rNode.FindOrAddChild(node);

            if (rNode != null)
            {
                rNode.Count++;
            }
        }
    }

    internal void Clear()
    {
        Node = null;
        Count = 0;
        Children.Clear();
    }

#if false
    private StringBuilder BuildString(StringBuilder builder, int depth)
    {
        builder.Append($"{NodeTypePrefix} {Name}:{Type} - {ValueText}\n".Indent(depth));

        return Children.Aggregate(builder, (current, child) => child.BuildString(current, depth + 1));
    }

    public override string ToString()
    {
        return BuildString(new StringBuilder().Append('\n'), 0).ToString();
    }
#endif

    internal delegate bool TraversalCallback(ResultNode<TNode> node, int depth);
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
