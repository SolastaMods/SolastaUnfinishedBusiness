using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Executioner : AbstractFightingStyle
{
    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create("Executioner")
        .SetGuiPresentation(Category.FightingStyle, PathMagebane)
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create("OnComputeAttackModifierFightingStyleExecutioner")
                .SetGuiPresentationNoContent()
                .SetCustomSubFeatures(new OnComputeAttackModifierFightingStyleExecutioner())
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };

    private sealed class OnComputeAttackModifierFightingStyleExecutioner : IOnComputeAttackModifier
    {
        public void ComputeAttackModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            // melee attack only
            if (attackMode == null || defender == null)
            {
                return;
            }

            // grant +2 hit if defender has
            // blinded, frightened, restrained, incapacitated, paralyzed, prone or stunned
            if (!defender.HasConditionOfType(ConditionBlinded)
                && !defender.HasConditionOfType(ConditionFrightened)
                && !defender.HasConditionOfType(ConditionRestrained)
                && !defender.HasConditionOfType(ConditionIncapacitated)
                && !defender.HasConditionOfType(ConditionParalyzed)
                && !defender.HasConditionOfType(ConditionProne)
                && !defender.HasConditionOfType(ConditionStunned))
            {
                return;
            }

            attackModifier.attackRollModifier += 2;
            attackModifier.attackToHitTrends.Add(new RuleDefinitions.TrendInfo(
                2, RuleDefinitions.FeatureSourceType.FightingStyle, "Executioner", myself));
        }
    }
}
