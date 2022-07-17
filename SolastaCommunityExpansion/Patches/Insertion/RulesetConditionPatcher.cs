using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Classes.Magus;

namespace SolastaCommunityExpansion.Patches.Insertion;

internal static class RulesetConditionPatcher
{
    [HarmonyPatch(typeof(RulesetCondition), "EffectDefinitionName", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCondition_EffectDefinitionName_Getter
    {
        internal static void Postfix([NotNull] RulesetCondition __instance, [CanBeNull] ref string __result)
        {
            if (__instance.conditionDefinition == Magus.AegisCondition)
            {
                __result = null;
            }
        }
    }
}
