using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Insertion;

internal static class RulesetCharacterHeroPatcher
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "AcknowledgeAttackUse")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AcknowledgeAttackUse
    {
        // ReSharper disable once RedundantAssignment
        internal static void Prefix(RulesetCharacterHero __instance,
            RulesetAttackMode mode)
        {
            CustomWeaponsContext.ProcessProducedFlameAttack(__instance, mode);
        }
    }
}
