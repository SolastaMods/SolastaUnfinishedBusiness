using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ActionPerformanceFilterPatcher
{
    [HarmonyPatch(typeof(ActionPerformanceFilter), nameof(ActionPerformanceFilter.Clear))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Clear_Patch
    {
        [UsedImplicitly]
        public static void Prefix(ActionPerformanceFilter __instance)
        {
            PerformanceFilterExtraData.ClearData(__instance);
        }
    }
}
