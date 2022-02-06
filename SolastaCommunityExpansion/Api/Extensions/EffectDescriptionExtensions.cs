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
    [TargetType(typeof(EffectDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class EffectDescriptionExtensions
    {
        public static T SetAdvantageForEnemies<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.AdvantageForEnemies = value;
            return entity;
        }

        public static T SetAnimationMagicEffect<T>(this T entity, AnimationDefinitions.AnimationMagicEffect value)
            where T : EffectDescription
        {
            entity.SetField("animationMagicEffect", value);
            return entity;
        }

        public static T SetCanBeDispersed<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.SetField("canBeDispersed", value);
            return entity;
        }

        public static T SetCanBePlacedOnCharacter<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.SetField("canBePlacedOnCharacter", value);
            return entity;
        }

        public static T SetCreatedByCharacter<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.SetField("createdByCharacter", value);
            return entity;
        }

        public static T SetDifficultyClassComputation<T>(this T entity, RuleDefinitions.EffectDifficultyClassComputation value)
            where T : EffectDescription
        {
            entity.SetField("difficultyClassComputation", value);
            return entity;
        }

        public static T SetDisableSavingThrowOnAllies<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.SetField("disableSavingThrowOnAllies", value);
            return entity;
        }

        public static T SetDurationParameter<T>(this T entity, System.Int32 value)
            where T : EffectDescription
        {
            entity.DurationParameter = value;
            return entity;
        }

        public static T SetDurationType<T>(this T entity, RuleDefinitions.DurationType value)
            where T : EffectDescription
        {
            entity.DurationType = value;
            return entity;
        }

        public static T SetEffectAdvancement<T>(this T entity, EffectAdvancement value)
            where T : EffectDescription
        {
            entity.SetField("effectAdvancement", value);
            return entity;
        }

        public static T SetEffectAIParameters<T>(this T entity, EffectAIParameters value)
            where T : EffectDescription
        {
            entity.SetField("effectAIParameters", value);
            return entity;
        }

        public static T SetEffectParticleParameters<T>(this T entity, EffectParticleParameters value)
            where T : EffectDescription
        {
            entity.SetField("effectParticleParameters", value);
            return entity;
        }

        public static T SetEffectPoolAmount<T>(this T entity, System.Int32 value)
            where T : EffectDescription
        {
            entity.SetField("effectPoolAmount", value);
            return entity;
        }

        public static T SetEmissiveBorder<T>(this T entity, RuleDefinitions.EmissiveBorder value)
            where T : EffectDescription
        {
            entity.SetField("emissiveBorder", value);
            return entity;
        }

        public static T SetEmissiveParameter<T>(this T entity, System.Int32 value)
            where T : EffectDescription
        {
            entity.SetField("emissiveParameter", value);
            return entity;
        }

        public static T SetEndOfEffect<T>(this T entity, RuleDefinitions.TurnOccurenceType value)
            where T : EffectDescription
        {
            entity.EndOfEffect = value;
            return entity;
        }

        public static T SetFixedSavingThrowDifficultyClass<T>(this T entity, System.Int32 value)
            where T : EffectDescription
        {
            entity.FixedSavingThrowDifficultyClass = value;
            return entity;
        }

        public static T SetGrantedConditionOnSave<T>(this T entity, ConditionDefinition value)
            where T : EffectDescription
        {
            entity.GrantedConditionOnSave = value;
            return entity;
        }

        public static T SetHalfDamageOnAMiss<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.SetField("halfDamageOnAMiss", value);
            return entity;
        }

        public static T SetHasLimitedEffectPool<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.SetField("hasLimitedEffectPool", value);
            return entity;
        }

        public static T SetHasSavingThrow<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.HasSavingThrow = value;
            return entity;
        }

        public static T SetHasShoveRoll<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.SetField("hasShoveRoll", value);
            return entity;
        }

        public static T SetHasVelocity<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.SetField("hasVelocity", value);
            return entity;
        }

        public static T SetIgnoreCover<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.IgnoreCover = value;
            return entity;
        }

        public static T SetInviteOptionalAlly<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.SetField("inviteOptionalAlly", value);
            return entity;
        }

        public static T SetItemSelectionType<T>(this T entity, ActionDefinitions.ItemSelectionType value)
            where T : EffectDescription
        {
            entity.SetField("itemSelectionType", value);
            return entity;
        }

        public static T SetOffsetImpactTimeBasedOnDistance<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.SetField("offsetImpactTimeBasedOnDistance", value);
            return entity;
        }

        public static T SetOffsetImpactTimeBasedOnDistanceFactor<T>(this T entity, System.Single value)
            where T : EffectDescription
        {
            entity.SetField("offsetImpactTimeBasedOnDistanceFactor", value);
            return entity;
        }

        public static T SetOffsetImpactTimePerTarget<T>(this T entity, System.Single value)
            where T : EffectDescription
        {
            entity.SetField("offsetImpactTimePerTarget", value);
            return entity;
        }

        public static T SetPoolFilterDiceNumber<T>(this T entity, System.Int32 value)
            where T : EffectDescription
        {
            entity.SetField("poolFilterDiceNumber", value);
            return entity;
        }

        public static T SetPoolFilterDieType<T>(this T entity, RuleDefinitions.DieType value)
            where T : EffectDescription
        {
            entity.SetField("poolFilterDieType", value);
            return entity;
        }

        public static T SetRangeParameter<T>(this T entity, System.Int32 value)
            where T : EffectDescription
        {
            entity.SetField("rangeParameter", value);
            return entity;
        }

        public static T SetRangeType<T>(this T entity, RuleDefinitions.RangeType value)
            where T : EffectDescription
        {
            entity.RangeType = value;
            return entity;
        }

        public static T SetRecurrentEffect<T>(this T entity, RuleDefinitions.RecurrentEffect value)
            where T : EffectDescription
        {
            entity.SetField("recurrentEffect", value);
            return entity;
        }

        public static T SetRequiresTargetProximity<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.SetField("requiresTargetProximity", value);
            return entity;
        }

        public static T SetRequiresVisibilityForPosition<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.SetField("requiresVisibilityForPosition", value);
            return entity;
        }

        public static T SetRetargetActionType<T>(this T entity, ActionDefinitions.ActionType value)
            where T : EffectDescription
        {
            entity.SetField("retargetActionType", value);
            return entity;
        }

        public static T SetRetargetAfterDeath<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.SetField("retargetAfterDeath", value);
            return entity;
        }

        public static T SetSavingThrowAbility<T>(this T entity, System.String value)
            where T : EffectDescription
        {
            entity.SavingThrowAbility = value;
            return entity;
        }

        public static T SetSavingThrowDifficultyAbility<T>(this T entity, System.String value)
            where T : EffectDescription
        {
            entity.SavingThrowDifficultyAbility = value;
            return entity;
        }

        public static T SetSpeedParameter<T>(this T entity, System.Single value)
            where T : EffectDescription
        {
            entity.SetField("speedParameter", value);
            return entity;
        }

        public static T SetSpeedType<T>(this T entity, RuleDefinitions.SpeedType value)
            where T : EffectDescription
        {
            entity.SetField("speedType", value);
            return entity;
        }

        public static T SetTargetConditionAsset<T>(this T entity, ConditionDefinition value)
            where T : EffectDescription
        {
            entity.SetField("targetConditionAsset", value);
            return entity;
        }

        public static T SetTargetConditionName<T>(this T entity, System.String value)
            where T : EffectDescription
        {
            entity.SetField("targetConditionName", value);
            return entity;
        }

        public static T SetTargetExcludeCaster<T>(this T entity, System.Boolean value)
            where T : EffectDescription
        {
            entity.SetField("targetExcludeCaster", value);
            return entity;
        }

        public static T SetTargetFilteringMethod<T>(this T entity, RuleDefinitions.TargetFilteringMethod value)
            where T : EffectDescription
        {
            entity.SetField("targetFilteringMethod", value);
            return entity;
        }

        public static T SetTargetFilteringTag<T>(this T entity, RuleDefinitions.TargetFilteringTag value)
            where T : EffectDescription
        {
            entity.SetField("targetFilteringTag", value);
            return entity;
        }

        public static T SetTargetParameter<T>(this T entity, System.Int32 value)
            where T : EffectDescription
        {
            entity.SetField("targetParameter", value);
            return entity;
        }

        public static T SetTargetParameter2<T>(this T entity, System.Int32 value)
            where T : EffectDescription
        {
            entity.SetField("targetParameter2", value);
            return entity;
        }

        public static T SetTargetProximityDistance<T>(this T entity, System.Int32 value)
            where T : EffectDescription
        {
            entity.SetField("targetProximityDistance", value);
            return entity;
        }

        public static T SetTargetSide<T>(this T entity, RuleDefinitions.Side value)
            where T : EffectDescription
        {
            entity.TargetSide = value;
            return entity;
        }

        public static T SetTargetType<T>(this T entity, RuleDefinitions.TargetType value)
            where T : EffectDescription
        {
            entity.TargetType = value;
            return entity;
        }

        public static T SetTrapRangeType<T>(this T entity, RuleDefinitions.TrapRangeType value)
            where T : EffectDescription
        {
            entity.SetField("trapRangeType", value);
            return entity;
        }

        public static T SetVelocityCellsPerRound<T>(this T entity, System.Int32 value)
            where T : EffectDescription
        {
            entity.SetField("velocityCellsPerRound", value);
            return entity;
        }

        public static T SetVelocityType<T>(this T entity, RuleDefinitions.VelocityType value)
            where T : EffectDescription
        {
            entity.SetField("velocityType", value);
            return entity;
        }
    }
}