using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Text;
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
    [TargetType(typeof(FeatureDefinitionCombatAffinity))]
    public static partial class FeatureDefinitionCombatAffinityExtensions
    {
        public static T SetAttackOfOpportunityImmunity<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("attackOfOpportunityImmunity", value);
            return entity;
        }

        public static T SetAttackOfOpportunityOnMeAdvantageType<T>(this T entity, RuleDefinitions.AdvantageType value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("attackOfOpportunityOnMeAdvantageType", value);
            return entity;
        }

        public static T SetAttackOnMeAdvantage<T>(this T entity, RuleDefinitions.AdvantageType value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("attackOnMeAdvantage", value);
            return entity;
        }

        public static T SetAttackOnMeCountLimit<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("attackOnMeCountLimit", value);
            return entity;
        }

        public static T SetAutoCritical<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("autoCritical", value);
            return entity;
        }

        public static T SetCanRageToOvercomeSurprise<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("canRageToOvercomeSurprise", value);
            return entity;
        }

        public static T SetCriticalHitImmunity<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("criticalHitImmunity", value);
            return entity;
        }

        public static T SetIgnoreCover<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("ignoreCover", value);
            return entity;
        }

        public static T SetIgnoreRangeAdvantage<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("ignoreRangeAdvantage", value);
            return entity;
        }

        public static T SetInitiativeAffinity<T>(this T entity, RuleDefinitions.AdvantageType value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("initiativeAffinity", value);
            return entity;
        }

        public static T SetMultiAttackAffinity<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("multiAttackAffinity", value);
            return entity;
        }

        public static T SetMultiAttackDefenseValue<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("multiAttackDefenseValue", value);
            return entity;
        }

        public static T SetMyAttackAdvantage<T>(this T entity, RuleDefinitions.AdvantageType value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("myAttackAdvantage", value);
            return entity;
        }

        public static T SetMyAttackAffinityFilter<T>(this T entity, RuleDefinitions.AttackAffinityFilter value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("myAttackAffinityFilter", value);
            return entity;
        }

        public static T SetMyAttackDamageMultiplier<T>(this T entity, System.Single value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("myAttackDamageMultiplier", value);
            return entity;
        }

        public static T SetMyAttackModifierDiceNumber<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("myAttackModifierDiceNumber", value);
            return entity;
        }

        public static T SetMyAttackModifierDieType<T>(this T entity, RuleDefinitions.DieType value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("myAttackModifierDieType", value);
            return entity;
        }

        public static T SetMyAttackModifierSign<T>(this T entity, RuleDefinitions.AttackModifierSign value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("myAttackModifierSign", value);
            return entity;
        }

        public static T SetMyAttackModifierValueDetermination<T>(this T entity, RuleDefinitions.CombatAddinityValueDetermination value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("myAttackModifierValueDetermination", value);
            return entity;
        }

        public static T SetRequiredCondition<T>(this T entity, ConditionDefinition value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("requiredCondition", value);
            return entity;
        }

        public static T SetSituationalContext<T>(this T entity, RuleDefinitions.SituationalContext value)
            where T : FeatureDefinitionCombatAffinity
        {
            entity.SetField("situationalContext", value);
            return entity;
        }
    }
}