using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Insertion;

internal static class RulesetConditionPatcher
{
    [HarmonyPatch(typeof(RulesetCondition), "EffectDefinitionName", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCondition_EffectDefinitionName
    {
        internal static void Postfix(RulesetCondition __instance, ref string __result)
        {
            if (__instance.conditionDefinition == Classes.Magus.Magus.AegisCondition)
            {
                __result = null;
            }
        }
    }
}
