using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Diagnostic;

// prevents TA trying to sync mod content over
[HarmonyPatch(typeof(GamingPlatformManager), "IsContentPackAvailable")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GamingPlatformManager_IsContentPackAvailable
{
    public static bool Prefix(GamingPlatformDefinitions.ContentPack contentPack, ref bool __result)
    {
        if (contentPack != CeContentPackContext.CeContentPack
            && Main.Settings.EnableDiagsDump != PlayerControllerManager.DmControllerId)
        {
            return true;
        }

        __result = true;

        return false;
    }
}
