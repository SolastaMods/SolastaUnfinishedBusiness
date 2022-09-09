using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches;

internal static class TooltipPanelPatcher
{
    //PATCH: swaps holding ALT behavior for tooltips
    [HarmonyPatch(typeof(TooltipPanel), "SetupFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SetupFeatures_Patch
    {
        internal static void Prefix(ref TooltipDefinitions.Scope scope)
        {
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
