using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ModKit;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SolastaCommunityExpansion.DataViewer;

public enum NodeType
{
    Root,
    Component,
    Item,
    Field,
    Property
}

// This structure has evolved into a reflection graph or DAG but for the sake of continuity we will stick with calling it a tree
public class ReflectionTree : ReflectionTree<object>
{
    public ReflectionTree(object root) : base(root) { }
}

public class ReflectionTree<TRoot>
{
    private RootNode<TRoot> _root;

    public ReflectionTree(TRoot root)
    {
        SetRoot(root);
    }

    public TRoot Root => _root.Value;

    public Node RootNode => _root;

    public void SetRoot(TRoot root)
    {
        if (_root != null)
        {
            _root.SetValue(root);
        }
        else
        {
            _root = new RootNode<TRoot>("<Root>", root);
        }
    }
}

public abstract class Node
{
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
    protected const BindingFlags ALL_FLAGS =
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

    // the graph will not show any child nodes of following types
    internal static readonly HashSet<Type> BASE_TYPES = new()
    {
        typeof(object),
        typeof(DBNull),
        typeof(bool),
        typeof(char),
        typeof(sbyte),
        typeof(byte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal),
        //typeof(DateTime),
        typeof(string),
        typeof(IntPtr),
        typeof(UIntPtr)
    };

    public readonly bool IsNullable;

    public readonly NodeType NodeType;
    public readonly Type Type;

    protected Node(Type type, NodeType nodeType)
    {
        NodeType = nodeType;
        Type = type;
        IsNullable = Type.IsGenericType && !Type.IsGenericTypeDefinition &&
                     Type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    [Obsolete("TODO - move this into a proper view model", false)]
    public ToggleState Expanded { get; set; }

    [Obsolete("TODO - move this into a proper view model", false)]
    public bool Matches { get; set; }

    public string NodeTypePrefix => NodeType switch
    {
        NodeType.Component => "c",
        NodeType.Item => "i",
        NodeType.Field => "f",
        NodeType.Property => "p",
        _ => string.Empty
    };

    public int ExpandedNodeCount
    {
        get
        {
            var count = 1;
            if (IsBaseType)
            {
                return count;
            }

#pragma warning disable CS0618 // Type or member is obsolete
            if (Expanded == ToggleState.On)
#pragma warning restore CS0618 // Type or member is obsolete
            {
                foreach (var child in GetItemNodes())
                {
                    count += child.ExpandedNodeCount;
                }

                foreach (var child in GetComponentNodes())
                {
                    count += child.ExpandedNodeCount;
                }

                foreach (var child in GetPropertyNodes())
                {
                    count += child.ExpandedNodeCount;
                }

                foreach (var child in GetFieldNodes())
                {
                    count += child.ExpandedNodeCount;
                }
            }

            return count;
        }
    }

    public int ChildrenCount
    {
        get
        {
            if (IsBaseType)
            {
                return 0;
            }

            return GetItemNodes().Count + GetComponentNodes().Count + GetFieldNodes().Count +
                   GetPropertyNodes().Count;
        }
    }

    public bool hasChildren
    {
        get
        {
            if (IsBaseType)
            {
                return false;
            }

            return ChildrenCount > 0;
        }
    }

    public string Name { get; protected set; }
    public abstract string ValueText { get; }
    public abstract Type InstType { get; }
    public abstract bool IsBaseType { get; }
    public abstract bool IsEnumerable { get; }
    public abstract bool IsException { get; }
    public abstract bool IsGameObject { get; }
    public abstract bool IsNull { get; }
    public abstract int? InstanceID { get; }

    public static IEnumerable<FieldInfo> GetFields(Type type)
    {
        var names = new HashSet<string>();
        foreach (var field in (Nullable.GetUnderlyingType(type) ?? type).GetFields(ALL_FLAGS))
        {
            if (!field.IsStatic &&
                !field.IsDefined(typeof(CompilerGeneratedAttribute), false) && // ignore backing field
                names.Add(field.Name))
            {
                yield return field;
            }
        }
    }

    public static IEnumerable<PropertyInfo> GetProperties(Type type)
    {
        var names = new HashSet<string>();
        foreach (var property in (Nullable.GetUnderlyingType(type) ?? type).GetProperties(ALL_FLAGS))
        {
            if (property.GetMethod != null &&
                !property.GetMethod.IsStatic &&
                property.GetMethod.GetParameters().Length == 0 &&
                names.Add(property.Name))
            {
                yield return property;
            }
        }
    }

    public abstract IReadOnlyCollection<Node> GetItemNodes();
    public abstract IReadOnlyCollection<Node> GetComponentNodes();
    public abstract IReadOnlyCollection<Node> GetPropertyNodes();
    public abstract IReadOnlyCollection<Node> GetFieldNodes();
    public abstract Node GetParent();

    private void AppendPathFromRoot(StringBuilder sb)
    {
        var parent = GetParent();
        if (parent != null)
        {
            parent.AppendPathFromRoot(sb);
            sb.Append('.');
        }

        sb.Append(Name);
    }

    public string GetPath()
    {
        var sb = new StringBuilder();
        AppendPathFromRoot(sb);
        return sb.ToString();
    }

    public abstract void SetDirty();
    public abstract bool IsDirty();
    internal abstract void SetValue(object value);
    protected abstract void UpdateValue();
}

internal abstract class GenericNode<TNode> : Node
{
    private bool _componentIsDirty;

