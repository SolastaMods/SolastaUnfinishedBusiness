using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches.Insertion;

internal static class RulesetCharacterPatcher
{
    [HarmonyPatch(typeof(RulesetCharacter), "IsSubjectToAttackOfOpportunity")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class IsSubjectToAttackOfOpportunity
    {
        // ReSharper disable once RedundantAssignment
        internal static void Postfix(RulesetCharacter __instance, ref bool __result, RulesetCharacter attacker)
        {
            __result = AttacksOfOpportunity.IsSubjectToAttackOfOpportunity(__instance, attacker, __result);
        }
    }
}