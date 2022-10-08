using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSizeDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class TitanFighting : AbstractFightingStyle
{
    internal TitanFighting()
    {
        void TitanFightingOnComputeAttackModifier(
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

            // grant +2 hit if defender is large or bigger
            if (defender.SizeDefinition != Large && defender.SizeDefinition != Huge &&
                defender.SizeDefinition != Gargantuan)
            {
                return;
            }

            attackModifier.attackRollModifier += 2;
            attackModifier.attackToHitTrends.Add(new RuleDefinitions.TrendInfo(
                2, RuleDefinitions.FeatureSourceType.FightingStyle, "Titan", this));
        }

        FightingStyle = CustomizableFightingStyleBuilder
            .Create("Titan")
            .SetGuiPresentation(Category.FightingStyle,
                PathBerserker.GuiPresentation.SpriteReference)
            .SetFeatures(
                FeatureDefinitionOnComputeAttackModifierBuilder
                    .Create("OnComputeAttackModifierFightingStyleTitan")
                    .SetGuiPresentationNoContent()
                    .SetOnComputeAttackModifierDelegate(TitanFightingOnComputeAttackModifier)
                    .AddToDB())
            .AddToDB();
    }

    internal override FightingStyleDefinition FightingStyle { get; }

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin
    };
}
