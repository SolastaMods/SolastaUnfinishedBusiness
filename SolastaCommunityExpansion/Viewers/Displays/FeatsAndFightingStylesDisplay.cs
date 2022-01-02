using System.Linq;
using ModKit;
using SolastaCommunityExpansion.Feats;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Viewers.Displays.Shared;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class FeatsAndFightingStylesDisplay
    {
        private const int MAX_COLUMNS = 4;

        private const float PIXELS_PER_COLUMN = 225;

        private static void DisplayFeats()
        {
            bool toggle;
            int intValue;
            bool selectAll = Main.Settings.FeatEnabled.Count == FeatsContext.Feats.Count;

            UI.Label("");

            toggle = Main.Settings.DisplayFeatsToggle;
            if (UI.DisclosureToggle("Feats:".yellow(), ref toggle, 200))
            {
                Main.Settings.DisplayFeatsToggle = toggle;
            }

            if (Main.Settings.DisplayFeatsToggle)
            {
                UI.Label("");

                intValue = Main.Settings.FeatPowerAttackModifier;
                if (UI.Slider("Power Attack".orange() + " modifier ".white() + RequiresRestart, ref intValue, 1, 6, 3, ""))
                {
                    Main.Settings.FeatPowerAttackModifier = intValue;
                    AcehighFeats.UpdatePowerAttackModifier();
                }

                UI.Label("");

                if (UI.Toggle("Select all " + RequiresRestart, ref selectAll))
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

        private static void DisplayFightingStyles()
        {
            bool toggle;
            int intValue;
            bool selectAll = Main.Settings.FightingStyleEnabled.Count == FightingStyleContext.Styles.Count;

            UI.Label("");

            toggle = Main.Settings.DisplayFightingStylesToggle;
            if (UI.DisclosureToggle("Fighting Styles:".yellow(), ref toggle, 200))
            {
                Main.Settings.DisplayFightingStylesToggle = toggle;
            }

            if (Main.Settings.DisplayFightingStylesToggle)
            {
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

        internal static void DisplayFeatsAndFightingStyles()
        {
            DisplayFeats();
            DisplayFightingStyles();
            UI.Label("");
        }
    }
}
