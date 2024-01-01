using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using SolastaUnfinishedBusiness.Api.ModKit;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.DataViewer;

internal enum NodeType
{
    Root,
    Component,
    Item,
    Field,
    Property
}

// This structure has evolved into a reflection graph or DAG but for the sake of continuity we will stick with calling it a tree
internal class ReflectionTree : ReflectionTree<object>
{
    internal ReflectionTree(object root) : base(root) { }
}

internal class ReflectionTree<TRoot>
{
    private RootNode<TRoot> _root;

    internal ReflectionTree(TRoot root)
    {
        SetRoot(root);
    }

    // internal TRoot Root => _root.Value;

    internal Node RootNode => _root;

    internal void SetRoot(TRoot root)
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

internal abstract class Node
{
    protected const BindingFlags AllFlags =
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    // the graph will not show any child nodes of following types
    internal static readonly HashSet<Type> BaseTypes =
    [
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
    ];

    internal readonly bool IsNullable;

    internal readonly NodeType NodeType;
    internal readonly Type Type;

    protected Node(Type type, NodeType nodeType)
    {
        NodeType = nodeType;
        Type = type;
        IsNullable = Type.IsGenericType && !Type.IsGenericTypeDefinition &&
                     Type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    [Obsolete("TODO - move this into a proper view model", false)]
    internal ToggleState Expanded { get; set; }

    [Obsolete("TODO - move this into a proper view model", false)]
    internal bool Matches { get; set; }

    internal string NodeTypePrefix => NodeType switch
    {
        NodeType.Component => "c",
        NodeType.Item => "i",
        NodeType.Field => "f",
        NodeType.Property => "p",
        _ => string.Empty
    };

#if false
    internal int ExpandedNodeCount
    {
        get
        {
            var count = 1;

            if (IsBaseType)
            {
                return count;
            }

#pragma warning disable CS0618 // Type or member is obsolete
            if (Expanded != ToggleState.On)
            {
                return count;
            }
#pragma warning restore CS0618 // Type or member is obsolete

            count += GetItemNodes().Sum(child => child.ExpandedNodeCount);

            count += GetComponentNodes().Sum(child => child.ExpandedNodeCount);

            count += GetPropertyNodes().Sum(child => child.ExpandedNodeCount);

            count += GetFieldNodes().Sum(child => child.ExpandedNodeCount);

            return count;
        }
    }
#endif

    internal int ChildrenCount
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

    internal bool HasChildren
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
    internal abstract string ValueText { get; }
    internal abstract Type InstType { get; }
    internal abstract bool IsBaseType { get; }
    internal abstract bool IsEnumerable { get; }
    internal abstract bool IsException { get; }
    internal abstract bool IsGameObject { get; }
    internal abstract bool IsNull { get; }
    internal abstract int? InstanceID { get; }

    internal static IEnumerable<FieldInfo> GetFields(Type type)
    {
        var names = new HashSet<string>();
        foreach (var field in (Nullable.GetUnderlyingType(type) ?? type).GetFields(AllFlags))
        {
            if (!field.IsStatic &&
                !field.IsDefined(typeof(CompilerGeneratedAttribute), false) && // ignore backing field
                names.Add(field.Name))
            {
                yield return field;
            }
        }
    }

    internal static IEnumerable<PropertyInfo> GetProperties(Type type)
    {
        var names = new HashSet<string>();
        foreach (var property in (Nullable.GetUnderlyingType(type) ?? type).GetProperties(AllFlags))
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

    internal abstract IReadOnlyCollection<Node> GetItemNodes();
    internal abstract IReadOnlyCollection<Node> GetComponentNodes();
    internal abstract IReadOnlyCollection<Node> GetPropertyNodes();
    internal abstract IReadOnlyCollection<Node> GetFieldNodes();
    internal abstract Node GetParent();

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

    internal string GetPath()
    {
        var sb = new StringBuilder();
        AppendPathFromRoot(sb);
        return sb.ToString();
    }

    internal abstract void SetDirty();
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
            if (!(!value?.Equals(_value) ?? _value != null))
            {
                return;
            }

            _value = value;

            if (Type.IsValueType && !IsNullable)
            {
                return;
            }

            var oldType = _instType;

            _instType = value?.GetType();

            if (_instType == oldType)
            {
                return;
            }

            _isBaseType = null;
            _isEnumerable = null;
            _isGameObject = null;
            _fieldIsDirty = true;
            _propertyIsDirty = true;
        }
    }

    internal override string ValueText => IsException ? "<exception>" : IsNull ? "<null>" : Value.ToString();

    internal override Type InstType
    {
        get
        {
            UpdateValue();
            return _instType;
        }
    }

    internal override bool IsBaseType
    {
        get
        {
            UpdateValue();
            return _isBaseType ??
                   (_isBaseType =
                       BaseTypes.Contains(Nullable.GetUnderlyingType(InstType ?? Type) ?? InstType ?? Type)).Value;
        }
    }

    internal override bool IsEnumerable
    {
        get
        {
            UpdateValue();
            return _isEnumerable ??
                   (_isEnumerable = (InstType ?? Type).GetInterfaces().Contains(typeof(IEnumerable))).Value;
        }
    }

    internal override bool IsGameObject
    {
        get
        {
            UpdateValue();
            return _isGameObject ?? (_isGameObject = typeof(GameObject).IsAssignableFrom(InstType ?? Type)).Value;
        }
    }

