#if false
// FightingStyle/&TitanDescription=You gain a +PB/2 (round up) to hit against creatures of size large or bigger.
// FightingStyle/&TitanTitle=Titan Hunter
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSizeDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Titan : AbstractFightingStyle
{
    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create("Titan")
        .SetGuiPresentation(Category.FightingStyle, DomainMischief)
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create("OnComputeAttackModifierFightingStyleTitan")
                .SetGuiPresentationNoContent(true)
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

            var proficiencyBonus =
                (myself.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue + 1) / 2;

            attackModifier.attackRollModifier += proficiencyBonus;
            attackModifier.attackToHitTrends.Add(new RuleDefinitions.TrendInfo(
                proficiencyBonus, RuleDefinitions.FeatureSourceType.FightingStyle, "Titan", myself));
        }
    }
}
#endif
