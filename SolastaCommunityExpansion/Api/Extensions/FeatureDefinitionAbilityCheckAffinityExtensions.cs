using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(FeatureDefinitionAbilityCheckAffinity))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class FeatureDefinitionAbilityCheckAffinityExtensions
    {
        public static T AddAffinityGroups<T>(this T entity,
            params FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup[] value)
            where T : FeatureDefinitionAbilityCheckAffinity
        {
            AddAffinityGroups(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAffinityGroups<T>(this T entity,
            IEnumerable<FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup> value)
            where T : FeatureDefinitionAbilityCheckAffinity
        {
            entity.AffinityGroups.AddRange(value);
            return entity;
        }

        public static T ClearAffinityGroups<T>(this T entity)
            where T : FeatureDefinitionAbilityCheckAffinity
        {
            entity.AffinityGroups.Clear();
            return entity;
        }

        public static T SetAffinityGroups<T>(this T entity,
            params FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup[] value)
            where T : FeatureDefinitionAbilityCheckAffinity
        {
            SetAffinityGroups(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAffinityGroups<T>(this T entity,
            IEnumerable<FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup> value)
            where T : FeatureDefinitionAbilityCheckAffinity
        {
            entity.AffinityGroups.SetRange(value);
            return entity;
        }

        public static T SetUseControllerAbilityChecks<T>(this T entity, Boolean value)
            where T : FeatureDefinitionAbilityCheckAffinity
        {
            entity.SetField("useControllerAbilityChecks", value);
            return entity;
        }
    }
}
