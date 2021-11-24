using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshActiveFightingStyles")]
    internal static class RulesetCharacterHero_RefreshActiveFightingStyles
    {
        internal static void Postfix(RulesetCharacterHero __instance)
        {
            foreach (FightingStyleDefinition fightingStyleDefinition in __instance.TrainedFightingStyles)
            {
                if (fightingStyleDefinition is ICustomFightingStyle)
                {
                    bool isActive = (fightingStyleDefinition as ICustomFightingStyle).IsActive(__instance);
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
}
