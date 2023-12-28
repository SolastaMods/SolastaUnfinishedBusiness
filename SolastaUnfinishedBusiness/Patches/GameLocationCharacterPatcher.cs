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
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MetamagicOptionDefinitions;

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

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.AttackImpactOn))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class AttackImpactOn_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            [NotNull] GameLocationCharacter __instance,
            GameLocationCharacter target,
            RollOutcome outcome,
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

            var features = character.GetSubFeaturesByType<IPhysicalAttackAfterDamage>();

            foreach (var effect in features)
            {
                effect.OnPhysicalAttackAfterDamage(__instance, target, outcome, actionParams, attackMode,
                    attackModifier);
            }
        }
    }

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
            var isWearingShieldMethod = typeof(RulesetCharacter).GetMethod("IsWearingShield");
            var trueMethod = new Func<RulesetActor, bool>(True).Method;

            return instructions.ReplaceCalls(isWearingShieldMethod,
                "GameLocationCharacter.GetActionStatus",
                new CodeInstruction(OpCodes.Call, trueMethod));

            //PATCH: Support for Pugilist Fighting Style
            // Removes check that makes `ShoveBonus` action unavailable if character has no shield
            static bool True(RulesetActor actor)
            {
                return true;
            }
        }

        [UsedImplicitly]
        public static void Postfix(
                GameLocationCharacter __instance,
                ref ActionDefinitions.ActionStatus __result,
                ActionDefinitions.Id actionId,
                ActionDefinitions.ActionScope scope,
                ActionDefinitions.ActionStatus actionTypeStatus,
                // RulesetAttackMode optionalAttackMode,
                bool ignoreMovePoints)
            // bool allowUsingDelegatedPowersAsPowers)
        {
            //PATCH: support for `IReplaceAttackWithCantrip` - allows `CastMain` action if character used attack
            ReplaceAttackWithCantrip.AllowCastDuringMainAttack(__instance, actionId, scope, ref __result);

            //PATCH: support for custom invocation action ids
            CustomActionIdContext.ProcessCustomActionIds(__instance, ref __result, actionId, scope, actionTypeStatus,
                ignoreMovePoints);

            //PATCH: support `EnableMonkDoNotRequireAttackActionForFlurry`
            if (Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry &&
                actionId
                    is ActionDefinitions.Id.FlurryOfBlows
                    or ActionDefinitions.Id.FlurryOfBlowsSwiftSteps
                    or ActionDefinitions.Id.FlurryOfBlowsUnendingStrikes &&
                __result == ActionDefinitions.ActionStatus.CannotPerform &&
                __instance.GetActionTypeStatus(ActionDefinitions.ActionType.Bonus) ==
                ActionDefinitions.ActionStatus.Available &&
                __result != ActionDefinitions.ActionStatus.CannotPerform)
            {
                __result = ActionDefinitions.ActionStatus.Available;
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
            return instructions
                //PATCH: Support for `IValidateDefinitionApplication`
                .ReplaceEnumerateFeaturesToBrowse<IActionPerformanceProvider>(
                    "GameLocationCharacter.RefreshActionPerformances.ValidateActionPerformanceProviders",
                    FeatureApplicationValidation.EnumerateActionPerformanceProviders)
                //PATCH: Support for action switching
                .ReplaceEnumerateFeaturesToBrowse<IAdditionalActionsProvider>(
                    "GameLocationCharacter.RefreshActionPerformances.ValidateAdditionalActionProviders",
                    FeatureApplicationValidation.EnumerateAdditionalActionProviders);
        }

        [UsedImplicitly]
        public static void Postfix(GameLocationCharacter __instance)
        {
            //PATCH: support for action switching
            ActionSwitching.ResortPerformances(__instance);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.SpendActionType))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SpendActionType_Patch
    {
        [UsedImplicitly]
        public static void Prefix(GameLocationCharacter __instance, ActionDefinitions.ActionType actionType)
        {
            //PATCH: support for action switching
            ActionSwitching.SpendActionType(__instance, actionType);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.RefundActionUse))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefundActionUse_Patch
    {
        [UsedImplicitly]
        public static void Prefix(GameLocationCharacter __instance, ActionDefinitions.ActionType actionType)
        {
            //PATCH: support for action switching
            ActionSwitching.RefundActionUse(__instance, actionType);

            //PATCH: fix reaction counted as not available even after refund
            if (actionType == ActionDefinitions.ActionType.Reaction)
            {
                __instance.ReactionEngaged = false;
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.HandleActionExecution))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class HandleActionExecution_Patch
    {
        private static int _mainAttacks, _bonusAttacks, _mainRank, _bonusRank;

        [UsedImplicitly]
        public static void Prefix(
            GameLocationCharacter __instance,
            CharacterActionParams actionParams,
            ActionDefinitions.ActionScope scope)
        {
            _mainRank = __instance.currentActionRankByType[ActionDefinitions.ActionType.Main];
            _bonusRank = __instance.currentActionRankByType[ActionDefinitions.ActionType.Bonus];
            _mainAttacks = __instance.UsedMainAttacks;
            _bonusAttacks = __instance.UsedBonusAttacks;
        }

        [UsedImplicitly]
        public static void Postfix(
            GameLocationCharacter __instance,
            CharacterActionParams actionParams,
            ActionDefinitions.ActionScope scope)
        {
            var rulesetCharacter = __instance.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            //PATCH: support for `IReplaceAttackWithCantrip` - counts cantrip casting as 1 main attack
            ReplaceAttackWithCantrip.AllowAttacksAfterCantrip(__instance, actionParams, scope);

            //PATCH: support for `IActionExecutionHandled` - allows processing after action has been fully accounted for
            rulesetCharacter.GetSubFeaturesByType<IActionExecutionHandled>()
                .ForEach(f => f.OnActionExecutionHandled(__instance, actionParams, scope));

            //PATCH: support for action switching interaction with metamagic quickened spell
            if (Main.Settings.EnableActionSwitching
                && actionParams.RulesetEffect is RulesetEffectSpell rulesetEffectSpell
                && rulesetEffectSpell.MetamagicOption == MetamagicQuickenedSpell)
            {
                // another hack to ensure we don't get offered more than we should on action switching
                // for whatever reason we get the spell casting state loaded before we get to this point
                // causing all spells to be offered after a quickened instead of only cantrips
                __instance.UsedBonusSpell = true;
            }

            //PATCH: support for action switching
            ActionSwitching.CheckIfActionSwitched(
                __instance, actionParams, scope, _mainRank, _mainAttacks, _bonusRank, _bonusAttacks);
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

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.IsActionOnGoing))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsActionOnGoing_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameLocationCharacter __instance, ref bool __result, ActionDefinitions.Id actionId)
        {
            if (!CustomActionIdContext.IsCustomActionIdToggle(actionId))
            {
                return true;
            }

            __result = __instance.RulesetCharacter.IsToggleEnabled(actionId);

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.CheckConcentration))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CheckConcentration_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            GameLocationCharacter __instance,
            int damage,
            string damageType,
            bool stillConscious)
        {
            //PATCH: support for EffectWithConcentrationCheck
            EffectWithConcentrationCheck.ProcessConcentratedEffects(__instance, damage, damageType, stillConscious);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.GenerateCharacterDescription))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GenerateCharacterDescription_Patch
    {
        [UsedImplicitly]
        public static void Postfix(EntityDescription entityDescription)
        {
            Tooltips.AddDistanceToTooltip(entityDescription);
        }
    }

    //PATCH: supports RaceLightSensitivityApplyOutdoorsOnly
    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.CheckLightingAffinityEffects))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CheckLightingAffinityEffects_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameLocationCharacter __instance)
        {
            __instance.RulesetCharacter.TryGetFirstConditionOfCategory(AttributeDefinitions.TagLightSensitivity,
                out var activeCondition);
            __instance.RulesetCharacter.RemoveAllConditionsOfCategory(AttributeDefinitions.TagLightSensitivity, false);

            for (var index = __instance.affectingLightEffects.Count - 1; index >= 0; --index)
            {
                __instance.affectingLightEffects[index].Terminate(false);
            }

            __instance.affectingLightEffects.Clear();

            var affinityFeatures = __instance.RulesetCharacter.GetLightAffinityFeatures();

            RulesetCondition newCondition = null;

            foreach (var lightingEffectAndConditionList in affinityFeatures
                         .Select(featureDefinition =>
                             (featureDefinition as ILightingAffinityProvider)?.LightingEffectAndConditionList)
                         .Where(lightingEffectAndConditionList => lightingEffectAndConditionList != null))
            {
                foreach (var effectAndCondition in lightingEffectAndConditionList.Where(effectAndCondition =>
                             effectAndCondition.lightingState == __instance.lightingState))
                {
                    //BEGIN PATCH
                    if (Main.Settings.RaceLightSensitivityApplyOutdoorsOnly &&
                        __instance.lightingState == LocationDefinitions.LightingState.Bright)
                    {
                        var isExterior = false;

                        if (__instance is IIlluminable illuminable)
                        {
                            var gameLocationVisibilityManager =
                                ServiceRepository.GetService<IGameLocationVisibilityService>() as
                                    GameLocationVisibilityManager;

                            if (gameLocationVisibilityManager != null)
                            {
                                illuminable.GetAllPositionsToCheck(gameLocationVisibilityManager.positionCache);

                                var gridAccessor = new GridAccessor(gameLocationVisibilityManager.positionCache[0]);

                                isExterior = gridAccessor.sector.IsExterior;
                            }
                        }

                        if (effectAndCondition.condition == CustomConditionsContext.LightSensitivity && !isExterior)
                        {
                            continue;
                        }
                    }
                    //END PATCH

                    if (effectAndCondition.condition != null)
                    {
                        newCondition = RulesetCondition.CreateActiveCondition(
                            __instance.RulesetCharacter.Guid,
                            effectAndCondition.condition,
                            DurationType.Irrelevant,
                            0,
                            TurnOccurenceType.StartOfTurn,
                            __instance.Guid,
                            __instance.RulesetCharacter.CurrentFaction.Name);

                        __instance.RulesetCharacter.AddConditionOfCategory(
                            AttributeDefinitions.TagLightSensitivity, newCondition, false);
                    }

                    if (effectAndCondition.effect != null)
                    {
                        var rulesetImplementationService =
                            ServiceRepository.GetService<IRulesetImplementationService>();

                        __instance.affectingLightEffects.Add(rulesetImplementationService
                            .InstantiateEffectEnvironment(
                                __instance.RulesetCharacter, effectAndCondition.effect, -1, 0, false, new BoxInt(),
                                new int3(), string.Empty, false));
                    }

                    break;
                }
            }

            if ((activeCondition == null || (newCondition != null &&
                                             newCondition.ConditionDefinition.Name ==
                                             activeCondition.ConditionDefinition.Name)) &&
                (activeCondition != null || newCondition == null))
            {
                return false;
            }

            __instance.RulesetCharacter.RefreshAll();

            return false;
        }
    }
}
