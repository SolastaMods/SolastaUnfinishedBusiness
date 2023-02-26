using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SolastaUnfinishedBusiness.Api.ModKit.Utility.Reflection;

public static partial class ReflectionCache
{
    private static readonly HashSet<Type> ActionAndFuncTypes = new()
    {
        typeof(Action),
        typeof(Action<>),
        typeof(Action<,>),
        typeof(Action<,,>),
        typeof(Action<,,,>),
        typeof(Action<,,,,>),
        typeof(Action<,,,,,>),
        typeof(Action<,,,,,,>),
        typeof(Action<,,,,,,,>),
        typeof(Action<,,,,,,,,>),
        typeof(Action<,,,,,,,,,>),
        typeof(Action<,,,,,,,,,,>),
        typeof(Action<,,,,,,,,,,,>),
        typeof(Action<,,,,,,,,,,,,>),
        typeof(Action<,,,,,,,,,,,,,>),
        typeof(Action<,,,,,,,,,,,,,,>),
        typeof(Action<,,,,,,,,,,,,,,,>),
        typeof(Func<>),
        typeof(Func<,>),
        typeof(Func<,,>),
        typeof(Func<,,,>),
        typeof(Func<,,,,>),
        typeof(Func<,,,,,>),
        typeof(Func<,,,,,,>),
        typeof(Func<,,,,,,,>),
        typeof(Func<,,,,,,,,>),
        typeof(Func<,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,,,,,,,>)
    };

    private static readonly TripleDictionary<Type, string, Type, WeakReference> MethodCache = new();

    private static CachedMethod<TMethod> GetMethodCache<T, TMethod>(string name) where TMethod : Delegate
    {
        object cache = null;
        if (MethodCache.TryGetValue(typeof(T), name, typeof(TMethod), out var weakRef))
        {
            cache = weakRef.Target;
        }

        if (cache != null)
        {
            return cache as CachedMethod<TMethod>;
        }

        cache = new CachedMethodOfNonStatic<T, TMethod>(name);
        MethodCache[typeof(T), name, typeof(TMethod)] = new WeakReference(cache);
        EnqueueCache(cache);

        return (CachedMethod<TMethod>)cache;
    }

    private static CachedMethod<TMethod> GetMethodCache<TMethod>(Type type, string name) where TMethod : Delegate
    {
        object cache = null;
        if (MethodCache.TryGetValue(type, name, typeof(TMethod), out var weakRef))
        {
            cache = weakRef.Target;
        }

        if (cache != null)
        {
            return cache as CachedMethod<TMethod>;
        }

        cache =
            IsStatic(type)
                ? Activator.CreateInstance(typeof(CachedMethodOfStatic<>).MakeGenericType(typeof(TMethod)), type,
                    name)
                : Activator.CreateInstance(
                    typeof(CachedMethodOfNonStatic<,>).MakeGenericType(type, typeof(TMethod)), name);
        MethodCache[type, name, typeof(TMethod)] = new WeakReference(cache);
        EnqueueCache(cache);

        return cache as CachedMethod<TMethod>;
    }

    public static MethodInfo GetMethodInfo<T, TMethod>(string name) where TMethod : Delegate
    {
        return GetMethodCache<T, TMethod>(name).Info;
    }

    public static MethodInfo GetMethodInfo<TMethod>(Type type, string name) where TMethod : Delegate
    {
        return GetMethodCache<TMethod>(type, name).Info;
    }

    public static TMethod GetMethodDel<T, TMethod>(string name) where TMethod : Delegate
    {
        return GetMethodCache<T, TMethod>(name).Del;
    }

    public static TMethod GetMethodDel<TMethod>(Type type, string name) where TMethod : Delegate
    {
        return GetMethodCache<TMethod>(type, name).Del;
    }

    public static TMethod GetMethod<T, TMethod>(string name) where TMethod : Delegate
    {
        return GetMethodCache<T, TMethod>(name).Del;
    }

    public static TMethod GetMethod<TMethod>(Type type, string name) where TMethod : Delegate
    {
        return GetMethodCache<TMethod>(type, name).Del;
    }

    private abstract class CachedMethod<TMethod> where TMethod : Delegate
    {
        public readonly MethodInfo Info;
        private TMethod _delegate;

