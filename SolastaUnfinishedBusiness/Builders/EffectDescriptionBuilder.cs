using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders;

internal class EffectDescriptionBuilder
{
    private readonly EffectDescription effect;

    internal EffectDescriptionBuilder()
    {
        effect = new EffectDescription
        {
            effectAdvancement = new EffectAdvancement { incrementMultiplier = 1 },
            effectParticleParameters =
                DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription.EffectParticleParameters
        };
    }

    internal EffectDescriptionBuilder(EffectDescription effect)
    {
        this.effect = effect.Copy();
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

    internal EffectDescriptionBuilder SetCreatedByCharacter()
    {
        effect.createdByCharacter = true;
        return this;
    }

    internal EffectDescriptionBuilder SetCanBePlacedOnCharacter(bool value)
    {
        effect.canBePlacedOnCharacter = true;
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

    internal EffectDescriptionBuilder SetEffectAdvancement(
        RuleDefinitions.EffectIncrementMethod effectIncrementMethod,
        int incrementMultiplier = 1,
        int additionalTargetsPerIncrement = 0,
        int additionalDicePerIncrement = 0,
        int additionalSpellLevelPerIncrement = 0,
        int additionalSummonsPerIncrement = 0,
        int additionalHPPerIncrement = 0,
        int additionalTempHPPerIncrement = 0,
        int additionalTargetCellsPerIncrement = 0,
        int additionalItemBonus = 0,
        RuleDefinitions.AdvancementDuration alteredDuration = RuleDefinitions.AdvancementDuration.None)
    {
        effect.effectAdvancement = new EffectAdvancement
        {
            effectIncrementMethod = effectIncrementMethod,
            incrementMultiplier = incrementMultiplier,
            additionalTargetsPerIncrement = additionalTargetsPerIncrement,
            additionalDicePerIncrement = additionalDicePerIncrement,
            additionalSpellLevelPerIncrement = additionalSpellLevelPerIncrement,
            additionalSummonsPerIncrement = additionalSummonsPerIncrement,
            additionalHPPerIncrement = additionalHPPerIncrement,
            additionalTempHPPerIncrement = additionalTempHPPerIncrement,
            additionalTargetCellsPerIncrement = additionalTargetCellsPerIncrement,
            additionalItemBonus = additionalItemBonus,
            alteredDuration = alteredDuration
        };

        return this;
    }

    internal EffectDescriptionBuilder SetTargetingData(
        RuleDefinitions.Side targetSide,
        RuleDefinitions.RangeType rangeType,
        int rangeParameter,
        RuleDefinitions.TargetType targetType,
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

#if false
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
#endif

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
        RuleDefinitions.TargetFilteringMethod targetFilteringMethod,
        RuleDefinitions.TargetFilteringTag targetFilteringTag = RuleDefinitions.TargetFilteringTag.No,
        int poolFilterDiceNumber = 0,
        RuleDefinitions.DieType poolFilterDieType = RuleDefinitions.DieType.D1
    )
    {
        effect.targetFilteringMethod = targetFilteringMethod;
        effect.targetFilteringTag = targetFilteringTag;
        effect.poolFilterDiceNumber = poolFilterDiceNumber;
        effect.poolFilterDieType = poolFilterDieType;
        return this;
    }

    internal EffectDescriptionBuilder SetRecurrentEffect(RuleDefinitions.RecurrentEffect recurrentEffect)
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
        RuleDefinitions.DurationType durationType,
        int durationParameter,
        RuleDefinitions.TurnOccurenceType endOfEffect)
    {
        effect.durationParameter = durationParameter;
        effect.durationType = durationType;
        effect.endOfEffect = endOfEffect;
        return this;
    }

    internal EffectDescriptionBuilder SetDurationData(
        RuleDefinitions.DurationType type,
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
        RuleDefinitions.EffectDifficultyClassComputation difficultyClassComputation,
        string savingThrowDifficultyAbility = AttributeDefinitions.Wisdom,
        int fixedSavingThrowDifficultyClass = 10,
        bool advantageForEnemies = false,
        params SaveAffinityBySenseDescription[] savingThrowAffinitiesBySense)
    {
        return SetSavingThrowData(
            hasSavingThrow, disableSavingThrowOnAllies,
            savingThrowAbility,
            ignoreCover,
            difficultyClassComputation,
            savingThrowDifficultyAbility,
            fixedSavingThrowDifficultyClass,
            advantageForEnemies,
            savingThrowAffinitiesBySense.AsEnumerable());
    }

    internal EffectDescriptionBuilder SetSavingThrowData(
        bool hasSavingThrow,
        bool disableSavingThrowOnAllies,
        string savingThrowAbility,
        bool ignoreCover,
        RuleDefinitions.EffectDifficultyClassComputation difficultyClassComputation,
        string savingThrowDifficultyAbility,
        int fixedSavingThrowDifficultyClass,
        bool advantageForEnemies,
        IEnumerable<SaveAffinityBySenseDescription> savingThrowAffinitiesBySense)
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

    internal EffectDescriptionBuilder SetSpeed(RuleDefinitions.SpeedType speedType, float speedParameter)
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

    internal EffectDescription Build()
    {
        return effect;
    }
}
