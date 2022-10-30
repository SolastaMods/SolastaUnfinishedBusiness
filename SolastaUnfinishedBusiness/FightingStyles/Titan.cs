using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSizeDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Titan : AbstractFightingStyle
{
    internal override FightingStyleDefinition FightingStyle { get; } = CustomizableFightingStyleBuilder
        .Create("Titan")
        .SetGuiPresentation(Category.FightingStyle, DomainMischief)
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create("OnComputeAttackModifierFightingStyleTitan")
                .SetGuiPresentationNoContent()
                .SetCustomSubFeatures(new OnComputeAttackModifierFightingStyleTitan())
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin
    };

    private sealed class OnComputeAttackModifierFightingStyleTitan : IOnComputeAttackModifier
    {
        public void ComputeAttackModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            // melee attack only
            if (attackMode == null || defender == null ||
                (defender.SizeDefinition != Large && defender.SizeDefinition != Huge &&
                 defender.SizeDefinition != Gargantuan))
            {
                return;
            }

            // grant +2 hit if defender is large or bigger
            attackModifier.attackRollModifier += 2;
            attackModifier.attackToHitTrends.Add(new RuleDefinitions.TrendInfo(
                2, RuleDefinitions.FeatureSourceType.FightingStyle, "Titan", myself));
        }
    }
}
