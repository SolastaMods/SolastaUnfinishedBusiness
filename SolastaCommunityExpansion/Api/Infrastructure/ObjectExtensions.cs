using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace SolastaCommunityExpansion.Api.Infrastructure;

public static class ObjectExtensions
{
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
    private static readonly MethodInfo CloneMethod =
        typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

    private static bool IsPrimitive([NotNull] this Type type)
    {
        if (type == typeof(string))
        {
            return true;
        }

        return type.IsValueType && type.IsPrimitive;
    }

    private static object InternalCopy([CanBeNull] object originalObject, IDictionary<object, object> visited)
    {
        if (originalObject == null)
        {
            return null;
        }

        var typeToReflect = originalObject.GetType();

        if (IsPrimitive(typeToReflect))
        {
            return originalObject;
        }

        if (visited.ContainsKey(originalObject))
        {
            return visited[originalObject];
        }

        if (typeof(Delegate).IsAssignableFrom(typeToReflect))
        {
            return null;
        }

        var cloneObject = CloneMethod.Invoke(originalObject, null);
        if (typeToReflect.IsArray)
        {
            var arrayType = typeToReflect.GetElementType();

            if (arrayType != null && IsPrimitive(arrayType))
            {
                var clonedArray = (Array)cloneObject;
                clonedArray.ForEach((array, indices) =>
                    array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
            }
        }

        visited.Add(originalObject, cloneObject);
        CopyFields(originalObject, visited, cloneObject, typeToReflect);
        RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
        return cloneObject;
    }

    private static void RecursiveCopyBaseTypePrivateFields(object originalObject,
        IDictionary<object, object> visited, object cloneObject, [NotNull] Type typeToReflect)
    {
        if (typeToReflect.BaseType == null)
        {
            return;
        }

        RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType,
            BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
    }

    private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject,
        [NotNull] Type typeToReflect,
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                                    BindingFlags.FlattenHierarchy, [CanBeNull] Func<FieldInfo, bool> filter = null)
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
    {
        foreach (var fieldInfo in typeToReflect.GetFields(bindingFlags))
        {
            if (filter != null && !filter(fieldInfo))
            {
                continue;
            }

            if (IsPrimitive(fieldInfo.FieldType))
            {
                continue;
            }

            var originalFieldValue = fieldInfo.GetValue(originalObject);
            var clonedFieldValue = InternalCopy(originalFieldValue, visited);
            fieldInfo.SetValue(cloneObject, clonedFieldValue);
        }
    }

    private static object DeepCopy(this object originalObject)
    {
        return InternalCopy(originalObject, new Dictionary<object, object>(new ReferenceEqualityComparer()));
    }

    public static T DeepCopy<T>(this T original)
    {
        if (original is Object)
        {
            throw new ArgumentException(
                "The object being copied is a UnityEngine.Object. Use Object.Instantiate to copy Unity objects.",
                nameof(original));
        }

        return (T)DeepCopy((object)original);
    }
}

public sealed class ReferenceEqualityComparer : EqualityComparer<object>
{
    public override bool Equals(object x, object y)
    {
        return ReferenceEquals(x, y);
    }

    public override int GetHashCode(object obj)
    {
        return obj.GetHashCode();
    }
}
