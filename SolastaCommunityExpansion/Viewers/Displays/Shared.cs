using System;
using System.Collections.Generic;
using System.Linq;
using ModKit;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class Shared
    {
        internal const int MAX_COLUMNS = 4;

        internal const float PIXELS_PER_COLUMN = 240;

        internal static readonly string RequiresRestart = "[requires restart]".italic().red().bold();

        internal static void DisplayDefinitions<T>(
            string label,
            Action<T, bool> switchAction,
            HashSet<T> registeredDefinitions,
            List<string> selectedDefinitions,
            ref bool displayToggle,
            ref int sliderPosition,
            Action additionalRendering = null,
            int maxColumns = MAX_COLUMNS,
            float pixelsPerColumn = PIXELS_PER_COLUMN) where T: BaseDefinition
        {
            bool toggle;
            bool selectAll = selectedDefinitions.Count == registeredDefinitions.Count;

            UI.Label("");

            toggle = displayToggle;
            if (UI.DisclosureToggle(label, ref toggle, 200))
            {
                displayToggle = toggle;
            }

            if (displayToggle)
            {
                if (registeredDefinitions.Count == 0)
                {
                    UI.Label("");
                    UI.Label("No unofficial definitions available on this mod yet...".bold().red());

                    return;
                }

                UI.Label("");

                using (UI.HorizontalScope())
                {
                    if (UI.Toggle("Select all", ref selectAll, UI.Width(PIXELS_PER_COLUMN)))
                    {
                        foreach (var registeredDefinition in registeredDefinitions)
                        {
                            switchAction.Invoke(registeredDefinition, selectAll);
                        }
                    }

                    additionalRendering?.Invoke();
                }

                UI.Slider("slide left for description / right to collapse".white().bold().italic(), ref sliderPosition, 1, maxColumns, 1, "");

                UI.Label("");

                int columns;
                var flip = false;
                var current = 0;
                var count = registeredDefinitions.Count;

                using (UI.VerticalScope())
                {
                    while (current < count)
                    {
                        columns = sliderPosition;

                        using (UI.HorizontalScope())
                        {
                            while (current < count && columns-- > 0)
                            {
                                var definition = registeredDefinitions.ElementAt(current);
                                var title = definition.FormatTitle();

                                if (flip)
                                {
                                    title = title.yellow();
                                }

                                toggle = selectedDefinitions.Contains(definition.Name);
                                if (UI.Toggle(title, ref toggle, UI.Width(pixelsPerColumn)))
                                {
                                    switchAction.Invoke(definition, toggle);
                                }

                                if (sliderPosition == 1)
                                {
                                    var description = definition.FormatDescription();

                                    if (flip)
                                    {
                                        description = description.yellow();
                                    }

                                    UI.Label(description, UI.Width(pixelsPerColumn * 3));

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
}
