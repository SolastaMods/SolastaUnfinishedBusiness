using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(ItemPropertyForm))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class ItemPropertyFormExtensions
    {
        public static T AddFeatureBySlotLevel<T>(this T entity, params FeatureUnlockByLevel[] value)
            where T : ItemPropertyForm
        {
            AddFeatureBySlotLevel(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFeatureBySlotLevel<T>(this T entity, IEnumerable<FeatureUnlockByLevel> value)
            where T : ItemPropertyForm
        {
            entity.FeatureBySlotLevel.AddRange(value);
            return entity;
        }

        public static T ClearFeatureBySlotLevel<T>(this T entity)
            where T : ItemPropertyForm
        {
            entity.FeatureBySlotLevel.Clear();
            return entity;
        }

        public static ItemPropertyForm Copy(this ItemPropertyForm entity)
        {
            var copy = new ItemPropertyForm();
            copy.Copy(entity);
            return copy;
        }

        public static T SetFeatureBySlotLevel<T>(this T entity, params FeatureUnlockByLevel[] value)
            where T : ItemPropertyForm
        {
            SetFeatureBySlotLevel(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFeatureBySlotLevel<T>(this T entity, IEnumerable<FeatureUnlockByLevel> value)
            where T : ItemPropertyForm
        {
            entity.FeatureBySlotLevel.SetRange(value);
            return entity;
        }

        public static T SetUsageLimitation<T>(this T entity, ItemPropertyUsage value)
            where T : ItemPropertyForm
        {
            entity.SetField("usageLimitation", value);
            return entity;
        }

        public static T SetUseAmount<T>(this T entity, Int32 value)
            where T : ItemPropertyForm
        {
            entity.SetField("useAmount", value);
            return entity;
        }
    }
}
