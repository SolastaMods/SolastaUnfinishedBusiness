using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.DataViewer;

internal static partial class ReflectionCache
{
    private static readonly DoubleDictionary<Type, string, WeakReference> PropertiesCache = new();

    private static CachedProperty<TProperty> GetPropertyCache<T, TProperty>(string name)
    {
        object cache = null;

        if (PropertiesCache.TryGetValue(typeof(T), name, out var weakRef))
        {
            cache = weakRef.Target;
        }

        if (cache != null)
        {
            return cache as CachedProperty<TProperty>;
        }

        if (typeof(T).IsValueType)
        {
            cache = new CachedPropertyOfStruct<T, TProperty>(name);
        }
        else
        {
            cache = new CachedPropertyOfClass<T, TProperty>(name);
        }

        PropertiesCache[typeof(T), name] = new WeakReference(cache);
        EnqueueCache(cache);

        return (CachedProperty<TProperty>)cache;
    }

    internal static TProperty GetPropertyValue<T, TProperty>(this ref T instance, string name) where T : struct
    {
        return ((CachedPropertyOfStruct<T, TProperty>)GetPropertyCache<T, TProperty>(name)).Get(ref instance);
    }

    internal static TProperty GetPropertyValue<T, TProperty>(this T instance, string name) where T : class
    {
        return ((CachedPropertyOfClass<T, TProperty>)GetPropertyCache<T, TProperty>(name)).Get(instance);
    }

    private abstract class CachedProperty<TProperty>
    {
        internal readonly PropertyInfo Info;

        protected CachedProperty(Type type, string name)
        {
            Info = type.GetProperties(AllFlags).FirstOrDefault(item => item.Name == name);

            if (Info == null || Info.PropertyType != typeof(TProperty))
            {
                throw new InvalidOperationException();
            }

            if (Info.DeclaringType != type)
            {
                Info = Info.DeclaringType?.GetProperties(AllFlags).FirstOrDefault(item => item.Name == name);
            }
        }

        // for static property
        [UsedImplicitly]
        internal abstract TProperty Get();

        // for static property
        [UsedImplicitly]
        internal abstract void Set(TProperty value);

        protected Delegate CreateGetter(Type delType, MethodInfo getter, bool isInstByRef)
        {
            if (!getter.IsStatic)
            {
                return Delegate.CreateDelegate(delType, getter);
            }

            var method = new DynamicMethod(
                "get_" + Info.Name,
                Info.PropertyType,
                [isInstByRef ? Info.DeclaringType?.MakeByRefType() : Info.DeclaringType],
                typeof(CachedProperty<TProperty>),
                true);

            method.DefineParameter(1, ParameterAttributes.In, "instance");

            var il = method.GetILGenerator();

            il.Emit(OpCodes.Call, getter);
            il.Emit(OpCodes.Ret);

            return method.CreateDelegate(delType);
        }

        protected Delegate CreateSetter(Type delType, MethodInfo setter, bool isInstByRef)
        {
            if (!setter.IsStatic)
            {
                return Delegate.CreateDelegate(delType, setter);
            }

            var method = new DynamicMethod(
                "set_" + Info.Name,
                null,
                [isInstByRef ? Info.DeclaringType?.MakeByRefType() : Info.DeclaringType, Info.PropertyType],
                typeof(CachedProperty<TProperty>),
                true);

            method.DefineParameter(1, ParameterAttributes.In, "instance");
            method.DefineParameter(2, ParameterAttributes.In, "value");

            var il = method.GetILGenerator();

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, setter);
            il.Emit(OpCodes.Ret);

            return method.CreateDelegate(delType);
        }
    }

    private class CachedPropertyOfStruct<T, TProperty> : CachedProperty<TProperty>
    {
        private T _dummy;
        private Getter _getter;
        private Setter _setter;

        internal CachedPropertyOfStruct(string name) : base(typeof(T), name) { }

        internal override TProperty Get()
        {
            return (_getter ??= (Getter)CreateGetter(typeof(Getter), Info.GetMethod, true))(ref _dummy);
        }

        internal TProperty Get(ref T instance)
        {
            return (_getter ??= (Getter)CreateGetter(typeof(Getter), Info.GetMethod, true))(ref instance);
        }

        internal override void Set(TProperty value)
        {
            (_setter ??= (Setter)CreateSetter(typeof(Setter), Info.SetMethod, true))(ref _dummy, value);
        }

        private delegate TProperty Getter(ref T instance);

        private delegate void Setter(ref T instance, TProperty value);
    }

    private class CachedPropertyOfClass<T, TProperty> : CachedProperty<TProperty>
    {
        private readonly T _dummy = default;
        private Getter _getter;
        private Setter _setter;

        internal CachedPropertyOfClass(string name) : base(typeof(T), name) { }

        internal override TProperty Get()
        {
            return (_getter ??= (Getter)CreateGetter(typeof(Getter), Info.GetMethod, false))(_dummy);
        }

        internal TProperty Get(T instance)
        {
            return (_getter ??= (Getter)CreateGetter(typeof(Getter), Info.GetMethod, false))(instance);
        }

        internal override void Set(TProperty value)
        {
            (_setter ??= (Setter)CreateSetter(typeof(Setter), Info.SetMethod, false))(_dummy, value);
        }

        private delegate TProperty Getter(T instance);

        private delegate void Setter(T instance, TProperty value);
    }
}
