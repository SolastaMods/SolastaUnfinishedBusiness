using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.ClassHoldingFeature
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "FindClassHoldingFeature")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_FindClassHoldingFeature
    {
        //
        // TODO @CHRIS: WE SHOULD PROTECT THIS PATCH
        //
        internal static void Postfix(
            RulesetCharacterHero __instance,
            FeatureDefinition featureDefinition,
            ref CharacterClassDefinition __result)
        {
            var overrideClassHoldingFeature = featureDefinition as IClassHoldingFeature;

            if (overrideClassHoldingFeature?.Class == null)
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
