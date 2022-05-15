using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Features;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.AttackModifcations;

internal static class RulesetChracterHeroPatcher
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "ComputeAttackModeAbilityScoreReplacement")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_ComputeAttackModeAbilityScoreReplacement
    {
        internal static void Prefix(RulesetCharacterHero __instance, RulesetAttackMode attackMode, RulesetItem weapon)
        {
            var modifiers = __instance.GetSubFeaturesByType<ICanUseAttributeForWeapon>();
            var attribute = attackMode.AbilityScore;
            var value = __instance.GetAttribute(attribute).CurrentValue;

            foreach (var modifier in modifiers)
            {
                var newAttribute = modifier.GetAttribute(__instance, attackMode, weapon);
                if (newAttribute == null)
                {
                    continue;
                }

                var newValue = __instance.GetAttribute(newAttribute).CurrentValue;
                if (newValue > value)
                {
                    attribute = newAttribute;
                    value = newValue;
                }
            }

            attackMode.AbilityScore = attribute;
        }
    }
}