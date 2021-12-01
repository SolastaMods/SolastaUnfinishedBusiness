using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.ClassHoldingFeature
{
    internal static class RulesetCharacterHeroPatcher
    {
        [HarmonyPatch(typeof(RulesetCharacterHero), "FindClassHoldingFeature")]
        internal static class RulesetCharacterHero_FindClassHoldingFeature
        {
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
}
