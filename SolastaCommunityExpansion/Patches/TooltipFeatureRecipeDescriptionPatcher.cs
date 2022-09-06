using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.PatchCode.CustomUI;

namespace SolastaCommunityExpansion.Patches;

[HarmonyPatch(typeof(TooltipFeatureDescription), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class TooltipFeatureDescription_Bind
{
    internal static void Postfix(TooltipFeatureDescription __instance, ITooltip tooltip)
    {
        //PATCH: adds crafting details to recipe tooltips
        Tooltips.UpdateCraftingTooltip(__instance, tooltip);
    }
}
