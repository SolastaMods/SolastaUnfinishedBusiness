//
// TO DO:
// 1. ref ReturnType
// 2. Nullable<T> support
//

using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace SolastaCommunityExpansion.DataViewer;

public static partial class ReflectionCache
{
    private const BindingFlags AllFlags =
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
        BindingFlags.NonPublic /*| BindingFlags.FlattenHierarchy*/;

    private static readonly Queue Cache = new();

    public static int Count => Cache.Count;

    private static int SizeLimit { get; } = 1000;

    //public static void Clear() {
    //    _fieldCache.Clear();
    //    _propertieCache.Clear();
    //    _methodCache.Clear();
    //    _cache.Clear();
    //}

    private static void EnqueueCache(object obj)
    {
        while (Cache.Count >= SizeLimit && Cache.Count > 0)
        {
            Cache.Dequeue();
        }

        Cache.Enqueue(obj);
    }

    private static bool IsStatic(Type type)
    {
        return type.IsAbstract && type.IsSealed;
    }

    private static TypeBuilder RequestTypeBuilder()
    {
        AssemblyName asmName = new(nameof(ReflectionCache) + "." + Guid.NewGuid());
        var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);
        var moduleBuilder = asmBuilder.DefineDynamicModule("<Module>");
        return moduleBuilder.DefineType("<Type>");
    }
}