    private List<Node> _componentNodes;
    private bool _fieldIsDirty;
    private List<Node> _fieldNodes;
    private Type _instType;
    private bool? _isBaseType;
    private bool? _isEnumerable;
    private bool? _isGameObject;
    private bool _itemIsDirty;
    private List<Node> _itemNodes;
    private bool _propertyIsDirty;
    private List<Node> _propertyNodes;

    private TNode _value;
    private bool _valueIsDirty = true;

    protected GenericNode(NodeType nodeType) : base(typeof(TNode), nodeType)
    {
        if (Type.IsValueType && !IsNullable)
        {
            _instType = Type;
        }
    }

    public TNode Value
    {
        get
        {
            UpdateValue();
            return _value;
        }
        protected set
        {
            if (!value?.Equals(_value) ?? _value != null)
            {
                _value = value;
                if (!Type.IsValueType || IsNullable)
                {
                    var oldType = _instType;
                    _instType = value?.GetType();
                    if (_instType != oldType)
                    {
                        _isBaseType = null;
                        _isEnumerable = null;
                        _isGameObject = null;
                        _fieldIsDirty = true;
                        _propertyIsDirty = true;
                    }
                }
            }
        }
    }

    public override string ValueText => IsException ? "<exception>" : IsNull ? "<null>" : Value.ToString();

    public override Type InstType
    {
        get
        {
            UpdateValue();
            return _instType;
        }
    }

    public override bool IsBaseType
    {
        get
        {
            UpdateValue();
            return _isBaseType ??
                   (_isBaseType =
                       BASE_TYPES.Contains(Nullable.GetUnderlyingType(InstType ?? Type) ?? InstType ?? Type)).Value;
        }
    }

    public override bool IsEnumerable
    {
        get
        {
            UpdateValue();
            return _isEnumerable ??
                   (_isEnumerable = (InstType ?? Type).GetInterfaces().Contains(typeof(IEnumerable))).Value;
        }
    }

    public override bool IsGameObject
    {
        get
        {
            UpdateValue();
            return _isGameObject ?? (_isGameObject = typeof(GameObject).IsAssignableFrom(InstType ?? Type)).Value;
        }
    }

    public override int? InstanceID
    {
        get
        {
            int? result = null;
            if (Value is Object unityObject)
            {
                result = unityObject.GetInstanceID();
            }

            if (Value is object obj)
            {
                return obj.GetHashCode();
            }

            return result;
        }
    }

    public override bool IsNull => Value == null || (Value is Object unityObject && !unityObject);

    public override IReadOnlyCollection<Node> GetComponentNodes()
    {
        UpdateComponentNodes();
        return _componentNodes.AsReadOnly();
    }

    public override IReadOnlyCollection<Node> GetItemNodes()
    {
        UpdateItemNodes();
        return _itemNodes.AsReadOnly();
    }

    public override IReadOnlyCollection<Node> GetFieldNodes()
    {
        UpdateFieldNodes();
        return _fieldNodes.AsReadOnly();
    }

