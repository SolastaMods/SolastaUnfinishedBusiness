using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ModKit;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Api.ModKit;

namespace SolastaCommunityExpansion.Displays;

internal static class Shared
{
    internal const float PixelsPerColumn = 220;

    internal static void DisplaySubMenu(ref int selectedPane, params NamedAction[] actions)
    {
        UI.Label("Welcome / 欢迎".Bold()
            .Khaki());
        UI.Div();

        if (Main.Enabled)
        {
            UI.TabBar(ref selectedPane, null, actions);
        }
    }

    internal static void DisplayDefinitions<T>(
        string label,
        Action<T, bool> switchAction,
        [NotNull] HashSet<T> registeredDefinitions,
        [NotNull] List<string> selectedDefinitions,
        ref bool displayToggle,
        ref int sliderPosition,
        [CanBeNull] Action additionalRendering = null) where T : BaseDefinition
    {
        var selectAll = selectedDefinitions.Count == registeredDefinitions.Count;

        UI.Label("");

        var toggle = displayToggle;
        if (UI.DisclosureToggle($"{label}:", ref toggle, 200))
        {
            displayToggle = toggle;
        }

        if (!displayToggle)
        {
            return;
        }

        if (registeredDefinitions.Count == 0)
        {
            UI.Label("");
            UI.Label(Gui.Format("ModUi/&NotAvailable", label));

            return;
        }

        UI.Label("");

        using (UI.HorizontalScope())
        {
            if (additionalRendering != null)
            {
                additionalRendering.Invoke();
            }
            else if (UI.Toggle(Gui.Localize("ModUi/&SelectAll"), ref selectAll, UI.Width(PixelsPerColumn)))
            {
                foreach (var registeredDefinition in registeredDefinitions)
                {
                    switchAction.Invoke(registeredDefinition, selectAll);
                }
            }

            toggle = sliderPosition == 1;
            if (UI.Toggle(Gui.Localize("ModUi/&ShowDescriptions"), ref toggle, UI.Width(PixelsPerColumn)))
            {
                sliderPosition = toggle ? 1 : 4;
            }
        }

        //UI.Slider("slide left for description / right to collapse".white().bold().italic(), ref sliderPosition, 1, maxColumns, 1, "");

        UI.Div();
        UI.Label("");

        var flip = false;
        var current = 0;
        var count = registeredDefinitions.Count;

        using (UI.VerticalScope())
        {
            while (current < count)
            {
                var columns = sliderPosition;

                using (UI.HorizontalScope())
                {
                    while (current < count && columns-- > 0)
                    {
                        var definition = registeredDefinitions.ElementAt(current);
                        var title = definition.FormatTitle();

                        if (flip)
                        {
                            title = title.Khaki();
                        }

                        toggle = selectedDefinitions.Contains(definition.Name);
                        if (UI.Toggle(title, ref toggle, UI.Width(PixelsPerColumn)))
                        {
                            switchAction.Invoke(definition, toggle);
                        }

                        if (sliderPosition == 1)
                        {
                            var description = definition.FormatDescription();

                            if (flip)
                            {
                                description = description.Khaki();
                            }

                            UI.Label(description, UI.Width(PixelsPerColumn * 3));

                            flip = !flip;
                        }

                        current++;
                    }
                }
            }
        }
    }
}
