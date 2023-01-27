using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class DatabaseSelectionModalPatcher
{
    [HarmonyPatch(typeof(DatabaseSelectionModal), nameof(DatabaseSelectionModal.BuildMonsters))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BuildMonsters_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(DatabaseSelectionModal __instance)
        {
            //PATCH: unleashes all NPC definitions to be used as monsters (DMP)
            if (!Main.Settings.UnleashNpcAsEnemy)
            {
                return true;
            }

            __instance.allMonsters
                .SetRange(DatabaseRepository
                    .GetDatabase<MonsterDefinition>()
                    .Where(x => !x.GuiPresentation.Hidden && x.IsContentAvailable)
                    .OrderBy(d => d.dungeonMakerPresence + Gui.Localize(d.GuiPresentation.Title)));

            var service = ServiceRepository.GetService<IGamingPlatformService>();

            __instance.allMonsters.RemoveAll(x => !service.IsContentPackAvailable(x.ContentPack));

            return false;
        }
    }

    [HarmonyPatch(typeof(DatabaseSelectionModal), nameof(DatabaseSelectionModal.BuildNpcs))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BuildNpcs_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(DatabaseSelectionModal __instance)
        {
            //PATCH: unleashes all monster definitions to be used as NPCs (DMP)
            if (!Main.Settings.UnleashEnemyAsNpc)
            {
                return true;
            }

            __instance.allNpcs
                .SetRange(DatabaseRepository
                    .GetDatabase<MonsterDefinition>()
                    .Where(x => !x.GuiPresentation.Hidden && x.IsContentAvailable)
                    .OrderBy(d => d.dungeonMakerPresence + Gui.Localize(d.GuiPresentation.Title)));

            var service = ServiceRepository.GetService<IGamingPlatformService>();

            __instance.allMonsters.RemoveAll(x => !service.IsContentPackAvailable(x.ContentPack));

            return false;
        }
    }
}
