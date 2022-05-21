using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomInterfaces;

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
            var classHolder = featureDefinition.GetFirstSubFeatureOfType<IClassHoldingFeature>();

            if (classHolder == null || classHolder.Class == null)
            {
                return;
            }

            // Only override if the character actually has levels in the class, to prevent errors
            if (__instance.ClassesAndLevels.TryGetValue(classHolder.Class, out var levelsInClass) && levelsInClass > 0)
            {
                __result = classHolder.Class;
            }
        }
    }
}
