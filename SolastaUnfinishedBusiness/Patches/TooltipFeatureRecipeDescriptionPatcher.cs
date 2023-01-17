using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class TooltipFeatureDescriptionPatcher
{
    [HarmonyPatch(typeof(TooltipFeatureDescription), nameof(TooltipFeatureDescription.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureDescription __instance, ITooltip tooltip)
        {
            //PATCH: adds crafting details to recipe tooltips
            Tooltips.UpdateCraftingTooltip(__instance, tooltip);
        }
    }
}
