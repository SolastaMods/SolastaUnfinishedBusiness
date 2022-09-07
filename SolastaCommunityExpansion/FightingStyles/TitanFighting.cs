using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSizeDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaCommunityExpansion.FightingStyles;

internal sealed class TitanFighting : AbstractFightingStyle
{
    private FightingStyleDefinitionCustomizable instance;

    [NotNull]
    internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
    {
        return new List<FeatureDefinitionFightingStyleChoice>
        {
            FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin
        };
    }

    internal override FightingStyleDefinition GetStyle()
    {
        void TitanFightingComputeAttackModifier(
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
            attackModifier.attackToHitTrends.Add(
                new RuleDefinitions.TrendInfo(2, RuleDefinitions.FeatureSourceType.FightingStyle,
                    "Titan", this));
        }

        if (instance != null)
        {
            return instance;
        }

        var titanFightingAttackModifier = FeatureDefinitionOnComputeAttackModifierBuilder
            .Create("AttackModifierTitan", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent()
            .SetOnComputeAttackModifierDelegate(TitanFightingComputeAttackModifier)
            .AddToDB();

        instance = CustomizableFightingStyleBuilder
            .Create("Titan", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.FightingStyle,
                PathBerserker.GuiPresentation.SpriteReference)
            .SetFeatures(titanFightingAttackModifier)
            .AddToDB();

        return instance;
    }
}
