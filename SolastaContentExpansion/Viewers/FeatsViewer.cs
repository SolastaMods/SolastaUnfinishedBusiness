using System.Linq;
using UnityModManagerNet;
using ModKit;

namespace SolastaContentExpansion.Viewers
{
    public class FeatsViewer : IMenuSelectablePage
    {
        public string Name => "Feats";

        public int Priority => 2;

        private static string reqRestart = "[requires restart]".italic().red().bold();
        private const int MAX_COLUMNS = 4;
        private const float PIXELS_PER_COLUMN = 250;

        public void DisplayFeatsSettings()
        {
            UI.Label("");
            UI.Label("Feats: ".yellow() + reqRestart);
            
            UI.Label("");

            var intValue = Main.Settings.FeatSliderPosition;

            if (UI.Slider("slide left for description / right to collapse".white(), ref intValue, 1, MAX_COLUMNS, 1, ""))
            {
                Main.Settings.FeatSliderPosition = intValue;
            }

            UI.Label("");

            int columns;
            var flip = false;
            var current = 0;
            var featsCount = Models.FeatsContext.Feats.Count;

            using (UI.VerticalScope())
            {
                while (current < featsCount)
                {
                    columns = Main.Settings.FeatSliderPosition;

                    using (UI.HorizontalScope())
                    {
                        while (current < featsCount && columns-- > 0)
                        {
                            var keyValuePair = Models.FeatsContext.Feats.ElementAt(current);
                            var toggle = !Main.Settings.FeatHidden.Contains(keyValuePair.Key);
                            var title = Gui.Format(keyValuePair.Value.GuiPresentation.Title);

                            if (flip)
                            {
                                title = title.yellow();
                            }

                            if (UI.Toggle(title, ref toggle, PIXELS_PER_COLUMN))
                            {
                                Models.FeatsContext.Switch(keyValuePair.Key, toggle);
                            }

                            if (Main.Settings.FeatSliderPosition == 1)
                            {
                                var description = Gui.Format(keyValuePair.Value.GuiPresentation.Description);

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

            DisplayFeatsSettings();
        }
    }
}