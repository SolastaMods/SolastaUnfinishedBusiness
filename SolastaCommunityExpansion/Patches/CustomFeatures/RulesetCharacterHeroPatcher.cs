using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "FindClassHoldingFeature")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_FindClassHoldingFeature
    {
        //
        // TODO @CHRIS: should we protect this patch? What is the purpose?
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

    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshActiveFightingStyles")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_RefreshActiveFightingStyles
    {
        internal static void Postfix(RulesetCharacterHero __instance)
        {
            foreach (FightingStyleDefinition fightingStyleDefinition in __instance.TrainedFightingStyles)
            {
                if (!(fightingStyleDefinition is ICustomFightingStyle customFightingStyle))
                {
                    continue;
                }

                bool isActive = customFightingStyle.IsActive(__instance);
                // We don't know what normal fighting style condition was used or if it was met.
                // The simplest thing to do is just make sure the active state of this fighting style is handled properly.
                if (isActive)
                {
                    if (!__instance.ActiveFightingStyles.Contains(fightingStyleDefinition))
                    {
                        __instance.ActiveFightingStyles.Add(fightingStyleDefinition);
                    }
                }
                else
                {
                    if (__instance.ActiveFightingStyles.Contains(fightingStyleDefinition))
                    {
                        __instance.ActiveFightingStyles.Remove(fightingStyleDefinition);
                    }
                }
            }
        }
    }
}
