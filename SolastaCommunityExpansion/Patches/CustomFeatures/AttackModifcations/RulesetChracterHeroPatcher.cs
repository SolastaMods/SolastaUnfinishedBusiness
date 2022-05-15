using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Features;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.AttackModifcations;

internal static class RulesetChracterHeroPatcher
{
    // Allows changing what attribute is used for weapon's attack and damage rolls
    [HarmonyPatch(typeof(RulesetCharacterHero), "ComputeAttackModeAbilityScoreReplacement")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_ComputeAttackModeAbilityScoreReplacement
    {
        internal static void Prefix(RulesetCharacterHero __instance, RulesetAttackMode attackMode, RulesetItem weapon)
        {
            var attributeModifiers = __instance.GetSubFeaturesByType<IModifyAttackModeForWeapon>();

            foreach (var modifier in attributeModifiers)
            {
                modifier.Apply(__instance, attackMode, weapon);
            }
        }
    }
}