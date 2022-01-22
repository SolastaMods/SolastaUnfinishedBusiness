using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.GameUi.Tooltip
{
    // always alt
    [HarmonyPatch(typeof(TooltipPanel), "SetupFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class TooltipPanel_SetupFeatures
    {
        internal static void Prefix(ref TooltipDefinitions.Scope scope)
        {
            if (Main.Settings.InvertAltBehaviorOnTooltips)
            {
                if (scope == TooltipDefinitions.Scope.Simplified)
                {
                    scope = TooltipDefinitions.Scope.Detailed;
                }
                else if (scope == TooltipDefinitions.Scope.Detailed)
                {
                    scope = TooltipDefinitions.Scope.Simplified;
                }
            }
        }
    }
}
