using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class UsablePowersProvider
{
    //TODO: think whether we need to cache these at all, and if we indeed do, maybe switch to caching per character?
    private static readonly Dictionary<FeatureDefinitionPower, RulesetUsablePower> UsablePowers = new();

    [NotNull]
    internal static RulesetUsablePower Get(FeatureDefinitionPower power, [CanBeNull] RulesetCharacter actor = null)
    {
        var result = (RulesetUsablePower)null;

        if (actor != null)
        {
            result = actor.UsablePowers.FirstOrDefault(u => u.PowerDefinition == power);
        }

        if (result != null)
        {
            return result;
        }

        if (UsablePowers.ContainsKey(power))
        {
            result = UsablePowers[power];
        }
        else
        {
            result = new RulesetUsablePower(power, null, null);
            UsablePowers.Add(power, result);
        }

        //Update properties to match actor
        UpdateSaveDc(actor, result);
        UpdatePoolUses(actor, result);

        return result;
    }

    private static void UpdatePoolUses([CanBeNull] RulesetCharacter character, RulesetUsablePower usablePower)
    {
        if (character == null)
        {
            return;
        }

        var pool = CustomFeaturesContext.GetPoolPower(usablePower, character);

        if (pool == null || pool == usablePower)
        {
            return;
        }

        var powerCost = usablePower.PowerDefinition.CostPerUse;
        var maxUsesForPool = CustomFeaturesContext.GetMaxUsesForPool(pool, character);

        usablePower.maxUses = maxUsesForPool / powerCost;
        usablePower.remainingUses = pool.RemainingUses / powerCost;
    }

    internal static void UpdateSaveDc([CanBeNull] RulesetCharacter actor, [NotNull] RulesetUsablePower usablePower)
    {
        var power = usablePower.PowerDefinition;
        var effectDescription = power.EffectDescription;

        if (actor == null || !effectDescription.HasSavingThrow)
        {
            return;
        }

        switch (effectDescription.DifficultyClassComputation)
        {
            case EffectDifficultyClassComputation.SpellCastingFeature:
            {
                var rulesetSpellRepertoire = (RulesetSpellRepertoire)null;

                foreach (var spellRepertoire in actor.SpellRepertoires)
                {
                    if (spellRepertoire.SpellCastingClass != null)
                    {
                        rulesetSpellRepertoire = spellRepertoire;
                        break;
                    }

                    if (spellRepertoire.SpellCastingSubclass == null)
                    {
                        continue;
                    }

                    rulesetSpellRepertoire = spellRepertoire;
                    break;
                }

                if (rulesetSpellRepertoire != null)
                {
                    usablePower.SaveDC = rulesetSpellRepertoire.SaveDC;
                }

                break;
            }
            case EffectDifficultyClassComputation.AbilityScoreAndProficiency:
                var attributeValue = actor.TryGetAttributeValue(effectDescription.SavingThrowDifficultyAbility);
                var proficiencyBonus = actor.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

                usablePower.SaveDC = ComputeAbilityScoreBasedDC(attributeValue, proficiencyBonus);

                break;
            case EffectDifficultyClassComputation.FixedValue:
                usablePower.SaveDC = effectDescription.FixedSavingThrowDifficultyClass;

                break;
        }
    }
}
