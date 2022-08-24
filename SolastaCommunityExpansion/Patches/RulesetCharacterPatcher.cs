using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

internal static class RulesetCharacterPatcher
{
    [HarmonyPatch(typeof(RulesetCharacter), "TerminateMatchingUniquePower")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class TerminateMatchingUniquePower_Patch
    {
        internal static void Postfix(RulesetCharacter __instance, FeatureDefinitionPower powerDefinition)
        {
            //PATCH: terminates all matching spells and powers of same group
            GlobalUniqueEffects.TerminateMatchingUniquePower(__instance, powerDefinition);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "TerminateMatchingUniqueSpell")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class TerminateMatchingUniqueSpell_Patch
    {
        internal static void Postfix(RulesetCharacter __instance, SpellDefinition spellDefinition)
        {
            //PATCH: terminates all matching spells and powers of same group
            GlobalUniqueEffects.TerminateMatchingUniqueSpell(__instance, spellDefinition);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "OnConditionAdded")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class OnConditionAdded_Patch
    {
        internal static void Postfix(RulesetCharacter __instance, RulesetCondition activeCondition)
        {
            //PATCH: notifies custom condition features that condition is applied 
            activeCondition.ConditionDefinition.Features
                .SelectMany(f => f.GetAllSubFeaturesOfType<ICustomConditionFeature>())
                .ToList()
                .ForEach(c => c.ApplyFeature(__instance));
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "OnConditionRemoved")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class OnConditionRemoved_Patch
    {
        internal static void Postfix(RulesetCharacter __instance, RulesetCondition activeCondition)
        {
            //PATCH: notifies custom condition features that condition is removed 
            activeCondition.ConditionDefinition.Features
                .SelectMany(f => f.GetAllSubFeaturesOfType<ICustomConditionFeature>())
                .ToList()
                .ForEach(c => c.RemoveFeature(__instance));
        }
    }
}