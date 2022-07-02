using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Api.Infrastructure;

public static class TraverseHelper
{
    private static bool FailOnMissingMember => true;

    /// <summary>
    ///     Usage
    ///     <code>
    /// var instanceWithPrivateFields = CreateInstance(...);
    /// instanceWithPrivateFields.privatefield = 2;
    /// </code>
    /// </summary>
    /// <remarks>
    ///     This does the same job as
    ///     <code>
    /// Traverse.Create(instanceWithPrivateFields).Field("privatefield").SetValue(2);
    /// </code>
    ///     But with more brevity and more error checking.
    ///     Traverse will happily continue without error if you supply a field name that doesn't exist.
    ///     SetField will throw an appropriate exception.
    /// </remarks>
    internal static void SetField<T, V>([NotNull] this T instance, [NotNull] string fieldName, V value) where T : class
    {
        Preconditions.IsNotNull(instance, nameof(instance));
        Preconditions.IsNotNullOrWhiteSpace(fieldName, nameof(fieldName));

        var t = Traverse.Create(instance);

        if (FailOnMissingMember && !t.Field(fieldName).FieldExists())
        {
            throw new MissingFieldException(typeof(T).Name, fieldName);
        }

        t.Field<V>(fieldName).Value = value;

        // It's also possible to do this, which may be more efficient
        // but doesn't give as nice exception information.
        // AccessTools.FieldRefAccess<T, V>(instance, fieldName) = value;
    }

    private static V GetField<T, V>([NotNull] this T instance, [NotNull] string fieldName) where T : class
    {
        Preconditions.IsNotNull(instance, nameof(instance));
        Preconditions.IsNotNullOrWhiteSpace(fieldName, nameof(fieldName));

        var t = Traverse.Create(instance);

        if (FailOnMissingMember && !t.Field(fieldName).FieldExists())
        {
            throw new MissingFieldException(instance.GetType().FullName, fieldName);
        }

        return t.Field<V>(fieldName).Value;
    }

    public static V GetField<V>([NotNull] this object instance, [NotNull] string fieldName)
    {
        return instance.GetField<object, V>(fieldName);
    }

#if DEBUG
    public static V GetProperty<V>([NotNull] this object instance, [NotNull] string propertyName)
    {
        Preconditions.IsNotNull(instance, nameof(instance));
        Preconditions.IsNotNullOrWhiteSpace(propertyName, nameof(propertyName));

        var t = Traverse.Create(instance);

        if (FailOnMissingMember && !t.Property(propertyName).PropertyExists())
        {
            throw new MissingMemberException(instance.GetType().FullName, propertyName);
        }

        return t.Property<V>(propertyName).Value;
    }

    public static void SetProperty<T, V>([NotNull] this T instance, [NotNull] string propertyName, V value)
        where T : class
    {
        Preconditions.IsNotNull(instance, nameof(instance));
        Preconditions.IsNotNullOrWhiteSpace(propertyName, nameof(propertyName));

        var t = Traverse.Create(instance);

        if (FailOnMissingMember && !t.Property(propertyName).PropertyExists())
        {
            throw new MissingMemberException(typeof(T).Name, propertyName);
        }

        t.Property<V>(propertyName).Value = value;
    }
#endif
}
