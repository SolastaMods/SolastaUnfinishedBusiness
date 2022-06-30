#if false
using System.Collections.Generic;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaCommunityExpansion.FightingStyles;

internal class BlindFighting : AbstractFightingStyle
{
    private FightingStyleDefinitionCustomizable instance;

    internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
    {
        return new List<FeatureDefinitionFightingStyleChoice>
        {
            FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
        };
    }

    internal override FightingStyleDefinition GetStyle()
    {
        return instance ??= CustomizableFightingStyleBuilder
            .Create("BlindFightingStlye", "a0df0cb6-640f-494e-b752-b746fa79bede")
            .SetGuiPresentation("BlindFighting", Category.FightingStyle,
                RangerShadowTamer.GuiPresentation.SpriteReference)
            .SetFeatures(DatabaseHelper.FeatureDefinitionSenses.SenseBlindSight2)
            .AddToDB();
    }
}
#endif
