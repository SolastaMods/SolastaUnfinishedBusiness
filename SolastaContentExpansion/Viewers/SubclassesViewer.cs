using ModKit;
using System.Linq;
using UnityModManagerNet;

namespace SolastaContentExpansion.Viewers
{
   public class SubclassesViewer : IMenuSelectablePage
    {
        public string Name => "Subclasses";

        public int Priority => 3;

        private const int MAX_COLUMNS = 4;
        private const float PIXELS_PER_COLUMN = 250;

        public void DisplaySubclassesSettings()
        {
            bool toggle;
            int intValue;

            UI.Label("");
            UI.Label("Settings:".yellow());

            UI.Label("");
            toggle = Main.Settings.SpellMasterUnlimitedArcaneRecovery;
            if (UI.Toggle("Enables unlimited ".white() + "Arcane Recovery".orange() + " on Wizard Spell Master ".white() + "must be enabled when the ability has available uses (or before character creation).".red(), ref toggle, 0, UI.AutoWidth()))
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
                Main.Settings.RogueConArtistSpellDCBoost = intValue;
            }

            UI.Label("");
            UI.Label("Subclasses:".yellow());

            UI.Label("");
            intValue = Main.Settings.SubclassSliderPosition;
            if (UI.Slider("slide left for description / right to collapse".white(), ref intValue, 1, MAX_COLUMNS, 1, ""))
            {
                Main.Settings.SubclassSliderPosition = intValue;
            }

            UI.Label("");

            int columns;
            var flip = false;
            var current = 0;
            var subclassesCount = Models.SubclassesContext.Subclasses.Count;

            using (UI.VerticalScope())
            {
                while (current < subclassesCount)
                {
                    columns = Main.Settings.SubclassSliderPosition;

                    using (UI.HorizontalScope())
                    {
                        while (current < subclassesCount && columns-- > 0)
                        {
                            var keyValuePair = Models.SubclassesContext.Subclasses.ElementAt(current);
                            toggle = !Main.Settings.SubclassHidden.Contains(keyValuePair.Key);
                            var subclass = keyValuePair.Value.GetSubclass();
                            var title = Gui.Format(subclass.GuiPresentation.Title);

                            if (flip)
                            {
                                title = title.yellow();
                            }

                            if (UI.Toggle(title, ref toggle, PIXELS_PER_COLUMN))
                            {
                                Models.SubclassesContext.Switch(keyValuePair.Key, toggle);
                            }

                            if (Main.Settings.SubclassSliderPosition == 1)
                            {
                                var description = Gui.Format(subclass.GuiPresentation.Description);

                                if (flip)
                                {
                                    description = description.yellow();
                                }

                                UI.Label(description, UI.Width(PIXELS_PER_COLUMN * 4));

                                flip = !flip;
                            }

                            current++;
                        }
                    }
                }
            }
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Content Expansion".yellow().bold());
            UI.Div();

            DisplaySubclassesSettings();
        }
    }
}
