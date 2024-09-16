using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameSerializationManagerPatcher
{
    [HarmonyPatch(typeof(GameSerializationManager), nameof(GameSerializationManager.Refresh))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameSerializationManager __instance)
        {
            //PATCH: update state of load buttons for SaveByLocation
            if (!Main.Settings.EnableSaveByLocation) { return; }

            __instance.hasSavedGames = SaveByLocationContext.GetMostRecentPlace().Count > 0;
        }
    }

    [HarmonyPatch(typeof(GameSerializationManager), nameof(GameSerializationManager.RefreshAsync))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshAsync_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameSerializationManager __instance)
        {
            //PATCH: update state of load buttons for SaveByLocation
            if (!Main.Settings.EnableSaveByLocation) { return; }

            __instance.hasSavedGames = SaveByLocationContext.GetMostRecentPlace().Count > 0;
        }
    }
}
