using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Diagnostics;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders
{
    public class EffectDescriptionBuilder
    {
        private readonly EffectDescription effect;

        public EffectDescriptionBuilder()
        {
            effect = new EffectDescription();

            EffectAdvancement effectAdvancement = new EffectAdvancement();
            effectAdvancement.SetIncrementMultiplier(1);
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

        public EffectDescriptionBuilder SetEffectAIParameters(EffectAIParameters effectAIParameters)
        {
            effect.SetEffectAIParameters(effectAIParameters);
            return this;
        }

        public EffectDescriptionBuilder SetEffectAIParameters(float aoeScoreMultiplier, int cooldownForCaster, int cooldownForBattle, bool dynamicCooldown)
        {
            EffectAIParameters aiParams = new EffectAIParameters();
            aiParams.SetAoeScoreMultiplier(aoeScoreMultiplier);
            aiParams.SetCooldownForCaster(cooldownForCaster);
            aiParams.SetCooldownForBattle(cooldownForBattle);
            aiParams.SetDynamicCooldown(dynamicCooldown);
            effect.SetEffectAIParameters(aiParams);
            return this;
        }

        public EffectDescriptionBuilder SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod effectIncrementMethod, int incrementMultiplier, int additionalTargetsPerIncrement,
            int additionalDicePerIncrement, int additionalSpellLevelPerIncrement, int additionalSummonsPerIncrement, int additionalHPPerIncrement, int additionalTempHPPerIncrement,
            int additionalTargetCellsPerIncrement, int additionalItemBonus, RuleDefinitions.AdvancementDuration alteredDuration)
        {
            EffectAdvancement effectAdvancement = new EffectAdvancement();
            effectAdvancement.SetEffectIncrementMethod(effectIncrementMethod);
            effectAdvancement.SetIncrementMultiplier(incrementMultiplier);
            effectAdvancement.SetAdditionalTargetsPerIncrement(additionalTargetsPerIncrement);
            effectAdvancement.SetAdditionalDicePerIncrement(additionalDicePerIncrement);
            effectAdvancement.SetAdditionalSpellLevelPerIncrement(additionalSpellLevelPerIncrement);
            effectAdvancement.SetAdditionalSummonsPerIncrement(additionalSummonsPerIncrement);
            effectAdvancement.SetAdditionalHPPerIncrement(additionalHPPerIncrement);
            effectAdvancement.SetAdditionalTempHPPerIncrement(additionalTempHPPerIncrement);
            effectAdvancement.SetAdditionalTargetCellsPerIncrement(additionalTargetCellsPerIncrement);
            effectAdvancement.SetAdditionalItemBonus(additionalItemBonus);
            effectAdvancement.SetAlteredDuration(alteredDuration);
            effect.SetEffectAdvancement(effectAdvancement);
            return this;
        }

        public EffectDescriptionBuilder SetTargetingData(RuleDefinitions.Side targetSide, RuleDefinitions.RangeType rangeType, int rangeParameter, RuleDefinitions.TargetType targetType, int targetParameter, int targetParameter2, ActionDefinitions.ItemSelectionType itemSelectionType)
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

        // TODO: combine with SetTargetingData - this method not currently used, the extension version is used
        /*        private EffectDescriptionBuilder SetRange(RuleDefinitions.RangeType type, int? range = null)
                {
                    switch (type)
                    {
                        case RuleDefinitions.RangeType.RangeHit:
                        case RuleDefinitions.RangeType.Distance:
                            if (range == null)
                            {
                                throw new ArgumentNullException(nameof(range), $"A range value is required for range type {type}.");
                            }
                            effect.SetRangeParameter(range.Value);
                            break;
                        case RuleDefinitions.RangeType.Touch:
                            effect.SetRangeParameter(range ?? 0);
                            break;
                        default: // Self, MeleeHit
                            if (range != null)
                            {
                                throw new SolastaModApiException($"A duration value is not expected for duration type {type}");
                            }
                            effect.SetRangeParameter(0);
                            break;
                    }

                    effect.SetRangeType(type);

                    return this;
                }*/

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
            HitAffinityByTag hitAffinity = new HitAffinityByTag();

            hitAffinity.SetTag(tag);
            hitAffinity.SetAdvantageType(advantageType);
            effect.HitAffinitiesByTargetTag.Add(hitAffinity);
            return this;
        }

        public EffectDescriptionBuilder ExcludeCaster()
        {
            effect.SetTargetExcludeCaster(true);
            return this;
        }

        public EffectDescriptionBuilder MustPlaceNotOnCharacter()
        {
            effect.SetCanBePlacedOnCharacter(false);
            return this;
        }

        public EffectDescriptionBuilder SetTargetProximityData(bool requiresTargetProximity, int targetProximityDistance)
        {
            effect.SetRequiresTargetProximity(requiresTargetProximity);
            effect.SetTargetProximityDistance(targetProximityDistance);
            return this;
        }

        public EffectDescriptionBuilder SetTargetFiltering(RuleDefinitions.TargetFilteringMethod targetFilteringMethod, RuleDefinitions.TargetFilteringTag targetFilteringTag,
            int poolFilterDiceNumber, RuleDefinitions.DieType poolFilterDieType)
        {
            effect.SetTargetFilteringMethod(targetFilteringMethod);
            effect.SetTargetFilteringTag(targetFilteringTag);
            effect.SetPoolFilterDiceNumber(poolFilterDiceNumber);
            effect.SetPoolFilterDieType(poolFilterDieType);
            return this;
        }

        public EffectDescriptionBuilder SetBorderData(RuleDefinitions.EmissiveBorder emissiveBorder, int emissiveParameter)
        {
            effect.SetEmissiveBorder(emissiveBorder);
            effect.SetEmissiveParameter(emissiveParameter);
            return this;
        }

        public EffectDescriptionBuilder SetRecurrentEffect(RuleDefinitions.RecurrentEffect recurrentEffect)
        {
            effect.SetRecurrentEffect(recurrentEffect);
            return this;
        }

        public EffectDescriptionBuilder SetRetargetData(bool retargetAfterDeath, ActionDefinitions.ActionType retargetActionType)
        {
            effect.SetRetargetAfterDeath(retargetAfterDeath);
            effect.SetRetargetActionType(retargetActionType);
            return this;
        }

        public EffectDescriptionBuilder SetTrapRange(RuleDefinitions.TrapRangeType trapRangeType)
        {
            effect.SetTrapRangeType(trapRangeType);
            return this;
        }

        public EffectDescriptionBuilder SetRequiredCondition(ConditionDefinition targetConditionAsset)
        {
            effect.SetTargetConditionAsset(targetConditionAsset);
            effect.SetTargetConditionName(targetConditionAsset.Name);
            return this;
        }

        public EffectDescriptionBuilder SetDurationData(RuleDefinitions.DurationType durationType, int durationParameter, RuleDefinitions.TurnOccurenceType endOfEffect)
        {
            SetDurationData(durationType, durationParameter);
            effect.SetEndOfEffect(endOfEffect);
            return this;
        }

        public EffectDescriptionBuilder SetDurationData(RuleDefinitions.DurationType type, int? duration = null)
        {
            if (RuleDefinitions.IsVariableDuration(type))
            {
#if DEBUG
                // TODO: should we have 'int duration = 0' 
                // and test for 'duration <= 0' here, and 'duration != 0' below?
                if (duration == null)
                {
                    throw new ArgumentNullException(nameof(duration), $"A duration value is required for duration type {type}.");
                }
#endif

                effect.SetDurationParameter(duration.Value);
            }
            else
            {
#if DEBUG
                if ((duration ?? 0) != 0)
                {
                    throw new SolastaModApiException($"A duration value is not expected for duration type {type}");
                }
#endif

                effect.SetDurationParameter(0);
            }

            effect.SetDurationType(type);

            return this;
        }

        public EffectDescriptionBuilder SetSavingThrowData(bool hasSavingThrow, bool disableSavingThrowOnAllies, string savingThrowAbility, bool ignoreCover,
            RuleDefinitions.EffectDifficultyClassComputation difficultyClassComputation, string savingThrowDifficultyAbility,
            int fixedSavingThrowDifficultyClass, bool advantageForEnemies, params SaveAffinityBySenseDescription[] savingThrowAffinitiesBySense)
        {
            return SetSavingThrowData(
                hasSavingThrow, disableSavingThrowOnAllies, savingThrowAbility,
                ignoreCover, difficultyClassComputation, savingThrowDifficultyAbility,
                fixedSavingThrowDifficultyClass, advantageForEnemies, savingThrowAffinitiesBySense.AsEnumerable());
        }

        public EffectDescriptionBuilder SetSavingThrowData(bool hasSavingThrow, bool disableSavingThrowOnAllies, string savingThrowAbility, bool ignoreCover,
            RuleDefinitions.EffectDifficultyClassComputation difficultyClassComputation, string savingThrowDifficultyAbility,
            int fixedSavingThrowDifficultyClass, bool advantageForEnemies, IEnumerable<SaveAffinityBySenseDescription> savingThrowAffinitiesBySense)
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

        public EffectDescriptionBuilder SetVelocity(int velocityCellsPerRound, RuleDefinitions.VelocityType velocityType)
        {
            effect.SetHasVelocity(true);
            effect.SetVelocityCellsPerRound(velocityCellsPerRound);
            effect.SetVelocityType(velocityType);
            return this;
        }

        public EffectDescriptionBuilder AddRestrictedCreatureFamilies(CharacterFamilyDefinition family)
        {
            effect.RestrictedCreatureFamilies.Add(family.Name);
            return this;
        }

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

        public EffectDescriptionBuilder SetSpeed(RuleDefinitions.SpeedType speedType, float speedParameter)
        {
            effect.SetSpeedType(speedType);
            effect.SetSpeedParameter(speedParameter);
            return this;
        }

        public EffectDescriptionBuilder SetOffsetImpactTime(float offsetImpactTimeBasedOnDistanceFactor, float offsetImpactTimePerTarget)
        {
            effect.SetOffsetImpactTimeBasedOnDistance(true);
            effect.SetOffsetImpactTimeBasedOnDistanceFactor(offsetImpactTimeBasedOnDistanceFactor);
            effect.SetOffsetImpactTimePerTarget(offsetImpactTimePerTarget);
            return this;
        }

        public EffectDescriptionBuilder AddEffectForm(EffectForm effectForm)
        {
            effect.EffectForms.Add(effectForm);
            return this;
        }

        public EffectDescription Build()
        {
            return effect;
        }
    }
}
