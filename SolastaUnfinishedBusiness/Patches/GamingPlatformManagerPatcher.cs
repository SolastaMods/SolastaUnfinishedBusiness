using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GamingPlatformManagerPatcher
{
    [HarmonyPatch(typeof(GamingPlatformManager), "IsContentPackAvailable")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsContentPackAvailable_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GamingPlatformDefinitions.ContentPack contentPack, ref bool __result)
        {
            //PATCH: prevents TA trying to sync mod content over
            if (contentPack != CeContentPackContext.CeContentPack
                && Main.Settings.EnableDiagsDump != PlayerControllerManager.DmControllerId)
            {
                return true;
            }

            __result = true;

            return false;
        }
    }
}
