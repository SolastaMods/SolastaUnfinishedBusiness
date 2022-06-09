using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches.Insertion;

internal static class RulesetImplementationManagerPatcher
{
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "IsSituationalContextValid")]
    [HarmonyPatch(new[] {typeof(RulesetImplementationDefinitions.SituationalContextParams)})]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class IsSituationalContextValid
    {
        internal static void Postfix(RulesetImplementationManagerLocation __instance, ref bool __result,
            RulesetImplementationDefinitions.SituationalContextParams contextParams)
        {
            __result = CustomSituationalContext.IsContextValid(contextParams, __result);
        }
    }
}