    public override IReadOnlyCollection<Node> GetPropertyNodes()
    {
        UpdatePropertyNodes();
        return _propertyNodes.AsReadOnly();
    }

    public override Node GetParent()
    {
        return null;
    }

    public override void SetDirty()
    {
        _valueIsDirty = true;
    }

    public override bool IsDirty()
    {
        return _valueIsDirty;
    }

    private static Node FindOrCreateChildForValue(Type type, params object[] childArgs)
    {
        return Activator.CreateInstance(type, ALL_FLAGS, null, childArgs, null) as Node;
    }

    private void UpdateComponentNodes()
    {
        UpdateValue();
        if (!_componentIsDirty && _componentNodes != null)
        {
            return;
        }

        _componentIsDirty = false;

        _componentNodes ??= new List<Node>();
        _componentNodes.Clear();

        if (IsException || IsNull || !IsGameObject)
        {
            return;
        }

        var nodeType = typeof(ComponentNode);
        var i = 0;

        if (Value is GameObject gameObject)
        {
            foreach (var item in gameObject.GetComponents<Component>())
            {
                _componentNodes.Add(FindOrCreateChildForValue(nodeType, this, "<component_" + i + ">", item));
                i++;
            }
        }
    }

    private void UpdateItemNodes()
    {
        UpdateValue();

        if (!_itemIsDirty && _itemNodes != null)
        {
            return;
        }

        _itemIsDirty = false;

        _itemNodes ??= new List<Node>();
        _itemNodes.Clear();

        if (IsException || IsNull || !IsEnumerable)
        {
            return;
        }

        var itemTypes = InstType.GetInterfaces()
            .Where(item => item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(item => item.GetGenericArguments()[0]);
        var itemType = itemTypes.Count() == 1 ? itemTypes.First() : typeof(object);
        var nodeType = typeof(ItemNode<>).MakeGenericType(itemType);
        var i = 0;
        foreach (var item in Value as IEnumerable)
        {
            _itemNodes.Add(FindOrCreateChildForValue(nodeType, this, "<item_" + i + ">", item));
            i++;
        }
    }

    private void UpdateFieldNodes()
    {
        UpdateValue();

        if (!_fieldIsDirty && _fieldNodes != null)
        {
            return;
        }

        _fieldIsDirty = false;

        _fieldNodes ??= new List<Node>();
        _fieldNodes.Clear();

        if (IsException || IsNull)
        {
            return;
        }

        var nodeType = InstType.IsValueType
            ? !IsNullable ? typeof(FieldOfStructNode<,,>) : typeof(FieldOfNullableNode<,,>)
            : typeof(FieldOfClassNode<,,>);

        _fieldNodes = GetFields(InstType).Select(child =>
                FindOrCreateChildForValue(nodeType.MakeGenericType(Type, InstType, child.FieldType), this,
                    child.Name))
            .ToList();
        _fieldNodes.Sort((x, y) => x.Name.CompareTo(y.Name));
    }

    private void UpdatePropertyNodes()
    {
        UpdateValue();

        if (!_propertyIsDirty && _propertyNodes != null)
        {
            return;
        }

        _propertyIsDirty = false;

        _propertyNodes ??= new List<Node>();
        _propertyNodes.Clear();

        if (IsException || IsNull)
        {
            return;
        }

        var nodeType = InstType.IsValueType
            ? !IsNullable ? typeof(PropertyOfStructNode<,,>) : typeof(PropertyOfNullableNode<,,>)
            : typeof(PropertyOfClassNode<,,>);

        _propertyNodes = GetProperties(InstType).Select(child =>
            FindOrCreateChildForValue(nodeType.MakeGenericType(Type, InstType, child.PropertyType), this,
                child.Name)).ToList();

        _propertyNodes.Sort((x, y) => x.Name.CompareTo(y.Name));
    }

    protected override void UpdateValue()
    {
        if (_valueIsDirty)
        {
            _valueIsDirty = false;

            _componentIsDirty = true;
            _itemIsDirty = true;

            if (_fieldNodes != null)
            {
                foreach (var child in _fieldNodes)
                {
                    child.SetDirty();
                }
            }

            if (_propertyNodes != null)
            {
                foreach (var child in _propertyNodes)
                {
                    child.SetDirty();
                }
            }

            UpdateValueImpl();
        }
    }

    protected abstract void UpdateValueImpl();
}

internal abstract class PassiveNode<TNode> : GenericNode<TNode>
{
    protected PassiveNode(string name, TNode value, NodeType nodeType) : base(nodeType)
    {
        Name = name;
        Value = value;
    }

