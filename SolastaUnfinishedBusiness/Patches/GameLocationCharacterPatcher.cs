using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.FightingStyles;

namespace SolastaUnfinishedBusiness.Patches;

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

    [HarmonyPatch(typeof(GameLocationCharacter), "AttackOn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AttackOn_Patch
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
    internal static class AttackImpactOn_Patch
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
    internal static class CanUseAtLeastOnPower_Patch
    {
        // This makes it so that if a character only has powers that take longer than an action to activate the "Use Power" button is available.
        // But only not during a battle.
        internal static void Postfix(GameLocationCharacter __instance, ActionDefinitions.ActionType actionType,
            ref bool __result, bool accountDelegatedPowers)
        {
            var rulesetCharacter = __instance.RulesetCharacter;

            if (rulesetCharacter == null)
            {
                return;
            }

            if (__result)
            {
                //PATCH: hide use power button if character has no valid powers
                if (!rulesetCharacter.UsablePowers
                        .Any(rulesetUsablePower => CanUsePower(rulesetCharacter, rulesetUsablePower)))
                {
                    __result = false;
                }
            }
            //PATCH: force show power use button during exploration if it has at least one usable power
            else if (!ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress
                     && actionType == ActionDefinitions.ActionType.Main
                     && rulesetCharacter.UsablePowers.Any(rulesetUsablePower =>
                         rulesetCharacter.GetRemainingUsesOfPower(rulesetUsablePower) > 0 &&
                         CanUsePower(rulesetCharacter, rulesetUsablePower)))
            {
                __result = true;
            }
        }

        private static bool CanUsePower(RulesetCharacter character, RulesetUsablePower usablePower)
        {
            var validator = usablePower.PowerDefinition.GetFirstSubFeatureOfType<IPowerUseValidity>();
            return validator == null || validator.CanUsePower(character);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "GetActionStatus")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GetActionStatus_Patch
    {
        [NotNull]
        internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            //PATCH: Support for Pugilist Fighting Style
            // Removes check that makes `ShoveBonus` action unavailable if character has no shield
            PugilistFightingStyle.RemoveShieldRequiredForBonusPush(codes);

            return codes.AsEnumerable();
        }

        internal static void Postfix(ref GameLocationCharacter __instance, ActionDefinitions.Id actionId,
            ActionDefinitions.ActionScope scope, ref ActionDefinitions.ActionStatus __result)
        {
            //PATCH: support for `IReplaceAttackWithCantrip` - allows `CastMain` action if character used attack
            ReplaceAttackWithCantrip.AllowCastDuringMainAttack(__instance, actionId, scope, ref __result);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "RefreshActionPerformances")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshActionPerformances_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            //PATCH: Support for `IFeatureApplicationValidator`
            FeatureApplicationValidation.ValidateActionPerformanceProviders(codes);

            //PATCH: Support for `IFeatureApplicationValidator`
            FeatureApplicationValidation.ValidateAdditionalActionProviders(codes);

            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "HandleActionExecution")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleActionExecution_Patch
    {
        internal static void Postfix(
            GameLocationCharacter __instance,
            CharacterActionParams actionParams,
            ActionDefinitions.ActionScope scope)
        {
            //PATCH: support for `IReplaceAttackWithCantrip` - counts cantrip casting as 1 main attack
            ReplaceAttackWithCantrip.AllowAttacksAfterCantrip(__instance, actionParams, scope);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "GetActionAvailableIterations")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GetActionAvailableIterations_Patch
    {
        [NotNull]
        internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Support for ExtraAttacksOnActionPanel
            //replaces calls to FindExtraActionAttackModes to custom method which supports forced attack modes for offhand attacks
            return ExtraAttacksOnActionPanel.ReplaceFindExtraActionAttackModesInLocationCharacter(instructions);
        }
    }
}
