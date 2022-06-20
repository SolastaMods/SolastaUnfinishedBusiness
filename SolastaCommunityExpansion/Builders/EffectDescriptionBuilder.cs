using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;

namespace SolastaCommunityExpansion.Builders;

public class EffectDescriptionBuilder
{
    private readonly EffectDescription effect;

    public EffectDescriptionBuilder()
    {
        effect = new EffectDescription();

        var effectAdvancement = new EffectAdvancement {incrementMultiplier = 1};

        effect.SetEffectAdvancement(effectAdvancement);

        var particleParams = new EffectParticleParameters();

        particleParams.Copy(DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription.EffectParticleParameters);

        effect.SetEffectParticleParameters(particleParams);
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
        effect.SetCreatedByCharacter(true);
        return this;
    }

    public EffectDescriptionBuilder SetParticleEffectParameters(EffectParticleParameters particleParameters)
    {
        effect.SetEffectParticleParameters(particleParameters);
        return this;
    }

    public EffectDescriptionBuilder SetParticleEffectParameters(SpellDefinition reference)
    {
        effect.SetEffectParticleParameters(reference.EffectDescription.EffectParticleParameters);
        return this;
    }

#if false
    public EffectDescriptionBuilder SetEffectAIParameters(EffectAIParameters effectAIParameters)
    {
        effect.SetEffectAIParameters(effectAIParameters);
        return this;
    }

    public EffectDescriptionBuilder SetEffectAIParameters(float aoeScoreMultiplier, int cooldownForCaster,
        int cooldownForBattle, bool dynamicCooldown)
    {
        var aiParams = new EffectAIParameters();
        aiParams.aoeScoreMultiplier = aoeScoreMultiplier;
        aiParams.cooldownForCaster = cooldownForCaster;
        aiParams.cooldownForBattle = cooldownForBattle;
        aiParams.dynamicCooldown = dynamicCooldown;
        effect.SetEffectAIParameters(aiParams);
        return this;
    }
#endif

    public EffectDescriptionBuilder SetEffectAdvancement(
        RuleDefinitions.EffectIncrementMethod effectIncrementMethod, int incrementMultiplier = 1,
        int additionalTargetsPerIncrement = 0,
        int additionalDicePerIncrement = 0, int additionalSpellLevelPerIncrement = 0,
        int additionalSummonsPerIncrement = 0, int additionalHPPerIncrement = 0,
        int additionalTempHPPerIncrement = 0,
        int additionalTargetCellsPerIncrement = 0, int additionalItemBonus = 0,
        RuleDefinitions.AdvancementDuration alteredDuration = RuleDefinitions.AdvancementDuration.None)
    {
        var effectAdvancement = new EffectAdvancement();
        effectAdvancement.effectIncrementMethod = effectIncrementMethod;
        effectAdvancement.incrementMultiplier = incrementMultiplier;
        effectAdvancement.additionalTargetsPerIncrement = additionalTargetsPerIncrement;
        effectAdvancement.additionalDicePerIncrement = additionalDicePerIncrement;
        effectAdvancement.additionalSpellLevelPerIncrement = additionalSpellLevelPerIncrement;
        effectAdvancement.additionalSummonsPerIncrement = additionalSummonsPerIncrement;
        effectAdvancement.additionalHPPerIncrement = additionalHPPerIncrement;
        effectAdvancement.additionalTempHPPerIncrement = additionalTempHPPerIncrement;
        effectAdvancement.additionalTargetCellsPerIncrement = additionalTargetCellsPerIncrement;
        effectAdvancement.additionalItemBonus = additionalItemBonus;
        effectAdvancement.alteredDuration = alteredDuration;
        effect.SetEffectAdvancement(effectAdvancement);
        return this;
    }

    public EffectDescriptionBuilder SetTargetingData(RuleDefinitions.Side targetSide,
        RuleDefinitions.RangeType rangeType, int rangeParameter, RuleDefinitions.TargetType targetType,
        int targetParameter = 1, int targetParameter2 = 1,
        ActionDefinitions.ItemSelectionType itemSelectionType = ActionDefinitions.ItemSelectionType.None)
    {
        effect.TargetSide = targetSide;
        effect.RangeType = rangeType;
        effect.SetRangeParameter(rangeParameter);
        effect.TargetType = targetType;
        effect.SetTargetParameter(targetParameter);
        effect.SetTargetParameter2(targetParameter2);
        effect.SetItemSelectionType(itemSelectionType);
        return this;
    }

#if false
    public EffectDescriptionBuilder NoVisibilityRequiredToTarget()
    {
        effect.SetRequiresVisibilityForPosition(false);
        return this;
    }

