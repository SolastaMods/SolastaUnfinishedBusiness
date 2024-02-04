using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class TooltipFeaturePowerParametersPatcher
{
    [HarmonyPatch(typeof(TooltipFeaturePowerParameters), nameof(TooltipFeaturePowerParameters.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeaturePowerParameters __instance, ITooltip tooltip)
        {
            //PATCH: updates power uses in the tooltip to include all usage bonuses
            Tooltips.UpdatePowerUses(tooltip, __instance);
            //PATCH: updates power save DC to show actual value
            Tooltips.UpdatePowerSaveDC(tooltip, __instance);
            
            //PATCH: support for power tooltip customization
            if (tooltip.DataProvider is not GuiPowerDefinition guiPowerDefinition)
            {
                return;
            }

            foreach (var modifier in guiPowerDefinition.PowerDefinition.GetAllSubFeaturesOfType<PowerTooltipModifier>())
            {
                modifier.ModifyPowerTooltip(tooltip, __instance);
            }
        }
    }
}
