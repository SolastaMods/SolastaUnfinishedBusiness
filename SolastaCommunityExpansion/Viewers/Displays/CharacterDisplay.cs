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

            toggle = Main.Settings.EnableLevel20;
            if (UI.Toggle("Enables Level 20 " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableLevel20 = toggle;
            }

            toggle = Main.Settings.EnableRespec;
            if (UI.Toggle("Enables RESPEC", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableRespec = toggle;
            }

            UI.Label("");

            // TODO: vision changes only take effect when creating a character. not sure if new block label is clear enough on intentions or we need more explanation here.
            toggle = Main.Settings.DisableSenseDarkVisionFromAllRaces;
            if (UI.Toggle("Disables " + "Sense Dark Vision".orange() + " from all races " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisableSenseDarkVisionFromAllRaces = toggle;
            }

            toggle = Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces;
            if (UI.Toggle("Disables " + "Superior Sense Dark Vision".orange() + " from all races " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces = toggle;
            }

            toggle = Main.Settings.IncreaseNormalVisionSenseRange;
            if (UI.Toggle("Increases " + "Sense Normal Vision".orange() + " range to enable long range attacks " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.IncreaseNormalVisionSenseRange = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableAlternateHuman;
            if (UI.Toggle("Enables the alternate human " + "[+1 feat / +2 attribute choices / +1 skill]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableAlternateHuman = toggle;
                InitialChoicesContext.RefreshAllRacesInitialFeats();
            }

            toggle = Main.Settings.EnableFlexibleBackgrounds;
            if (UI.Toggle("Enables flexible backgrounds " + "[select skill and tool proficiencies from backgrounds]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableFlexibleBackgrounds = toggle;
                FlexibleBackgroundsContext.Switch(toggle);
            }

            toggle = Main.Settings.EnableFlexibleRaces;
            if (UI.Toggle("Enables flexible races " + "[assign ability score points instead of the racial defaults]".italic().yellow() + "\ni.e.: High Elf has 3 points to assign instead of +2 Dex / +1 Int".italic(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableFlexibleRaces = toggle;
                FlexibleRacesContext.Switch(toggle);
            }
            UI.Label("");

            UI.Label("");

            toggle = Main.Settings.EnablesAsiAndFeat;
            if (UI.Toggle("Enables both ASI and feat", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnablesAsiAndFeat = toggle;
                AsiAndFeatContext.Switch(toggle);
            }

            toggle = Main.Settings.EnableEpicPoints;
            if (UI.Toggle("Enables an epic 35 points buy system " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableEpicPoints = toggle;
            }

            toggle = Main.Settings.EnableEpicArray;
            if (UI.Toggle("Enables an epic [17,15,13,12,10,8] array instead of a standard [15,14,13,12,10,8]", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableEpicArray = toggle;
                EpicArrayContext.Load();
            }

            UI.Label("");

            intValue = Main.Settings.AllRacesInitialFeats;
            if (UI.Slider("Total feats granted at first level".white(), ref intValue, Settings.MIN_INITIAL_FEATS, Settings.MAX_INITIAL_FEATS, 0, "", UI.AutoWidth()))
            {
                Main.Settings.AllRacesInitialFeats = intValue;
                InitialChoicesContext.RefreshAllRacesInitialFeats();
            }

            UI.Label("");

            toggle = Main.Settings.AllowExtraKeyboardCharactersInNames;
            if (UI.Toggle("Allows extra keyboard characters in names", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.AllowExtraKeyboardCharactersInNames = toggle;
            }

            toggle = Main.Settings.OfferAdditionalNames;
            if (UI.Toggle("Offers additional lore friendly names on character creation " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.OfferAdditionalNames = toggle;
            }

            UI.Label("");

            toggle = DisplayFaceUnlockSettings;
            if (UI.DisclosureToggle("Face unlocks: ".yellow() + RequiresRestart, ref toggle, 200)) 
            {
                DisplayFaceUnlockSettings = toggle;
            }

            if (DisplayFaceUnlockSettings)
            {
                UI.Label("");

                toggle = Main.Settings.EnableFaceUnlockNpcs;
                if (UI.Toggle("Unlocks all NPC faces", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableFaceUnlockNpcs = toggle;
                }

                toggle = Main.Settings.EnableFaceUnlockUnmarkedSorcerers;
                if (UI.Toggle("Allows unmarked " + "Sorcerers".orange(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableFaceUnlockUnmarkedSorcerers = toggle;
                }

                toggle = Main.Settings.EnableFaceUnlockMarkingsForAll;
                if (UI.Toggle("Unlocks markings and tattoos for all characters", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableFaceUnlockMarkingsForAll = toggle;
                }

                toggle = Main.Settings.EnableFaceUnlockEyeStyles;
                if (UI.Toggle("Unlocks eye styles", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableFaceUnlockEyeStyles = toggle;
                }

                toggle = Main.Settings.EnableFaceUnlockGlowingEyes;
                if (UI.Toggle("Unlocks glowing eye colors", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableFaceUnlockGlowingEyes = toggle;
                }

                toggle = Main.Settings.EnableFaceUnlockGlowingBodyDecorations;
                if (UI.Toggle("Unlocks glowing colors for all markings and tattoos", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableFaceUnlockGlowingBodyDecorations = toggle;
                }
            }

            UI.Label("");
        }
    }
}