    internal override int? InstanceID =>
        Value switch
        {
            Object unityObject => unityObject.GetInstanceID(),
            object obj => obj.GetHashCode(),
            _ => null
        };

    internal override bool IsNull => Value == null || (Value is Object unityObject && !unityObject);

    internal override IReadOnlyCollection<Node> GetComponentNodes()
    {
        UpdateComponentNodes();
        return _componentNodes.AsReadOnly();
    }

    internal override IReadOnlyCollection<Node> GetItemNodes()
    {
        UpdateItemNodes();
        return _itemNodes.AsReadOnly();
    }

    internal override IReadOnlyCollection<Node> GetFieldNodes()
    {
        UpdateFieldNodes();
        return _fieldNodes.AsReadOnly();
    }

    internal override IReadOnlyCollection<Node> GetPropertyNodes()
    {
        UpdatePropertyNodes();
        return _propertyNodes.AsReadOnly();
    }

    internal override Node GetParent()
    {
        return null;
    }

    internal override void SetDirty()
    {
        _valueIsDirty = true;
    }

    private static Node FindOrCreateChildForValue(Type type, params object[] childArgs)
    {
        return Activator.CreateInstance(type, AllFlags, null, childArgs, null) as Node;
    }

    private void UpdateComponentNodes()
    {
        UpdateValue();
        if (!_componentIsDirty && _componentNodes != null)
        {
            return;
        }

        _componentIsDirty = false;

        _componentNodes ??= [];
        _componentNodes.Clear();

        if (IsException || IsNull || !IsGameObject)
        {
            return;
        }

        var nodeType = typeof(ComponentNode);
        var i = 0;

        if (Value is not GameObject gameObject)
        {
            return;
        }

        foreach (var item in gameObject.GetComponents<Component>())
        {
            _componentNodes.Add(FindOrCreateChildForValue(nodeType, this, "<component_" + i + ">", item));
            i++;
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

        _itemNodes ??= [];
        _itemNodes.Clear();

        if (IsException || IsNull || !IsEnumerable)
        {
            return;
        }

        var itemTypes = InstType.GetInterfaces()
            .Where(item => item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(item => item.GetGenericArguments()[0]);
        var enumerable = itemTypes as Type[] ?? itemTypes.ToArray();
        var itemType = enumerable.Length == 1 ? enumerable.First() : typeof(object);
        var nodeType = typeof(ItemNode<>).MakeGenericType(itemType);
        var i = 0;
        foreach (var item in (IEnumerable)Value)
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

        _fieldNodes ??= [];
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
        _fieldNodes.Sort((x, y) => String.Compare(x.Name, y.Name, StringComparison.CurrentCultureIgnoreCase));
    }

    private void UpdatePropertyNodes()
    {
        UpdateValue();

        if (!_propertyIsDirty && _propertyNodes != null)
        {
            return;
        }

        _propertyIsDirty = false;

        _propertyNodes ??= [];
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

        _propertyNodes.Sort((x, y) => String.Compare(x.Name, y.Name, StringComparison.CurrentCultureIgnoreCase));
    }

    internal void UpdateValue()
    {
        if (!_valueIsDirty)
        {
            return;
        }

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

    protected abstract void UpdateValueImpl();
}

internal abstract class PassiveNode<TNode> : GenericNode<TNode>
{
    protected PassiveNode(string name, TNode value, NodeType nodeType) : base(nodeType)
    {
        Name = name;
        Value = value;
    }

    internal override bool IsException => false;

    internal void SetValue(TNode value)
    {
        SetDirty();
        Value = value;
    }

    protected override void UpdateValueImpl() { }
}

internal class RootNode<TNode> : PassiveNode<TNode>
{
    internal RootNode(string name, TNode value) : base(name, value, NodeType.Root) { }
}

internal class ComponentNode : PassiveNode<Component>
{
    private readonly WeakReference<Node> _parentNode;

    protected ComponentNode(Node parentNode, string name, Component value) : base(name, value, NodeType.Component)
    {
        _parentNode = new WeakReference<Node>(parentNode);
    }

    internal override Node GetParent()
    {
        return _parentNode.TryGetTarget(out var parent) ? parent : null;
    }
}

internal class ItemNode<TNode> : PassiveNode<TNode>
{
    private readonly WeakReference<Node> _parentNode;

    protected ItemNode(Node parentNode, string name, TNode value) : base(name, value, NodeType.Item)
    {
        _parentNode = new WeakReference<Node>(parentNode);
    }

    internal override Node GetParent()
    {
        return _parentNode.TryGetTarget(out var parent) ? parent : null;
    }
}

internal abstract class ChildNode<TParent, TNode> : GenericNode<TNode>
{
    protected readonly WeakReference<GenericNode<TParent>> ParentNode;

    // ReSharper disable once InconsistentNaming
    protected bool _isException;

    protected ChildNode(GenericNode<TParent> parentNode, string name, NodeType nodeType) : base(nodeType)
    {
        ParentNode = new WeakReference<GenericNode<TParent>>(parentNode);
        Name = name;
    }

    internal override bool IsException
    {
        get
        {
            UpdateValue();
            return _isException;
        }
    }

    internal override Node GetParent()
    {
        return ParentNode.TryGetTarget(out var parent) ? parent : null;
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
        if (ParentNode.TryGetTarget(out var parent) && parent.InstType == typeof(TParentInst))
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
        if (ParentNode.TryGetTarget(out var parent))
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
        if (ParentNode.TryGetTarget(out var parent) && (value = parent.Value as TParentInst) != null)
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
