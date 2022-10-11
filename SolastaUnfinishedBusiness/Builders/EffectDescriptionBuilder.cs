using System;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Diagnostics;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

internal class EffectDescriptionBuilder
{
    private readonly EffectDescription effect;

    private EffectDescriptionBuilder()
    {
        effect = new EffectDescription
        {
            effectAdvancement = new EffectAdvancement { incrementMultiplier = 1 },
            effectParticleParameters =
                DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription.EffectParticleParameters
        };
    }

    private EffectDescriptionBuilder(EffectDescription effect)
    {
        this.effect = new EffectDescription();
        this.effect.Copy(effect);
    }

    internal EffectDescription Build()
    {
        return effect;
    }

    internal static EffectDescriptionBuilder Create()
    {
        return new EffectDescriptionBuilder();
    }

    internal static EffectDescriptionBuilder Create(EffectDescription effect)
    {
        return new EffectDescriptionBuilder(effect);
    }

    internal EffectDescriptionBuilder ClearEffectAdvancements()
    {
        effect.effectAdvancement.Clear();
        return this;
    }

    internal EffectDescriptionBuilder SetTargetParameter2(Int32 value)
    {
        effect.targetParameter2 = value;
        return this;
    }

    internal EffectDescriptionBuilder ClearRestrictedCreatureFamilies()
    {
        effect.RestrictedCreatureFamilies.Clear();
        return this;
    }

