using System.Linq;
using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class FightingStyles
    {
        private static bool selectAll = false;
        private const int MAX_COLUMNS = 4;
        private const float PIXELS_PER_COLUMN = 225;
        private static readonly string reqRestart = "[requires restart]".italic().red().bold();

        internal static void DisplayFightingStyles()
        {
            bool toggle;
            int intValue;

            selectAll = Main.Settings.FightingStyleEnabled.Count == FightingStyleContext.Styles.Count;

         

            UI.Label("");
            UI.Label("Fighting Styles: ".yellow());
            UI.Label("");

            using (UI.HorizontalScope())
            {
                if (UI.Toggle("Select all", ref selectAll))
                {
                    foreach (var keyValuePair in FightingStyleContext.Styles)
                    {
                        FightingStyleContext.Switch(keyValuePair.Key, selectAll);
                    }
                }

                intValue = Main.Settings.FightingStyleSliderPosition;
                if (UI.Slider("[slide left for description / right to collapse]".red().bold().italic(), ref intValue, 1, MAX_COLUMNS, 1, ""))
                {
                    Main.Settings.FightingStyleSliderPosition = intValue;
                }
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
                            toggle = Main.Settings.FightingStyleEnabled.Contains(keyValuePair.Key);
                            var title = Gui.Format(keyValuePair.Value.GetStyle().GuiPresentation.Title);

                            if (flip)
                            {
                                title = title.yellow();
                            }

                            if (UI.Toggle(title, ref toggle, PIXELS_PER_COLUMN))
                            {
                                selectAll = false;
                                FightingStyleContext.Switch(keyValuePair.Key, toggle);
                            }

                            if (Main.Settings.FightingStyleSliderPosition == 1)
                            {
                                var description = Gui.Format(keyValuePair.Value.GetStyle().GuiPresentation.Description);

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