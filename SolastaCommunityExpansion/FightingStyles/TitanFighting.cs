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
    private readonly Guid titanFightingBaseGuid = new("3f7f25de-0ff9-4b63-b38d-8cd7f3a381fc");
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
                    "TitanFighting", this));
        }

        if (instance != null)
        {
            return instance;
        }

        var titanFightingAttackModifier = FeatureDefinitionOnComputeAttackModifierBuilder
            .Create("TitanFightingAttackModifier", titanFightingBaseGuid)
            .SetGuiPresentationNoContent()
            .SetOnComputeAttackModifierDelegate(TitanFightingComputeAttackModifier)
            .AddToDB();

        instance = CustomizableFightingStyleBuilder
            .Create("TitanFighting", "edc2a2d1-9f72-4825-b204-d810e911ed12")
            .SetGuiPresentation("TitanFighting", Category.FightingStyle,
                PathBerserker.GuiPresentation.SpriteReference)
            .SetFeatures(titanFightingAttackModifier)
            .AddToDB();

        return instance;
    }
}
