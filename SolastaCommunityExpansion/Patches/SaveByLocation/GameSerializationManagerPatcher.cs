using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.SaveByLocation
{
    [HarmonyPatch(typeof(GameSerializationManager), "CanLoad", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameSerializationManager_CanLoad
    {
        public static bool Prefix(ref bool __result, bool ___saving, bool ___loading, HashSet<EPermissionToken> ___loadDisabledTokens)
        {
            if (!Main.Settings.EnableSaveByLocation)
            {
                return true;
            }

            __result = !___saving && !___loading && ___loadDisabledTokens.Count <= 0;

            return false;
        }
    }
}