    public EffectDescriptionBuilder HalfDamageOnMiss()
    {
        effect.SetHalfDamageOnAMiss(true);
        return this;
    }

    public EffectDescriptionBuilder OptionalAdditionalAlly()
    {
        effect.SetInviteOptionalAlly(true);
        return this;
    }

    public EffectDescriptionBuilder AddHitAffinity(string tag, RuleDefinitions.AdvantageType advantageType)
    {
        var hitAffinity = new HitAffinityByTag();

        hitAffinity.tag = tag;
        hitAffinity.advantageType = advantageType;
        effect.HitAffinitiesByTargetTag.Add(hitAffinity);
        return this;
    }
#endif

    public EffectDescriptionBuilder ExcludeCaster()
    {
        effect.SetTargetExcludeCaster(true);
        return this;
    }

#if false
    public EffectDescriptionBuilder MustPlaceNotOnCharacter()
    {
        effect.SetCanBePlacedOnCharacter(false);
        return this;
    }
#endif

    public EffectDescriptionBuilder SetTargetProximityData(bool requiresTargetProximity,
        int targetProximityDistance)
    {
        effect.SetRequiresTargetProximity(requiresTargetProximity);
        effect.SetTargetProximityDistance(targetProximityDistance);
        return this;
    }

    public EffectDescriptionBuilder SetTargetFiltering(
        RuleDefinitions.TargetFilteringMethod targetFilteringMethod,
        RuleDefinitions.TargetFilteringTag targetFilteringTag = RuleDefinitions.TargetFilteringTag.No,
        int poolFilterDiceNumber = 0,
        RuleDefinitions.DieType poolFilterDieType = RuleDefinitions.DieType.D1
    )
    {
        effect.SetTargetFilteringMethod(targetFilteringMethod);
        effect.SetTargetFilteringTag(targetFilteringTag);
        effect.SetPoolFilterDiceNumber(poolFilterDiceNumber);
        effect.SetPoolFilterDieType(poolFilterDieType);
        return this;
    }

#if false
    public EffectDescriptionBuilder SetBorderData(RuleDefinitions.EmissiveBorder emissiveBorder,
        int emissiveParameter)
    {
        effect.SetEmissiveBorder(emissiveBorder);
        effect.SetEmissiveParameter(emissiveParameter);
        return this;
    }
#endif

    public EffectDescriptionBuilder SetRecurrentEffect(RuleDefinitions.RecurrentEffect recurrentEffect)
    {
        effect.SetRecurrentEffect(recurrentEffect);
        return this;
    }

#if false
    public EffectDescriptionBuilder SetRetargetData(bool retargetAfterDeath,
        ActionDefinitions.ActionType retargetActionType)
    {
        effect.SetRetargetAfterDeath(retargetAfterDeath);
        effect.SetRetargetActionType(retargetActionType);
        return this;
    }

    public EffectDescriptionBuilder SetTrapRange(RuleDefinitions.TrapRangeType trapRangeType)
    {
        effect.trapRangeType = trapRangeType;
        return this;
    }
#endif

    public EffectDescriptionBuilder SetRequiredCondition(ConditionDefinition targetConditionAsset)
    {
        effect.SetTargetConditionAsset(targetConditionAsset);
        effect.SetTargetConditionName(targetConditionAsset.Name);
        return this;
    }

    public EffectDescriptionBuilder SetDurationData(RuleDefinitions.DurationType durationType,
        int durationParameter, RuleDefinitions.TurnOccurenceType endOfEffect)
    {
        effect.SetDurationType(durationType);
        effect.SetDurationParameter(durationParameter);
        effect.SetEndOfEffect(endOfEffect);
        return this;
    }

    public EffectDescriptionBuilder SetDurationData(RuleDefinitions.DurationType type, int duration = 0,
        bool validate = true)
    {
        if (validate)
        {
            Preconditions.IsValidDuration(type, duration);
        }

        effect.SetDurationParameter(duration);
        effect.SetDurationType(type);

        return this;
    }

