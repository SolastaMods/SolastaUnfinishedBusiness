using ModKit;
using SolastaCommunityExpansion.Models;
using System.Linq;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class FightingStylesDisplay
    {
        private const int MAX_COLUMNS = 4;
        private const float PIXELS_PER_COLUMN = 225;

        internal static void DisplayFightingStyles()
        {
            bool toggle;
            int intValue;
            bool selectAll = Main.Settings.FightingStyleEnabled.Count == FightingStyleContext.Styles.Count;

            UI.Label("");
            UI.Label("Fighting Styles: ".yellow());
            UI.Label("");

            if (UI.Toggle("Select all", ref selectAll))
            {
                foreach (var keyValuePair in FightingStyleContext.Styles)
                {
                    FightingStyleContext.Switch(keyValuePair.Key, selectAll);
                }
            }

            intValue = Main.Settings.FightingStyleSliderPosition;
            if (UI.Slider("slide left for description / right to collapse".white().bold().italic(), ref intValue, 1, MAX_COLUMNS, 1, ""))
            {
                Main.Settings.FightingStyleSliderPosition = intValue;
            }

            UI.Label("");

            int columns;
            var flip = false;
            var current = 0;
            var stylesCount = FightingStyleContext.Styles.Count;

            using (UI.VerticalScope())
            {
                while (current < stylesCount)
                {
                    columns = Main.Settings.FightingStyleSliderPosition;

                    using (UI.HorizontalScope())
                    {
                        while (current < stylesCount && columns-- > 0)
                        {
                            var keyValuePair = FightingStyleContext.Styles.ElementAt(current);
                            var title = keyValuePair.Value.GetStyle().FormatTitle();

                            if (flip)
                            {
                                title = title.yellow();
                            }

                            toggle = Main.Settings.FightingStyleEnabled.Contains(keyValuePair.Key);
                            if (UI.Toggle(title, ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                            {
                                FightingStyleContext.Switch(keyValuePair.Key, toggle);
                            }

                            if (Main.Settings.FightingStyleSliderPosition == 1)
                            {
                                var description = keyValuePair.Value.GetStyle().FormatDescription();

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
