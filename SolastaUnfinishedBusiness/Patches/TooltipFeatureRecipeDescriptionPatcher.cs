using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

public static class TooltipFeatureDescriptionPatcher
{
    [HarmonyPatch(typeof(TooltipFeatureDescription), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static void Postfix(TooltipFeatureDescription __instance, ITooltip tooltip)
        {
            //PATCH: adds crafting details to recipe tooltips
            Tooltips.UpdateCraftingTooltip(__instance, tooltip);
        }
    }
}
