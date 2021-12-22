using ModKit;
using SolastaCommunityExpansion.Models;
using System.Linq;
using static SolastaCommunityExpansion.Viewers.Displays.Shared;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class PowersDisplay
    {
        private const int MAX_COLUMNS = 4;
        private const float PIXELS_PER_COLUMN = 225;

        internal static void DisplayPowers()
        {
            bool toggle;
            int intValue;
            bool selectAll = Main.Settings.PowerEnabled.Count == PowersContext.Powers.Count;

            UI.Label("");
            UI.Label("General:".yellow());
            UI.Label("");

            UI.Label("");
            UI.Label("Powers: ".yellow());
            UI.Label("");

            if (UI.Toggle("Select all " + RequiresRestart, ref selectAll))
            {
                foreach (var keyValuePair in PowersContext.Powers)
                {
                    PowersContext.Switch(keyValuePair.Key, selectAll);
                }
            }

            intValue = Main.Settings.PowerSliderPosition;
            if (UI.Slider("slide left for description / right to collapse".white().bold().italic(), ref intValue, 1, MAX_COLUMNS, 1, ""))
            {
                Main.Settings.PowerSliderPosition = intValue;
            }

            UI.Label("");

            int columns;
            var flip = false;
            var current = 0;
            var powersCount = PowersContext.Powers.Count;

            using (UI.VerticalScope())
            {
                while (current < powersCount)
                {
                    columns = Main.Settings.PowerSliderPosition;

                    using (UI.HorizontalScope())
                    {
                        while (current < powersCount && columns-- > 0)
                        {
                            var keyValuePair = PowersContext.Powers.ElementAt(current);
                            var title = keyValuePair.Value.FormatTitle();                          

                            if (flip)
                            {
                                title = title.yellow();
                            }

                            toggle = Main.Settings.PowerEnabled.Contains(keyValuePair.Key);
                            if (UI.Toggle(title, ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                            {
                                PowersContext.Switch(keyValuePair.Key, toggle);
                            }

                            if (Main.Settings.PowerSliderPosition == 1)
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

            UI.Label("");
        }
    }
}
