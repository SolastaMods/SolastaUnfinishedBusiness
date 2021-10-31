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

        private static int maxColumns = MAX_COLUMNS;

        public void DisplaySubclassesSettings()
        {
            UI.Label("");
            UI.Label("Settings:".yellow());

            UI.Label("");
            int intValue = Main.Settings.RogueConArtistSpellDCBoost;
            if (UI.Slider("Rogue Con Artist 'Improved Manipulation' Spell DC Boost", ref intValue, 0, 5, 3, "", UI.AutoWidth()))
            {
                Main.Settings.RogueConArtistSpellDCBoost = intValue;
            }

            UI.Label("");
            UI.Slider("Max columns below", ref maxColumns, 1, MAX_COLUMNS * 2, MAX_COLUMNS, "", UI.AutoWidth());

            UI.Label("");

            int columns;
            var current = 0;
            var subclassesCount = Models.SubclassesContext.Subclasses.Count;

            using (UI.VerticalScope())
            {
                while (current < subclassesCount)
                {
                    columns = maxColumns;

                    using (UI.HorizontalScope())
                    {
                        while (current < subclassesCount && columns-- > 0)
                        {
                            var keyValuePair = Models.SubclassesContext.Subclasses.ElementAt(current);
                            var toggle = !Main.Settings.SubclassHidden.Contains(keyValuePair.Key);
                            var title = Gui.Format(keyValuePair.Value.GetSubclass().GuiPresentation.Title);

                            if (UI.Toggle(title, ref toggle, PIXELS_PER_COLUMN))
                            {
                                Models.SubclassesContext.Switch(keyValuePair.Key, toggle);
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
