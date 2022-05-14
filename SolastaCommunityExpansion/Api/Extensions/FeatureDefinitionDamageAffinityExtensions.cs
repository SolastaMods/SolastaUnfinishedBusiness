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
    [TargetType(typeof(FeatureDefinitionDamageAffinity)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionDamageAffinityExtensions
    {
        public static T AddTagsIgnoringAffinity<T>(this T entity, params System.String[] value)
            where T : FeatureDefinitionDamageAffinity
        {
            AddTagsIgnoringAffinity(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddTagsIgnoringAffinity<T>(this T entity, IEnumerable<System.String> value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.TagsIgnoringAffinity.AddRange(value);
            return entity;
        }

        public static T ClearTagsIgnoringAffinity<T>(this T entity)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.TagsIgnoringAffinity.Clear();
            return entity;
        }

        public static T SetAncestryDefinesDamageType<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("ancestryDefinesDamageType", value);
            return entity;
        }

        public static T SetDamageAffinityType<T>(this T entity, RuleDefinitions.DamageAffinityType value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.DamageAffinityType = value;
            return entity;
        }

        public static T SetDamageType<T>(this T entity, System.String value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.DamageType = value;
            return entity;
        }

        public static T SetHealBackCap<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("healBackCap", value);
            return entity;
        }

        public static T SetHealsBack<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("healsBack", value);
            return entity;
        }

        public static T SetInstantDeathImmunity<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("instantDeathImmunity", value);
            return entity;
        }

        public static T SetKnockOutAddDC<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("knockOutAddDC", value);
            return entity;
        }

        public static T SetKnockOutAffinity<T>(this T entity, RuleDefinitions.KnockoutAffinity value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("knockOutAffinity", value);
            return entity;
        }

        public static T SetKnockOutDCAttribute<T>(this T entity, System.String value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("knockOutDCAttribute", value);
            return entity;
        }

        public static T SetKnockOutOccurencesNumber<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("knockOutOccurencesNumber", value);
            return entity;
        }

        public static T SetKnockOutRequiredCondition<T>(this T entity, ConditionDefinition value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("knockOutRequiredCondition", value);
            return entity;
        }

        public static T SetRetaliatePower<T>(this T entity, FeatureDefinitionPower value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("retaliatePower", value);
            return entity;
        }

        public static T SetRetaliateProximity<T>(this T entity, RuleDefinitions.AttackProximity value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("retaliateProximity", value);
            return entity;
        }

        public static T SetRetaliateRangeCells<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("retaliateRangeCells", value);
            return entity;
        }

        public static T SetRetaliateWhenHit<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("retaliateWhenHit", value);
            return entity;
        }

        public static T SetSavingThrowAdvantageType<T>(this T entity, RuleDefinitions.AdvantageType value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SavingThrowAdvantageType = value;
            return entity;
        }

        public static T SetSavingThrowModifier<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("savingThrowModifier", value);
            return entity;
        }

        public static T SetSituationalContext<T>(this T entity, RuleDefinitions.SituationalContext value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.SetField("situationalContext", value);
            return entity;
        }

        public static T SetTagsIgnoringAffinity<T>(this T entity, params System.String[] value)
            where T : FeatureDefinitionDamageAffinity
        {
            SetTagsIgnoringAffinity(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetTagsIgnoringAffinity<T>(this T entity, IEnumerable<System.String> value)
            where T : FeatureDefinitionDamageAffinity
        {
            entity.TagsIgnoringAffinity.SetRange(value);
            return entity;
        }
    }
}