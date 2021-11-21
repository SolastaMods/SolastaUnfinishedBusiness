using ModKit;
using SolastaCommunityExpansion.Models;
using System.Linq;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class SubClassesDisplay
    {
        private const int MAX_COLUMNS = 4;
        private const float PIXELS_PER_COLUMN = 225;

        internal static void DisplaySubclasses()
        {
            bool toggle;
            int intValue;
            bool selectAll = Main.Settings.SubclassEnabled.Count == SubclassesContext.Subclasses.Count;

            UI.Label("");
            UI.Label("General:".yellow());

            UI.Label("");
            toggle = Main.Settings.SpellMasterUnlimitedArcaneRecovery;
            if (UI.Toggle("Enables unlimited ".white() + "Arcane Recovery".orange() + " on Wizard Spell Master\n".white() + "Must be enabled when the ability has available uses (or before character creation)".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.SpellMasterUnlimitedArcaneRecovery = toggle;
                Subclasses.Wizard.SpellMaster.UpdateRecoveryLimited();
            }

            UI.Label("");
            UI.Label("Overrides Rogue Con Artist ".white() + "Improved Manipulation".orange() + " Spell DC".white());
            intValue = Main.Settings.RogueConArtistSpellDCBoost;
            if (UI.Slider("", ref intValue, 0, 5, 3, "", UI.AutoWidth()))
            {
                Main.Settings.RogueConArtistSpellDCBoost = intValue;
            }
            UI.Label("");
            UI.Label("Overrides Wizard Master Manipulator ".white() + "Arcane Manipulation".orange() + " Spell DC".white());
            intValue = Main.Settings.MasterManipulatorSpellDCBoost;
            if (UI.Slider("", ref intValue, 0, 5, 2, "", UI.AutoWidth()))
            {
                Main.Settings.MasterManipulatorSpellDCBoost = intValue;
            }

            UI.Label("");
            UI.Label("Subclasses:".yellow());
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
                            toggle = Main.Settings.SubclassEnabled.Contains(keyValuePair.Key);
                            var subclass = keyValuePair.Value.GetSubclass();
                            var title = Gui.Format(subclass.GuiPresentation.Title);

                            if (flip)
                            {
                                title = title.yellow();
                            }

                            if (UI.Toggle(title, ref toggle, UI.ChecklyphOn, UI.CheckGlyphOff, PIXELS_PER_COLUMN))
                            {
                                SubclassesContext.Switch(keyValuePair.Key, toggle);
                            }

                            if (Main.Settings.SubclassSliderPosition == 1)
                            {
                                var description = Gui.Format(subclass.GuiPresentation.Description);

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
}
