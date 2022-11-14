using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class BaseBlueprintPatcher
{
    [HarmonyPatch(typeof(BaseBlueprint), "GetAssetKey")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class GetAssetKey_Patch
    {
        public static bool Prefix(
            BaseBlueprint __instance,
            ref string __result,
            BaseBlueprint.PrefabByEnvironmentDescription prefabByEnvironmentDescription,
            EnvironmentDefinition environmentDefinition,
            bool perspective)
        {
            //PATCH: ensures custom props display the proper icon (DMP)
            return DmProRendererContext.ExtendedGetAssetKey(
                __instance, ref __result, prefabByEnvironmentDescription, environmentDefinition, perspective);
        }
    }
}
