using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api.Diagnostics;
using SolastaCommunityExpansion.Api.Infrastructure;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Api.Extensions;

#if DEBUG
[TargetType(typeof(EffectDescription))]
#endif
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class EffectDescriptionExtensions
{
    public static T SetDuration<T>(this T entity, DurationType type, int? duration = null)
        where T : EffectDescription
    {
        switch (type)
        {
            case DurationType.Round:
            case DurationType.Minute:
            case DurationType.Hour:
            case DurationType.Day:
                if (duration == null)
                {
                    throw new ArgumentNullException(nameof(duration),
                        $"A duration value is required for duration type {type}.");
                }

                entity.SetDurationParameter(duration.Value);
                break;

            default:
                if (duration != null)
                {
                    throw new SolastaCommunityExpansionException(
                        $"A duration value is not expected for duration type {type}");
                }

                entity.SetDurationParameter(0);
                break;
        }

        entity.SetDurationType(type);

        return entity;
    }

    public static T SetRange<T>(this T entity, RangeType type, int? range = null)
        where T : EffectDescription
    {
        switch (type)
        {
            case RangeType.RangeHit:
            case RangeType.Distance:
                if (range == null)
                {
                    throw new ArgumentNullException(nameof(range),
                        $"A range value is required for range type {type}.");
                }

                entity.SetRangeParameter(range.Value);
                break;

            case RangeType.Touch:
                entity.SetRangeParameter(range ?? 0);
                break;

            default: // Self, MeleeHit
                if (range != null)
                {
                    throw new SolastaCommunityExpansionException(
                        $"A duration value is not expected for duration type {type}");
                }

                entity.SetRangeParameter(0);
                break;
        }

        entity.SetRangeType(type);

        return entity;
    }

    public static T AddEffectForms<T>(this T entity, params EffectForm[] value)
        where T : EffectDescription
    {
        AddEffectForms(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddEffectForms<T>(this T entity, IEnumerable<EffectForm> value)
        where T : EffectDescription
    {
        entity.EffectForms.AddRange(value);
        return entity;
    }

    public static T ClearRestrictedCreatureFamilies<T>(this T entity)
        where T : EffectDescription
    {
        entity.RestrictedCreatureFamilies.Clear();
        return entity;
    }

    public static EffectDescription Copy(this EffectDescription entity)
    {
        var effectDescription = new EffectDescription();

        effectDescription.Copy(entity);

        return effectDescription;
    }

    public static T SetAnimationMagicEffect<T>(this T entity, AnimationDefinitions.AnimationMagicEffect value)
        where T : EffectDescription
    {
        entity.animationMagicEffect = value;
        return entity;
    }

    public static T SetCanBeDispersed<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.canBeDispersed = value;
        return entity;
    }

    public static T SetCanBePlacedOnCharacter<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.canBePlacedOnCharacter = value;
        return entity;
    }

    public static T SetCreatedByCharacter<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.createdByCharacter = value;
        return entity;
    }

    public static T SetDifficultyClassComputation<T>(this T entity, EffectDifficultyClassComputation value)
        where T : EffectDescription
    {
        entity.difficultyClassComputation = value;
        return entity;
    }

    public static T SetDisableSavingThrowOnAllies<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.disableSavingThrowOnAllies = value;
        return entity;
    }

    public static T SetDurationParameter<T>(this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.DurationParameter = value;
        return entity;
    }

    public static T SetDurationType<T>(this T entity, DurationType value)
        where T : EffectDescription
    {
        entity.DurationType = value;
        return entity;
    }

    public static T SetEffectAdvancement<T>(this T entity, EffectAdvancement value)
        where T : EffectDescription
    {
        entity.effectAdvancement = value;
        return entity;
    }

    public static T SetEffectAIParameters<T>(this T entity, EffectAIParameters value)
        where T : EffectDescription
    {
        entity.effectAIParameters = value;
        return entity;
    }

    public static T SetEffectForms<T>(this T entity, params EffectForm[] value)
        where T : EffectDescription
    {
        SetEffectForms(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetEffectForms<T>(this T entity, IEnumerable<EffectForm> value)
        where T : EffectDescription
    {
        entity.EffectForms.SetRange(value);
        return entity;
    }

    public static T SetEffectParticleParameters<T>(this T entity, EffectParticleParameters value)
        where T : EffectDescription
    {
        entity.effectParticleParameters = value;
        return entity;
    }

    public static T SetEffectPoolAmount<T>(this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.effectPoolAmount = value;
        return entity;
    }

    public static T SetEmissiveBorder<T>(this T entity, EmissiveBorder value)
        where T : EffectDescription
    {
        entity.emissiveBorder = value;
        return entity;
    }

    public static T SetEmissiveParameter<T>(this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.emissiveParameter = value;
        return entity;
    }

    public static T SetEndOfEffect<T>(this T entity, TurnOccurenceType value)
        where T : EffectDescription
    {
        entity.EndOfEffect = value;
        return entity;
    }

    public static T SetFixedSavingThrowDifficultyClass<T>(this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.FixedSavingThrowDifficultyClass = value;
        return entity;
    }

    public static T SetHalfDamageOnAMiss<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.halfDamageOnAMiss = value;
        return entity;
    }

    public static T SetHasLimitedEffectPool<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.hasLimitedEffectPool = value;
        return entity;
    }

    public static T SetHasSavingThrow<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.HasSavingThrow = value;
        return entity;
    }

    public static T SetHasShoveRoll<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.hasShoveRoll = value;
        return entity;
    }

    public static T SetHasVelocity<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.hasVelocity = value;
        return entity;
    }

    public static T SetInviteOptionalAlly<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.inviteOptionalAlly = value;
        return entity;
    }

    public static T SetItemSelectionType<T>(this T entity, ActionDefinitions.ItemSelectionType value)
        where T : EffectDescription
    {
        entity.itemSelectionType = value;
        return entity;
    }

    public static T SetOffsetImpactTimeBasedOnDistance<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.offsetImpactTimeBasedOnDistance = value;
        return entity;
    }

    public static T SetOffsetImpactTimeBasedOnDistanceFactor<T>(this T entity, Single value)
        where T : EffectDescription
    {
        entity.offsetImpactTimeBasedOnDistanceFactor = value;
        return entity;
    }

    public static T SetOffsetImpactTimePerTarget<T>(this T entity, Single value)
        where T : EffectDescription
    {
        entity.offsetImpactTimePerTarget = value;
        return entity;
    }

    public static T SetPoolFilterDiceNumber<T>(this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.poolFilterDiceNumber = value;
        return entity;
    }

    public static T SetPoolFilterDieType<T>(this T entity, DieType value)
        where T : EffectDescription
    {
        entity.poolFilterDieType = value;
        return entity;
    }

    public static T SetRangeParameter<T>(this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.rangeParameter = value;
        return entity;
    }

    public static T SetRangeType<T>(this T entity, RangeType value)
        where T : EffectDescription
    {
        entity.RangeType = value;
        return entity;
    }

    public static T SetRecurrentEffect<T>(this T entity, RecurrentEffect value)
        where T : EffectDescription
    {
        entity.recurrentEffect = value;
        return entity;
    }

    public static T SetRequiresTargetProximity<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.requiresTargetProximity = value;
        return entity;
    }

    public static T SetRequiresVisibilityForPosition<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.requiresVisibilityForPosition = value;
        return entity;
    }

    public static T SetRetargetActionType<T>(this T entity, ActionDefinitions.ActionType value)
        where T : EffectDescription
    {
        entity.retargetActionType = value;
        return entity;
    }

    public static T SetRetargetAfterDeath<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.retargetAfterDeath = value;
        return entity;
    }

    public static T SetSavingThrowAbility<T>(this T entity, String value)
        where T : EffectDescription
    {
        entity.SavingThrowAbility = value;
        return entity;
    }

    public static T SetSavingThrowDifficultyAbility<T>(this T entity, String value)
        where T : EffectDescription
    {
        entity.SavingThrowDifficultyAbility = value;
        return entity;
    }

    public static T SetSpeedParameter<T>(this T entity, Single value)
        where T : EffectDescription
    {
        entity.speedParameter = value;
        return entity;
    }

    public static T SetSpeedType<T>(this T entity, SpeedType value)
        where T : EffectDescription
    {
        entity.speedType = value;
        return entity;
    }

    public static T SetTargetConditionAsset<T>(this T entity, ConditionDefinition value)
        where T : EffectDescription
    {
        entity.targetConditionAsset = value;
        return entity;
    }

    public static T SetTargetConditionName<T>(this T entity, String value)
        where T : EffectDescription
    {
        entity.targetConditionName = value;
        return entity;
    }

    public static T SetTargetExcludeCaster<T>(this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.targetExcludeCaster = value;
        return entity;
    }

    public static T SetTargetFilteringMethod<T>(this T entity, TargetFilteringMethod value)
        where T : EffectDescription
    {
        entity.targetFilteringMethod = value;
        return entity;
    }

    public static T SetTargetFilteringTag<T>(this T entity, TargetFilteringTag value)
        where T : EffectDescription
    {
        entity.targetFilteringTag = value;
        return entity;
    }

    public static T SetTargetParameter<T>(this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.targetParameter = value;
        return entity;
    }

    public static T SetTargetParameter2<T>(this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.targetParameter2 = value;
        return entity;
    }

    public static T SetTargetProximityDistance<T>(this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.targetProximityDistance = value;
        return entity;
    }

    public static T SetTargetSide<T>(this T entity, Side value)
        where T : EffectDescription
    {
        entity.TargetSide = value;
        return entity;
    }

    public static T SetTargetType<T>(this T entity, TargetType value)
        where T : EffectDescription
    {
        entity.TargetType = value;
        return entity;
    }
}
