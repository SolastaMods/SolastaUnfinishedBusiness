using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.FightingStyles
{
    internal class BlindFighting : AbstractFightingStyle
    {
        private CustomizableFightingStyle instance;

        internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
        {
            return new List<FeatureDefinitionFightingStyleChoice>() {
                DatabaseHelper.FeatureDefinitionFightingStyleChoices.FightingStyleChampionAdditional,
                DatabaseHelper.FeatureDefinitionFightingStyleChoices.FightingStyleFighter,
                DatabaseHelper.FeatureDefinitionFightingStyleChoices.FightingStylePaladin,
                DatabaseHelper.FeatureDefinitionFightingStyleChoices.FightingStyleRanger,};
        }

        internal override FightingStyleDefinition GetStyle()
        {
            if (instance == null)
            {
                GuiPresentationBuilder gui = new GuiPresentationBuilder("FightingStyle/&BlindFightingDescription", "FightingStyle/&BlindFightingTitle");
                gui.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.RangerShadowTamer.GuiPresentation.SpriteReference);
                CustomizableFightingStyleBuilder builder = new CustomizableFightingStyleBuilder("BlindFightingStlye", "a0df0cb6-640f-494e-b752-b746fa79bede",
                    new List<FeatureDefinition>() { DatabaseHelper.FeatureDefinitionSenses.SenseBlindSight2 },
                    gui.Build());
                instance = builder.AddToDB();
            }
            return instance;
        }
    }
}
