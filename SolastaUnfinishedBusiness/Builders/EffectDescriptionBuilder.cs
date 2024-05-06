using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

internal class EffectDescriptionBuilder
{
    private readonly EffectDescription _effect;

    private EffectDescriptionBuilder()
    {
        _effect = new EffectDescription
        {
            effectAdvancement = new EffectAdvancement { incrementMultiplier = 1 },
            effectParticleParameters = new EffectParticleParameters(),
            // there are many places in code where we use GLC.RSC and is null when aiming gadgets
            targetFilteringMethod = TargetFilteringMethod.CharacterOnly,
            createdByCharacter = true
        };
        _effect.effectParticleParameters.Copy(MagicWeapon.EffectDescription.EffectParticleParameters);
    }

    private EffectDescriptionBuilder(EffectDescription effect)
    {
        _effect = new EffectDescription
        {
            effectAdvancement = new EffectAdvancement { incrementMultiplier = 1 },
            effectParticleParameters = new EffectParticleParameters()
        };
        _effect.Copy(effect);
    }

    internal EffectDescription Build()
    {
        return _effect;
    }

    internal static EffectDescriptionBuilder Create()
    {
        return new EffectDescriptionBuilder();
    }

    internal static EffectDescriptionBuilder Create(EffectDescription effect)
    {
        return new EffectDescriptionBuilder(effect);
    }

    internal static EffectDescriptionBuilder Create(IMagicEffect effect)
    {
        return new EffectDescriptionBuilder(effect.EffectDescription);
    }

    internal EffectDescriptionBuilder ClearEffectAdvancements()
    {
        _effect.effectAdvancement.Clear();
        return this;
    }

    internal EffectDescriptionBuilder RollSaveOnlyIfRelevantForms()
    {
        _effect.RollSaveOnlyIfRelevantForms = true;
        return this;
    }

    internal EffectDescriptionBuilder SetRestrictedCreatureFamilies(params string[] values)
    {
        _effect.RestrictedCreatureFamilies.SetRange(values);
        return this;
    }

    internal EffectDescriptionBuilder SetParticleEffectParameters(IMagicEffect reference)
    {
        return SetParticleEffectParameters(reference.EffectDescription.EffectParticleParameters);
    }

    internal EffectDescriptionBuilder SetParticleEffectParameters(EffectParticleParameters parameters)
    {
        _effect.effectParticleParameters.Copy(parameters);
        return this;
    }

    internal EffectDescriptionBuilder SetCasterEffectParameters(IMagicEffect reference)
    {
        return SetCasterEffectParameters(
            reference.EffectDescription.EffectParticleParameters.casterParticleReference,
            reference.EffectDescription.EffectParticleParameters.casterSelfParticleReference,
            reference.EffectDescription.EffectParticleParameters.casterQuickSpellParticleReference);
    }

    internal EffectDescriptionBuilder SetCasterEffectParameters(
        AssetReference casterParticleReference,
        AssetReference casterSelfParticleReference = null,
        AssetReference casterQuickSpellParticleReference = null)
    {
        _effect.effectParticleParameters.casterParticleReference = casterParticleReference;
        _effect.effectParticleParameters.casterSelfParticleReference = casterSelfParticleReference;
        _effect.effectParticleParameters.casterQuickSpellParticleReference = casterQuickSpellParticleReference;
        return this;
    }

    internal EffectDescriptionBuilder SetEffectEffectParameters(IMagicEffect reference)
    {
        return SetEffectEffectParameters(reference.EffectDescription.EffectParticleParameters.effectParticleReference);
    }

    internal EffectDescriptionBuilder SetEffectEffectParameters(AssetReference assetReference)
    {
        _effect.effectParticleParameters.effectParticleReference = assetReference;
        return this;
    }

    internal EffectDescriptionBuilder SetImpactEffectParameters(IMagicEffect reference)
    {
        return SetImpactEffectParameters(reference.EffectDescription.EffectParticleParameters.impactParticleReference);
    }

    internal EffectDescriptionBuilder SetImpactEffectParameters(AssetReference assetReference)
    {
        _effect.effectParticleParameters.impactParticleReference = assetReference;
        return this;
    }

    internal EffectDescriptionBuilder UseQuickAnimations()
    {
        _effect.speedParameter = -1;
        var particles = _effect.effectParticleParameters;
        if (particles.casterQuickSpellParticleReference == null
            || !particles.casterQuickSpellParticleReference.IsValid())
        {
            particles.casterQuickSpellParticleReference = particles.casterParticleReference;
        }

        return this;
    }

    internal EffectDescriptionBuilder SetNoSavingThrow()
    {
        _effect.hasSavingThrow = false;
        return this;
    }

    internal EffectDescriptionBuilder InviteOptionalAlly()
    {
        _effect.inviteOptionalAlly = true;
        return this;
    }

