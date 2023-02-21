using System;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Api.Helpers;

internal static class TraverseHelper
{
    private static bool FailOnMissingMember => true;

    private static TV GetField<T, TV>([NotNull] this T instance, [NotNull] string fieldName) where T : class
    {
        // ReSharper disable once InvocationIsSkipped
        PreConditions.ArgumentIsNotNull(instance, nameof(instance));
        // ReSharper disable once InvocationIsSkipped
        PreConditions.IsNotNullOrWhiteSpace(fieldName, nameof(fieldName));

        var t = Traverse.Create(instance);

        if (FailOnMissingMember && !t.Field(fieldName).FieldExists())
        {
            throw new MissingFieldException(instance.GetType().FullName, fieldName);
        }

        return t.Field<TV>(fieldName).Value;
    }

    internal static TV GetField<TV>([NotNull] this object instance, [NotNull] string fieldName)
    {
        return instance.GetField<object, TV>(fieldName);
    }

#if false
    internal static void SetField<T, TV>([NotNull] this T instance, [NotNull] string fieldName, TV value)
        where T : class
    {
        PreConditions.ArgumentIsNotNull(instance, nameof(instance));
        PreConditions.IsNotNullOrWhiteSpace(fieldName, nameof(fieldName));

        var t = Traverse.Create(instance);

        if (FailOnMissingMember && !t.Field(fieldName).FieldExists())
        {
            throw new MissingFieldException(typeof(T).Name, fieldName);
        }

        t.Field<TV>(fieldName).Value = value;

        // It's also possible to do this, which may be more efficient
        // but doesn't give as nice exception information.
        // AccessTools.FieldRefAccess<T, V>(instance, fieldName) = value;
    }

    internal static TV GetProperty<TV>([NotNull] this object instance, [NotNull] string propertyName)
    {
        PreConditions.ArgumentIsNotNull(instance, nameof(instance));
        PreConditions.IsNotNullOrWhiteSpace(propertyName, nameof(propertyName));

        var t = Traverse.Create(instance);

        if (FailOnMissingMember && !t.Property(propertyName).PropertyExists())
        {
            throw new MissingMemberException(instance.GetType().FullName, propertyName);
        }

        return t.Property<TV>(propertyName).Value;
    }

    internal static void SetProperty<T, TV>([NotNull] this T instance, [NotNull] string propertyName, TV value)
        where T : class
    {
        PreConditions.ArgumentIsNotNull(instance, nameof(instance));
        PreConditions.IsNotNullOrWhiteSpace(propertyName, nameof(propertyName));

        var t = Traverse.Create(instance);

        if (FailOnMissingMember && !t.Property(propertyName).PropertyExists())
        {
            throw new MissingMemberException(typeof(T).Name, propertyName);
        }

        t.Property<TV>(propertyName).Value = value;
    }
#endif
}
