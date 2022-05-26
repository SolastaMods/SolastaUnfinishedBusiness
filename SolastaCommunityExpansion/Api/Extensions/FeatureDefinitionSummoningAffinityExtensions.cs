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
    [TargetType(typeof(FeatureDefinitionSummoningAffinity))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class FeatureDefinitionSummoningAffinityExtensions
    {
        public static T AddAddedConditions<T>(this T entity, params ConditionDefinition[] value)
            where T : FeatureDefinitionSummoningAffinity
        {
            AddAddedConditions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAddedConditions<T>(this T entity, IEnumerable<ConditionDefinition> value)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.AddedConditions.AddRange(value);
            return entity;
        }

        public static T AddEffectForms<T>(this T entity, params EffectForm[] value)
            where T : FeatureDefinitionSummoningAffinity
        {
            AddEffectForms(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddEffectForms<T>(this T entity, IEnumerable<EffectForm> value)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.EffectForms.AddRange(value);
            return entity;
        }

        public static T ClearAddedConditions<T>(this T entity)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.AddedConditions.Clear();
            return entity;
        }

        public static T ClearEffectForms<T>(this T entity)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.EffectForms.Clear();
            return entity;
        }

        public static T SetAddedConditions<T>(this T entity, params ConditionDefinition[] value)
            where T : FeatureDefinitionSummoningAffinity
        {
            SetAddedConditions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAddedConditions<T>(this T entity, IEnumerable<ConditionDefinition> value)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.AddedConditions.SetRange(value);
            return entity;
        }

        public static T SetEffectForms<T>(this T entity, params EffectForm[] value)
            where T : FeatureDefinitionSummoningAffinity
        {
            SetEffectForms(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetEffectForms<T>(this T entity, IEnumerable<EffectForm> value)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.EffectForms.SetRange(value);
            return entity;
        }

        public static T SetEffectOnConjuredDeath<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.SetField("effectOnConjuredDeath", value);
            return entity;
        }

        public static T SetRequiredMonsterTag<T>(this T entity, System.String value)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.SetField("requiredMonsterTag", value);
            return entity;
        }
    }
}