    internal EffectDescriptionBuilder SetEffectAdvancement(
        EffectIncrementMethod effectIncrementMethod,
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
        _effect.effectAdvancement = new EffectAdvancement
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

    internal EffectDescriptionBuilder AllowRetarget(
        ActionDefinitions.ActionType action = ActionDefinitions.ActionType.Bonus)
    {
        _effect.retargetAfterDeath = true;
        _effect.retargetActionType = action;
        return this;
    }

    internal EffectDescriptionBuilder SetTargetingData(
        Side targetSide,
        RangeType rangeType,
        int rangeParameter,
        TargetType targetType,
        int targetParameter = 1,
        int targetParameter2 = 2,
        ActionDefinitions.ItemSelectionType itemSelectionType = ActionDefinitions.ItemSelectionType.None)
    {
        _effect.targetSide = targetSide;
        _effect.rangeType = rangeType;
        _effect.rangeParameter = rangeParameter;
        _effect.targetType = targetType;
        _effect.targetParameter = targetParameter;
        _effect.targetParameter2 = targetParameter2;
        _effect.itemSelectionType = itemSelectionType;
        return this;
    }

    internal EffectDescriptionBuilder ExcludeCaster()
    {
        _effect.targetExcludeCaster = true;
        return this;
    }

    internal EffectDescriptionBuilder SetTargetFiltering(
        TargetFilteringMethod targetFilteringMethod,
        TargetFilteringTag targetFilteringTag = TargetFilteringTag.No,
        int poolFilterDiceNumber = 0,
        DieType poolFilterDieType = DieType.D1)
    {
        _effect.targetFilteringMethod = targetFilteringMethod;
        _effect.targetFilteringTag = targetFilteringTag;
        _effect.poolFilterDiceNumber = poolFilterDiceNumber;
        _effect.poolFilterDieType = poolFilterDieType;
        return this;
    }

    internal EffectDescriptionBuilder SetRecurrentEffect(RecurrentEffect recurrentEffect)
    {
        _effect.recurrentEffect = recurrentEffect;
        return this;
    }

    internal EffectDescriptionBuilder SetRequiredCondition(ConditionDefinition targetConditionAsset)
    {
        _effect.targetConditionAsset = targetConditionAsset;
        _effect.targetConditionName = targetConditionAsset.Name;
        return this;
    }

    internal EffectDescriptionBuilder SetDurationData(
        DurationType durationType,
        int durationParameter = 0,
        TurnOccurenceType endOfEffect = TurnOccurenceType.EndOfTurn)
    {
        // ReSharper disable once InvocationIsSkipped
        // PreConditions.IsValidDuration(durationType, durationParameter);

        _effect.durationParameter = durationParameter;
        _effect.durationType = durationType;
        _effect.endOfEffect = endOfEffect;
        return this;
    }

    internal EffectDescriptionBuilder SetIgnoreCover()
    {
        _effect.ignoreCover = true;
        return this;
    }

    internal EffectDescriptionBuilder SetSavingThrowData(
        bool disableSavingThrowOnAllies,
        string savingThrowAbility,
        bool ignoreCover,
        EffectDifficultyClassComputation difficultyClassComputation,
        string savingThrowDifficultyAbility = AttributeDefinitions.Wisdom,
        int fixedSavingThrowDifficultyClass = 10,
        bool advantageForEnemies = false,
        params SaveAffinityBySenseDescription[] savingThrowAffinitiesBySense)
    {
        _effect.hasSavingThrow = true;
        _effect.disableSavingThrowOnAllies = disableSavingThrowOnAllies;
        _effect.savingThrowAbility = savingThrowAbility;
        _effect.ignoreCover = ignoreCover;
        _effect.difficultyClassComputation = difficultyClassComputation;
        _effect.savingThrowDifficultyAbility = savingThrowDifficultyAbility;
        _effect.fixedSavingThrowDifficultyClass = fixedSavingThrowDifficultyClass;
        _effect.advantageForEnemies = advantageForEnemies;
        _effect.savingThrowAffinitiesBySense.SetRange(savingThrowAffinitiesBySense);
        return this;
    }

    internal EffectDescriptionBuilder AddImmuneCreatureFamilies(params CharacterFamilyDefinition[] families)
    {
        _effect.ImmuneCreatureFamilies.AddRange(families.Select(f => f.Name));
        return this;
    }

#if false
    internal EffectDescriptionBuilder AddRestrictedCreatureFamilies(params CharacterFamilyDefinition[] families)
    {
        effect.RestrictedCreatureFamilies.AddRange(families.Select(f => f.Name));
        return this;
    }

    internal EffectDescriptionBuilder InviteOptionalAlly(bool value = true)
    {
        effect.inviteOptionalAlly = value;
        return this;
    }
#endif

    internal EffectDescriptionBuilder SetSpeed(SpeedType speedType, float speedParameter = 0f)
    {
        _effect.speedType = speedType;
        _effect.speedParameter = speedParameter;
        return this;
    }

    internal EffectDescriptionBuilder SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect value)
    {
        _effect.animationMagicEffect = value;
        return this;
    }

    internal EffectDescriptionBuilder SetEffectForms(params EffectForm[] effectForms)
    {
        _effect.EffectForms.SetRange(effectForms);
        return this;
    }

    internal EffectDescriptionBuilder AddEffectForms(params EffectForm[] effectForms)
    {
        _effect.EffectForms.AddRange(effectForms);
        return this;
    }

    internal EffectDescriptionBuilder SetupImpactOffsets(
        bool offsetImpactTimeBasedOnDistance = false,
        float offsetImpactTimeBasedOnDistanceFactor = 0.1f,
        float offsetImpactTimePerTarget = 0.0f)
    {
        _effect.offsetImpactTimeBasedOnDistance = offsetImpactTimeBasedOnDistance;
        _effect.offsetImpactTimeBasedOnDistanceFactor = offsetImpactTimeBasedOnDistanceFactor;
        _effect.offsetImpactTimePerTarget = offsetImpactTimePerTarget;
        return this;
    }
}
