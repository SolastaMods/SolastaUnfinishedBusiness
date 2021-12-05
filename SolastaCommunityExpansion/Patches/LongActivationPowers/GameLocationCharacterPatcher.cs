
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.LongActivationPowers
{
    internal static class GameLocationCharacterPatcher
    {
        // Yes the actual game typos this it is "OnPower" and not the expected "OnePower".
        [HarmonyPatch(typeof(GameLocationCharacter), "CanUseAtLeastOnPower")]
        internal static class GameLocationCharacter_CanUseAtLeastOnPowerShowLong
        {
            // This makes it so that if a character only has powers that take longer than an action to activate the "Use Power" button is available.
            // But only not during a battle.
            internal static void Postfix(GameLocationCharacter __instance, ActionDefinitions.ActionType actionType, ref bool __result, bool accountDelegatedPowers)
            {
                if (__result)
                {
                    return;
                }
                if (__instance.RulesetCharacter != null)
                {
                    foreach (RulesetUsablePower rulesetUsablePower in __instance.RulesetCharacter.UsablePowers)
                    {
                        if (__instance.RulesetCharacter.GetRemainingUsesOfPower(rulesetUsablePower) > 0 && !(!accountDelegatedPowers && rulesetUsablePower.PowerDefinition.DelegatedToAction))
                        {
                            if (!ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress)
                            {
                                if (actionType == ActionDefinitions.ActionType.Main && (rulesetUsablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.Minute1 ||
                                    rulesetUsablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.Minute10 ||
                                    rulesetUsablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.Hours1 ||
                                    rulesetUsablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.Hours24))
                                {
                                    __result = true;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
