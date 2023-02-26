using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SolastaUnfinishedBusiness.Api.ModKit.Utility.Reflection;

public static partial class ReflectionCache
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

    private static CachedProperty<TProperty> GetPropertyCache<TProperty>(Type type, string name)
    {
        object cache = null;
        if (PropertiesCache.TryGetValue(type, name, out var weakRef))
        {
            cache = weakRef.Target;
        }

        if (cache != null)
        {
            return cache as CachedProperty<TProperty>;
        }

        cache =
            IsStatic(type)
                ? new CachedPropertyOfStatic<TProperty>(type, name)
                : type.IsValueType
                    ? Activator.CreateInstance(
                        typeof(CachedPropertyOfStruct<,>).MakeGenericType(type, typeof(TProperty)),
                        name)
                    : Activator.CreateInstance(
                        typeof(CachedPropertyOfClass<,>).MakeGenericType(type, typeof(TProperty)),
                        name);
        PropertiesCache[type, name] = new WeakReference(cache);
        EnqueueCache(cache);

        return cache as CachedProperty<TProperty>;
    }

    public static PropertyInfo GetPropertyInfo<T, TProperty>(string name)
    {
        return GetPropertyCache<T, TProperty>(name).Info;
    }

    public static PropertyInfo GetPropertyInfo<TProperty>(this Type type, string name)
    {
        return GetPropertyCache<TProperty>(type, name).Info;
    }

    public static TProperty GetPropertyValue<T, TProperty>(this ref T instance, string name) where T : struct
    {
        return (GetPropertyCache<T, TProperty>(name) as CachedPropertyOfStruct<T, TProperty>).Get(ref instance);
    }

    public static TProperty GetPropertyValue<T, TProperty>(this T instance, string name) where T : class
    {
        return (GetPropertyCache<T, TProperty>(name) as CachedPropertyOfClass<T, TProperty>).Get(instance);
    }

    public static TProperty GetPropertyValue<T, TProperty>(string name)
    {
        return GetPropertyCache<T, TProperty>(name).Get();
    }

    public static TProperty GetPropertyValue<TProperty>(this Type type, string name)
    {
        return GetPropertyCache<TProperty>(type, name).Get();
    }

    public static void SetPropertyValue<T, TProperty>(this ref T instance, string name, TProperty value)
        where T : struct
    {
        (GetPropertyCache<T, TProperty>(name) as CachedPropertyOfStruct<T, TProperty>).Set(ref instance, value);
    }

    public static void SetPropertyValue<T, TProperty>(this T instance, string name, TProperty value) where T : class
    {
        (GetPropertyCache<T, TProperty>(name) as CachedPropertyOfClass<T, TProperty>).Set(instance, value);
    }

    public static void SetPropertyValue<T, TProperty>(string name, TProperty value)
    {
        GetPropertyCache<T, TProperty>(name).Set(value);
    }

    public static void SetPropertyValue<TProperty>(this Type type, string name, TProperty value)
    {
        GetPropertyCache<TProperty>(type, name).Set(value);
    }

    private abstract class CachedProperty<TProperty>
    {
        public readonly PropertyInfo Info;

        protected CachedProperty(Type type, string name)
        {
            Info = type.GetProperties(AllFlags).FirstOrDefault(item => item.Name == name);

            if (Info == null || Info.PropertyType != typeof(TProperty))
            {
                throw new InvalidOperationException();
            }

            if (Info.DeclaringType != type)
            {
                Info = Info.DeclaringType.GetProperties(AllFlags).FirstOrDefault(item => item.Name == name);
            }
        }

        // for static property
        public abstract TProperty Get();

        // for static property
        public abstract void Set(TProperty value);

        protected Delegate CreateGetter(Type delType, MethodInfo getter, bool isInstByRef)
        {
            if (!getter.IsStatic)
            {
                return Delegate.CreateDelegate(delType, getter);
            }

            DynamicMethod method = new(
                "get_" + Info.Name,
                Info.PropertyType,
                new[] { isInstByRef ? Info.DeclaringType.MakeByRefType() : Info.DeclaringType },
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

            DynamicMethod method = new(
                "set_" + Info.Name,
                null,
                new[] { isInstByRef ? Info.DeclaringType.MakeByRefType() : Info.DeclaringType, Info.PropertyType },
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

        public CachedPropertyOfStruct(string name) : base(typeof(T), name) { }

        public override TProperty Get()
        {
            return (_getter ??= CreateGetter(typeof(Getter), Info.GetMethod, true) as Getter)(ref _dummy);
        }

        public TProperty Get(ref T instance)
        {
            return (_getter ??= CreateGetter(typeof(Getter), Info.GetMethod, true) as Getter)(ref instance);
        }

        public override void Set(TProperty value)
        {
            (_setter ??= CreateSetter(typeof(Setter), Info.SetMethod, true) as Setter)(ref _dummy, value);
        }

        public void Set(ref T instance, TProperty value)
        {
            (_setter ??= CreateSetter(typeof(Setter), Info.SetMethod, true) as Setter)(ref instance, value);
        }

        private delegate TProperty Getter(ref T instance);

        private delegate void Setter(ref T instance, TProperty value);
    }

    private class CachedPropertyOfClass<T, TProperty> : CachedProperty<TProperty>
    {
        private readonly T _dummy = default;
        private Getter _getter;
        private Setter _setter;

        public CachedPropertyOfClass(string name) : base(typeof(T), name) { }

        public override TProperty Get()
        {
            return (_getter ??= CreateGetter(typeof(Getter), Info.GetMethod, false) as Getter)(_dummy);
        }

        public TProperty Get(T instance)
        {
            return (_getter ??= CreateGetter(typeof(Getter), Info.GetMethod, false) as Getter)(instance);
        }

        public override void Set(TProperty value)
        {
            (_setter ??= CreateSetter(typeof(Setter), Info.SetMethod, false) as Setter)(_dummy, value);
        }

        public void Set(T instance, TProperty value)
        {
            (_setter ??= CreateSetter(typeof(Setter), Info.SetMethod, false) as Setter)(instance, value);
        }

        private delegate TProperty Getter(T instance);

        private delegate void Setter(T instance, TProperty value);
    }

    private class CachedPropertyOfStatic<TProperty> : CachedProperty<TProperty>
    {
        private Getter _getter;
        private Setter _setter;

        public CachedPropertyOfStatic(Type type, string name) : base(type, name)
        {
            //if (!IsStatic(type))
            //    throw new InvalidOperationException();
        }

        public override TProperty Get()
        {
            return (_getter ??= CreateGetter())();
        }

        public override void Set(TProperty value)
        {
            (_setter ??= CreateSetter())(value);
        }

        private Getter CreateGetter()
        {
            return Delegate.CreateDelegate(typeof(Getter), Info.GetMethod) as Getter;
        }

        private Setter CreateSetter()
        {
            return Delegate.CreateDelegate(typeof(Setter), Info.SetMethod) as Setter;
        }

        private delegate TProperty Getter();

        private delegate void Setter(TProperty value);
    }
}
