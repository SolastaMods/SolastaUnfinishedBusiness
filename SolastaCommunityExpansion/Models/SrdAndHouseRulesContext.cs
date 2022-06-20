using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;

namespace SolastaCommunityExpansion.Models;

internal static class SrdAndHouseRulesContext
{
    internal static void Load()
    {
        AllowTargetingSelectionWhenCastingChainLightningSpell();
        ApplyConditionBlindedShouldNotAllowOpportunityAttack();
        ApplyBarbarianUnarmoredDefenseNotStacking();
    }

    internal static void ApplyConditionBlindedShouldNotAllowOpportunityAttack()
    {
        // Use the shocked condition affinity which has the desired effect
        if (Main.Settings.BlindedConditionDontAllowAttackOfOpportunity)
        {
            if (!ConditionBlinded.Features.Contains(ActionAffinityConditionShocked))
            {
                ConditionBlinded.Features.Add(ActionAffinityConditionShocked);
            }
        }
        else
        {
            if (ConditionBlinded.Features.Contains(ActionAffinityConditionShocked))
            {
                ConditionBlinded.Features.Remove(ActionAffinityConditionShocked);
            }
        }
    }

    /// <summary>
    ///     Allow the user to select targets when using 'Chain Lightning'.
    /// </summary>
    internal static void AllowTargetingSelectionWhenCastingChainLightningSpell()
    {
        var spell = DatabaseHelper.SpellDefinitions.ChainLightning.EffectDescription;

        if (Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell)
        {
            // This is half bug-fix, half houses rules since it's not completely SRD but better than implemented.
            // Spell should arc from target (range 150ft) onto upto 3 extra selectable targets (range 30ft from first).
            // Fix by allowing 4 selectable targets.
            spell.TargetType = RuleDefinitions.TargetType.IndividualsUnique;
            spell.SetTargetParameter(4);
            spell.effectAdvancement.additionalTargetsPerIncrement = 1;

            // TODO: may need to tweak range parameters but it works as is.
        }
        else
        {
            spell.TargetType = RuleDefinitions.TargetType.ArcFromIndividual;
            spell.SetTargetParameter(3);
            spell.effectAdvancement.additionalTargetsPerIncrement = 0;
        }
    }

    internal static void ApplyBarbarianUnarmoredDefenseNotStacking()
    {
        DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierBarbarianUnarmoredDefense
            .SetCustomSubFeatures(ExclusiveArmorClassBonus.MARKER);
    }
}
