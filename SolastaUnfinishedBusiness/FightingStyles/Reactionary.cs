using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Reactionary : AbstractFightingStyle
{
    internal override FightingStyleDefinition FightingStyle { get; } = CustomizableFightingStyleBuilder
        .Create("Reactionary")
        .SetGuiPresentation(Category.FightingStyle, PathBerserker)
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create("OnComputeAttackModifierFightingStyleReactionary")
                .SetGuiPresentationNoContent()
                .SetCustomSubFeatures(new OnComputeAttackModifierFightingStyleReactionary())
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };

    private sealed class OnComputeAttackModifierFightingStyleReactionary : IOnComputeAttackModifier
    {
        public void ComputeAttackModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            if (attackMode == null || defender == null ||
                attackMode.actionType != ActionDefinitions.ActionType.Reaction)
            {
                return;
            }

            // grant +2 if attacker is performing an opportunity attack
            attackModifier.attackRollModifier += 2;
            attackModifier.attackToHitTrends.Add(new RuleDefinitions.TrendInfo(
                2, RuleDefinitions.FeatureSourceType.FightingStyle, "Reactionary", myself));
        }
    }
}
