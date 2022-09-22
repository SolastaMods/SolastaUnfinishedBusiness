using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

internal static class TooltipFeatureDescriptionPatcher
{
    [HarmonyPatch(typeof(TooltipFeatureDescription), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static void Postfix(TooltipFeatureDescription __instance, ITooltip tooltip)
        {
            //PATCH: adds crafting details to recipe tooltips
            Tooltips.UpdateCraftingTooltip(__instance, tooltip);
        }
    }
}