        protected CachedMethod(Type type, string name, bool hasThis)
        {
            var delType = typeof(TMethod);
            var delSign = delType.GetMethod("Invoke", AllFlags);
            var delParams = delSign.GetParameters();

            if (hasThis)
            {
                if (delParams.Length == 0)
                {
                    throw new InvalidOperationException();
                }

                if (type.IsValueType)
                {
                    if (!delParams[0].ParameterType.IsByRef || delParams[0].ParameterType.GetElementType() != type)
                    {
                        throw new InvalidOperationException();
                    }
                }
                else if (delParams[0].ParameterType.IsByRef || delParams[0].ParameterType != type)
                {
                    throw new InvalidOperationException();
                }
            }

            IEnumerable<MethodInfo> methods = type.GetMethods(AllFlags);
            if (delType.IsGenericType && !ActionAndFuncTypes.Contains(delType.GetGenericTypeDefinition()))
            {
                if (hasThis)
                {
                    delParams = delParams.Skip(1).ToArray();
                }

                var delGenericArgs = delType.GetGenericArguments();
                methods = methods.Where(m =>
                    m.IsGenericMethod &&
                    m.Name == name &&
                    m.ReturnType == delSign.ReturnType &&
                    m.GetGenericArguments().Length == delGenericArgs.Length &&
                    CheckParamsOfGenericMethod(m.GetParameters(), delParams, delGenericArgs));
                if (methods.Count() > 1)
                {
                    throw new AmbiguousMatchException();
                }

                Info = methods.FirstOrDefault()?.MakeGenericMethod(delGenericArgs);
            }
            else
            {
                var delParamTypes = hasThis
                    ? delParams.Select(p => p.ParameterType).Skip(1)
                    : delParams.Select(p => p.ParameterType);
                methods = methods.Where(m =>
                    !m.IsGenericMethod &&
                    m.Name == name &&
                    m.ReturnType == delSign.ReturnType &&
                    m.GetParameters().Select(p => p.ParameterType).SequenceEqual(delParamTypes));
                if (methods.Count() > 1)
                {
                    throw new AmbiguousMatchException();
                }

                Info = methods.FirstOrDefault();
            }

            if (Info == null)
            {
                throw new InvalidOperationException();
            }
        }

        public TMethod Del
            => _delegate ??= CreateDelegate();

        private static bool CheckParamsOfGenericMethod(
            IReadOnlyList<ParameterInfo> @params,
            IReadOnlyList<ParameterInfo> delParams,
            IReadOnlyList<Type> delGenericArgs)
        {
            if (@params.Count != delParams.Count)
            {
                return false;
            }

            for (var i = 0; i < @params.Count; i++)
            {
                if (!@params[i].ParameterType.IsGenericParameter)
                {
                    if (@params[i].ParameterType != delParams[i].ParameterType)
                    {
                        return false;
                    }
                }
                else
                {
                    if (delGenericArgs[@params[i].ParameterType.GenericParameterPosition] != delParams[i].ParameterType)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected abstract TMethod CreateDelegate();
    }

    private class CachedMethodOfStatic<TMethod> : CachedMethod<TMethod> where TMethod : Delegate
    {
        public CachedMethodOfStatic(Type type, string name) : base(type, name, false)
        {
            //if (!IsStatic(type))
            //    throw new InvalidOperationException();
        }

        protected override TMethod CreateDelegate()
        {
            var parameters = Info.GetParameters();
            DynamicMethod method = new(
                Info.Name,
                Info.ReturnType,
                parameters.Select(item => item.ParameterType).ToArray(),
                typeof(CachedMethodOfStatic<TMethod>),
                true);

            var il = method.GetILGenerator();
            for (var i = 0; i < parameters.Length; i++)
            {
                il.Emit(OpCodes.Ldarg, i);
            }

            il.Emit(OpCodes.Call, Info);
            il.Emit(OpCodes.Ret);

            return method.CreateDelegate(typeof(TMethod)) as TMethod;
        }
    }

    private class CachedMethodOfNonStatic<T, TMethod> : CachedMethod<TMethod> where TMethod : Delegate
    {
        public CachedMethodOfNonStatic(string name) : base(typeof(T), name, true)
        {
        }

        protected override TMethod CreateDelegate()
        {
            var type = typeof(T);
            var parameters = Info.GetParameters();
            var method = new DynamicMethod(
                Info.Name,
                Info.ReturnType,
                new[] { type.IsValueType ? type.MakeByRefType() : type }
                    .Concat(parameters.Select(item => item.ParameterType)).ToArray(),
                typeof(CachedMethodOfNonStatic<T, TMethod>),
                true);
            method.DefineParameter(1, ParameterAttributes.In, "instance");

            var il = method.GetILGenerator();

            if (Info.IsStatic)
            {
                for (var i = 1; i <= parameters.Length; i++)
                {
                    il.Emit(OpCodes.Ldarg, i);
                }

                il.Emit(OpCodes.Call, Info);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                for (var i = 1; i <= parameters.Length; i++)
                {
                    il.Emit(OpCodes.Ldarg, i);
                }

                il.Emit(Info.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, Info);
            }

            il.Emit(OpCodes.Ret);

            return method.CreateDelegate(typeof(TMethod)) as TMethod;
        }
    }
}
