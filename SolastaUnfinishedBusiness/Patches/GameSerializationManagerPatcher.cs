using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GameSerializationManagerPatcher
{
    [HarmonyPatch(typeof(GameSerializationManager), "CanLoad", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CanLoad_Getter_Patch
    {
        public static bool Prefix(
            GameSerializationManager __instance,
            ref bool __result)
        {
            //PATCH: EnableSaveByLocation
            if (!Main.Settings.EnableSaveByLocation)
            {
                return true;
            }

            // Enable/disable the 'load' button in the load save panel
            __result = !__instance.saving && !__instance.loading && __instance.loadDisabledTokens.Count == 0;

            return false;
        }
    }
}
