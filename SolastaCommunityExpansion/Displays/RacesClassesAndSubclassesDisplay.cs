using ModKit;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Subclasses.Rogue;
using SolastaCommunityExpansion.Subclasses.Wizard;
using static SolastaCommunityExpansion.Displays.Shared;

namespace SolastaCommunityExpansion.Displays;

internal static class RacesClassesAndSubclassesDisplay
{
    internal static void DisplayGeneral()
    {
        bool toggle;
        int intValue;

        UI.Label("");

        toggle = Main.Settings.DisplayGeneralRaceClassSubClassToggle;
        if (UI.DisclosureToggle(Gui.Localize("ModUi/&General"), ref toggle, 200))
        {
            Main.Settings.DisplayGeneralRaceClassSubClassToggle = toggle;
        }

        if (!Main.Settings.DisplayGeneralRaceClassSubClassToggle)
        {
            return;
        }

        UI.Label("");

        toggle = Main.Settings.EnableShortRestRechargeOfArcaneWeaponOnWizardArcaneFighter;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableShortRestRechargeOfArcaneWeaponOnWizardArcaneFighter"),
                ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableShortRestRechargeOfArcaneWeaponOnWizardArcaneFighter = toggle;
            ArcaneFighter.UpdateEnchantWeapon();
        }

        toggle = Main.Settings.EnableUnlimitedArcaneRecoveryOnWizardSpellMaster;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableUnlimitedArcaneRecoveryOnWizardSpellMaster"),
                ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableUnlimitedArcaneRecoveryOnWizardSpellMaster = toggle;
            SpellMaster.UpdateBonusRecovery();
        }

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&OverrideRogueConArtistImprovedManipulationSpellDc"));
        intValue = Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc;
        if (UI.Slider("", ref intValue, 0, 5, 3, "", UI.AutoWidth()))
        {
            Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc = intValue;
            ConArtist.UpdateSpellDcBoost();
        }

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&OverrideWizardMasterManipulatorArcaneManipulationSpellDc"));
        intValue = Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc;
        if (UI.Slider("", ref intValue, 0, 5, 2, "", UI.AutoWidth()))
        {
            Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc = intValue;
            MasterManipulator.UpdateSpellDcBoost();
        }

        UI.Label("");
        toggle = Main.Settings.ReduceDarkelfLightPenalty;
        if (UI.Toggle(Gui.Localize("ModUi/&ReduceDarkelfLightPenalty"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ReduceDarkelfLightPenalty = toggle;
        }
    }

    internal static void DisplayClassesAndSubclasses()
    {
        bool displayToggle;
        int sliderPos;

        DisplayGeneral();

        displayToggle = Main.Settings.DisplayRacesToggle;
        sliderPos = Main.Settings.RaceSliderPosition;
        DisplayDefinitions(
            Gui.Localize("ModUi/&Races"),
            RacesContext.Switch,
            RacesContext.Races,
            Main.Settings.RaceEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplayRacesToggle = displayToggle;
        Main.Settings.RaceSliderPosition = sliderPos;

        displayToggle = Main.Settings.DisplayClassesToggle;
        sliderPos = Main.Settings.ClassSliderPosition;
        DisplayDefinitions(
            Gui.Localize("ModUi/&Classes"),
            ClassesContext.Switch,
            ClassesContext.Classes,
            Main.Settings.ClassEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplayClassesToggle = displayToggle;
        Main.Settings.ClassSliderPosition = sliderPos;

        displayToggle = Main.Settings.DisplaySubclassesToggle;
        sliderPos = Main.Settings.SubclassSliderPosition;
        DisplayDefinitions(
            Gui.Localize("ModUi/&Subclasses"),
            SubclassesContext.Switch,
            SubclassesContext.Subclasses,
            Main.Settings.SubclassEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplaySubclassesToggle = displayToggle;
        Main.Settings.SubclassSliderPosition = sliderPos;

        UI.Label("");
    }
}
