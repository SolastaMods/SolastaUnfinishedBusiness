using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Classes;

namespace SolastaUnfinishedBusiness.Models;

internal static class ClassesContext
{
    internal static HashSet<CharacterClassDefinition> Classes { get; private set; } = new();

    internal static void Load()
    {
        LoadClass(InventorClass.Build());

        // sorting
        Classes = Classes.OrderBy(x => x.FormatTitle()).ToHashSet();

        // settings paring
        foreach (var name in Main.Settings.ClassEnabled
                     .Where(name => Classes.All(x => x.Name != name))
                     .ToList())
        {
            Main.Settings.ClassEnabled.Remove(name);
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            DatabaseRepository.GetDatabase<CharacterClassDefinition>()
                .Do(x => x.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock));
        }
    }

    private static void LoadClass([NotNull] CharacterClassDefinition characterClassDefinition)
    {
        Classes.Add(characterClassDefinition);
        UpdateClassVisibility(characterClassDefinition);
    }

    private static void UpdateClassVisibility([NotNull] BaseDefinition characterClassDefinition)
    {
        characterClassDefinition.GuiPresentation.hidden =
            !Main.Settings.ClassEnabled.Contains(characterClassDefinition.Name);
    }

    internal static void Switch(CharacterClassDefinition characterClassDefinition, bool active)
    {
        if (!Classes.Contains(characterClassDefinition))
        {
            return;
        }

        var name = characterClassDefinition.Name;

        if (active)
        {
            Main.Settings.ClassEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.ClassEnabled.Remove(name);
        }

        UpdateClassVisibility(characterClassDefinition);
    }
}
