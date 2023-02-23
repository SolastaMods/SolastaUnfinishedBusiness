using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Models;

internal static class DeitiesContext
{
    internal static HashSet<DeityDefinition> Deities { get; private set; } = new();

    internal static void Load()
    {
        // sorting
        Deities = Deities.OrderBy(x => x.FormatTitle()).ToHashSet();

        // settings paring
        foreach (var name in Main.Settings.DeityEnabled
                     .Where(name => Deities.All(x => x.Name != name))
                     .ToList())
        {
            Main.Settings.DeityEnabled.Remove(name);
        }
    }

    [UsedImplicitly]
    internal static void LoadDeity([NotNull] DeityDefinition deityDefinition)
    {
        if (!Deities.Contains(deityDefinition))
        {
            Deities.Add(deityDefinition);
        }

        UpdateDeityVisibility(deityDefinition);
    }

    private static void UpdateDeityVisibility([NotNull] BaseDefinition deityDefinition)
    {
        deityDefinition.GuiPresentation.hidden =
            !Main.Settings.DeityEnabled.Contains(deityDefinition.Name);
    }

    internal static void Switch(DeityDefinition deityDefinition, bool active)
    {
        if (!Deities.Contains(deityDefinition))
        {
            return;
        }

        var name = deityDefinition.Name;

        if (active)
        {
            Main.Settings.DeityEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.DeityEnabled.Remove(name);
        }

        UpdateDeityVisibility(deityDefinition);
    }
}
