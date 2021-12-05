using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.StartOfTurnRecharge
{
    internal static class RulesetCharacterPatcher
    {
        [HarmonyPatch(typeof(RulesetCharacter), "RechargePowersForTurnStart")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class RulesetCharacter_RechargePowersForTurnStart
        {
            internal static void Postfix(RulesetCharacter __instance)
            {
                foreach (RulesetUsablePower usablePower in __instance.UsablePowers)
                {
                    var startOfTurnRecharge = usablePower?.PowerDefinition as IStartOfTurnRecharge;

                    if (startOfTurnRecharge != null)
                    {
                        if (usablePower.RemainingUses < usablePower.MaxUses)
                        {
                            usablePower.Recharge();

                            if (!startOfTurnRecharge.IsRechargeSilent && __instance.PowerRecharged != null)
                            {
                                __instance.PowerRecharged(__instance, usablePower);
                            }
                        }
                    }
                }
            }
        }
    }
}
