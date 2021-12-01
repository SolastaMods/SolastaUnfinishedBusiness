using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.StartOfTurnRecharge
{
    internal static class RulesetCharacterPatcher
    {
        [HarmonyPatch(typeof(RulesetCharacter), "RechargePowersForTurnStart")]
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
