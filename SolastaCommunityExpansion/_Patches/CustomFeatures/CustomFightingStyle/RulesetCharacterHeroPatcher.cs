using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaModApi.Extensions;
using static FightingStyleDefinition;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomFightingStyle
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshActiveFightingStyles")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_RefreshActiveFightingStyles
    {
        internal static void Postfix(RulesetCharacterHero __instance)
        {
            foreach (var fightingStyleDefinition in __instance.TrainedFightingStyles)
            {
                bool? isActive = null;
                if (fightingStyleDefinition is ICustomFightingStyle customFightingStyle)
                {
                    isActive = customFightingStyle.IsActive(__instance);
                }

                // Count shield as weapons if character has `AddBonusShieldAttack` feature
                if (fightingStyleDefinition.Condition == TriggerCondition.TwoMeleeWeaponsWielded)
                {
                    if (__instance.IsWearingShield() &&
                        __instance.HasSubFeatureOfType<AddBonusShieldAttack>())
                    {
                        isActive = true;
                    }
                }

                if (isActive == null)
                {
                    continue;
                }

                // We don't know what normal fighting style condition was used or if it was met.
                // The simplest thing to do is just make sure the active state of this fighting style is handled properly.
                if (isActive.Value)
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
