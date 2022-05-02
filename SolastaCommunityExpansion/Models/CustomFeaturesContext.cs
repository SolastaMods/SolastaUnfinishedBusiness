using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Extensions;
using UnityEngine;

namespace SolastaCommunityExpansion.Models
{
    public static class CustomFeaturesContext
    {
        internal static void RecursiveGrantCustomFeatures(RulesetCharacterHero hero, List<FeatureDefinition> features, string tag)
        {
            foreach (var grantedFeature in features)
            {
                if (grantedFeature is FeatureDefinitionFeatureSet set && set.Mode == FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                {
                    RecursiveGrantCustomFeatures(hero, set.FeatureSet, tag);
                }
                if (grantedFeature is IFeatureDefinitionCustomCode customFeature)
                {
                    customFeature.ApplyFeature(hero, tag);
                }
                if (grantedFeature is not FeatureDefinitionProficiency featureDefinitionProficiency)
                {
                    continue;
                }
                if (featureDefinitionProficiency.ProficiencyType != RuleDefinitions.ProficiencyType.FightingStyle)
                {
                    continue;
                }
                featureDefinitionProficiency.Proficiencies.ForEach(prof => hero.TrainedFightingStyles.Add(DatabaseRepository.GetDatabase<FightingStyleDefinition>().GetElement(prof, false)));
            }
        }

        public static void RecursiveRemoveCustomFeatures(RulesetCharacterHero hero, List<FeatureDefinition> features, string tag)
        {
            foreach (var grantedFeature in features)
            {
                if (grantedFeature is FeatureDefinitionFeatureSet set && set.Mode == FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                {
                    RecursiveRemoveCustomFeatures(hero, set.FeatureSet, tag);
                }
                if (grantedFeature is IFeatureDefinitionCustomCode customFeature)
                {
                    customFeature.RemoveFeature(hero, tag);
                }
                if (grantedFeature is not FeatureDefinitionProficiency featureDefinitionProficiency)
                {
                    continue;
                }
                if (featureDefinitionProficiency.ProficiencyType != RuleDefinitions.ProficiencyType.FightingStyle)
                {
                    continue;
                }
                featureDefinitionProficiency.Proficiencies.ForEach(prof => hero.TrainedFightingStyles.Remove(DatabaseRepository.GetDatabase<FightingStyleDefinition>().GetElement(prof, false)));
            }
        }

        internal static void RechargeLinkedPowers(RulesetCharacter character, RuleDefinitions.RestType restType)
        {
            var pointPoolPowerDefinitions = new List<FeatureDefinitionPower>();

            foreach (var usablePower in character.UsablePowers)
            {
                if (usablePower.PowerDefinition is IPowerSharedPool pool)
                {
                    var pointPoolPower = pool.GetUsagePoolPower();

                    // Only add to recharge here if it (recharges on a short rest and this is a short or long rest) or 
                    // it recharges on a long rest and this is a long rest.
                    if (!pointPoolPowerDefinitions.Contains(pointPoolPower)
                        && ((pointPoolPower.RechargeRate == RuleDefinitions.RechargeRate.ShortRest &&
                            (restType == RuleDefinitions.RestType.ShortRest || restType == RuleDefinitions.RestType.LongRest)) ||
                            (pointPoolPower.RechargeRate == RuleDefinitions.RechargeRate.LongRest && restType == RuleDefinitions.RestType.LongRest)))
                    {
                        pointPoolPowerDefinitions.Add(pointPoolPower);
                    }
                }
            }

            // Find the UsablePower of the point pool powers.
            foreach (var poolPower in character.UsablePowers)
            {
                if (pointPoolPowerDefinitions.Contains(poolPower.PowerDefinition))
                {
                    var poolSize = GetMaxUsesForPool(poolPower, character);

                    poolPower.SetRemainingUses(poolSize);

                    AssignUsesToSharedPowersForPool(character, poolPower, poolSize, poolSize);
                }
            }
        }

        internal static void AssignUsesToSharedPowersForPool(RulesetCharacter character, RulesetUsablePower poolPower, int remainingUses, int totalUses)
        {
            // Find powers that rely on this pool
            foreach (var usablePower in character.UsablePowers)
            {
                if (usablePower.PowerDefinition is IPowerSharedPool pool)
                {
                    var pointPoolPower = pool.GetUsagePoolPower();

                    if (pointPoolPower == poolPower.PowerDefinition)
                    {
                        usablePower.SetMaxUses(totalUses / usablePower.PowerDefinition.CostPerUse);
                        usablePower.SetRemainingUses(remainingUses / usablePower.PowerDefinition.CostPerUse);
                    }
                }
            }
        }

        internal static int GetMaxUsesForPool(RulesetUsablePower poolPower, RulesetCharacter character)
        {
            int totalPoolSize = poolPower.MaxUses;

            foreach (var modifierPower in character.UsablePowers)
            {
                if (modifierPower.PowerDefinition is IPowerPoolModifier modifier && modifier.GetUsagePoolPower() == poolPower.PowerDefinition)
                {
                    totalPoolSize += modifierPower.MaxUses;
                }
            }

            return totalPoolSize;
        }

        internal static void UpdateUsageForPowerPool(this RulesetCharacter character, RulesetUsablePower modifiedPower, int poolUsage)
        {
            if (modifiedPower.PowerDefinition is not IPowerSharedPool sharedPoolPower)
            {
                return;
            }

            var pointPoolPower = sharedPoolPower.GetUsagePoolPower();

            foreach (var poolPower in character.UsablePowers)
            {
                if (poolPower.PowerDefinition == pointPoolPower)
                {
                    var maxUses = GetMaxUsesForPool(poolPower, character);
                    var remainingUses = Mathf.Clamp(poolPower.RemainingUses - poolUsage, 0, maxUses);

                    poolPower.SetRemainingUses(remainingUses);
                    AssignUsesToSharedPowersForPool(character, poolPower, remainingUses, maxUses);

                    return;
                }
            }
        }

        public static EffectDescription ModifySpellEffect(EffectDescription original, RulesetEffectSpell spell)
        {
            //TODO: find a way to cache result, so it works faster - this method is called sveral times per spell cast
            var result = original;
            var caster = spell.Caster;
            
            if (spell.SpellDefinition is ICustomMagicEffectBasedOnCaster baseDefinition && caster != null)
            {
                result = baseDefinition.GetCustomEffect(caster);
            }

            var modifiers = caster.GetFeaturesByType<IModifySpellEffect>();

            if (!modifiers.Empty())
            {
                result = modifiers.Aggregate(result.Copy(), (current, f) => f.ModifyEffect(spell, current));
            }

            return result;
        }

        /**Modifies spell description for GUI purposes. Uses only modifiers based on ICustomMagicEffectBasedOnCaster*/
        public static EffectDescription ModifySpellEffectGui(EffectDescription original, GuiSpellDefinition spell)
        {
            var result = original;
            var caster = Global.ActivePlayerCharacter?.RulesetCharacter;

            if (spell.SpellDefinition is ICustomMagicEffectBasedOnCaster baseDefinition && caster != null)
            {
                result = baseDefinition.GetCustomEffect(caster);
            }

            return result;
        }
        
        public static EffectDescription AddEffectForms(EffectDescription baseEffect, params EffectForm[] effectForms)
        {
            var newEffect = baseEffect.Copy();

            newEffect.EffectForms.AddRange(effectForms);

            return newEffect;
        }
    }
}
