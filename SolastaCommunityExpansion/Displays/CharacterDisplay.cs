using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Displays;

internal static class CharacterDisplay
{
    internal static void DisplayCharacter()
    {
        int intValue;
        bool toggle;

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&InitialChoices"));
        UI.Label("");

        toggle = Main.Settings.AddHelpActionToAllRaces;
        if (UI.Toggle(Gui.Localize("ModUi/&AddHelpActionToAllRaces"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddHelpActionToAllRaces = toggle;
            PowersContext.Switch();
        }

        // TODO: vision changes only take effect when creating a character. not sure if new block label is clear enough on intentions or we need more explanation here.
        toggle = Main.Settings.DisableSenseDarkVisionFromAllRaces;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableSenseDarkVisionFromAllRaces"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableSenseDarkVisionFromAllRaces = toggle;
        }

        toggle = Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableSenseSuperiorDarkVisionFromAllRaces"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces = toggle;
        }

        UI.Label("");

        toggle = Main.Settings.EnableAlternateHuman;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableAlternateHuman"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableAlternateHuman = toggle;
            InitialChoicesContext.SwitchFirstLevelTotalFeats();
        }

        toggle = Main.Settings.EnableFlexibleBackgrounds;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFlexibleBackgrounds"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFlexibleBackgrounds = toggle;
            FlexibleBackgroundsContext.Switch();
        }

        toggle = Main.Settings.EnableFlexibleRaces;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFlexibleRaces"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFlexibleRaces = toggle;
            FlexibleRacesContext.Switch();
        }

        toggle = Main.Settings.EnableEpicPointsAndArray;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableEpicPointsAndArray"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableEpicPointsAndArray = toggle;
        }

        UI.Label("");

        intValue = Main.Settings.TotalFeatsGrantedFistLevel;
        if (UI.Slider(Gui.Localize("ModUi/&TotalFeatsGrantedFistLevel"), ref intValue,
                InitialChoicesContext.MIN_INITIAL_FEATS, InitialChoicesContext.MAX_INITIAL_FEATS, 0, "",
                UI.AutoWidth()))
        {
            Main.Settings.TotalFeatsGrantedFistLevel = intValue;
            InitialChoicesContext.SwitchFirstLevelTotalFeats();
        }

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&Multiclass"));
        UI.Label("");

        toggle = Main.Settings.EnableMinInOutAttributes;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMinInOutAttributes"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMinInOutAttributes = toggle;
        }

        toggle = Main.Settings.EnableRelearnSpells;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRelearnSpells"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRelearnSpells = toggle;
        }

        toggle = Main.Settings.DisplayAllKnownSpellsDuringLevelUp;
        if (UI.Toggle(Gui.Localize("ModUi/&DisplayAllKnownSpellsDuringLevelUp"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisplayAllKnownSpellsDuringLevelUp = toggle;
        }

        UI.Label("");

        intValue = Main.Settings.MaxAllowedClasses;
        if (UI.Slider(Gui.Localize("ModUi/&MaxAllowedClasses"), ref intValue, 1,
                MulticlassContext.MAX_CLASSES, MulticlassContext.MAX_CLASSES, "",
                UI.AutoWidth()))
        {
            Main.Settings.MaxAllowedClasses = intValue;
        }

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&Progression"));
        UI.Label("");

        toggle = Main.Settings.EnablesAsiAndFeat;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablesAsiAndFeat"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablesAsiAndFeat = toggle;
            InitialChoicesContext.SwitchAsiAndFeat();
        }

        toggle = Main.Settings.EnableFeatsAtEvenLevels;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFeatsAtEvenLevels"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFeatsAtEvenLevels = toggle;
            InitialChoicesContext.SwitchEvenLevelFeats();
        }

        toggle = Main.Settings.EnableLevel20;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableLevel20"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableLevel20 = toggle;
        }

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&Visuals"));
        UI.Label("");

        toggle = Main.Settings.OfferAdditionalLoreFriendlyNames;
        if (UI.Toggle(Gui.Localize("ModUi/&OfferAdditionalLoreFriendlyNames"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.OfferAdditionalLoreFriendlyNames = toggle;
        }

        toggle = Main.Settings.UnlockAllNpcFaces;
        if (UI.Toggle(Gui.Localize("ModUi/&UnlockAllNpcFaces"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UnlockAllNpcFaces = toggle;
        }

        UI.Label("");

        toggle = Main.Settings.AllowUnmarkedSorcerers;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowUnmarkedSorcerers"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowUnmarkedSorcerers = toggle;
        }

        toggle = Main.Settings.UnlockMarkAndTatoosForAllCharacters;
        if (UI.Toggle(Gui.Localize("ModUi/&UnlockMarkAndTatoosForAllCharacters"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UnlockMarkAndTatoosForAllCharacters = toggle;
        }

        toggle = Main.Settings.UnlockGlowingColorsForAllMarksAndTatoos;
        if (UI.Toggle(Gui.Localize("ModUi/&UnlockGlowingColorsForAllMarksAndTatoos"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UnlockGlowingColorsForAllMarksAndTatoos = toggle;
        }

        UI.Label("");

        toggle = Main.Settings.UnlockGlowingEyeColors;
        if (UI.Toggle(Gui.Localize("ModUi/&UnlockGlowingEyeColors"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UnlockGlowingEyeColors = toggle;
        }

        toggle = Main.Settings.AddNewBrightEyeColors;
        if (UI.Toggle(Gui.Localize("ModUi/&AddNewBrightEyeColors"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddNewBrightEyeColors = toggle;
        }

        toggle = Main.Settings.UnlockEyeStyles;
        if (UI.Toggle(Gui.Localize("ModUi/&UnlockEyeStyles"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UnlockEyeStyles = toggle;
        }

        UI.Label("");
    }
}
