using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Powers;

// Yes the actual game typos this it is "OnPower" and not the expected "OnePower"
//
// this patch shouldn't be protected
//
[HarmonyPatch(typeof(GameLocationCharacter), "CanUseAtLeastOnPower")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationCharacter_CanUseAtLeastOnPower
{
    // This makes it so that if a character only has powers that take longer than an action to activate the "Use Power" button is available.
    // But only not during a battle.
    internal static void Postfix(GameLocationCharacter __instance, ActionDefinitions.ActionType actionType,
        ref bool __result, bool accountDelegatedPowers)
    {
        var rulesetCharacter = __instance.RulesetCharacter;
        if (__result)
        {
            if (rulesetCharacter == null)
            {
                return;
            }

            if (!rulesetCharacter.UsablePowers
                    .Any(rulesetUsablePower => CanUsePower(rulesetCharacter, rulesetUsablePower)))
            {
                __result = false;

                return;
            }

            __result = true;
        }

        if (rulesetCharacter == null)
        {
            return;
        }

        {
            foreach (var rulesetUsablePower in rulesetCharacter.UsablePowers)
            {
                if (rulesetCharacter.GetRemainingUsesOfPower(rulesetUsablePower) > 0 &&
                    CanUsePower(rulesetCharacter, rulesetUsablePower) &&
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

    private static bool CanUsePower(RulesetCharacter character, RulesetUsablePower usablePower)
    {
        var validator = usablePower.PowerDefinition.GetFirstSubFeatureOfType<IPowerUseValidity>();
        return validator == null || validator.CanUsePower(character);
    }
}
