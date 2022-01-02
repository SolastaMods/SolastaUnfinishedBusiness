using ModKit;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Viewers.Displays.Shared;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class CharacterDisplay
    {
        private static bool DisplayFaceUnlockSettings { get; set; }

        internal static void DisplayCharacter()
        {
            int intValue;
            bool toggle;

            UI.Label("");
            UI.Label("Initial choices:".yellow());
            UI.Label("");

            UI.Label(". All these settings only apply when creating a new hero as they get embed in the hero save file");

            UI.Label("");

            //
            // TODO: Test the help power...
            //

            //toggle = Main.Settings.AddHelpActionToAllClasses;
            //if (UI.Toggle("Add the " + "Help".orange() + " action to all classes", ref toggle, UI.AutoWidth()))
            //{
            //    Main.Settings.AddHelpActionToAllClasses = toggle;
            //    PowersContext.Switch();
            //}

            // TODO: vision changes only take effect when creating a character. not sure if new block label is clear enough on intentions or we need more explanation here.
            toggle = Main.Settings.DisableSenseDarkVisionFromAllRaces;
            if (UI.Toggle("Disable " + "Sense Dark Vision".orange() + " from all races " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisableSenseDarkVisionFromAllRaces = toggle;
            }

            toggle = Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces;
            if (UI.Toggle("Disable " + "Superior Sense Dark Vision".orange() + " from all races " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableAlternateHuman;
            if (UI.Toggle("Enable the alternate human " + "[+1 feat / +2 attribute choices / +1 skill]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableAlternateHuman = toggle;
                InitialChoicesContext.RefreshTotalFeatsGrantedFistLevel();
            }

            toggle = Main.Settings.EnableFlexibleBackgrounds;
            if (UI.Toggle("Enable flexible backgrounds " + "[select skill and tool proficiencies from backgrounds]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableFlexibleBackgrounds = toggle;
                FlexibleBackgroundsContext.Switch(toggle);
            }

            toggle = Main.Settings.EnableFlexibleRaces;
            if (UI.Toggle("Enable flexible races " + "[assign ability score points instead of the racial defaults]".italic().yellow() + "\ni.e.: High Elf has 3 points to assign instead of +2 Dex / +1 Int".italic(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableFlexibleRaces = toggle;
                FlexibleRacesContext.SwitchFlexibleRaces();
            }

            UI.Label("");

            toggle = Main.Settings.EnableEpicPoints;
            if (UI.Toggle("Enable an epic 35 points buy system " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableEpicPoints = toggle;
            }

            toggle = Main.Settings.EnableEpicArray;
            if (UI.Toggle("Enable an epic " + "[17,15,13,12,10,8]".italic().yellow() + " array instead of a standard " + "[15,14,13,12,10,8]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableEpicArray = toggle;
                EpicArrayContext.Load();
            }

            UI.Label("");

            intValue = Main.Settings.TotalFeatsGrantedFistLevel;
            if (UI.Slider("Total feats granted at first level".white(), ref intValue, InitialChoicesContext.MIN_INITIAL_FEATS, InitialChoicesContext.MAX_INITIAL_FEATS, 0, "", UI.AutoWidth()))
            {
                Main.Settings.TotalFeatsGrantedFistLevel = intValue;
                InitialChoicesContext.RefreshTotalFeatsGrantedFistLevel();
            }


            UI.Label("");
            UI.Label("Miscellaneous:".yellow());
            UI.Label("");

            toggle = Main.Settings.AllowExtraKeyboardCharactersInNames;
            if (UI.Toggle("Allow extra keyboard characters in names", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.AllowExtraKeyboardCharactersInNames = toggle;
            }

            toggle = Main.Settings.OfferAdditionalLoreFriendlyNames;
            if (UI.Toggle("Offer additional lore friendly names on character creation " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.OfferAdditionalLoreFriendlyNames = toggle;
            }

            UI.Label("");
            UI.Label("Progression:".yellow());
            UI.Label("");

            toggle = Main.Settings.EnablesAsiAndFeat;
            if (UI.Toggle("Enable both ASI and feat", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnablesAsiAndFeat = toggle;
                AsiAndFeatContext.Switch(toggle);
            }

            toggle = Main.Settings.EnableLevel20;
            if (UI.Toggle("Enable Level 20 " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableLevel20 = toggle;
            }

            toggle = Main.Settings.EnableRespec;
            if (UI.Toggle("Enable RESPEC", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableRespec = toggle;
            }

            UI.Label("");

            toggle = DisplayFaceUnlockSettings;
            if (UI.DisclosureToggle("Visuals: ".yellow() + RequiresRestart, ref toggle, 200))
            {
                DisplayFaceUnlockSettings = toggle;
            }

            if (DisplayFaceUnlockSettings)
            {
                UI.Label("");

                toggle = Main.Settings.UnlockAllNpcFaces;
                if (UI.Toggle("Unlock all NPC faces", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.UnlockAllNpcFaces = toggle;
                }

                toggle = Main.Settings.AllowUnmarkedSorcerers;
                if (UI.Toggle("Allow unmarked " + "Sorcerers".orange(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.AllowUnmarkedSorcerers = toggle;
                }

                toggle = Main.Settings.UnlockMarkAndTatoosForAllCharacters;
                if (UI.Toggle("Unlock markings and tattoos for all characters", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.UnlockMarkAndTatoosForAllCharacters = toggle;
                }

                toggle = Main.Settings.UnlockEyeStyles;
                if (UI.Toggle("Unlock eye styles", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.UnlockEyeStyles = toggle;
                }

                toggle = Main.Settings.UnlockGlowingEyeColors;
                if (UI.Toggle("Unlock glowing eye colors", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.UnlockGlowingEyeColors = toggle;
                }

                toggle = Main.Settings.UnlockGlowingColorsForAllMarksAndTatoos;
                if (UI.Toggle("Unlock glowing colors for all markings and tattoos", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.UnlockGlowingColorsForAllMarksAndTatoos = toggle;
                }
            }

            UI.Label("");
        }
    }
}
