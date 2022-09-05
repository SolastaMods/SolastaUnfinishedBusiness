using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches;

internal static class RulesetImplementationManagerPatcher
{
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "IsSituationalContextValid")]
    [HarmonyPatch(new[] { typeof(RulesetImplementationDefinitions.SituationalContextParams) })]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class IsSituationalContextValid_Patch
    {
        internal static void Postfix(
            ref bool __result,
            RulesetImplementationDefinitions.SituationalContextParams contextParams)
        {
            //PATCH: supports custom situational context
            //used to twean `Reckless` feat to properly work with reach weapons
            //and for Blade Dancer subclass features
            __result = CustomSituationalContext.IsContextValid(contextParams, __result);
        }
    }
}
