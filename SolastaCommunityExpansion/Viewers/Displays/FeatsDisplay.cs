using ModKit;
using SolastaCommunityExpansion.Models;
using System.Linq;
using static SolastaCommunityExpansion.Viewers.Displays.Shared;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class FeatsDisplay
    {
        private const int MAX_COLUMNS = 4;
        private const float PIXELS_PER_COLUMN = 225;

        internal static void DisplayFeats()
        {
            bool toggle;
            int intValue;
            bool selectAll = Main.Settings.FeatEnabled.Count == FeatsContext.Feats.Count;

            UI.Label("");
            UI.Label("General:".yellow());
            UI.Label("");

            intValue = Main.Settings.FeatPowerAttackModifier;
            if (UI.Slider("Power Attack modifier ".white() + RequiresRestart, ref intValue, 1, 6, 3, ""))
            {
                Main.Settings.FeatPowerAttackModifier = intValue;
            }

            UI.Label("");
            UI.Label("Feats: ".yellow() + RequiresRestart);
            UI.Label("");

            if (UI.Toggle("Select all", ref selectAll))
            {
                foreach (var keyValuePair in FeatsContext.Feats)
                {
                    FeatsContext.Switch(keyValuePair.Key, selectAll);
                }
            }

            intValue = Main.Settings.FeatSliderPosition;
            if (UI.Slider("slide left for description / right to collapse".white().bold().italic(), ref intValue, 1, MAX_COLUMNS, 1, ""))
            {
                Main.Settings.FeatSliderPosition = intValue;
            }

            UI.Label("");

            int columns;
            var flip = false;
            var current = 0;
            var featsCount = FeatsContext.Feats.Count;

            using (UI.VerticalScope())
            {
                while (current < featsCount)
                {
                    columns = Main.Settings.FeatSliderPosition;

                    using (UI.HorizontalScope())
                    {
                        while (current < featsCount && columns-- > 0)
                        {
                            var keyValuePair = FeatsContext.Feats.ElementAt(current);
                            var title = keyValuePair.Value.FormatTitle();                          

                            if (flip)
                            {
                                title = title.yellow();
                            }

                            toggle = Main.Settings.FeatEnabled.Contains(keyValuePair.Key);
                            if (UI.Toggle(title, ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                            {
                                FeatsContext.Switch(keyValuePair.Key, toggle);
                            }

                            if (Main.Settings.FeatSliderPosition == 1)
                            {
                                var description = keyValuePair.Value.FormatDescription();

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
