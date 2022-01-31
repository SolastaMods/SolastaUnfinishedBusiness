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
    [TargetType(typeof(ConditionDefinition))]
    public static partial class ConditionDefinitionExtensions
    {
        public static T SetAcidParticleParameters<T>(this T entity, ConditionParticleParameters value)
            where T : ConditionDefinition
        {
            entity.SetField("acidParticleParameters", value);
            return entity;
        }

        public static T SetAdditionalCondition<T>(this T entity, ConditionDefinition value)
            where T : ConditionDefinition
        {
            entity.SetField("additionalCondition", value);
            return entity;
        }

        public static T SetAdditionalConditionDurationParameter<T>(this T entity, System.Int32 value)
            where T : ConditionDefinition
        {
            entity.SetField("additionalConditionDurationParameter", value);
            return entity;
        }

        public static T SetAdditionalConditionDurationType<T>(this T entity, RuleDefinitions.DurationType value)
            where T : ConditionDefinition
        {
            entity.SetField("additionalConditionDurationType", value);
            return entity;
        }

        public static T SetAdditionalConditionTurnOccurenceType<T>(this T entity, RuleDefinitions.TurnOccurenceType value)
            where T : ConditionDefinition
        {
            entity.SetField("additionalConditionTurnOccurenceType", value);
            return entity;
        }

        public static T SetAdditionalConditionWhenHit<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("additionalConditionWhenHit", value);
            return entity;
        }

        public static T SetAdditionalDamageDieNumber<T>(this T entity, System.Int32 value)
            where T : ConditionDefinition
        {
            entity.SetField("additionalDamageDieNumber", value);
            return entity;
        }

        public static T SetAdditionalDamageDieType<T>(this T entity, RuleDefinitions.DieType value)
            where T : ConditionDefinition
        {
            entity.SetField("additionalDamageDieType", value);
            return entity;
        }

        public static T SetAdditionalDamageQuantity<T>(this T entity, ConditionDefinition.DamageQuantity value)
            where T : ConditionDefinition
        {
            entity.SetField("additionalDamageQuantity", value);
            return entity;
        }

        public static T SetAdditionalDamageType<T>(this T entity, System.String value)
            where T : ConditionDefinition
        {
            entity.SetField("additionalDamageType", value);
            return entity;
        }

        public static T SetAdditionalDamageWhenHit<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("additionalDamageWhenHit", value);
            return entity;
        }

        public static T SetAdditiveAmount<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("additiveAmount", value);
            return entity;
        }

        public static T SetAllowMultipleInstances<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("allowMultipleInstances", value);
            return entity;
        }

        public static T SetAmountOrigin<T>(this T entity, ConditionDefinition.OriginOfAmount value)
            where T : ConditionDefinition
        {
            entity.SetField("amountOrigin", value);
            return entity;
        }

        public static T SetBaseAmount<T>(this T entity, System.Int32 value)
            where T : ConditionDefinition
        {
            entity.SetField("baseAmount", value);
            return entity;
        }

        public static T SetBattlePackage<T>(this T entity, TA.AI.DecisionPackageDefinition value)
            where T : ConditionDefinition
        {
            entity.SetField("battlePackage", value);
            return entity;
        }

        public static T SetCharacterShaderReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : ConditionDefinition
        {
            entity.SetField("characterShaderReference", value);
            return entity;
        }

        public static T SetColdParticleParameters<T>(this T entity, ConditionParticleParameters value)
            where T : ConditionDefinition
        {
            entity.SetField("coldParticleParameters", value);
            return entity;
        }

        public static T SetConditionEndParticleReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : ConditionDefinition
        {
            entity.SetField("conditionEndParticleReference", value);
            return entity;
        }

        public static T SetConditionParticleReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : ConditionDefinition
        {
            entity.SetField("conditionParticleReference", value);
            return entity;
        }

        public static T SetConditionStartParticleReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : ConditionDefinition
        {
            entity.SetField("conditionStartParticleReference", value);
            return entity;
        }

        public static T SetConditionType<T>(this T entity, RuleDefinitions.ConditionType value)
            where T : ConditionDefinition
        {
            entity.SetField("conditionType", value);
            return entity;
        }

        public static T SetDisolveCharacterOnDeath<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("disolveCharacterOnDeath", value);
            return entity;
        }

        public static T SetDisolveParameters<T>(this T entity, GraphicsCharacterDefinitions.DisolveParameters value)
            where T : ConditionDefinition
        {
            entity.SetField("disolveParameters", value);
            return entity;
        }

        public static T SetDurationParameter<T>(this T entity, System.Int32 value)
            where T : ConditionDefinition
        {
            entity.SetField("durationParameter", value);
            return entity;
        }

        public static T SetDurationParameterDie<T>(this T entity, RuleDefinitions.DieType value)
            where T : ConditionDefinition
        {
            entity.SetField("durationParameterDie", value);
            return entity;
        }

        public static T SetDurationType<T>(this T entity, RuleDefinitions.DurationType value)
            where T : ConditionDefinition
        {
            entity.SetField("durationType", value);
            return entity;
        }

        public static T SetExplorationPackage<T>(this T entity, TA.AI.DecisionPackageDefinition value)
            where T : ConditionDefinition
        {
            entity.SetField("explorationPackage", value);
            return entity;
        }

        public static T SetFearSource<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("fearSource", value);
            return entity;
        }

        public static T SetFireParticleParameters<T>(this T entity, ConditionParticleParameters value)
            where T : ConditionDefinition
        {
            entity.SetField("fireParticleParameters", value);
            return entity;
        }

        public static T SetFirstCharacterShaderColor<T>(this T entity, UnityEngine.Color value)
            where T : ConditionDefinition
        {
            entity.SetField("firstCharacterShaderColor", value);
            return entity;
        }

        public static T SetFollowSourcePosition<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("followSourcePosition", value);
            return entity;
        }

        public static T SetForceBehavior<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("forceBehavior", value);
            return entity;
        }

        public static T SetInDungeonEditor<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("inDungeonEditor", value);
            return entity;
        }

        public static T SetInterruptionDamageThreshold<T>(this T entity, System.Int32 value)
            where T : ConditionDefinition
        {
            entity.SetField("interruptionDamageThreshold", value);
            return entity;
        }

        public static T SetInterruptionRequiresSavingThrow<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("interruptionRequiresSavingThrow", value);
            return entity;
        }

        public static T SetLightningParticleParameters<T>(this T entity, ConditionParticleParameters value)
            where T : ConditionDefinition
        {
            entity.SetField("lightningParticleParameters", value);
            return entity;
        }

        public static T SetOverrideCharacterShaderColors<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("overrideCharacterShaderColors", value);
            return entity;
        }

        public static T SetParentCondition<T>(this T entity, ConditionDefinition value)
            where T : ConditionDefinition
        {
            entity.SetField("parentCondition", value);
            return entity;
        }

        public static T SetParticlesBasedOnAncestryDamageType<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("particlesBasedOnAncestryDamageType", value);
            return entity;
        }

        public static T SetPermanentlyRemovedIfExtraPlanar<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("permanentlyRemovedIfExtraPlanar", value);
            return entity;
        }

        public static T SetPoisonParticleParameters<T>(this T entity, ConditionParticleParameters value)
            where T : ConditionDefinition
        {
            entity.SetField("poisonParticleParameters", value);
            return entity;
        }

        public static T SetPossessive<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("possessive", value);
            return entity;
        }

        public static T SetRemovedFromTheGame<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("removedFromTheGame", value);
            return entity;
        }

        public static T SetSecondCharacterShaderColor<T>(this T entity, UnityEngine.Color value)
            where T : ConditionDefinition
        {
            entity.SetField("secondCharacterShaderColor", value);
            return entity;
        }

        public static T SetSilentWhenAdded<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("silentWhenAdded", value);
            return entity;
        }

        public static T SetSilentWhenRemoved<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("silentWhenRemoved", value);
            return entity;
        }

        public static T SetSourceAbilityBonusMinValue<T>(this T entity, System.Int32 value)
            where T : ConditionDefinition
        {
            entity.SetField("sourceAbilityBonusMinValue", value);
            return entity;
        }

        public static T SetSpecialDuration<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("specialDuration", value);
            return entity;
        }

        public static T SetSubsequentDCIncrease<T>(this T entity, System.Int32 value)
            where T : ConditionDefinition
        {
            entity.SetField("subsequentDCIncrease", value);
            return entity;
        }

        public static T SetSubsequentHasSavingThrow<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("subsequentHasSavingThrow", value);
            return entity;
        }

        public static T SetSubsequentOnRemoval<T>(this T entity, ConditionDefinition value)
            where T : ConditionDefinition
        {
            entity.SetField("subsequentOnRemoval", value);
            return entity;
        }

        public static T SetSubsequentSavingThrowAbilityScore<T>(this T entity, System.String value)
            where T : ConditionDefinition
        {
            entity.SetField("subsequentSavingThrowAbilityScore", value);
            return entity;
        }

        public static T SetSubsequentVariableForDC<T>(this T entity, System.String value)
            where T : ConditionDefinition
        {
            entity.SetField("subsequentVariableForDC", value);
            return entity;
        }

        public static T SetTerminateWhenRemoved<T>(this T entity, System.Boolean value)
            where T : ConditionDefinition
        {
            entity.SetField("terminateWhenRemoved", value);
            return entity;
        }

        public static T SetTimeToWaitBeforeApplyingShader<T>(this T entity, System.Single value)
            where T : ConditionDefinition
        {
            entity.SetField("timeToWaitBeforeApplyingShader", value);
            return entity;
        }

        public static T SetTimeToWaitBeforeRemovingShader<T>(this T entity, System.Single value)
            where T : ConditionDefinition
        {
            entity.SetField("timeToWaitBeforeRemovingShader", value);
            return entity;
        }

        public static T SetTurnOccurence<T>(this T entity, RuleDefinitions.TurnOccurenceType value)
            where T : ConditionDefinition
        {
            entity.SetField("turnOccurence", value);
            return entity;
        }
    }
}