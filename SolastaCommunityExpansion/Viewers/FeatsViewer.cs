using System.Linq;
using UnityModManagerNet;
using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Viewers
{
    public class FeatsViewer : IMenuSelectablePage
    {
        public string Name => "Feats";

        public int Priority => 2;

        private static bool selectAll = false;
        private const int MAX_COLUMNS = 4;
        private const float PIXELS_PER_COLUMN = 250;
        private static readonly string reqRestart = "[requires restart]".italic().red().bold();

        public void DisplayFeatsSettings()
        {
            bool toggle;
            int intValue;

            selectAll = Main.Settings.FeatEnabled.Count == FeatsContext.Feats.Count;

            UI.Label("");
            UI.Label("Settings: ".yellow() + reqRestart);

            UI.Label("");
            toggle = Main.Settings.PickPocketEnabled;
            if (UI.Toggle("Adds pickpocketable loot to monsters", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.PickPocketEnabled = toggle;
                if (toggle)
                {
                    PickPocketContext.Load();
                }
            }

            // todo make the acehigh power attack feats tunable here. It is already in the settings (FeatPowerAttackModifier),
            // but the text does not currently update to reflect the actual tuning.

            UI.Label("");
            UI.Label("Feats: ".yellow() + reqRestart);
            UI.Label("");

            using (UI.HorizontalScope())
            {
                if (UI.Toggle("Select all", ref selectAll))
                {
                    foreach (var keyValuePair in Models.FeatsContext.Feats)
                    {
                        Models.FeatsContext.Switch(keyValuePair.Key, selectAll);
                    }
                }

                intValue = Main.Settings.FeatSliderPosition;
                if (UI.Slider("[slide left for description / right to collapse]".red().bold().italic(), ref intValue, 1, MAX_COLUMNS, 1, ""))
                {
                    Main.Settings.FeatSliderPosition = intValue;
                }
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
                            toggle = Main.Settings.FeatEnabled.Contains(keyValuePair.Key);
                            var title = Gui.Format(keyValuePair.Value.GuiPresentation.Title);

                            if (flip)
                            {
                                title = title.yellow();
                            }

                            if (UI.Toggle(title, ref toggle, PIXELS_PER_COLUMN))
                            {
                                selectAll = false;
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