    public override bool IsException => false;

    internal override void SetValue(object value)
    {
        SetDirty();
        Value = (TNode)value;
    }

    internal void SetValue(TNode value)
    {
        SetDirty();
        Value = value;
    }

    protected override void UpdateValueImpl() { }
}

internal class RootNode<TNode> : PassiveNode<TNode>
{
    public RootNode(string name, TNode value) : base(name, value, NodeType.Root) { }
}

internal class ComponentNode : PassiveNode<Component>
{
    protected readonly WeakReference<Node> _parentNode;

    protected ComponentNode(Node parentNode, string name, Component value) : base(name, value, NodeType.Component)
    {
        _parentNode = new WeakReference<Node>(parentNode);
    }

    public override Node GetParent()
    {
        if (_parentNode.TryGetTarget(out var parent))
        {
            return parent;
        }

        return null;
    }
}

internal class ItemNode<TNode> : PassiveNode<TNode>
{
    protected readonly WeakReference<Node> _parentNode;

    protected ItemNode(Node parentNode, string name, TNode value) : base(name, value, NodeType.Item)
    {
        _parentNode = new WeakReference<Node>(parentNode);
    }

    public override Node GetParent()
    {
        if (_parentNode.TryGetTarget(out var parent))
        {
            return parent;
        }

        return null;
    }
}

internal abstract class ChildNode<TParent, TNode> : GenericNode<TNode>
{
    protected readonly WeakReference<GenericNode<TParent>> _parentNode;
    protected bool _isException;

    protected ChildNode(GenericNode<TParent> parentNode, string name, NodeType nodeType) : base(nodeType)
    {
        _parentNode = new WeakReference<GenericNode<TParent>>(parentNode);
        Name = name;
    }

    public override bool IsException
    {
        get
        {
            UpdateValue();
            return _isException;
        }
    }

