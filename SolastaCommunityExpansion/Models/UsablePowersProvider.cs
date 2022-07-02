using System.Collections.Generic;
using System.Linq;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Models;

public static class UsablePowersProvider
{
    //TODO: think whether we ned to cache these at all, and if we indeed do, maybe switch to caching per character?
    private static readonly Dictionary<FeatureDefinitionPower, RulesetUsablePower> UsablePowers = new();

    public static RulesetUsablePower Get(FeatureDefinitionPower power, RulesetCharacter actor = null)
    {
        RulesetUsablePower result = null;
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
        UpdateSaveDC(actor, result);
        UpdatePoolUses(actor, result);

        return result;
    }

    private static void UpdatePoolUses(RulesetCharacter character, RulesetUsablePower usablePower)
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

    public static void UpdateSaveDC(RulesetCharacter actor, RulesetUsablePower usablePower)
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
