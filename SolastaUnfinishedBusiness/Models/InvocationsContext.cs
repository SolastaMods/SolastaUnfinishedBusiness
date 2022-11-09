using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Invocations.InvocationsBuilders;

namespace SolastaUnfinishedBusiness.Models;

internal static class InvocationsContext
{
    internal static HashSet<InvocationDefinition> Invocations { get; private set; } = new();

    internal static void Load()
    {
        LoadInvocation(BuildEldritchSmite());

        // sorting
        Invocations = Invocations.OrderBy(x => x.FormatTitle()).ToHashSet();

        // settings paring
        foreach (var name in Main.Settings.InvocationEnabled
                     .Where(name => Invocations.All(x => x.Name != name))
                     .ToList())
        {
            Main.Settings.InvocationEnabled.Remove(name);
        }
    }

    private static void LoadInvocation([NotNull] InvocationDefinition invocationDefinition)
    {
        if (!Invocations.Contains(invocationDefinition))
        {
            Invocations.Add(invocationDefinition);
        }

        UpdateInvocationVisibility(invocationDefinition);
    }

    private static void UpdateInvocationVisibility([NotNull] BaseDefinition invocationDefinition)
    {
        invocationDefinition.GuiPresentation.hidden =
            !Main.Settings.InvocationEnabled.Contains(invocationDefinition.Name);
    }

    internal static void Switch(InvocationDefinition invocationDefinition, bool active)
    {
        if (!Invocations.Contains(invocationDefinition))
        {
            return;
        }

        var name = invocationDefinition.Name;

        if (active)
        {
            Main.Settings.InvocationEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.InvocationEnabled.Remove(name);
        }

        UpdateInvocationVisibility(invocationDefinition);
    }
}
