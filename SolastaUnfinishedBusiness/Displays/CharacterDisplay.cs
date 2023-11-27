using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class CharacterDisplay
{
    internal static void DisplayCharacter()
    {
        UI.Label();
        UI.Label(Gui.Localize("ModUi/&InitialChoices"));
        UI.Label();

        var toggle = Main.Settings.EnableFlexibleBackgrounds;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFlexibleBackgrounds"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFlexibleBackgrounds = toggle;
            FlexibleBackgroundsContext.SwitchFlexibleBackgrounds();
        }

        UI.Label();

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

        UI.Label();

        toggle = Main.Settings.AddHelpActionToAllRaces;
        if (UI.Toggle(Gui.Localize("ModUi/&AddHelpActionToAllRaces"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddHelpActionToAllRaces = toggle;
            CharacterContext.SwitchHelpPower();
        }

        toggle = Main.Settings.ChangeDragonbornElementalBreathUsages;
        if (UI.Toggle(Gui.Localize("ModUi/&ChangeDragonbornElementalBreathUsages"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ChangeDragonbornElementalBreathUsages = toggle;
            CharacterContext.SwitchDragonbornElementalBreathUsages();
        }

        toggle = Main.Settings.EnableAlternateHuman;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableAlternateHuman"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableAlternateHuman = toggle;
            CharacterContext.SwitchFirstLevelTotalFeats();
        }

        toggle = Main.Settings.EnableFlexibleRaces;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFlexibleRaces"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFlexibleRaces = toggle;
            FlexibleRacesContext.SwitchFlexibleRaces();
        }

        UI.Label();

        toggle = Main.Settings.AddDarknessPerceptiveToDarkRaces;
        if (UI.Toggle(Gui.Localize("ModUi/&AddDarknessPerceptiveToDarkRaces"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddDarknessPerceptiveToDarkRaces = toggle;
            CharacterContext.SwitchDarknessPerceptive();
        }

        UI.Label();

        toggle = Main.Settings.RaceLightSensitivityApplyOutdoorsOnly;
        if (UI.Toggle(Gui.Localize("ModUi/&RaceLightSensitivityApplyOutdoorsOnly"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RaceLightSensitivityApplyOutdoorsOnly = toggle;
        }

        UI.Label();
        UI.Label();

        toggle = Main.Settings.EnableEpicPointsAndArray;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableEpicPointsAndArray"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableEpicPointsAndArray = toggle;
        }

        toggle = Main.Settings.ImproveLevelUpFeaturesSelection;
        if (UI.Toggle(Gui.Localize("ModUi/&ImproveLevelUpFeaturesSelection"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ImproveLevelUpFeaturesSelection = toggle;
            CharacterContext.SwitchRangerHumanoidFavoredEnemy();
        }

        UI.Label();
        UI.Label();

        var intValue = Main.Settings.TotalFeatsGrantedFirstLevel;
        if (UI.Slider(Gui.Localize("ModUi/&TotalFeatsGrantedFirstLevel"), ref intValue,
                CharacterContext.MinInitialFeats, CharacterContext.MaxInitialFeats, 0, "",
                UI.AutoWidth()))
        {
            Main.Settings.TotalFeatsGrantedFirstLevel = intValue;
            CharacterContext.SwitchFirstLevelTotalFeats();
        }

        UI.Label();

        toggle = Main.Settings.DisableLevelPrerequisitesOnModFeats;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableClassPrerequisitesOnModFeats"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableLevelPrerequisitesOnModFeats = toggle;
        }

        toggle = Main.Settings.DisableRacePrerequisitesOnModFeats;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableRacePrerequisitesOnModFeats"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableRacePrerequisitesOnModFeats = toggle;
        }

        toggle = Main.Settings.DisableCastSpellPreRequisitesOnModFeats;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableCastSpellPreRequisitesOnModFeats"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableCastSpellPreRequisitesOnModFeats = toggle;
        }

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&Progression"));
        UI.Label();

        toggle = Main.Settings.EnableLevel20;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableLevel20"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableLevel20 = toggle;
        }

        toggle = Main.Settings.EnableMulticlass;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMulticlass"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMulticlass = toggle;
            Main.Settings.MaxAllowedClasses = MulticlassContext.DefaultClasses;
            Main.Settings.EnableMinInOutAttributes = true;
            Main.Settings.DisplayAllKnownSpellsDuringLevelUp = true;
            Main.Settings.DisplayPactSlotsOnSpellSelectionPanel = true;
        }

        if (Main.Settings.EnableMulticlass)
        {
            UI.Label();

            intValue = Main.Settings.MaxAllowedClasses;
            if (UI.Slider(Gui.Localize("ModUi/&MaxAllowedClasses"), ref intValue,
                    2, MulticlassContext.MaxClasses, MulticlassContext.DefaultClasses, "", UI.AutoWidth()))
            {
                Main.Settings.MaxAllowedClasses = intValue;
            }

            UI.Label();

            toggle = Main.Settings.EnableMinInOutAttributes;
            if (UI.Toggle(Gui.Localize("ModUi/&EnableMinInOutAttributes"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableMinInOutAttributes = toggle;
            }

            UI.Label();

            toggle = Main.Settings.DisplayAllKnownSpellsDuringLevelUp;
            if (UI.Toggle(Gui.Localize("ModUi/&DisplayAllKnownSpellsDuringLevelUp"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisplayAllKnownSpellsDuringLevelUp = toggle;
            }

            toggle = Main.Settings.DisplayPactSlotsOnSpellSelectionPanel;
            if (UI.Toggle(Gui.Localize("ModUi/&DisplayPactSlotsOnSpellSelectionPanel"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.DisplayPactSlotsOnSpellSelectionPanel = toggle;
            }

            UI.Label();
            UI.Label(Gui.Localize("ModUi/&MulticlassKeyHelp"));
            UI.Label();
        }

        toggle = Main.Settings.EnableRelearnSpells;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRelearnSpells"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRelearnSpells = toggle;
        }

        UI.Label();

        toggle = Main.Settings.EnablesAsiAndFeat;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablesAsiAndFeat"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablesAsiAndFeat = toggle;
            CharacterContext.SwitchAsiAndFeat();
        }

        toggle = Main.Settings.EnableFeatsAtEveryFourLevels;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFeatsAtEvenLevels"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFeatsAtEveryFourLevels = toggle;
            CharacterContext.SwitchEveryFourLevelsFeats();
        }

        toggle = Main.Settings.EnableFeatsAtEveryFourLevelsMiddle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFeatsAtEvenLevelsMiddle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFeatsAtEveryFourLevelsMiddle = toggle;
            CharacterContext.SwitchEveryFourLevelsFeats(true);
        }

        UI.Label();

        toggle = Main.Settings.EnableBarbarianFightingStyle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBarbarianFightingStyle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBarbarianFightingStyle = toggle;
            CharacterContext.SwitchBarbarianFightingStyle();
        }

        toggle = Main.Settings.GrantScimitarSpecializationToBardRogue;
        if (UI.Toggle(Gui.Localize("ModUi/&GrantScimitarSpecializationToBarkMonkRogue"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.GrantScimitarSpecializationToBardRogue = toggle;
            CharacterContext.SwitchScimitarWeaponSpecialization();
        }

        UI.Label();

        toggle = Main.Settings.AddFighterLevelToIndomitableSavingReroll;
        if (UI.Toggle(Gui.Localize("ModUi/&AddFighterLevelToIndomitableSavingReroll"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddFighterLevelToIndomitableSavingReroll = toggle;

            DatabaseHelper.ActionDefinitions.UseIndomitableResistance.GuiPresentation.description = toggle
                ? "Feature/&EnhancedIndomitableResistanceDescription"
                : "Feature/&IndomitableResistanceDescription";
        }

        toggle = Main.Settings.EnableFighterWeaponSpecialization;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFighterWeaponSpecialization"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFighterWeaponSpecialization = toggle;
            CharacterContext.SwitchFighterWeaponSpecialization();
        }

        UI.Label();

        toggle = Main.Settings.AddHumanoidFavoredEnemyToRanger;
        if (UI.Toggle(Gui.Localize("ModUi/&AddHumanoidFavoredEnemyToRanger"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddHumanoidFavoredEnemyToRanger = toggle;
            CharacterContext.SwitchRangerHumanoidFavoredEnemy();
        }

        toggle = Main.Settings.EnableRangerNatureShroudAt10;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRangerNatureShroudAt10"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRangerNatureShroudAt10 = toggle;
            CharacterContext.SwitchRangerNatureShroud();
        }

        UI.Label();

        toggle = Main.Settings.EnableMonkAbundantKi;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkAbundantKi"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkAbundantKi = toggle;
            CharacterContext.SwitchMonkAbundantKi();
        }

        toggle = Main.Settings.EnableMonkFightingStyle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkFightingStyle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkFightingStyle = toggle;
            CharacterContext.SwitchMonkFightingStyle();
        }

        toggle = Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkDoNotRequireAttackActionForFlurry"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry = toggle;
        }

        toggle = Main.Settings.EnableMonkWeaponSpecialization;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableMonkWeaponSpecialization"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableMonkWeaponSpecialization = toggle;
            CharacterContext.SwitchMonkWeaponSpecialization();
        }

        UI.Label();

        toggle = Main.Settings.EnableRogueCunningStrike;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueCunningStrike"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueCunningStrike = toggle;
            CharacterContext.SwitchRogueCunningStrike();
        }

        toggle = Main.Settings.EnableRogueFightingStyle;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueFightingStyle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueFightingStyle = toggle;
            CharacterContext.SwitchRogueFightingStyle();
        }

        toggle = Main.Settings.EnableRogueSteadyAim;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRogueSteadyAim"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRogueSteadyAim = toggle;
            CharacterContext.SwitchRogueSteadyAim();
        }

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&Visuals"));
        UI.Label();

        toggle = Main.Settings.AllowBeardlessDwarves;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowBeardlessDwarves"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowBeardlessDwarves = toggle;
        }

        UI.Label();

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

        UI.Label();

        toggle = Main.Settings.AllowUnmarkedSorcerers;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowUnmarkedSorcerers"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowUnmarkedSorcerers = toggle;
        }

        toggle = Main.Settings.UnlockMarkAndTattoosForAllCharacters;
        if (UI.Toggle(Gui.Localize("ModUi/&UnlockMarkAndTattoosForAllCharacters"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UnlockMarkAndTattoosForAllCharacters = toggle;
        }

        toggle = Main.Settings.UnlockGlowingColorsForAllMarksAndTattoos;
        if (UI.Toggle(Gui.Localize("ModUi/&UnlockGlowingColorsForAllMarksAndTattoos"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UnlockGlowingColorsForAllMarksAndTattoos = toggle;
        }

        UI.Label();

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

        toggle = Main.Settings.UnlockSkinColors;
        if (UI.Toggle(Gui.Localize("ModUi/&UnlockSkinColors"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UnlockSkinColors = toggle;
        }

        UI.Label();
    }
}
