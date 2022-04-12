using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.ClassHoldingFeature
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "FindClassHoldingFeature")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_FindClassHoldingFeature
    {
        internal static void Postfix(
            RulesetCharacterHero __instance,
            FeatureDefinition featureDefinition,
            ref CharacterClassDefinition __result)
        {
            if (featureDefinition is not IClassHoldingFeature overrideClassHoldingFeature || overrideClassHoldingFeature.Class == null)
            {
                return;
            }

            // Only override if the character actually has levels in the class, to prevent errors
            if (__instance.ClassesAndLevels.TryGetValue(overrideClassHoldingFeature.Class, out int levelsInClass) && levelsInClass > 0)
            {
                __result = overrideClassHoldingFeature.Class;
            }
        }
    }
}
