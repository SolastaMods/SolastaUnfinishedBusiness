using System.Linq;
using UnityModManagerNet;
using ModKit;

namespace SolastaContentExpansion.Viewers
{
    public class FeatsViewer : IMenuSelectablePage
    {
        public string Name => "Feats";

        public int Priority => 2;

        private const int MAX_COLUMNS = 4;
        private const float PIXELS_PER_COLUMN = 250;

        private static int maxColumns = MAX_COLUMNS;

        public void DisplayFeatsSettings()
        {
            UI.Label("");
            UI.Label("Settings: [game restart required]".yellow());
            
            UI.Label("");
            UI.Slider("Max columns below", ref maxColumns, 1, MAX_COLUMNS * 2, MAX_COLUMNS, "", UI.AutoWidth());

            UI.Label("");

            int columns;
            var current = 0;
            var featsCount = Models.FeatsContext.Feats.Count;

            using (UI.VerticalScope())
            {
                while (current < featsCount)
                {
                    columns = maxColumns;

                    using (UI.HorizontalScope())
                    {
                        while (current < featsCount && columns-- > 0)
                        {
                            var keyValuePair = Models.FeatsContext.Feats.ElementAt(current);
                            var toggle = !Main.Settings.FeatHidden.Contains(keyValuePair.Key);
                            var title = Gui.Format(keyValuePair.Value.GuiPresentation.Title);

                            if (UI.Toggle(title, ref toggle, PIXELS_PER_COLUMN))
                            {
                                Models.FeatsContext.Switch(keyValuePair.Key, toggle);
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

            DisplayFeatsSettings();
        }
    }
}