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
    [TargetType(typeof(FeatureDefinitionAdditionalDamage))]
    public static partial class FeatureDefinitionAdditionalDamageExtensions
    {
        public static T SetAcidImpactParticleReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("acidImpactParticleReference", value);
            return entity;
        }

        public static T SetAdditionalDamageType<T>(this T entity, RuleDefinitions.AdditionalDamageType value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("additionalDamageType", value);
            return entity;
        }

        public static T SetAddLightSource<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("addLightSource", value);
            return entity;
        }

        public static T SetAttackModeOnly<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("attackModeOnly", value);
            return entity;
        }

        public static T SetColdImpactParticleReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("coldImpactParticleReference", value);
            return entity;
        }

        public static T SetComputeDescription<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("computeDescription", value);
            return entity;
        }

        public static T SetDamageAdvancement<T>(this T entity, RuleDefinitions.AdditionalDamageAdvancement value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("damageAdvancement", value);
            return entity;
        }

        public static T SetDamageDiceNumber<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("damageDiceNumber", value);
            return entity;
        }

        public static T SetDamageDieType<T>(this T entity, RuleDefinitions.DieType value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("damageDieType", value);
            return entity;
        }

        public static T SetDamageSaveAffinity<T>(this T entity, RuleDefinitions.EffectSavingThrowType value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("damageSaveAffinity", value);
            return entity;
        }

        public static T SetDamageValueDetermination<T>(this T entity, RuleDefinitions.AdditionalDamageValueDetermination value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("damageValueDetermination", value);
            return entity;
        }

        public static T SetDcComputation<T>(this T entity, RuleDefinitions.EffectDifficultyClassComputation value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("dcComputation", value);
            return entity;
        }

        public static T SetFamiliesDiceNumber<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("familiesDiceNumber", value);
            return entity;
        }

        public static T SetFireImpactParticleReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("fireImpactParticleReference", value);
            return entity;
        }

        public static T SetHasSavingThrow<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("hasSavingThrow", value);
            return entity;
        }

        public static T SetIgnoreCriticalDoubleDice<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("ignoreCriticalDoubleDice", value);
            return entity;
        }

        public static T SetImpactParticleReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("impactParticleReference", value);
            return entity;
        }

        public static T SetLightningImpactParticleReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("lightningImpactParticleReference", value);
            return entity;
        }

        public static T SetLightSourceForm<T>(this T entity, LightSourceForm value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("lightSourceForm", value);
            return entity;
        }

        public static T SetLimitedUsage<T>(this T entity, RuleDefinitions.FeatureLimitedUsage value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("limitedUsage", value);
            return entity;
        }

        public static T SetNotificationTag<T>(this T entity, System.String value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("notificationTag", value);
            return entity;
        }

        public static T SetParticlesBasedOnAncestryDamageType<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("particlesBasedOnAncestryDamageType", value);
            return entity;
        }

        public static T SetPoisonImpactParticleReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("poisonImpactParticleReference", value);
            return entity;
        }

        public static T SetRequiredCharacterFamily<T>(this T entity, CharacterFamilyDefinition value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("requiredCharacterFamily", value);
            return entity;
        }

        public static T SetRequiredProperty<T>(this T entity, RuleDefinitions.AdditionalDamageRequiredProperty value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("requiredProperty", value);
            return entity;
        }

        public static T SetRequiredTargetCondition<T>(this T entity, ConditionDefinition value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("requiredTargetCondition", value);
            return entity;
        }

        public static T SetRequiredTargetCreatureTag<T>(this T entity, System.String value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("requiredTargetCreatureTag", value);
            return entity;
        }

        public static T SetRequiredTargetSenseType<T>(this T entity, SenseMode.Type value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("requiredTargetSenseType", value);
            return entity;
        }

        public static T SetSavingThrowAbility<T>(this T entity, System.String value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("savingThrowAbility", value);
            return entity;
        }

        public static T SetSavingThrowDC<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("savingThrowDC", value);
            return entity;
        }

        public static T SetSpecificDamageType<T>(this T entity, System.String value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("specificDamageType", value);
            return entity;
        }

        public static T SetTriggerCondition<T>(this T entity, RuleDefinitions.AdditionalDamageTriggerCondition value)
            where T : FeatureDefinitionAdditionalDamage
        {
            entity.SetField("triggerCondition", value);
            return entity;
        }
    }
}