    internal EffectDescriptionBuilder SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect value)
    {
        effect.animationMagicEffect = value;
        return this;
    }

    internal EffectDescriptionBuilder SetCreatedByCharacter()
    {
        effect.createdByCharacter = true;
        return this;
    }

    internal EffectDescriptionBuilder SetCanBePlacedOnCharacter(bool value)
    {
        effect.canBePlacedOnCharacter = value;
        return this;
    }

    internal EffectDescriptionBuilder SetDifficultyClassComputation(EffectDifficultyClassComputation value)
    {
        effect.difficultyClassComputation = value;
        return this;
    }

    internal EffectDescriptionBuilder SetDuration(DurationType type, int? duration = null)
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
                        $@"A duration value is required for duration type {type}.");
                }

                effect.durationParameter = duration.Value;
                break;

            case DurationType.Instantaneous:
            case DurationType.Dispelled:
            case DurationType.Permanent:
            case DurationType.Irrelevant:
            case DurationType.UntilShortRest:
            case DurationType.UntilLongRest:
            case DurationType.UntilAnyRest:
            case DurationType.Deprecated_Turn:
            case DurationType.HalfClassLevelHours:
            default:
                if (duration != null)
                {
                    throw new SolastaUnfinishedBusinessException(
                        $"A duration value is not expected for duration type {type}");
                }

                effect.durationParameter = 0;
                break;
        }

        effect.durationType = type;
        return this;
    }

    internal EffectDescriptionBuilder SetParticleEffectParameters(EffectParticleParameters particleParameters)
    {
        effect.effectParticleParameters = particleParameters;
        return this;
    }

    internal EffectDescriptionBuilder SetParticleEffectParameters(IMagicEffect reference)
    {
        effect.effectParticleParameters = reference.EffectDescription.EffectParticleParameters;
        return this;
    }

    internal EffectDescriptionBuilder SetEffectAdvancement(EffectIncrementMethod effectIncrementMethod,
        int incrementMultiplier = 1,
        int additionalTargetsPerIncrement = 0,
        int additionalDicePerIncrement = 0,
        int additionalSpellLevelPerIncrement = 0,
        int additionalSummonsPerIncrement = 0,
        int additionalHpPerIncrement = 0,
        int additionalTempHpPerIncrement = 0,
        int additionalTargetCellsPerIncrement = 0,
        int additionalItemBonus = 0,
        AdvancementDuration alteredDuration = AdvancementDuration.None)
    {
        effect.effectAdvancement = new EffectAdvancement
        {
            effectIncrementMethod = effectIncrementMethod,
            incrementMultiplier = incrementMultiplier,
            additionalTargetsPerIncrement = additionalTargetsPerIncrement,
            additionalDicePerIncrement = additionalDicePerIncrement,
            additionalSpellLevelPerIncrement = additionalSpellLevelPerIncrement,
            additionalSummonsPerIncrement = additionalSummonsPerIncrement,
            additionalHPPerIncrement = additionalHpPerIncrement,
            additionalTempHPPerIncrement = additionalTempHpPerIncrement,
            additionalTargetCellsPerIncrement = additionalTargetCellsPerIncrement,
            additionalItemBonus = additionalItemBonus,
            alteredDuration = alteredDuration
        };
        return this;
    }

    internal EffectDescriptionBuilder SetRange(RangeType type, int? range = null)
    {
        switch (type)
        {
            case RangeType.RangeHit:
            case RangeType.Distance:
                if (range == null)
                {
                    throw new ArgumentNullException(nameof(range),
                        $@"A range value is required for range type {type}.");
                }

                effect.rangeParameter = range.Value;
                break;

            case RangeType.Touch:
                effect.rangeParameter = range ?? 0;
                break;

            case RangeType.Self:
            case RangeType.MeleeHit:
            default:
                if (range != null)
                {
                    throw new SolastaUnfinishedBusinessException(
                        $"A duration value is not expected for duration type {type}");
                }

                effect.rangeParameter = 0;
                break;
        }

        effect.rangeType = type;
        return this;
    }

    internal EffectDescriptionBuilder SetTargetingData(
        Side targetSide,
        RangeType rangeType,
        int rangeParameter,
        TargetType targetType,
        int targetParameter = 1,
        int targetParameter2 = 1,
        ActionDefinitions.ItemSelectionType itemSelectionType = ActionDefinitions.ItemSelectionType.None)
    {
        effect.targetSide = targetSide;
        effect.rangeType = rangeType;
        effect.rangeParameter = rangeParameter;
        effect.targetType = targetType;
        effect.targetParameter = targetParameter;
        effect.targetParameter2 = targetParameter2;
        effect.itemSelectionType = itemSelectionType;
        return this;
    }

    internal EffectDescriptionBuilder SetSlotTypes(params string[] slots)
    {
        effect.slotTypes.SetRange(slots);
        return this;
    }

    internal EffectDescriptionBuilder SetSlotTypes(params SlotTypeDefinition[] slots)
    {
        effect.slotTypes.SetRange(slots.Select(s => s.Name));
        return this;
    }

    internal EffectDescriptionBuilder ExcludeCaster()
    {
        effect.targetExcludeCaster = true;
        return this;
    }

    internal EffectDescriptionBuilder SetTargetProximityData(
        bool requiresTargetProximity,
        int targetProximityDistance)
    {
        effect.requiresTargetProximity = requiresTargetProximity;
        effect.targetProximityDistance = targetProximityDistance;
        return this;
    }

    internal EffectDescriptionBuilder SetTargetFiltering(
        TargetFilteringMethod targetFilteringMethod,
        TargetFilteringTag targetFilteringTag = TargetFilteringTag.No,
        int poolFilterDiceNumber = 0,
        DieType poolFilterDieType = DieType.D1
    )
    {
        effect.targetFilteringMethod = targetFilteringMethod;
        effect.targetFilteringTag = targetFilteringTag;
        effect.poolFilterDiceNumber = poolFilterDiceNumber;
        effect.poolFilterDieType = poolFilterDieType;
        return this;
    }

    internal EffectDescriptionBuilder SetRecurrentEffect(RecurrentEffect recurrentEffect)
    {
        effect.recurrentEffect = recurrentEffect;
        return this;
    }

    internal EffectDescriptionBuilder SetRequiredCondition(ConditionDefinition targetConditionAsset)
    {
        effect.targetConditionAsset = targetConditionAsset;
        effect.targetConditionName = targetConditionAsset.Name;
        return this;
    }

    internal EffectDescriptionBuilder SetDurationData(
        DurationType durationType,
        int durationParameter,
        TurnOccurenceType endOfEffect)
    {
        effect.durationParameter = durationParameter;
        effect.durationType = durationType;
        effect.endOfEffect = endOfEffect;
        return this;
    }

    internal EffectDescriptionBuilder SetDurationData(
        DurationType type,
        int duration = 0,
        bool validate = true)
    {
        if (validate)
        {
            Preconditions.IsValidDuration(type, duration);
        }

        effect.durationParameter = duration;
        effect.durationType = type;
        return this;
    }

    internal EffectDescriptionBuilder SetSavingThrowData(
        bool hasSavingThrow,
        bool disableSavingThrowOnAllies,
        string savingThrowAbility,
        bool ignoreCover,
        EffectDifficultyClassComputation difficultyClassComputation,
        string savingThrowDifficultyAbility = AttributeDefinitions.Wisdom,
        int fixedSavingThrowDifficultyClass = 10,
        bool advantageForEnemies = false,
        params SaveAffinityBySenseDescription[] savingThrowAffinitiesBySense)
    {
        effect.hasSavingThrow = hasSavingThrow;
        effect.disableSavingThrowOnAllies = disableSavingThrowOnAllies;
        effect.savingThrowAbility = savingThrowAbility;
        effect.ignoreCover = ignoreCover;
        effect.difficultyClassComputation = difficultyClassComputation;
        effect.savingThrowDifficultyAbility = savingThrowDifficultyAbility;
        effect.fixedSavingThrowDifficultyClass = fixedSavingThrowDifficultyClass;
        effect.advantageForEnemies = advantageForEnemies;
        effect.savingThrowAffinitiesBySense.SetRange(savingThrowAffinitiesBySense);
        return this;
    }

    internal EffectDescriptionBuilder AddImmuneCreatureFamilies(params CharacterFamilyDefinition[] families)
    {
        effect.ImmuneCreatureFamilies.AddRange(families.Select(f => f.Name));
        return this;
    }

    internal EffectDescriptionBuilder SetSpeed(SpeedType speedType, float speedParameter = 0f)
    {
        effect.speedType = speedType;
        effect.speedParameter = speedParameter;
        return this;
    }

    internal EffectDescriptionBuilder SetAnimation(AnimationDefinitions.AnimationMagicEffect animation)
    {
        effect.animationMagicEffect = animation;
        return this;
    }

    internal EffectDescriptionBuilder SetRequiresVisibilityForPosition(Boolean value)
    {
        effect.requiresVisibilityForPosition = value;
        return this;
    }

    internal EffectDescriptionBuilder AddEffectForm(EffectForm effectForm)
    {
        effect.EffectForms.Add(effectForm);
        return this;
    }

    internal EffectDescriptionBuilder AddEffectForms(params EffectForm[] effectForms)
    {
        effect.EffectForms.AddRange(effectForms);
        return this;
    }

    internal EffectDescriptionBuilder SetEffectForms(params EffectForm[] effectForms)
    {
        effect.EffectForms.SetRange(effectForms);
        return this;
    }

    internal EffectDescriptionBuilder ClearEffectForms()
    {
        effect.EffectForms.Clear();
        return this;
    }
}
