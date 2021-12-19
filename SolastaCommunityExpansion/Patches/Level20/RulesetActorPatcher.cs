using HarmonyLib;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.Level20
{
    [HarmonyPatch(typeof(RulesetActor), "RollDie")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetActor_RollDie
    {
        private static readonly ConcurrentDictionary<RulesetActor, int> NextAbilityCheckMinimum = new ConcurrentDictionary<RulesetActor, int>();

        internal static void Postfix(
            RulesetActor __instance,
            RuleDefinitions.DieType dieType,
            RuleDefinitions.RollContext rollContext,
            ref int __result)
        {
            if (!Main.Settings.EnableLevel20)
            {
                return;
            }

            if (dieType != RuleDefinitions.DieType.D20 || rollContext != RuleDefinitions.RollContext.AbilityCheck)
            {
                return;
            }

            // This will only come up when RulesetCharacter.ResolveContestCheck is called (usually for shove checks).
            // The ResolveContestCheck patch checks for what the minimum die roll should be when RollDie is called.

            if (!NextAbilityCheckMinimum.TryRemove(__instance, out int minimum))
            {
                // There isn't an entry for the current instance; do nothing
                return;
            }

            if (minimum > __result)
            {
                __result = minimum;
            }
        }

        internal static void SetNextAbilityCheckMinimum(RulesetActor actor, int minimum)
        {
            if (actor == null)
            {
                return;
            }

            NextAbilityCheckMinimum[actor] = minimum;
        }
    }
}
