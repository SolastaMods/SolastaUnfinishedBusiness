using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

public static class TooltipPanelPatcher
{
    [HarmonyPatch(typeof(TooltipPanel), "SetupFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class SetupFeatures_Patch
    {
        public static void Prefix(ref TooltipDefinitions.Scope scope)
        {
            //PATCH: swaps holding ALT behavior for tooltips
            if (!Main.Settings.InvertAltBehaviorOnTooltips)
            {
                return;
            }

            scope = scope switch
            {
                TooltipDefinitions.Scope.Simplified => TooltipDefinitions.Scope.Detailed,
                TooltipDefinitions.Scope.Detailed => TooltipDefinitions.Scope.Simplified,
                _ => scope
            };
        }
    }
}
