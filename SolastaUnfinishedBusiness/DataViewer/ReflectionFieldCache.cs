using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.DataViewer;

internal static partial class ReflectionCache
{
    private static readonly DoubleDictionary<Type, string, WeakReference> FieldCache = new();

    [NotNull]
    private static CachedField<TField> GetFieldCache<T, TField>(string name)
    {
        object cache = null;

        if (FieldCache.TryGetValue(typeof(T), name, out var weakRef))
        {
            cache = weakRef.Target;
        }

        if (cache != null)
        {
            return (CachedField<TField>)cache;
        }

        if (typeof(T).IsValueType)
        {
            cache = new CachedFieldOfStruct<T, TField>(name);
        }
        else
        {
            cache = new CachedFieldOfClass<T, TField>(name);
        }

        FieldCache[typeof(T), name] = new WeakReference(cache);
        EnqueueCache(cache);

        return (CachedField<TField>)cache;
    }

    internal static TField GetFieldValue<T, TField>(this ref T instance, string name) where T : struct
    {
        return ((CachedFieldOfStruct<T, TField>)GetFieldCache<T, TField>(name)).Get(ref instance);
    }

    internal static TField GetFieldValue<T, TField>(this T instance, string name) where T : class
    {
        return ((CachedFieldOfClass<T, TField>)GetFieldCache<T, TField>(name)).Get(instance);
    }

    private abstract class CachedField<TField>
    {
        private readonly FieldInfo _info;

        protected CachedField(IReflect type, string name)
        {
            _info = type.GetFields(AllFlags).FirstOrDefault(item => item.Name == name);

            if (_info == null || _info.FieldType != typeof(TField))
            {
                throw new InvalidOperationException();
            }
        }

        // for static field
        [UsedImplicitly]
        internal abstract TField Get();

        // for static field
        [UsedImplicitly]
        internal abstract void Set(TField value);

        protected Delegate CreateGetter(Type delType, bool isInstByRef)
        {
            var method = new DynamicMethod(
                "get_" + _info.Name,
                _info.FieldType,
                [isInstByRef ? _info.DeclaringType?.MakeByRefType() : _info.DeclaringType],
                typeof(CachedField<TField>),
                true);

            method.DefineParameter(1, ParameterAttributes.In, "instance");

            var il = method.GetILGenerator();

            if (_info.IsStatic)
            {
                il.Emit(OpCodes.Ldsfld, _info);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, _info);
            }

            il.Emit(OpCodes.Ret);

            return method.CreateDelegate(delType);
        }

        protected Delegate CreateSetter(Type delType, bool isInstByRef)
        {
            var method = new DynamicMethod(
                "set_" + _info.Name,
                null,
                [isInstByRef ? _info.DeclaringType?.MakeByRefType() : _info.DeclaringType, _info.FieldType],
                typeof(CachedField<TField>),
                true);

            method.DefineParameter(1, ParameterAttributes.In, "instance");
            method.DefineParameter(2, ParameterAttributes.In, "value");

            var il = method.GetILGenerator();

            if (_info.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stsfld, _info);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, _info);
            }

            il.Emit(OpCodes.Ret);

            return method.CreateDelegate(delType);
        }
    }

    private class CachedFieldOfStruct<T, TField> : CachedField<TField>
    {
        private T _dummy;

        private Getter _getter;

        private Setter _setter;

        internal CachedFieldOfStruct(string name) : base(typeof(T), name) { }

        internal override TField Get()
        {
            return (_getter ??= (Getter)CreateGetter(typeof(Getter), true))(ref _dummy);
        }

        internal TField Get(ref T instance)
        {
            return (_getter ??= (Getter)CreateGetter(typeof(Getter), true))(ref instance);
        }

        internal override void Set(TField value)
        {
            (_setter ??= (Setter)CreateSetter(typeof(Setter), true))(ref _dummy, value);
        }

        private delegate TField Getter(ref T instance);

        private delegate void Setter(ref T instance, TField value);
    }

    private class CachedFieldOfClass<T, TField> : CachedField<TField>
    {
        private readonly T _dummy = default;

        private Getter _getter;

        private Setter _setter;

        internal CachedFieldOfClass(string name) : base(typeof(T), name) { }

        internal override TField Get()
        {
            return (_getter ??= (Getter)CreateGetter(typeof(Getter), false))(_dummy);
        }

        internal TField Get(T instance)
        {
            return (_getter ??= (Getter)CreateGetter(typeof(Getter), false))(instance);
        }

        internal override void Set(TField value)
        {
            (_setter ??= (Setter)CreateSetter(typeof(Setter), false))(_dummy, value);
        }

        private delegate TField Getter(T instance);

        private delegate void Setter(T instance, TField value);
    }
}
