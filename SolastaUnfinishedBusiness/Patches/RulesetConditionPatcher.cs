using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomBehaviors;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetConditionPatcher
{
    [HarmonyPatch(typeof(RulesetCondition), nameof(RulesetCondition.CreateCondition))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CreateCondition_Patch
    {
        [UsedImplicitly]
        public static void Postfix([NotNull] RulesetCondition __result)
        {
            //PATCH: support for adding 'markers' to ruleset conditions when they are created
            AddMarkerToCondition.AddMarkers(__result);
        }
    }

    [HarmonyPatch(typeof(RulesetCondition), nameof(RulesetCondition.CreateActiveCondition))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CreateActiveCondition_Patch
    {
        [UsedImplicitly]
        public static void Postfix([NotNull] RulesetCondition __result)
        {
            //PATCH: support for adding 'markers' to ruleset conditions when they are created
            AddMarkerToCondition.AddMarkers(__result);
        }
    }
}
