using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(FeatureDefinitionLightAffinity))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class FeatureDefinitionLightAffinityExtensions
    {
        public static T AddLightingEffectAndConditionList<T>(this T entity,
            params FeatureDefinitionLightAffinity.LightingEffectAndCondition[] value)
            where T : FeatureDefinitionLightAffinity
        {
            AddLightingEffectAndConditionList(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddLightingEffectAndConditionList<T>(this T entity,
            IEnumerable<FeatureDefinitionLightAffinity.LightingEffectAndCondition> value)
            where T : FeatureDefinitionLightAffinity
        {
            entity.LightingEffectAndConditionList.AddRange(value);
            return entity;
        }

        public static T ClearLightingEffectAndConditionList<T>(this T entity)
            where T : FeatureDefinitionLightAffinity
        {
            entity.LightingEffectAndConditionList.Clear();
            return entity;
        }

        public static T SetLightingEffectAndConditionList<T>(this T entity,
            params FeatureDefinitionLightAffinity.LightingEffectAndCondition[] value)
            where T : FeatureDefinitionLightAffinity
        {
            SetLightingEffectAndConditionList(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetLightingEffectAndConditionList<T>(this T entity,
            IEnumerable<FeatureDefinitionLightAffinity.LightingEffectAndCondition> value)
            where T : FeatureDefinitionLightAffinity
        {
            entity.LightingEffectAndConditionList.SetRange(value);
            return entity;
        }
    }
}
