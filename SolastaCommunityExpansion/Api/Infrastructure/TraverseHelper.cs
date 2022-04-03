using System;
using HarmonyLib;

namespace SolastaModApi.Infrastructure
{
    public static class TraverseHelper
    {
        public static bool FailOnMissingMember { get; set; } = true;

        /// <summary>
        /// Usage
        /// <code>
        /// var instanceWithPrivateFields = CreateInstance(...);
        /// instanceWithPrivateFields.SetField("privatefield", 2);
        /// </code>
        /// </summary>
        /// <remarks>
        /// This does the same job as
        /// <code>
        /// Traverse.Create(instanceWithPrivateFields).Field("privatefield").SetValue(2);
        /// </code>
        /// But with more brevity and more error checking.
        /// Traverse will happily continue without error if you supply a field name that doesn't exist.
        /// SetField will throw an appropriate exception.
        /// </remarks>
        public static void SetField<T, V>(this T instance, string fieldName, V value) where T : class
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

        public static V GetField<T, V>(this T instance, string fieldName) where T : class
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

        public static V GetField<V>(this object instance, string fieldName)
        {
            return instance.GetField<object, V>(fieldName);
        }

        public static V GetProperty<V>(this object instance, string propertyName)
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

        public static void SetProperty<T, V>(this T instance, string propertyName, V value) where T : class
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
    }
}
