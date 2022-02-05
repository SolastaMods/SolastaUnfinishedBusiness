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
    [TargetType(typeof(FeatureDefinitionAttackModifier)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionAttackModifierExtensions
    {
        public static T SetAbilityScoreReplacement<T>(this T entity, RuleDefinitions.AbilityScoreReplacement value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("abilityScoreReplacement", value);
            return entity;
        }

        public static T SetAdditionalAttackTag<T>(this T entity, System.String value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("additionalAttackTag", value);
            return entity;
        }

        public static T SetAdditionalBonusAttackFromMain<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("additionalBonusAttackFromMain", value);
            return entity;
        }

        public static T SetAdditionalDamageDice<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("additionalDamageDice", value);
            return entity;
        }

        public static T SetAttackRollAbilityScore<T>(this T entity, System.String value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("attackRollAbilityScore", value);
            return entity;
        }

        public static T SetAttackRollModifier<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("attackRollModifier", value);
            return entity;
        }

        public static T SetAttackRollModifierMethod<T>(this T entity, RuleDefinitions.AttackModifierMethod value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("attackRollModifierMethod", value);
            return entity;
        }

        public static T SetCanAddAbilityBonusToSecondary<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("canAddAbilityBonusToSecondary", value);
            return entity;
        }

        public static T SetCanDualWieldNonLight<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("canDualWieldNonLight", value);
            return entity;
        }

        public static T SetDamageDieReplacement<T>(this T entity, RuleDefinitions.DamageDieReplacement value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("damageDieReplacement", value);
            return entity;
        }

        public static T SetDamageRollAbilityScore<T>(this T entity, System.String value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("damageRollAbilityScore", value);
            return entity;
        }

        public static T SetDamageRollModifier<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("damageRollModifier", value);
            return entity;
        }

        public static T SetDamageRollModifierMethod<T>(this T entity, RuleDefinitions.AttackModifierMethod value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("damageRollModifierMethod", value);
            return entity;
        }

        public static T SetFollowUpAddAbilityBonus<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("followUpAddAbilityBonus", value);
            return entity;
        }

        public static T SetFollowUpDamageDie<T>(this T entity, RuleDefinitions.DieType value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("followUpDamageDie", value);
            return entity;
        }

        public static T SetFollowUpStrike<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("followUpStrike", value);
            return entity;
        }

        public static T SetImpactParticleReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("impactParticleReference", value);
            return entity;
        }

        public static T SetReplacedDieType<T>(this T entity, RuleDefinitions.DieType value)
            where T : FeatureDefinitionAttackModifier
        {
            entity.SetField("replacedDieType", value);
            return entity;
        }
    }
}