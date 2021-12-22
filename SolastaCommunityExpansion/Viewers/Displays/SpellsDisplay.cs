using ModKit;
using SolastaCommunityExpansion.Models;
using System.Linq;
using static SolastaCommunityExpansion.Viewers.Displays.Shared;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class SpellsDisplay
    {
        private const int MAX_COLUMNS = 4;
        private const float PIXELS_PER_COLUMN = 225;

        internal static void DisplaySpells()
        {
            bool toggle;
            int intValue;
            bool selectAll = Main.Settings.SpellEnabled.Count == SpellsContext.Spells.Count;

            UI.Label("");
            UI.Label("General:".yellow());
            UI.Label("");

            UI.Label("");
            UI.Label("Spells: ".yellow());
            UI.Label("");

            if (UI.Toggle("Select all " + RequiresRestart, ref selectAll))
            {
                foreach (var keyValuePair in SpellsContext.Spells)
                {
                    SpellsContext.Switch(keyValuePair.Key, selectAll);
                }
            }

            intValue = Main.Settings.SpellSliderPosition;
            if (UI.Slider("slide left for description / right to collapse".white().bold().italic(), ref intValue, 1, MAX_COLUMNS, 1, ""))
            {
                Main.Settings.SpellSliderPosition = intValue;
            }

            UI.Label("");

            int columns;
            var flip = false;
            var current = 0;
            var spellsCount = SpellsContext.Spells.Count;

            using (UI.VerticalScope())
            {
                while (current < spellsCount)
                {
                    columns = Main.Settings.SpellSliderPosition;

                    using (UI.HorizontalScope())
                    {
                        while (current < spellsCount && columns-- > 0)
                        {
                            var keyValuePair = SpellsContext.Spells.ElementAt(current);
                            var title = keyValuePair.Value.FormatTitle();                          

                            if (flip)
                            {
                                title = title.yellow();
                            }

                            toggle = Main.Settings.SpellEnabled.Contains(keyValuePair.Key);
                            if (UI.Toggle(title, ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                            {
                                SpellsContext.Switch(keyValuePair.Key, toggle);
                            }

                            if (Main.Settings.SpellSliderPosition == 1)
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
