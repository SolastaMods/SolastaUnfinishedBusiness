using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Classes.Inventor;

namespace SolastaUnfinishedBusiness.Models;

internal static class ClassesContext
{
    internal static HashSet<CharacterClassDefinition> Classes { get; private set; } = new();

    private static void SortClassesFeatures()
    {
        foreach (var characterClassDefinition in Classes)
        {
            characterClassDefinition.FeatureUnlocks.Sort((a, b) =>
            {
                var result = a.Level - b.Level;

                if (result == 0)
                {
                    result = String.Compare(a.FeatureDefinition.FormatTitle(), b.FeatureDefinition.FormatTitle(),
                        StringComparison.CurrentCulture);
                }

                return result;
            });
        }
    }

    internal static void Load()
    {
        LoadClass(InventorClass.Build());

        // sorting
        Classes = Classes.OrderBy(x => x.FormatTitle()).ToHashSet();

        // settings paring
        foreach (var name in Main.Settings.ClassEnabled
                     .Where(name => Classes.All(x => x.Name != name)))
        {
            Main.Settings.ClassEnabled.Remove(name);
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            SortClassesFeatures();
        }
    }

    private static void LoadClass([NotNull] CharacterClassDefinition characterClassDefinition)
    {
        if (!Classes.Contains(characterClassDefinition))
        {
            Classes.Add(characterClassDefinition);
        }

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
