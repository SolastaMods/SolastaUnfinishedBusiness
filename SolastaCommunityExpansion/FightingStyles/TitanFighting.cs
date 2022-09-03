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
        void TitanFightingRollAttackMode(
            RulesetCharacter attacker,
            ref RulesetAttackMode attackMode, 
            RulesetActor target,
            BaseDefinition attackMethod,
            ref List<RuleDefinitions.TrendInfo> toHitTrends,
            bool ignoreAdvantage, 
            ref List<RuleDefinitions.TrendInfo> advantageTrends,
            bool opportunity, 
            int rollModifier)
        {
            // melee attack only
            if (attacker == null || target == null)
            {
                return;
            }

            var defender = GameLocationCharacter.GetFromActor(target);
            // grant +2 hit if defender is large or bigger
            // temporary disable for debug
            if (defender.RulesetCharacter.SizeDefinition != Large && defender.RulesetCharacter.SizeDefinition != Huge &&
                defender.RulesetCharacter.SizeDefinition != Gargantuan)
            {
                return;
            }
            
            attackMode.toHitBonus += 2;
            toHitTrends.Add(
                new RuleDefinitions.TrendInfo(2, RuleDefinitions.FeatureSourceType.FightingStyle,
                    "TitanFighting", this));
        }

        if (instance != null)
        {
            return instance;
        }

        var titanFightingAttackModifier = FeatureDefinitionOnRollAttackModeBuilder
            .Create("TitanFightingAttackModifier", titanFightingBaseGuid)
            .SetGuiPresentationNoContent()
            .SetOnRollAttackModeDelegate(TitanFightingRollAttackMode)
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
