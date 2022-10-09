using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Executioner : AbstractFightingStyle
{
    internal override FightingStyleDefinition FightingStyle { get; } = CustomizableFightingStyleBuilder
        .Create("Executioner")
        .SetGuiPresentation(Category.FightingStyle, PathMagebane.GuiPresentation.SpriteReference)
        .SetFeatures(
            FeatureDefinitionOnComputeAttackModifierBuilder
                .Create("OnComputeAttackModifierFightingStyleExecutioner")
                .SetGuiPresentationNoContent()
                .SetOnComputeAttackModifierDelegate(
                    (
                        RulesetCharacter myself,
                        RulesetCharacter defender,
                        RulesetAttackMode attackMode,
                        ref ActionModifier attackModifier) =>
                    {
                        // melee attack only
                        if (attackMode == null || defender == null)
                        {
                            return;
                        }

                        // grant +2 hit if defender has
                        // blind, frightened, restrained, incapacitated, paralyzed, prone or unconscious
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
                    })
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin
    };
}
