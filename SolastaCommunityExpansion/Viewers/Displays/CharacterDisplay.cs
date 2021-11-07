using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class CharacterDisplay
    {
        private static readonly string reqRestart = "[requires restart]".italic().red();

        internal static void DisplayCharacter()
        {
            int intValue;
            bool toggle;

            UI.Label("");
            UI.Label("Progression:".yellow());
            UI.Label("");

            toggle = Main.Settings.EnableLevel20;
            if (UI.Toggle("Enables Level 20 " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableLevel20 = toggle;
            }

            toggle = Main.Settings.EnablesAsiAndFeat;
            if (UI.Toggle("Enables both ASI and feat", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnablesAsiAndFeat = toggle;
                AsiAndFeatContext.Switch(toggle);
            }

            toggle = Main.Settings.EnableEpicPoints;
            if (UI.Toggle("Enables epic [17,15,13,12,10,8] array instead of standard [15,14,13,12,10,8]", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableEpicPoints = toggle;
                EpicArrayContext.Load();
            }

            toggle = Main.Settings.EnableAlternateHuman;
            if (UI.Toggle("Enables the Alternate Human [+1 feat / +2 attribute choices / +1 skill]", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableAlternateHuman = toggle;
                InitialChoicesContext.RefreshAllRacesInitialFeats();
            }

            toggle = Main.Settings.EnableFlexibleBackgrounds;
            if (UI.Toggle("Enables flexible backgrounds [Select skill and tool proficiencies from backgrounds]", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableFlexibleBackgrounds = toggle;
                FlexibleBackgroundsContext.Switch(toggle);
            }

            toggle = Main.Settings.EnableFlexibleRaces;
            if (UI.Toggle("Enables flexible races [Assign ability score points instead of the racial defaults (High Elf has 3 points to assign instead of +2 Dex/+1 Int)]", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableFlexibleRaces = toggle;
                FlexibleRacesContext.Switch(toggle);
            }

            UI.Label("");
            intValue = Main.Settings.AllRacesInitialFeats;
            if (UI.Slider("Total feats granted at first level".white(), ref intValue, Settings.MIN_INITIAL_FEATS, Settings.MAX_INITIAL_FEATS, 0, "", UI.AutoWidth()))
            {
                Main.Settings.AllRacesInitialFeats = intValue;
                InitialChoicesContext.RefreshAllRacesInitialFeats();
            }
        }

        public static void DisplayRulesSettings()
        {
            bool toggle;

            UI.Label("");
            UI.Label("SRD / House Rules:".yellow());

            UI.Label("");

            toggle = Main.Settings.EnableSRDAdvantageRules;
            if (UI.Toggle("Uses official advantage / disadvantage rules", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableSRDAdvantageRules = toggle;
            }

            toggle = Main.Settings.EnableConditionBlindedShouldNotAllowOpportunityAttack;
            if (UI.Toggle("Blinded".orange() + " condition doesn't allow attack of opportunity", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableConditionBlindedShouldNotAllowOpportunityAttack = toggle;
                SrdAndHouseRulesContext.ApplyConditionBlindedShouldNotAllowOpportunityAttack();
            }

            UI.Label("");

            toggle = Main.Settings.PickPocketEnabled;
            if (UI.Toggle("Adds pickpocketable loot", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.PickPocketEnabled = toggle;
                if (toggle)
                {
                    PickPocketContext.Load();
                }
            }

            toggle = Main.Settings.DisableAutoEquip;
            if (UI.Toggle("Disables auto-equip of items in inventory", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.DisableAutoEquip = toggle;
            }

            // TODO: vision changes only take effect when creating a character. not sure if new block label is clear enough on intentions or we need more explanation here.
            toggle = Main.Settings.DisableSenseDarkVisionFromAllRaces;
            if (UI.Toggle("Disables " + "Sense Dark Vision".orange() + " from all races " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.DisableSenseDarkVisionFromAllRaces = toggle;
            }

            toggle = Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces;
            if (UI.Toggle("Disables " + "Superior Sense Dark Vision".orange() + " from all races " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces = toggle;
            }

            toggle = Main.Settings.ExactMerchantCostScaling;
            if (UI.Toggle("Scales merchant prices correctly / exactly", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.ExactMerchantCostScaling = toggle;
            }
        }
    }
}
