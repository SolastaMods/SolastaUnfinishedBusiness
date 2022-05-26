using ModKit;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Displays.Shared;

namespace SolastaCommunityExpansion.Displays
{
    internal static class CharacterDisplay
    {
        internal static void DisplayCharacter()
        {
            int intValue;
            bool toggle;

            UI.Label("");
            UI.Label("Initial choices:".yellow());
            UI.Label("");

            toggle = Main.Settings.AddHelpActionToAllRaces;
            if (UI.Toggle("Add the " + "Help".orange() + " action to all races", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.AddHelpActionToAllRaces = toggle;
                PowersContext.Switch();
            }

            // TODO: vision changes only take effect when creating a character. not sure if new block label is clear enough on intentions or we need more explanation here.
            toggle = Main.Settings.DisableSenseDarkVisionFromAllRaces;
            if (UI.Toggle("Disable " + "Sense Dark Vision".orange() + " from all races " + RequiresRestart, ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.DisableSenseDarkVisionFromAllRaces = toggle;
            }

            toggle = Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces;
            if (UI.Toggle("Disable " + "Superior Sense Dark Vision".orange() + " from all races " + RequiresRestart,
                    ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableAlternateHuman;
            if (UI.Toggle(
                    "Enable the alternate human " + "[+1 feat / +2 attribute choices / +1 skill]".italic().yellow(),
                    ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableAlternateHuman = toggle;
                InitialChoicesContext.SwitchFirstLevelTotalFeats();
            }

            toggle = Main.Settings.EnableFlexibleBackgrounds;
            if (UI.Toggle(
                    "Enable flexible backgrounds " +
                    "[select skill and tool proficiencies from backgrounds]".italic().yellow(), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.EnableFlexibleBackgrounds = toggle;
                FlexibleBackgroundsContext.Switch();
            }

            toggle = Main.Settings.EnableFlexibleRaces;
            if (UI.Toggle(
                    "Enable flexible races " +
                    "[assign ability score points instead of the racial defaults]".italic().yellow(), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.EnableFlexibleRaces = toggle;
                FlexibleRacesContext.Switch();
            }

            toggle = Main.Settings.EnableEpicPointsAndArray;
            if (UI.Toggle(
                    "Enable an epic 35 points buy system and array " + "[17,15,13,12,10,8] ".italic().yellow() +
                    RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableEpicPointsAndArray = toggle;
            }

            UI.Label("");

            intValue = Main.Settings.TotalFeatsGrantedFistLevel;
            if (UI.Slider("Total feats granted at first level".white(), ref intValue,
                    InitialChoicesContext.MIN_INITIAL_FEATS, InitialChoicesContext.MAX_INITIAL_FEATS, 0, "",
                    UI.AutoWidth()))
            {
                Main.Settings.TotalFeatsGrantedFistLevel = intValue;
                InitialChoicesContext.SwitchFirstLevelTotalFeats();
            }

            UI.Label("");
            UI.Label("Multiclass:".yellow());
            UI.Label("");

            toggle = Main.Settings.EnableMinInOutAttributes;
            if (UI.Toggle("Enforce ability scores minimum in & out pre-requisites", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableMinInOutAttributes = toggle;
            }

            toggle = Main.Settings.EnableRelearnSpells;
            if (UI.Toggle("Can select cantrips or spells already learned from other classes", ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.EnableRelearnSpells = toggle;
            }

            toggle = Main.Settings.DisplayAllKnownSpellsDuringLevelUp;
            if (UI.Toggle("Display all known spells from other classes during level up", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisplayAllKnownSpellsDuringLevelUp = toggle;
            }

            UI.Label("");

            intValue = Main.Settings.MaxAllowedClasses;
            if (UI.Slider("Max allowed classes ".white() + RequiresRestart, ref intValue, 1,
                    MulticlassContext.MAX_CLASSES, MulticlassContext.MAX_CLASSES, "", UI.Width(50)))
            {
                Main.Settings.MaxAllowedClasses = intValue;
            }

            UI.Label("");
            UI.Label("Progression:".yellow());
            UI.Label("");

            toggle = Main.Settings.EnablesAsiAndFeat;
            if (UI.Toggle(
                    "Enable both attribute scores increase and feat selection " +
                    "[instead of an exclusive choice]".yellow().italic(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnablesAsiAndFeat = toggle;
                InitialChoicesContext.SwitchAsiAndFeat();
            }

            toggle = Main.Settings.EnableFeatsAtEvenLevels;
            if (UI.Toggle("Enable feats selection at class levels 2, 6, 10 and 14", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableFeatsAtEvenLevels = toggle;
                InitialChoicesContext.SwitchEvenLevelFeats();
            }

            UI.Label("");

            intValue = Main.Settings.MaxAllowedLevels;
            if (UI.Slider("Max allowed levels ".white() + RequiresRestart, ref intValue, Level20Context.GAME_MAX_LEVEL,
                    Level20Context.MOD_MAX_LEVEL, Level20Context.GAME_MAX_LEVEL, "", UI.Width(50)))
            {
                Main.Settings.MaxAllowedLevels = intValue;
            }

            UI.Label("");
            UI.Label("Visuals:".yellow());
            UI.Label("");

            toggle = Main.Settings.AllowUnmarkedSorcerers;
            if (UI.Toggle("Allow unmarked " + "Sorcerers".orange(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.AllowUnmarkedSorcerers = toggle;
            }

            toggle = Main.Settings.OfferAdditionalLoreFriendlyNames;
            if (UI.Toggle("Offer additional lore friendly names on character creation", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.OfferAdditionalLoreFriendlyNames = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.UnlockAllNpcFaces;
            if (UI.Toggle("Unlock all NPC faces", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.UnlockAllNpcFaces = toggle;
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

            UI.Label("");
        }
    }
}
