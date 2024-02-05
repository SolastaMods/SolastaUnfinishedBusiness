using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public class FeatureElementEffectLinePatcher
{
    //PATCH: clear tooltip on bind, so it does not show previous value if new effect has no trends
    [HarmonyPatch(typeof(FeatureElementEffectLine), nameof(FeatureElementEffectLine.Bind))]
    [HarmonyPatch([
            typeof(EffectForm), typeof(bool), typeof(int), typeof(bool), typeof(bool),
            typeof(Gui.VersatilityDisplay), typeof(int), typeof(string), typeof(RuleDefinitions.EffectApplication)
        ],
        [
            ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal,
            ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal
        ])]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] FeatureElementEffectLine __instance)
        {
            var tooltip = __instance.tooltip;
            if (tooltip != null)
            {
                tooltip.Content = String.Empty;
            }
        }
    }
}
