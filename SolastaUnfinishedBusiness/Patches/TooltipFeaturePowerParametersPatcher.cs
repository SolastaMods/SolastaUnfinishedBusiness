using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

public static class TooltipFeaturePowerParametersPatcher
{
    [HarmonyPatch(typeof(TooltipFeaturePowerParameters), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static void Postfix(TooltipFeaturePowerParameters __instance, ITooltip tooltip)
        {
            //PATCH: updates power uses in the tooltip to include all usage bonuses
            Tooltips.UpdatePowerUses(tooltip, __instance);
        }
    }
}
