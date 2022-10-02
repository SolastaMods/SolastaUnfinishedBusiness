using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders;

public class EffectDescriptionBuilder
{
    private readonly EffectDescription effect;

    public EffectDescriptionBuilder()
    {
        effect = new EffectDescription
        {
            effectAdvancement = new EffectAdvancement { incrementMultiplier = 1 },
            effectParticleParameters =
                DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription.EffectParticleParameters
        };
    }

    public EffectDescriptionBuilder(EffectDescription effect)
    {
        this.effect = effect.Copy();
    }

    public static EffectDescriptionBuilder Create()
    {
        return new EffectDescriptionBuilder();
    }

    public static EffectDescriptionBuilder Create(EffectDescription effect)
    {
        return new EffectDescriptionBuilder(effect);
    }

    public EffectDescriptionBuilder SetCreatedByCharacter()
    {
        effect.createdByCharacter = true;
        return this;
    }

    public EffectDescriptionBuilder SetCanBePlacedOnCharacter(bool value)
    {
        effect.canBePlacedOnCharacter = true;
        return this;
    }

    public EffectDescriptionBuilder SetParticleEffectParameters(EffectParticleParameters particleParameters)
    {
        effect.effectParticleParameters = particleParameters;
        return this;
    }

    public EffectDescriptionBuilder SetParticleEffectParameters(IMagicEffect reference)
    {
        effect.effectParticleParameters = reference.EffectDescription.EffectParticleParameters;
        return this;
    }

    public EffectDescriptionBuilder SetEffectAdvancement(
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

    public EffectDescriptionBuilder SetTargetingData(
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

    public EffectDescriptionBuilder SetSlotTypes(params string[] slots)
    {
        effect.slotTypes.SetRange(slots);
        return this;
    }

    public EffectDescriptionBuilder SetSlotTypes(params SlotTypeDefinition[] slots)
    {
        effect.slotTypes.SetRange(slots.Select(s => s.Name));
        return this;
    }

    public EffectDescriptionBuilder ExcludeCaster()
    {
        effect.targetExcludeCaster = true;
        return this;
    }

    public EffectDescriptionBuilder SetTargetProximityData(
        bool requiresTargetProximity,
        int targetProximityDistance)
    {
        effect.requiresTargetProximity = requiresTargetProximity;
        effect.targetProximityDistance = targetProximityDistance;
        return this;
    }

    public EffectDescriptionBuilder SetTargetFiltering(
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

    public EffectDescriptionBuilder SetRecurrentEffect(RuleDefinitions.RecurrentEffect recurrentEffect)
    {
        effect.recurrentEffect = recurrentEffect;
        return this;
    }

    public EffectDescriptionBuilder SetRequiredCondition(ConditionDefinition targetConditionAsset)
    {
        effect.targetConditionAsset = targetConditionAsset;
        effect.targetConditionName = targetConditionAsset.Name;
        return this;
    }

    public EffectDescriptionBuilder SetDurationData(
        RuleDefinitions.DurationType durationType,
        int durationParameter,
        RuleDefinitions.TurnOccurenceType endOfEffect)
    {
        effect.durationParameter = durationParameter;
        effect.durationType = durationType;
        effect.endOfEffect = endOfEffect;
        return this;
    }

    public EffectDescriptionBuilder SetDurationData(
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

    public EffectDescriptionBuilder SetSavingThrowData(
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

    public EffectDescriptionBuilder SetSavingThrowData(
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

    public EffectDescriptionBuilder AddImmuneCreatureFamilies(params CharacterFamilyDefinition[] families)
    {
        effect.ImmuneCreatureFamilies.AddRange(families.Select(f => f.Name));
        return this;
    }

    public EffectDescriptionBuilder SetSpeed(RuleDefinitions.SpeedType speedType, float speedParameter)
    {
        effect.speedType = speedType;
        effect.speedParameter = speedParameter;
        return this;
    }

    public EffectDescriptionBuilder SetAnimation(AnimationDefinitions.AnimationMagicEffect animation)
    {
        effect.animationMagicEffect = animation;
        return this;
    }

    public EffectDescriptionBuilder AddEffectForm(EffectForm effectForm)
    {
        effect.EffectForms.Add(effectForm);
        return this;
    }

    public EffectDescriptionBuilder AddEffectForms(params EffectForm[] effectForms)
    {
        effect.EffectForms.AddRange(effectForms);
        return this;
    }

    public EffectDescriptionBuilder SetEffectForms(params EffectForm[] effectForms)
    {
        effect.EffectForms.SetRange(effectForms);
        return this;
    }

    public EffectDescriptionBuilder ClearEffectForms()
    {
        effect.EffectForms.Clear();
        return this;
    }

    public EffectDescription Build()
    {
        return effect;
    }
}