    public EffectDescriptionBuilder SetSavingThrowData(bool hasSavingThrow, bool disableSavingThrowOnAllies,
        string savingThrowAbility, bool ignoreCover,
        RuleDefinitions.EffectDifficultyClassComputation difficultyClassComputation,
        string savingThrowDifficultyAbility,
        int fixedSavingThrowDifficultyClass = 10, bool advantageForEnemies = false,
        params SaveAffinityBySenseDescription[] savingThrowAffinitiesBySense)
    {
        return SetSavingThrowData(
            hasSavingThrow, disableSavingThrowOnAllies, savingThrowAbility,
            ignoreCover, difficultyClassComputation, savingThrowDifficultyAbility,
            fixedSavingThrowDifficultyClass, advantageForEnemies, savingThrowAffinitiesBySense.AsEnumerable());
    }

    public EffectDescriptionBuilder SetSavingThrowData(bool hasSavingThrow, bool disableSavingThrowOnAllies,
        string savingThrowAbility, bool ignoreCover,
        RuleDefinitions.EffectDifficultyClassComputation difficultyClassComputation,
        string savingThrowDifficultyAbility,
        int fixedSavingThrowDifficultyClass, bool advantageForEnemies,
        IEnumerable<SaveAffinityBySenseDescription> savingThrowAffinitiesBySense)
    {
        effect.HasSavingThrow = hasSavingThrow;
        effect.SetDisableSavingThrowOnAllies(disableSavingThrowOnAllies);
        effect.SavingThrowAbility = savingThrowAbility;
        effect.IgnoreCover = ignoreCover;
        effect.SetDifficultyClassComputation(difficultyClassComputation);
        effect.SetSavingThrowDifficultyAbility(savingThrowDifficultyAbility);
        effect.FixedSavingThrowDifficultyClass = fixedSavingThrowDifficultyClass;
        effect.AdvantageForEnemies = advantageForEnemies;
        effect.SavingThrowAffinitiesBySense.SetRange(savingThrowAffinitiesBySense);
        return this;
    }

#if false
    public EffectDescriptionBuilder RequireShoveToHit()
    {
        effect.SetHasShoveRoll(true);
        return this;
    }

    public EffectDescriptionBuilder CanBeDispersed()
    {
        effect.SetCanBeDispersed(true);
        return this;
    }

    public EffectDescriptionBuilder SetVelocity(int velocityCellsPerRound,
        RuleDefinitions.VelocityType velocityType)
    {
        effect.hasVelocity = true;
        effect.velocityCellsPerRound = velocityCellsPerRound;
        effect.velocityType = velocityType;
        return this;
    }
#endif

    public EffectDescriptionBuilder AddRestrictedCreatureFamily(CharacterFamilyDefinition family)
    {
        effect.RestrictedCreatureFamilies.Add(family.Name);
        return this;
    }

#if false
    public EffectDescriptionBuilder AddImmuneCreatureFamilies(CharacterFamilyDefinition family)
    {
        effect.ImmuneCreatureFamilies.Add(family.Name);
        return this;
    }

    public EffectDescriptionBuilder AddRestrictedCharacterSize(RuleDefinitions.CreatureSize size)
    {
        effect.RestrictedCharacterSizes.Add(size);
        return this;
    }

    public EffectDescriptionBuilder SetEffectPool(int effectPoolAmount)
    {
        effect.SetHasLimitedEffectPool(true);
        effect.SetEffectPoolAmount(effectPoolAmount);
        return this;
    }
#endif

    public EffectDescriptionBuilder SetSpeed(RuleDefinitions.SpeedType speedType, float speedParameter)
    {
        effect.SetSpeedType(speedType);
        effect.SetSpeedParameter(speedParameter);
        return this;
    }

#if false
    public EffectDescriptionBuilder SetOffsetImpactTime(float offsetImpactTimeBasedOnDistanceFactor,
        float offsetImpactTimePerTarget)
    {
        effect.SetOffsetImpactTimeBasedOnDistance(true);
        effect.SetOffsetImpactTimeBasedOnDistanceFactor(offsetImpactTimeBasedOnDistanceFactor);
        effect.SetOffsetImpactTimePerTarget(offsetImpactTimePerTarget);
        return this;
    }
#endif

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
