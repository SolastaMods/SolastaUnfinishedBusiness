using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.LongActivationPowers
{
    // Yes the actual game typos this it is "OnPower" and not the expected "OnePower".
    [HarmonyPatch(typeof(GameLocationCharacter), "CanUseAtLeastOnPower")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationCharacter_CanUseAtLeastOnPower
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
                    if (__instance.RulesetCharacter.GetRemainingUsesOfPower(rulesetUsablePower) > 0 &&
                        !(!accountDelegatedPowers && rulesetUsablePower.PowerDefinition.DelegatedToAction) &&
                        !ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress &&
                        actionType == ActionDefinitions.ActionType.Main &&
                        (rulesetUsablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.Minute1 ||
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
