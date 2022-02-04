using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;

namespace SolastaCommunityExpansion.FightingStyles
{
    internal class Crippling : AbstractFightingStyle
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

                // Prevent Dash until end of next turn -> how? it's not an action, but has a lot of dedicated code
                // Reduce speed by 10 until end of next turn
                // Must be a successful melee attack
                // NO LIMIT per round (wow!)


                
                GuiPresentationBuilder gui = new GuiPresentationBuilder("FightingStyle/&CripplingTitle", "FightingStyle/&CripplingDescription");
                gui.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.PathBerserker.GuiPresentation.SpriteReference);
                CustomizableFightingStyleBuilder builder = new CustomizableFightingStyleBuilder("Crippling", "b570d166-c65c-4a68-ab78-aeb16d491fce",
                    new List<FeatureDefinition>() { DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityConditionHindered },
                    gui.Build());
                instance = builder.AddToDB();
            }
            return instance;
        }
    }
}