    internal override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override Node GetParent()
    {
        if (_parentNode.TryGetTarget(out var parent))
        {
            return parent;
        }

        return null;
    }
}

internal abstract class ChildOfStructNode<TParent, TParentInst, TNode> : ChildNode<TParent, TNode>
    where TParentInst : struct
{
    private readonly Func<TParent, TParentInst> _forceCast = UnsafeForceCast.GetDelegate<TParent, TParentInst>();

    protected ChildOfStructNode(GenericNode<TParent> parentNode, string name, NodeType nodeType) : base(parentNode,
        name, nodeType)
    {
    }

    protected bool TryGetParentValue(out TParentInst value)
    {
        if (_parentNode.TryGetTarget(out var parent) && parent.InstType == typeof(TParentInst))
        {
            value = _forceCast(parent.Value);
            return true;
        }

        value = default;
        return false;
    }
}

internal abstract class ChildOfNullableNode<TParent, TUnderlying, TNode> : ChildNode<TParent, TNode>
    where TUnderlying : struct
{
    private readonly Func<TParent, TUnderlying?> _forceCast = UnsafeForceCast.GetDelegate<TParent, TUnderlying?>();

    protected ChildOfNullableNode(GenericNode<TParent> parentNode, string name, NodeType nodeType) : base(
        parentNode, name, nodeType)
    {
    }

    protected bool TryGetParentValue(out TUnderlying value)
    {
        if (_parentNode.TryGetTarget(out var parent))
        {
            var parentValue = _forceCast(parent.Value);
            if (parentValue.HasValue)
            {
                value = parentValue.Value;
                return true;
            }
        }

        value = default;
        return false;
    }
}

internal abstract class ChildOfClassNode<TParent, TParentInst, TNode> : ChildNode<TParent, TNode>
    where TParentInst : class
{
    protected ChildOfClassNode(GenericNode<TParent> parentNode, string name, NodeType nodeType) : base(parentNode,
        name, nodeType)
    {
    }

    protected bool TryGetParentValue(out TParentInst value)
    {
        if (_parentNode.TryGetTarget(out var parent) && (value = parent.Value as TParentInst) != null)
        {
            return true;
        }

        value = null;
        return false;
    }
}

internal class FieldOfStructNode<TParent, TParentInst, TNode> : ChildOfStructNode<TParent, TParentInst, TNode>
    where TParentInst : struct
{
    protected FieldOfStructNode(GenericNode<TParent> parentNode, string name) : base(parentNode, name,
        NodeType.Field)
    {
    }

    protected override void UpdateValueImpl()
    {
        if (TryGetParentValue(out var parentValue))
        {
            _isException = false;
            Value = parentValue.GetFieldValue<TParentInst, TNode>(Name);
        }
        else
        {
            _isException = true;
            Value = default;
        }
    }
}

internal class PropertyOfStructNode<TParent, TParentInst, TNode> : ChildOfStructNode<TParent, TParentInst, TNode>
    where TParentInst : struct
{
    protected PropertyOfStructNode(GenericNode<TParent> parentNode, string name) : base(parentNode, name,
        NodeType.Property)
    {
    }

    protected override void UpdateValueImpl()
    {
        if (TryGetParentValue(out var parentValue))
        {
            try
            {
                _isException = false;
                Value = parentValue.GetPropertyValue<TParentInst, TNode>(Name);
            }
            catch
            {
                _isException = true;
                Value = default;
            }
        }
        else
        {
            _isException = true;
            Value = default;
        }
    }
}

internal class FieldOfNullableNode<TParent, TUnderlying, TNode> : ChildOfNullableNode<TParent, TUnderlying, TNode>
    where TUnderlying : struct
{
    protected FieldOfNullableNode(GenericNode<TParent> parentNode, string name) : base(parentNode, name,
        NodeType.Field)
    {
    }

    protected override void UpdateValueImpl()
    {
        if (TryGetParentValue(out var parentValue))
        {
            _isException = false;
            Value = parentValue.GetFieldValue<TUnderlying, TNode>(Name);
        }
        else
        {
            _isException = true;
            Value = default;
        }
    }
}

internal class
    PropertyOfNullableNode<TParent, TUnderlying, TNode> : ChildOfNullableNode<TParent, TUnderlying, TNode>
    where TUnderlying : struct
{
    protected PropertyOfNullableNode(GenericNode<TParent> parentNode, string name) : base(parentNode, name,
        NodeType.Property)
    {
    }

    protected override void UpdateValueImpl()
    {
        if (TryGetParentValue(out var parentValue))
        {
            try
            {
                _isException = false;
                Value = parentValue.GetPropertyValue<TUnderlying, TNode>(Name);
            }
            catch
            {
                _isException = true;
                Value = default;
            }
        }
        else
        {
            _isException = true;
            Value = default;
        }
    }
}

internal class FieldOfClassNode<TParent, TParentInst, TNode> : ChildOfClassNode<TParent, TParentInst, TNode>
    where TParentInst : class
{
    protected FieldOfClassNode(GenericNode<TParent> parentNode, string name) : base(parentNode, name,
        NodeType.Field)
    {
    }

    protected override void UpdateValueImpl()
    {
        if (TryGetParentValue(out var parentValue))
        {
            _isException = false;
            Value = parentValue.GetFieldValue<TParentInst, TNode>(Name);
        }
        else
        {
            _isException = true;
            Value = default;
        }
    }
}

internal class PropertyOfClassNode<TParent, TParentInst, TNode> : ChildOfClassNode<TParent, TParentInst, TNode>
    where TParentInst : class
{
    protected PropertyOfClassNode(GenericNode<TParent> parentNode, string name) : base(parentNode, name,
        NodeType.Property)
    {
    }

    protected override void UpdateValueImpl()
    {
        if (TryGetParentValue(out var parentValue))
        {
            try
            {
                _isException = false;
                Value = parentValue.GetPropertyValue<TParentInst, TNode>(Name);
            }
            catch
            {
                _isException = true;
                Value = default;
            }
        }
        else
        {
            _isException = true;
            Value = default;
        }
    }
}
