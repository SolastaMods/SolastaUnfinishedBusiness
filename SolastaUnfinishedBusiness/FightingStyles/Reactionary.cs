using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Reactionary : AbstractFightingStyle
{
    private const string ReactionaryName = "Reactionary";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(ReactionaryName)
        .SetGuiPresentation(Category.FightingStyle, PathBerserker)
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create("OnComputeAttackModifierFightingStyleReactionary")
                .SetGuiPresentationNoContent(true)
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
            // grant +PB if attacker is performing an opportunity attack
            if (attackMode == null || defender == null ||
                attackMode.actionType != ActionDefinitions.ActionType.Reaction)
            {
                return;
            }

            var proficiencyBonus = myself.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;

            attackModifier.attackRollModifier += proficiencyBonus;
            attackModifier.attackToHitTrends.Add(new RuleDefinitions.TrendInfo(
                2, RuleDefinitions.FeatureSourceType.FightingStyle, ReactionaryName, myself));
        }
    }
}
