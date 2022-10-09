using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Reactionary : AbstractFightingStyle
{
    internal override FightingStyleDefinition FightingStyle { get; } = CustomizableFightingStyleBuilder
        .Create("Reactionary")
        .SetGuiPresentation(Category.FightingStyle,
            PathBerserker.GuiPresentation.SpriteReference)
        .SetFeatures(
            FeatureDefinitionOnComputeAttackModifierBuilder
                .Create("OnComputeAttackModifierFightingStyleReactionary")
                .SetGuiPresentationNoContent()
                .SetOnComputeAttackModifierDelegate(
                    (
                        RulesetCharacter myself,
                        RulesetCharacter defender,
                        RulesetAttackMode attackMode,
                        ref ActionModifier attackModifier) =>
                    {
                        if (attackMode == null || defender == null)
                        {
                            return;
                        }

                        var hero = GameLocationCharacter.GetFromActor(myself);
                        var target = GameLocationCharacter.GetFromActor(defender);

                        if (attackMode.actionType != ActionDefinitions.ActionType.Reaction)
                        {
                            return;
                        }

                        // grant +2 if attacker is performing an opportunity attack
                        attackModifier.attackRollModifier += 2;
                        attackModifier.attackToHitTrends.Add(new RuleDefinitions.TrendInfo(
                            2, RuleDefinitions.FeatureSourceType.FightingStyle, "Reactionary", myself));
                    })
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };
}
