using ModKit;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Subclasses.Rogue;
using SolastaCommunityExpansion.Subclasses.Wizard;
using System.Linq;


namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class ClassesAndSubclassesDisplay
    {
        private const int MAX_COLUMNS = 4;

        private const float PIXELS_PER_COLUMN = 240;

        private static void DisplayClasses()
        {
            bool toggle;
            int intValue;
            bool selectAll = Main.Settings.ClassEnabled.Count == ClassesContext.Classes.Count;

            UI.Label("");

            toggle = Main.Settings.DisplayClassesToggle;
            if (UI.DisclosureToggle("Classes:".yellow(), ref toggle, 200))
            {
                Main.Settings.DisplayClassesToggle = toggle;
            }

            if (Main.Settings.DisplayClassesToggle)
            {
                if (ClassesContext.Classes.Count == 0)
                {
                    UI.Label("");
                    UI.Label("No unofficial classes available on this mod yet...".bold().red());

                    return;
                }

                UI.Label("");
                if (UI.Toggle("Select all", ref selectAll))
                {
                    foreach (var keyValuePair in ClassesContext.Classes)
                    {
                        ClassesContext.Switch(keyValuePair.Key, selectAll);
                    }
                }

                intValue = Main.Settings.ClassSliderPosition;
                if (UI.Slider("slide left for description / right to collapse".white().bold().italic(), ref intValue, 1, MAX_COLUMNS, 1, ""))
                {
                    Main.Settings.ClassSliderPosition = intValue;
                }

                UI.Label("");

                int columns;
                var flip = false;
                var current = 0;
                var classesCount = ClassesContext.Classes.Count;

                using (UI.VerticalScope())
                {
                    while (current < classesCount)
                    {
                        columns = Main.Settings.ClassSliderPosition;

                        using (UI.HorizontalScope())
                        {
                            while (current < classesCount && columns-- > 0)
                            {
                                var keyValuePair = ClassesContext.Classes.ElementAt(current);
                                var characterClass = keyValuePair.Value;
                                var title = characterClass.FormatTitle();

                                if (flip)
                                {
                                    title = title.yellow();
                                }

                                toggle = Main.Settings.ClassEnabled.Contains(keyValuePair.Key);
                                if (UI.Toggle(title, ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                                {
                                    ClassesContext.Switch(keyValuePair.Key, toggle);
                                }

                                if (Main.Settings.ClassSliderPosition == 1)
                                {
                                    var description = characterClass.FormatDescription();

                                    if (flip)
                                    {
                                        description = description.yellow();
                                    }

                                    UI.Label(description, UI.Width(PIXELS_PER_COLUMN * 3));

                                    flip = !flip;
                                }

                                current++;
                            }
                        }
                    }
                }
            }
        }

        private static void DisplaySubclasses()
        {
            bool toggle;
            int intValue;
            bool selectAll = Main.Settings.SubclassEnabled.Count == SubclassesContext.Subclasses.Count;

            UI.Label("");

            toggle = Main.Settings.DisplaySubclassesToggle;
            if (UI.DisclosureToggle("Subclasses:".yellow(), ref toggle, 200))
            {
                Main.Settings.DisplaySubclassesToggle = toggle;
            }

            if (Main.Settings.DisplaySubclassesToggle)
            {
                UI.Label("");
                toggle = Main.Settings.EnableUnlimitedArcaneRecoveryOnWizardSpellMaster;
                if (UI.Toggle("Enable unlimited ".white() + "Arcane Recovery".orange() + " on " + "Wizard".orange() + " Spell Master\n".white() + "Must be enabled when the ability has available uses (or before character creation)".italic().yellow(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableUnlimitedArcaneRecoveryOnWizardSpellMaster = toggle;
                    SpellMaster.UpdateRecoveryLimited();
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

                UI.Label("");
                if (UI.Toggle("Select all", ref selectAll))
                {
                    foreach (var keyValuePair in SubclassesContext.Subclasses)
                    {
                        SubclassesContext.Switch(keyValuePair.Key, selectAll);
                    }
                }

                intValue = Main.Settings.SubclassSliderPosition;
                if (UI.Slider("slide left for description / right to collapse".white().bold().italic(), ref intValue, 1, MAX_COLUMNS, 1, ""))
                {
                    Main.Settings.SubclassSliderPosition = intValue;
                }

                UI.Label("");

                int columns;
                var flip = false;
                var current = 0;
                var subclassesCount = SubclassesContext.Subclasses.Count;

                using (UI.VerticalScope())
                {
                    while (current < subclassesCount)
                    {
                        columns = Main.Settings.SubclassSliderPosition;

                        using (UI.HorizontalScope())
                        {
                            while (current < subclassesCount && columns-- > 0)
                            {
                                var keyValuePair = SubclassesContext.Subclasses.ElementAt(current);
                                var subclass = keyValuePair.Value.GetSubclass();
                                var suffix = keyValuePair.Value.GetSubclassChoiceList().SubclassSuffix;
                                var title = $"{subclass.FormatTitle()} ({suffix})";

                                if (flip)
                                {
                                    title = title.yellow();
                                }

                                toggle = Main.Settings.SubclassEnabled.Contains(keyValuePair.Key);
                                if (UI.Toggle(title, ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                                {
                                    SubclassesContext.Switch(keyValuePair.Key, toggle);
                                }

                                if (Main.Settings.SubclassSliderPosition == 1)
                                {
                                    var description = subclass.FormatDescription();

                                    if (flip)
                                    {
                                        description = description.yellow();
                                    }

                                    UI.Label(description, UI.Width(PIXELS_PER_COLUMN * 3));

                                    flip = !flip;
                                }

                                current++;
                            }
                        }
                    }
                }
            }
        }

        internal static void DisplayClassesAndSubclasses()
        {
            DisplayClasses();
            DisplaySubclasses();
            UI.Label("");
        }
    }

}
