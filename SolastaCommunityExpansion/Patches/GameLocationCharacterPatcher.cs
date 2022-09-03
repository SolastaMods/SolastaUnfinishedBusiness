using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.CustomUI;

namespace SolastaCommunityExpansion.Patches;

internal static class GameLocationCharacterPatcher
{
    [HarmonyPatch(typeof(GameLocationCharacter), "StartBattleTurn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class StartBattleTurn_Patch
    {
        internal static void Postfix(GameLocationCharacter __instance)
        {
            //PATCH: acts as a callback for the character's combat turn started event
            CharacterBattleListenersPatch.OnChracterTurnStarted(__instance);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "EndBattleTurn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class EndBattleTurn_Patch
    {
        internal static void Postfix(GameLocationCharacter __instance)
        {
            //PATCH: acts as a callback for the character's combat turn ended event
            CharacterBattleListenersPatch.OnChracterTurnEnded(__instance);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "StartBattle")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class StartBattle_Patch
    {
        internal static void Postfix(GameLocationCharacter __instance, bool surprise)
        {
            //PATCH: acts as a callback for the character's combat started event
            //while there already is callback for this event it doesn't have character or surprise flag arguments
            CharacterBattleListenersPatch.OnChracterBattleStarted(__instance, surprise);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "EndBattle")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class EndBattle_Patch
    {
        internal static void Postfix(GameLocationCharacter __instance)
        {
            //PATCH: acts as a callback for the character's combat ended event
            //while there already is callback for this event it doesn't have character argument
            CharacterBattleListenersPatch.OnChracterBattleEnded(__instance);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "FindActionAttackMode")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FindActionAttackMode_Patch
    {
        internal static void Postfix(GameLocationCharacter __instance, ref RulesetAttackMode __result,
            ActionDefinitions.Id actionId)
        {
            //PATCH: Skips specified amount of attack modes for main and bonus action
            //used for displaying multiple attacks on the actions panel
            __result = ExtraAttacksOnActionPanel.FindExtraActionAttackModes(__instance, __result, actionId);
        }
    }
    
    [HarmonyPatch(typeof(GameLocationCharacter), "AttackOn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AttackOn
    {
        internal static void Prefix(
            [NotNull] GameLocationCharacter __instance,
            GameLocationCharacter target,
            RuleDefinitions.RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            //PATCH: support for `IOnAttackHitEffect` - calls before attack handlers
            var character = __instance.RulesetCharacter;

            if (character == null)
            {
                return;
            }

            var features = character.GetSubFeaturesByType<IOnAttackHitEffect>();

            foreach (var effect in features)
            {
                effect.BeforeOnAttackHit(__instance, target, outcome, actionParams, attackMode, attackModifier);
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "AttackImpactOn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AttackImpactOn
    {
        internal static void Prefix(
            [NotNull] GameLocationCharacter __instance,
            GameLocationCharacter target,
            RuleDefinitions.RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            //PATCH: support for `IOnAttackHitEffect` - calls after attack handlers
            var character = __instance.RulesetCharacter;
            if (character == null)
            {
                return;
            }

            var features = character.GetSubFeaturesByType<IOnAttackHitEffect>();
            foreach (var effect in features)
            {
                effect.AfterOnAttackHit(__instance, target, outcome, actionParams, attackMode, attackModifier);
            }
        }
    }
    
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
                if (!rulesetCharacter.UsablePowers.Any(rulesetUsablePower =>
                        rulesetCharacter.GetRemainingUsesOfPower(rulesetUsablePower) > 0 &&
                        CanUsePower(rulesetCharacter, rulesetUsablePower) &&
                        (accountDelegatedPowers || !rulesetUsablePower.PowerDefinition.DelegatedToAction) &&
                        !ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress &&
                        actionType == ActionDefinitions.ActionType.Main &&
                        rulesetUsablePower.PowerDefinition.ActivationTime is RuleDefinitions.ActivationTime.Minute1
                            or RuleDefinitions.ActivationTime.Minute10 or RuleDefinitions.ActivationTime.Hours1
                            or RuleDefinitions.ActivationTime.Hours24))
                {
                    return;
                }

                __result = true;
            }
        }

        private static bool CanUsePower(RulesetCharacter character, RulesetUsablePower usablePower)
        {
            var validator = usablePower.PowerDefinition.GetFirstSubFeatureOfType<IPowerUseValidity>();
            return validator == null || validator.CanUsePower(character);
        }
    }
}
