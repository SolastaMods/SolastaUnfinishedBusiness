using System;
using System.Reflection.Emit;

namespace SolastaUnfinishedBusiness.DataViewer;

internal static class UnsafeForceCast
{
    private static readonly DoubleDictionary<Type, Type, WeakReference> Cache = new();

    internal static Func<TInput, TOutput> GetDelegate<TInput, TOutput>()
    {
        Func<TInput, TOutput> cache = null;

        if (Cache.TryGetValue(typeof(TInput), typeof(TOutput), out var weakRef))
        {
            cache = weakRef.Target as Func<TInput, TOutput>;
        }

        if (cache != null)
        {
            return cache;
        }

        cache = CreateDelegate<TInput, TOutput>();
        Cache[typeof(TInput), typeof(TOutput)] = new WeakReference(cache);

        return cache;
    }

    private static Func<TInput, TOutput> CreateDelegate<TInput, TOutput>()
    {
        var method = new DynamicMethod(
            "UnsafeForceCast",
            typeof(TOutput),
            [typeof(TInput)]);

        var il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);

        if (typeof(TInput) == typeof(object) && typeof(TOutput).IsValueType)
        {
            il.Emit(OpCodes.Unbox_Any, typeof(TOutput));
        }

        il.Emit(OpCodes.Ret);

        return method.CreateDelegate(typeof(Func<TInput, TOutput>)) as Func<TInput, TOutput>;
    }
}
