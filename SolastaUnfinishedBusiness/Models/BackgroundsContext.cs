using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Backgrounds;

namespace SolastaUnfinishedBusiness.Models;

internal static class BackgroundsContext
{
    internal static HashSet<CharacterBackgroundDefinition> Backgrounds { get; private set; } = [];

    internal static void Load()
    {
        LoadBackground(BackgroundsBuilders.BuildBackgroundDevoted());
        LoadBackground(BackgroundsBuilders.BuildBackgroundFarmer());

        // sorting
        Backgrounds = Backgrounds.OrderBy(x => x.FormatTitle()).ToHashSet();

        // settings paring
        foreach (var name in Main.Settings.BackgroundEnabled
                     .Where(name => Backgrounds.All(x => x.Name != name))
                     .ToList())
        {
            Main.Settings.BackgroundEnabled.Remove(name);
        }
    }

    [UsedImplicitly]
    internal static void LoadBackground([NotNull] CharacterBackgroundDefinition characterBackgroundDefinition)
    {
        Backgrounds.Add(characterBackgroundDefinition);
        UpdateBackgroundVisibility(characterBackgroundDefinition);
    }

    private static void UpdateBackgroundVisibility([NotNull] BaseDefinition characterBackgroundDefinition)
    {
        characterBackgroundDefinition.GuiPresentation.hidden =
            !Main.Settings.BackgroundEnabled.Contains(characterBackgroundDefinition.Name);
    }

    internal static void Switch(CharacterBackgroundDefinition characterBackgroundDefinition, bool active)
    {
        if (!Backgrounds.Contains(characterBackgroundDefinition))
        {
            return;
        }

        var name = characterBackgroundDefinition.Name;

        if (active)
        {
            Main.Settings.BackgroundEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.BackgroundEnabled.Remove(name);
        }

        UpdateBackgroundVisibility(characterBackgroundDefinition);
    }
}
