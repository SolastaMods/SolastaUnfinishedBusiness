using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationCharacterPatcher
{
    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.StartBattleTurn))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class StartBattleTurn_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameLocationCharacter __instance)
        {
            //PATCH: acts as a callback for the character's combat turn started event
            CharacterBattleListenersPatch.OnCharacterTurnStarted(__instance);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.EndBattleTurn))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EndBattleTurn_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameLocationCharacter __instance)
        {
            //PATCH: acts as a callback for the character's combat turn ended event
            CharacterBattleListenersPatch.OnCharacterTurnEnded(__instance);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.StartBattle))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class StartBattle_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameLocationCharacter __instance, bool surprise)
        {
            //PATCH: acts as a callback for the character's combat started event
            //while there already is callback for this event it doesn't have character or surprise flag arguments
            CharacterBattleListenersPatch.OnCharacterBattleStarted(__instance, surprise);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.EndBattle))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EndBattle_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameLocationCharacter __instance)
        {
            //PATCH: acts as a callback for the character's combat ended event
            //while there already is callback for this event it doesn't have character argument
            CharacterBattleListenersPatch.OnCharacterBattleEnded(__instance);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.AttackOn))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class AttackOn_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
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

            var features = character.GetSubFeaturesByType<IBeforeAttackEffect>();

            foreach (var effect in features)
            {
                effect.BeforeOnAttackHit(__instance, target, outcome, actionParams, attackMode, attackModifier);
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.AttackImpactOn))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class AttackImpactOn_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
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

            var features = character.GetSubFeaturesByType<IAfterAttackEffect>();

            foreach (var effect in features)
            {
                effect.AfterOnAttackHit(__instance, target, outcome, actionParams, attackMode, attackModifier);
            }

            //PATCH: registers which weapon types were used so far on attacks
            ValidatorsCharacter.RegisterWeaponTypeUsed(__instance, attackMode);
        }
    }

    // Yes the actual game typos this it is "OnPower" and not the expected "OnePower"
    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.CanUseAtLeastOnPower))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CanUseAtLeastOnPower_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            GameLocationCharacter __instance,
            ActionDefinitions.ActionType actionType,
            ref bool __result,
            bool accountDelegatedPowers)
        {
            var rulesetCharacter = __instance.RulesetCharacter;

            if (rulesetCharacter == null)
            {
                return;
            }

            var battleInProgress = Gui.Battle != null;

            //PATCH: force show power use button during exploration if it has at least one usable power
            //This makes it so that if a character only has powers that take longer than an action to activate the "Use Power" button is available.
            if (__result || battleInProgress)
            {
                return;
            }

            if (actionType == ActionDefinitions.ActionType.Main
                && rulesetCharacter.UsablePowers.Any(rulesetUsablePower =>
                    CanUsePower(rulesetCharacter, rulesetUsablePower, accountDelegatedPowers)))

            {
                __result = true;
            }
        }

        private static bool CanUsePower(RulesetCharacter character, RulesetUsablePower usablePower,
            bool accountDelegatedPowers)
        {
            var power = usablePower.PowerDefinition;
            return (accountDelegatedPowers || !power.DelegatedToAction)
                   && character.CanUsePower(power, false);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.GetActionStatus))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetActionStatus_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Support for Pugilist Fighting Style
            // Removes check that makes `ShoveBonus` action unavailable if character has no shield
            static bool True(RulesetActor actor)
            {
                return true;
            }

            var isWearingShieldMethod = typeof(RulesetCharacter).GetMethod("IsWearingShield");
            var trueMethod = new Func<RulesetActor, bool>(True).Method;

            return instructions.ReplaceCalls(isWearingShieldMethod,
                "GameLocationCharacter.GetActionStatus",
                new CodeInstruction(OpCodes.Call, trueMethod));
        }

        [UsedImplicitly]
        public static void Postfix(GameLocationCharacter __instance,
            ref ActionDefinitions.ActionStatus __result,
            ActionDefinitions.Id actionId,
            ActionDefinitions.ActionScope scope,
            ActionDefinitions.ActionStatus actionTypeStatus,
            RulesetAttackMode optionalAttackMode,
            bool ignoreMovePoints,
            bool allowUsingDelegatedPowersAsPowers)
        {
            //PATCH: support for `IReplaceAttackWithCantrip` - allows `CastMain` action if character used attack
            ReplaceAttackWithCantrip.AllowCastDuringMainAttack(__instance, actionId, scope, ref __result);

            //PATCH: support for custom invocation action ids
            CustomActionIdContext.ProcessCustomActionIds(__instance, ref __result, actionId, scope, actionTypeStatus,
                ignoreMovePoints);

            //PATCH: support `Poisoner` feat to only allow poisons to be used on use item bonus
            var hero = __instance.RulesetCharacter as RulesetCharacterHero ??
                       __instance.RulesetCharacter.OriginalFormCharacter as RulesetCharacterHero;

            // only Roguish Thief should be allowed to use any item as a bonus action
            if (actionId != ActionDefinitions.Id.UseItemBonus ||
                (hero.ClassesAndSubclasses.TryGetValue(Rogue, out var characterSubclassDefinition) &&
                 characterSubclassDefinition.Name == "RoguishThief"))
            {
                return;
            }

            __instance.RulesetCharacter.RefreshUsableDeviceFunctions();

            foreach (var enumerateAvailableDevice in __instance.RulesetCharacter
                         .EnumerateAvailableDevices(false))
            {
                if (!__instance.RulesetCharacter.UsableDeviceFunctionsByDevice.ContainsKey(enumerateAvailableDevice))
                {
                    continue;
                }

                // change result if not a poison
                if (!enumerateAvailableDevice.UsableDeviceDescription.UsableDeviceTags.Contains("Poison"))
                {
                    __result = ActionDefinitions.ActionStatus.CannotPerform;
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.RefreshActionPerformances))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshActionPerformances_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var enumerate1 = new Action<
                RulesetActor,
                List<FeatureDefinition>,
                Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
            >(FeatureApplicationValidation.EnumerateActionPerformanceProviders).Method;

            var enumerate2 = new Action<
                RulesetActor,
                List<FeatureDefinition>,
                Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
            >(FeatureApplicationValidation.EnumerateAdditionalActionProviders).Method;

            return instructions
                //PATCH: Support for `IDefinitionApplicationValidator`
                .ReplaceEnumerateFeaturesToBrowse("IActionPerformanceProvider",
                    -1, "GameLocationCharacter.RefreshActionPerformances.ValidateActionPerformanceProviders",
                    new CodeInstruction(OpCodes.Call, enumerate1))
                //PATCH: Support for `IDefinitionApplicationValidator`
                //PATCH: also moves on `HasDownedAnEnemy` bonus actions to the end of the list, preserving order
                //fixes main attacks stopping working if Horde Breaker's extra action on kill is triggered after Action Surge
                .ReplaceEnumerateFeaturesToBrowse("IAdditionalActionsProvider",
                    -1, "GameLocationCharacter.RefreshActionPerformances.ValidateAdditionalActionProviders",
                    new CodeInstruction(OpCodes.Call, enumerate2));
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.HandleActionExecution))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class HandleActionExecution_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            GameLocationCharacter __instance,
            CharacterActionParams actionParams,
            ActionDefinitions.ActionScope scope)
        {
            //PATCH: support for `IReplaceAttackWithCantrip` - counts cantrip casting as 1 main attack
            ReplaceAttackWithCantrip.AllowAttacksAfterCantrip(__instance, actionParams, scope);
            ReplaceAttackWithCantrip.MightRefundOneAttackOfMainAction(__instance, actionParams, scope);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.GetActionAvailableIterations))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetActionAvailableIterations_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Support for ExtraAttacksOnActionPanel
            //replaces calls to FindExtraActionAttackModes to custom method which supports forced attack modes for offhand attacks
            var findAttacks = typeof(GameLocationCharacter).GetMethod("FindActionAttackMode");
            var method = new Func<
                GameLocationCharacter,
                ActionDefinitions.Id,
                bool,
                bool,
                ActionDefinitions.ReadyActionType,
                RulesetAttackMode,
                RulesetAttackMode
            >(ExtraAttacksOnActionPanel.FindExtraActionAttackModesFromForcedAttack).Method;

            return instructions.ReplaceCalls(findAttacks,
                "GameLocationCharacter.GetActionAvailableIterations",
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, method));
        }
    }
}
