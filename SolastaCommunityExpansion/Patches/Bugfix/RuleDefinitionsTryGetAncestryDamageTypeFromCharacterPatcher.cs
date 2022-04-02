using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    // Warlock has DamageForce set as ancestry damage type. This causes issues interacting with Rage powers so we fix this scenario here
    [HarmonyPatch(typeof(RuleDefinitions), "TryGetAncestryDamageTypeFromCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RuleDefinitions_TryGetAncestryDamageTypeFromCharacter
        {
            internal static void Postfix(ref bool __result, string ancestryDamageType)
            {
                if (ancestryDamageType == "DamageForce")
                {
                    __result = false;
                }
            }
        }
}
