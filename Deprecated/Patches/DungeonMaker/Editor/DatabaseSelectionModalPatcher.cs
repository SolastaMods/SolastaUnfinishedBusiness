using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.Editor;

[HarmonyPatch(typeof(DatabaseSelectionModal), "BuildMonsters")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class DatabaseSelectionModal_BuildMonsters
{
    internal static bool Prefix(DatabaseSelectionModal __instance)
    {
        if (!Main.Settings.UnleashNpcAsEnemy)
        {
            return true;
        }

        __instance.allMonsters.Clear();

        __instance.allMonsters.AddRange(DatabaseRepository.GetDatabase<MonsterDefinition>()
            .Where(x => !x.GuiPresentation.Hidden)
            .OrderBy(d => Gui.Localize(d.GuiPresentation.Title)));

        var service = ServiceRepository.GetService<IGamingPlatformService>();

        for (var index = __instance.allMonsters.Count - 1; index >= 0; --index)
        {
            if (!service.IsContentPackAvailable(__instance.allMonsters[index].ContentPack))
            {
                __instance.allMonsters.RemoveAt(index);
            }
        }

        return false;
    }
}

// this patch unleashes all monster definitions to be used as NPCs
[HarmonyPatch(typeof(DatabaseSelectionModal), "BuildNpcs")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class DatabaseSelectionModal_BuildNpcs
{
    internal static bool Prefix(DatabaseSelectionModal __instance)
    {
        if (!Main.Settings.UnleashEnemyAsNpc)
        {
            return true;
        }

        __instance.allNpcs.Clear();

        __instance.allNpcs.AddRange(DatabaseRepository.GetDatabase<MonsterDefinition>()
            .Where(x => !x.GuiPresentation.Hidden)
            .OrderBy(d => Gui.Localize(d.GuiPresentation.Title)));

        var service = ServiceRepository.GetService<IGamingPlatformService>();

        for (var index = __instance.allNpcs.Count - 1; index >= 0; --index)
        {
            if (!service.IsContentPackAvailable(__instance.allNpcs[index].ContentPack))
            {
                __instance.allNpcs.RemoveAt(index);
            }
        }

        return false;
    }
}
