using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class TooltipPanelPatcher
{
    [HarmonyPatch(typeof(TooltipPanel), "SetupFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SetupFeatures_Patch
    {
        [UsedImplicitly]
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
