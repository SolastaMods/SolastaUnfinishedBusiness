using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSizeDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaCommunityExpansion.FightingStyles;

internal class TitanFighting : AbstractFightingStyle
{
    private readonly Guid TITAN_FIGHTING_BASE_GUID = new("3f7f25de-0ff9-4b63-b38d-8cd7f3a381fc");
    private FightingStyleDefinitionCustomizable instance;

    internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
    {
        return new List<FeatureDefinitionFightingStyleChoice>
        {
            FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin
        };
    }

    internal override FightingStyleDefinition GetStyle()
    {
        void TitanFightingOnAttackDelegate(GameLocationCharacter attacker, GameLocationCharacter defender,
            ActionModifier attackModifier, RulesetAttackMode attackerAttackMode)
        {
            // melee attack only
            if (attacker == null || defender == null)
            {
                return;
            }

            // grant +2 hit if defender is large or bigger
            if (defender.RulesetCharacter.SizeDefinition != Large && defender.RulesetCharacter.SizeDefinition != Huge &&
                defender.RulesetCharacter.SizeDefinition != Gargantuan)
            {
                return;
            }

            attackerAttackMode.toHitBonus += 2;
            attackModifier.AttacktoHitTrends.Add(
                new RuleDefinitions.TrendInfo(2, RuleDefinitions.FeatureSourceType.FightingStyle,
                    "TitanFighting", this));
        }

        if (instance != null)
        {
            return instance;
        }

        var titanFightingAttackModifier = FeatureDefinitionOnAttackEffectBuilder
            .Create("TitanFightingAttackModifier", TITAN_FIGHTING_BASE_GUID)
            .SetGuiPresentationNoContent()
            .SetOnAttackDelegates(TitanFightingOnAttackDelegate, null)
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
