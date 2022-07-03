using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
    [NotNull]
    public static T SetDuration<T>([NotNull] this T entity, DurationType type, int? duration = null)
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

    [NotNull]
    public static T SetRange<T>([NotNull] this T entity, RangeType type, int? range = null)
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

    [NotNull]
    public static T AddEffectForms<T>([NotNull] this T entity, params EffectForm[] value)
        where T : EffectDescription
    {
        AddEffectForms(entity, value.AsEnumerable());
        return entity;
    }

    [NotNull]
    public static T AddEffectForms<T>([NotNull] this T entity, [NotNull] IEnumerable<EffectForm> value)
        where T : EffectDescription
    {
        entity.EffectForms.AddRange(value);
        return entity;
    }

    [NotNull]
    public static T ClearRestrictedCreatureFamilies<T>([NotNull] this T entity)
        where T : EffectDescription
    {
        entity.RestrictedCreatureFamilies.Clear();
        return entity;
    }

    [NotNull]
    public static EffectDescription Copy(this EffectDescription entity)
    {
        var effectDescription = new EffectDescription();

        effectDescription.Copy(entity);

        return effectDescription;
    }

    [NotNull]
    public static T SetAnimationMagicEffect<T>([NotNull] this T entity, AnimationDefinitions.AnimationMagicEffect value)
        where T : EffectDescription
    {
        entity.animationMagicEffect = value;
        return entity;
    }

    [NotNull]
    public static T SetCanBeDispersed<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.canBeDispersed = value;
        return entity;
    }

    [NotNull]
    public static T SetCanBePlacedOnCharacter<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.canBePlacedOnCharacter = value;
        return entity;
    }

    [NotNull]
    public static T SetCreatedByCharacter<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.createdByCharacter = value;
        return entity;
    }

    [NotNull]
    public static T SetDifficultyClassComputation<T>([NotNull] this T entity, EffectDifficultyClassComputation value)
        where T : EffectDescription
    {
        entity.difficultyClassComputation = value;
        return entity;
    }

    [NotNull]
    public static T SetDisableSavingThrowOnAllies<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.disableSavingThrowOnAllies = value;
        return entity;
    }

    [NotNull]
    public static T SetDurationParameter<T>([NotNull] this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.DurationParameter = value;
        return entity;
    }

    [NotNull]
    public static T SetDurationType<T>([NotNull] this T entity, DurationType value)
        where T : EffectDescription
    {
        entity.DurationType = value;
        return entity;
    }

    [NotNull]
    public static T SetEffectAdvancement<T>([NotNull] this T entity, EffectAdvancement value)
        where T : EffectDescription
    {
        entity.effectAdvancement = value;
        return entity;
    }

    [NotNull]
    public static T SetEffectAIParameters<T>([NotNull] this T entity, EffectAIParameters value)
        where T : EffectDescription
    {
        entity.effectAIParameters = value;
        return entity;
    }

    [NotNull]
    public static T SetEffectForms<T>([NotNull] this T entity, params EffectForm[] value)
        where T : EffectDescription
    {
        SetEffectForms(entity, value.AsEnumerable());
        return entity;
    }

    [NotNull]
    public static T SetEffectForms<T>([NotNull] this T entity, IEnumerable<EffectForm> value)
        where T : EffectDescription
    {
        entity.EffectForms.SetRange(value);
        return entity;
    }

    [NotNull]
    public static T SetEffectParticleParameters<T>([NotNull] this T entity, EffectParticleParameters value)
        where T : EffectDescription
    {
        entity.effectParticleParameters = value;
        return entity;
    }

    [NotNull]
    public static T SetEffectPoolAmount<T>([NotNull] this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.effectPoolAmount = value;
        return entity;
    }

    [NotNull]
    public static T SetEmissiveBorder<T>([NotNull] this T entity, EmissiveBorder value)
        where T : EffectDescription
    {
        entity.emissiveBorder = value;
        return entity;
    }

    [NotNull]
    public static T SetEmissiveParameter<T>([NotNull] this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.emissiveParameter = value;
        return entity;
    }

    [NotNull]
    public static T SetEndOfEffect<T>([NotNull] this T entity, TurnOccurenceType value)
        where T : EffectDescription
    {
        entity.EndOfEffect = value;
        return entity;
    }

    [NotNull]
    public static T SetFixedSavingThrowDifficultyClass<T>([NotNull] this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.FixedSavingThrowDifficultyClass = value;
        return entity;
    }

    [NotNull]
    public static T SetHalfDamageOnAMiss<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.halfDamageOnAMiss = value;
        return entity;
    }

    [NotNull]
    public static T SetHasLimitedEffectPool<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.hasLimitedEffectPool = value;
        return entity;
    }

    [NotNull]
    public static T SetHasSavingThrow<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.HasSavingThrow = value;
        return entity;
    }

    [NotNull]
    public static T SetHasShoveRoll<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.hasShoveRoll = value;
        return entity;
    }

    [NotNull]
    public static T SetHasVelocity<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.hasVelocity = value;
        return entity;
    }

    [NotNull]
    public static T SetInviteOptionalAlly<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.inviteOptionalAlly = value;
        return entity;
    }

    [NotNull]
    public static T SetItemSelectionType<T>([NotNull] this T entity, ActionDefinitions.ItemSelectionType value)
        where T : EffectDescription
    {
        entity.itemSelectionType = value;
        return entity;
    }

    [NotNull]
    public static T SetOffsetImpactTimeBasedOnDistance<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.offsetImpactTimeBasedOnDistance = value;
        return entity;
    }

    [NotNull]
    public static T SetOffsetImpactTimeBasedOnDistanceFactor<T>([NotNull] this T entity, Single value)
        where T : EffectDescription
    {
        entity.offsetImpactTimeBasedOnDistanceFactor = value;
        return entity;
    }

    [NotNull]
    public static T SetOffsetImpactTimePerTarget<T>([NotNull] this T entity, Single value)
        where T : EffectDescription
    {
        entity.offsetImpactTimePerTarget = value;
        return entity;
    }

    [NotNull]
    public static T SetPoolFilterDiceNumber<T>([NotNull] this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.poolFilterDiceNumber = value;
        return entity;
    }

    [NotNull]
    public static T SetPoolFilterDieType<T>([NotNull] this T entity, DieType value)
        where T : EffectDescription
    {
        entity.poolFilterDieType = value;
        return entity;
    }

    [NotNull]
    public static T SetRangeParameter<T>([NotNull] this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.rangeParameter = value;
        return entity;
    }

    [NotNull]
    public static T SetRangeType<T>([NotNull] this T entity, RangeType value)
        where T : EffectDescription
    {
        entity.RangeType = value;
        return entity;
    }

    [NotNull]
    public static T SetRecurrentEffect<T>([NotNull] this T entity, RecurrentEffect value)
        where T : EffectDescription
    {
        entity.recurrentEffect = value;
        return entity;
    }

    [NotNull]
    public static T SetRequiresTargetProximity<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.requiresTargetProximity = value;
        return entity;
    }

    [NotNull]
    public static T SetRequiresVisibilityForPosition<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.requiresVisibilityForPosition = value;
        return entity;
    }

    [NotNull]
    public static T SetRetargetActionType<T>([NotNull] this T entity, ActionDefinitions.ActionType value)
        where T : EffectDescription
    {
        entity.retargetActionType = value;
        return entity;
    }

    [NotNull]
    public static T SetRetargetAfterDeath<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.retargetAfterDeath = value;
        return entity;
    }

    [NotNull]
    public static T SetSavingThrowAbility<T>([NotNull] this T entity, String value)
        where T : EffectDescription
    {
        entity.SavingThrowAbility = value;
        return entity;
    }

    [NotNull]
    public static T SetSavingThrowDifficultyAbility<T>([NotNull] this T entity, String value)
        where T : EffectDescription
    {
        entity.SavingThrowDifficultyAbility = value;
        return entity;
    }

    [NotNull]
    public static T SetSpeedParameter<T>([NotNull] this T entity, Single value)
        where T : EffectDescription
    {
        entity.speedParameter = value;
        return entity;
    }

    [NotNull]
    public static T SetSpeedType<T>([NotNull] this T entity, SpeedType value)
        where T : EffectDescription
    {
        entity.speedType = value;
        return entity;
    }

    [NotNull]
    public static T SetTargetConditionAsset<T>([NotNull] this T entity, ConditionDefinition value)
        where T : EffectDescription
    {
        entity.targetConditionAsset = value;
        return entity;
    }

    [NotNull]
    public static T SetTargetConditionName<T>([NotNull] this T entity, String value)
        where T : EffectDescription
    {
        entity.targetConditionName = value;
        return entity;
    }

    [NotNull]
    public static T SetTargetExcludeCaster<T>([NotNull] this T entity, Boolean value)
        where T : EffectDescription
    {
        entity.targetExcludeCaster = value;
        return entity;
    }

    [NotNull]
    public static T SetTargetFilteringMethod<T>([NotNull] this T entity, TargetFilteringMethod value)
        where T : EffectDescription
    {
        entity.targetFilteringMethod = value;
        return entity;
    }

    [NotNull]
    public static T SetTargetFilteringTag<T>([NotNull] this T entity, TargetFilteringTag value)
        where T : EffectDescription
    {
        entity.targetFilteringTag = value;
        return entity;
    }

    [NotNull]
    public static T SetTargetParameter<T>([NotNull] this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.targetParameter = value;
        return entity;
    }

    [NotNull]
    public static T SetTargetParameter2<T>([NotNull] this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.targetParameter2 = value;
        return entity;
    }

    [NotNull]
    public static T SetTargetProximityDistance<T>([NotNull] this T entity, Int32 value)
        where T : EffectDescription
    {
        entity.targetProximityDistance = value;
        return entity;
    }

    [NotNull]
    public static T SetTargetSide<T>([NotNull] this T entity, Side value)
        where T : EffectDescription
    {
        entity.TargetSide = value;
        return entity;
    }

    [NotNull]
    public static T SetTargetType<T>([NotNull] this T entity, TargetType value)
        where T : EffectDescription
    {
        entity.TargetType = value;
        return entity;
    }
}
