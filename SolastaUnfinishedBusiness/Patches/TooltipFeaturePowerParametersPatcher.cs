using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

internal static class TooltipFeaturePowerParametersPatcher
{
    //PATCH: updates power uses in the tooltip to include all usage bonuses
    [HarmonyPatch(typeof(TooltipFeaturePowerParameters), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static void Postfix(TooltipFeaturePowerParameters __instance, ITooltip tooltip)
        {
            Tooltips.UpdatePowerUses(tooltip, __instance);
        }
    }
}
