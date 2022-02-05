using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Text;
using System.CodeDom.Compiler;
using TA.AI;
using TA;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using  static  ActionDefinitions ;
using  static  TA . AI . DecisionPackageDefinition ;
using  static  TA . AI . DecisionDefinition ;
using  static  RuleDefinitions ;
using  static  BanterDefinitions ;
using  static  Gui ;
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(FeatureDefinitionDamageAffinity)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionDamageAffinityExtensions
    {
        public static System.Collections.Generic.List<System.String> GetTagsIgnoringAffinity<T>(this T entity)
            where T : FeatureDefinitionDamageAffinity
        {
            return entity.GetField<System.Collections.Generic.List<System.String>>("tagsIgnoringAffinity");
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
    }
}