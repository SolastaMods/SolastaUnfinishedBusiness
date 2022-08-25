using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.Patches;

internal static class RulesetCharacterHeroPatcher
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "FindClassHoldingFeature")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FindClassHoldingFeature_Patch
    {
        internal static void Postfix(
            RulesetCharacterHero __instance,
            FeatureDefinition featureDefinition,
            ref CharacterClassDefinition __result)
        {
            //PATCH: replaces feature holding class with one provided by custom interface
            //used for features that are not granted directly through class but need to scale with class levels
            var classHolder = featureDefinition.GetFirstSubFeatureOfType<IClassHoldingFeature>()?.Class;
            if (classHolder == null)
            {
                return;
            }

            // Only override if the character actually has levels in the class, to prevent errors
            if (__instance.ClassesAndLevels.TryGetValue(classHolder, out var levelsInClass) && levelsInClass > 0)
            {
                __result = classHolder;
            }
        }
    }
}