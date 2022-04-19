using ModKit;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Subclasses.Rogue;
using SolastaCommunityExpansion.Subclasses.Wizard;
using static SolastaCommunityExpansion.Displays.Shared;

namespace SolastaCommunityExpansion.Displays
{
    internal static class RacesClassesAndSubclassesDisplay
    {
        internal static void DisplayGeneral()
        {
            bool toggle;
            int intValue;

            UI.Label("");

            toggle = Main.Settings.DisplayGeneralRaceClassSubClassToggle;
            if (UI.DisclosureToggle("General:".yellow(), ref toggle, 200))
            {
                Main.Settings.DisplayGeneralRaceClassSubClassToggle = toggle;
            }

            if (!Main.Settings.DisplayGeneralRaceClassSubClassToggle)
            {
                return;
            }

            UI.Label("");
            toggle = Main.Settings.EnableUnlimitedArcaneRecoveryOnWizardSpellMaster;
            if (UI.Toggle("Enable unlimited ".white() + "Arcane Recovery".orange() + " on " + "Wizard".orange() + " Spell Master\n".white() + "Must be enabled when the ability has available uses or before character creation.".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableUnlimitedArcaneRecoveryOnWizardSpellMaster = toggle;
                SpellMaster.UpdateBonusRecovery();
            }

            UI.Label("");
            toggle = Main.Settings.EnableShortRestRechargeOfArcaneWeaponOnWizardArcaneFighter;
            if (UI.Toggle("Enable short rest recharge of ".white() + "Arcane Weapon".orange() + " on " + "Wizard".orange() + " Arcane Fighter\n".white(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableShortRestRechargeOfArcaneWeaponOnWizardArcaneFighter = toggle;
                ArcaneFighter.UpdateEnchantWeapon();
            }

            UI.Label("");
            UI.Label("Override " + "Rogue".orange() + " Con Artist ".white() + "Improved Manipulation".orange() + " Spell DC".white());
            intValue = Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc;
            if (UI.Slider("", ref intValue, 0, 5, 3, "", UI.AutoWidth()))
            {
                Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc = intValue;
                ConArtist.UpdateSpellDCBoost();
            }

            UI.Label("");
            UI.Label("Override " + "Wizard".orange() + " Master Manipulator ".white() + "Arcane Manipulation".orange() + " Spell DC".white());
            intValue = Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc;
            if (UI.Slider("", ref intValue, 0, 5, 2, "", UI.AutoWidth()))
            {
                Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc = intValue;
                MasterManipulator.UpdateSpellDCBoost();
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
                "Races".yellow(),
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
                "Classes".yellow(),
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
                "Subclasses".yellow(),
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
}
