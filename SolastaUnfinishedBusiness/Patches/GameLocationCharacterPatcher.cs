using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Spells;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Validators;
using TA;
using UnityEngine;
using static ActionDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationCharacterPatcher
{
    //PATCH: supports IForceLightingState
    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.LightingState), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class LightingState_Getter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            GameLocationCharacter __instance,
            ref LocationDefinitions.LightingState __result)
        {
            var modifiers = __instance.RulesetActor.GetSubFeaturesByType<IForceLightingState>();

            if (modifiers.Count == 0)
            {
                return true;
            }

            __result = modifiers[0].GetLightingState(__instance, __result);

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.CanMoveInSituation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CanMoveInSituation_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameLocationCharacter __instance, ref bool __result,
            RulesetCharacter.MotionRange motionRange)
        {
            if (__result) { return; }

            //PATCH: allow standing on air if grappled
            if (motionRange == RulesetCharacter.MotionRange.AboveGround)
            {
                __result = __instance.IsGrappled();
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.CheckMotionValidity))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CheckMotionValidity_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameLocationCharacter __instance)
        {
            //PATCH: always allow prone when grappled
            return !__instance.Prone || !__instance.IsGrappled();
        }
    }

    //PATCH: supports `UseOfficialLightingObscurementAndVisionRules`
    //let ADV/DIS be handled elsewhere in `GLBM.CanAttack` if alternate lighting and obscurement rules in place
    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.ComputeLightingModifierForIlluminable))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeLightingModifierForIlluminable_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            GameLocationCharacter __instance,
            IIlluminable target,
            LocationDefinitions.LightingState targetLightingState,
            Vector3 gravityCenter,
            Vector3 targetGravityCenter,
            ActionModifier actionModifier)
        {
            if (!Main.Settings.UseOfficialLightingObscurementAndVisionRules)
            {
                ComputeLightingModifierForLightingState(
                    __instance,
                    (gravityCenter - targetGravityCenter).magnitude,
                    targetLightingState,
                    actionModifier,
                    target.TargetSource);
            }

            return false;
        }

        private static void ComputeLightingModifierForLightingState(
            GameLocationCharacter __instance,
            float distance,
            LocationDefinitions.LightingState lightingState,
            ActionModifier actionModifier,
            object source = null)
        {
            var isValidForLightingState = false;
            var isDarkvisionOffRange = false;
            var isWithinRange = false;

            // BEGIN PATCH
            var senseModesToPrevent = new List<SenseMode.Type>();

            if (source is RulesetCharacter rulesetCharacter)
            {
                foreach (var modifier in rulesetCharacter.GetSubFeaturesByType<IPreventEnemySenseMode>())
                {
                    senseModesToPrevent.AddRange(modifier.PreventedSenseModes(__instance, rulesetCharacter));
                }
            }
            // END PATCH

            foreach (var senseMode in __instance.RulesetCharacter.SenseModes
                         .Where(x => !senseModesToPrevent.Contains(x.SenseType)))
            {
                if (distance > (double)senseMode.SenseRange)
                {
                    if (senseMode.SenseType == SenseMode.Type.Darkvision)
                    {
                        isDarkvisionOffRange = true;
                    }
                }
                else
                {
                    isWithinRange = true;

                    if (!SenseMode.ValidForLighting(senseMode.SenseType, lightingState))
                    {
                        continue;
                    }

                    isValidForLightingState = true;
                    break;
                }
            }

            if (isValidForLightingState || !isWithinRange)
            {
                return;
            }

            var additionalDetails = isDarkvisionOffRange && lightingState == LocationDefinitions.LightingState.Unlit
                ? "Tooltip/&OutOfDarkvisionRangeFormat"
                : string.Empty;

            actionModifier.AttackAdvantageTrends.Add(new TrendInfo(-1, FeatureSourceType.Lighting,
                lightingState.ToString(), source, additionalDetails));
            actionModifier.AbilityCheckAdvantageTrends.Add(new TrendInfo(-1, FeatureSourceType.Lighting,
                lightingState.ToString(), source, additionalDetails));
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.StartBattleTurn))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class StartBattleTurn_Patch
    {
        [UsedImplicitly]
        public static void Prefix(GameLocationCharacter __instance)
        {
            //PATCH: acts as a callback for the character's before combat turn started event
            CharacterBattleListenersPatch.OnCharacterBeforeTurnStarted(__instance);
        }

        [UsedImplicitly]
        public static void Postfix(GameLocationCharacter __instance)
        {
            //PATCH: acts as a callback for the character's combat turn started event
            CharacterBattleListenersPatch.OnCharacterTurnStarted(__instance);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.StartBattleTurnOtherContender))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class StartBattleTurnOtherContender_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameLocationCharacter __instance)
        {
            var rulesetCharacter = __instance.RulesetCharacter;

            if (__instance.GetActionTypeStatus(ActionType.Reaction) != ActionStatus.Available)
            {
                rulesetCharacter.EnumerateFeaturesToBrowse<IActionPerformanceProvider>(
                    __instance.featuresCache, rulesetCharacter.FeaturesOrigin);

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var feature in __instance.featuresCache)
                {
                    var actionAffinity = feature as FeatureDefinitionActionAffinity;

                    if (!actionAffinity ||
                        !actionAffinity.RechargeReactionsAtEveryTurn)
                    {
                        continue;
                    }

                    //---- START ----
                    //PATCH: support for feature usage limits
                    if (feature.GetFirstSubFeatureOfType<FeatureUseLimiter>()?.CanBeUsed(__instance, feature) == false)
                    {
                        continue;
                    }

                    __instance.IncrementSpecialFeatureUses(actionAffinity.Name);

                    //---- END ----
                    __instance.RefundActionUse(ActionType.Reaction);

                    var actionRefunded = __instance.ActionRefunded;
                    actionRefunded?.Invoke(__instance, ActionType.Reaction);
                }
            }

            var db = DatabaseRepository.GetDatabase<FeatureDefinition>();

            foreach (var key in __instance.usedSpecialFeatures.Keys)
            {
                if (db.TryGetElement(key, out var result)
                    && result is FeatureDefinitionAdditionalDamage { LimitedUsage: FeatureLimitedUsage.OncePerTurn })
                {
                    __instance.restoredFeatures.Add(result);
                }
            }

            foreach (var restoredFeature in __instance.restoredFeatures)
            {
                __instance.usedSpecialFeatures.Remove(restoredFeature.Name);
            }

            __instance.restoredFeatures.Clear();
            foreach (var proxy in rulesetCharacter.ControlledEffectProxies)
            {
                GameLocationCharacter.GetFromActor(proxy)?.StartBattleTurnOtherContender();
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.EndBattleTurn))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EndBattleTurn_Patch
    {
        [UsedImplicitly]
        public static void Prefix(GameLocationCharacter __instance)
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

    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.CanUseAtLeastOnPower))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CanUseAtLeastOnPower_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            GameLocationCharacter __instance,
            ActionType actionType,
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

            if (actionType == ActionType.Main &&
                rulesetCharacter.UsablePowers.Any(rulesetUsablePower =>
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
            ref ActionStatus __result,
            Id actionId,
            ActionScope scope,
            ActionStatus actionTypeStatus,
            // RulesetAttackMode optionalAttackMode,
            bool ignoreMovePoints)
        {
            var rulesetCharacter = __instance.RulesetCharacter;
            var flurryOfBlowActions = new[]
            {
                Id.FlurryOfBlows, Id.FlurryOfBlowsSwiftSteps, Id.FlurryOfBlowsUnendingStrikes
            };

            //PATCH: support for `IReplaceAttackWithCantrip` - allows `CastMain` action if character used attack
            ReplaceAttackWithCantrip.AllowCastDuringMainAttack(__instance, actionId, scope, ref __result);

            //PATCH: support for custom invocation action ids
            CustomActionIdContext.ProcessCustomActionIds(__instance, ref __result, actionId, scope,
                actionTypeStatus,
                ignoreMovePoints);

            ActionSwitching.CheckSpellcastingAvailability(__instance, actionId, scope, ref __result);

            //PATCH: support grapple
            GrappleContext.ValidateActionAvailability(__instance, ref __result, actionId);

            //PATCH: support `EnableMonkFocus2024`
            if (Main.Settings.EnableMonkFocus2024 &&
                __result == ActionStatus.CannotPerform &&
                (rulesetCharacter.RemainingKiPoints > 0 ||
                 rulesetCharacter.HasConditionOfType(WayOfShadow.ConditionCloakOfShadowsName)) &&
                __instance.GetActionTypeStatus(ActionType.Bonus) == ActionStatus.Available)
            {
                foreach (var flurryOfBlowAction in flurryOfBlowActions)
                {
                    if (actionId == flurryOfBlowAction &&
                        (!__instance.ActionPerformancesByType.TryGetValue(
                             ActionType.Bonus, out var actionPerformanceFilters) ||
                         actionPerformanceFilters.All(x => x.ForbiddenActions.All(y => y != flurryOfBlowAction))))
                    {
                        __result = ActionStatus.Available;
                    }
                }
            }

            //PATCH: support Swift Quiver spell interaction with Flurry of Blows
            if (__result == ActionStatus.Available &&
                flurryOfBlowActions.Contains(actionId) &&
                __instance.UsedSpecialFeatures.ContainsKey(SpellBuilders.SwiftQuiverAttackTag))
            {
                __result = ActionStatus.CannotPerform;
            }

            var traditionFreedomLevel =
                rulesetCharacter.GetSubclassLevel(DatabaseHelper.CharacterClassDefinitions.Monk, "TraditionFreedom");

            //BUGFIX: Hide other Flurry of Blows actions on Way of Freedom Monk as it levels up
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (actionId)
            {
                case Id.FlurryOfBlows when
                    traditionFreedomLevel >= 3:
                case Id.FlurryOfBlowsSwiftSteps when
                    traditionFreedomLevel >= 11:
                    __result = ActionStatus.Unavailable;
                    break;
            }

            //BUGFIX: if character can use only 1 of Main or Bonus - auto fail status if another type is used
            //Fixes Slow in various cases where we add extra attacks or other actions that manually consume main or bonus action
            var either = rulesetCharacter.GetSubFeaturesByType<IActionPerformanceProvider>()
                .Any(provider => provider.EitherMainOrBonus);

            if (!either)
            {
                return;
            }

            var action = ServiceRepository.GetService<IGameLocationActionService>().AllActionDefinitions[actionId];
            var actionType = action.actionType;
            var mainRanks = __instance.currentActionRankByType[ActionType.Main];
            var bonusRanks = __instance.currentActionRankByType[ActionType.Bonus];

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (actionType)
            {
                case ActionType.Bonus when mainRanks > 0:
                case ActionType.Main when bonusRanks > 0:
                    __result = ActionStatus.Unavailable;
                    break;
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
                    ValidateFeatureApplication.EnumerateActionPerformanceProviders)
                //PATCH: Support for action switching
                .ReplaceEnumerateFeaturesToBrowse<IAdditionalActionsProvider>(
                    "GameLocationCharacter.RefreshActionPerformances.ValidateAdditionalActionProviders",
                    ValidateFeatureApplication.EnumerateAdditionalActionProviders);
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
        public static void Prefix(GameLocationCharacter __instance, ActionType actionType)
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
        public static void Prefix(GameLocationCharacter __instance, ActionType actionType)
        {
            //PATCH: support for action switching
            ActionSwitching.RefundActionUse(__instance, actionType);

            //PATCH: fix reaction counted as not available even after refund
            if (actionType == ActionType.Reaction)
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
        public static void Prefix(GameLocationCharacter __instance)
        {
            _mainRank = __instance.currentActionRankByType[ActionType.Main];
            _bonusRank = __instance.currentActionRankByType[ActionType.Bonus];
            _mainAttacks = __instance.UsedMainAttacks;
            _bonusAttacks = __instance.UsedBonusAttacks;
        }

        [UsedImplicitly]
        public static void Postfix(
            GameLocationCharacter __instance,
            CharacterActionParams actionParams,
            ActionScope scope)
        {
            //PATCH: support for `IReplaceAttackWithCantrip`
            ReplaceAttackWithCantrip.AllowAttacksAfterCantrip(__instance, actionParams, scope);

            //PATCH: support for action switching
            ActionSwitching.CheckIfActionSwitched(
                __instance, actionParams, scope, _mainRank, _mainAttacks, _bonusRank, _bonusAttacks);

            //PATCH: only call refresh once after above methods are called
            //BUGFIX: vanilla doesn't refresh attack modes on free attacks
            if (scope == ActionScope.Battle &&
                actionParams.ActionDefinition.Id
                    is Id.AttackFree
                    or Id.AttackMain
                    or Id.AttackOff)
            {
                __instance.RulesetCharacter.RefreshAttackModes();
            }
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
                Id,
                bool,
                bool,
                bool,
                ReadyActionType,
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
        public static bool Prefix(GameLocationCharacter __instance, ref bool __result,
            Id actionId)
        {
            if (!CustomActionIdContext.IsToggleId(actionId))
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
        public static bool Prefix(GameLocationCharacter __instance, int damage)
        {
            //BUGFIX: don't roll if zero damage
            if (damage <= 0)
            {
                return false;
            }

            //PATCH: supports `IPreventRemoveConcentrationOnDamage`
            if (__instance.RulesetCharacter is not { } rulesetCharacter)
            {
                return true;
            }

            var hero = rulesetCharacter.GetOriginalHero();
            var concentratedSpell = hero?.ConcentratedSpell;

            if (concentratedSpell == null)
            {
                return true;
            }

            var shouldKeepConcentration = hero.GetSubFeaturesByType<IPreventRemoveConcentrationOnDamage>()
                .Any(x =>
                    x.SpellsThatShouldNotRollConcentrationCheckFromDamage(hero)
                        .Contains(concentratedSpell.SpellDefinition));

            return !shouldKeepConcentration;
        }

#if false
        [UsedImplicitly]
        public static void Postfix(
            GameLocationCharacter __instance,
            int damage,
            string damageType,
            bool stillConscious)
        {
            //PATCH: support for EffectWithConcentrationCheck
            ForceConcentrationCheck.ProcessConcentratedEffects(__instance, damage, damageType, stillConscious);
        }
#endif
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
            var rulesetCharacter = __instance.RulesetCharacter;

            rulesetCharacter.TryGetFirstConditionOfCategory(AttributeDefinitions.TagLightSensitivity,
                out var activeCondition);
            rulesetCharacter.RemoveAllConditionsOfCategory(AttributeDefinitions.TagLightSensitivity, false);

            for (var index = __instance.affectingLightEffects.Count - 1; index >= 0; --index)
            {
                __instance.affectingLightEffects[index].Terminate(false);
            }

            __instance.affectingLightEffects.Clear();

            var affinityFeatures = rulesetCharacter.GetLightAffinityFeatures();

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
                            var visibilityManager =
                                ServiceRepository.GetService<IGameLocationVisibilityService>() as
                                    GameLocationVisibilityManager;

                            if (visibilityManager)
                            {
                                illuminable.GetAllPositionsToCheck(visibilityManager.positionCache);

                                var gridAccessor = new GridAccessor(visibilityManager.positionCache[0]);

                                isExterior = gridAccessor.sector.IsExterior;
                            }
                        }

                        if (effectAndCondition.condition == CustomConditionsContext.LightSensitivity && !isExterior)
                        {
                            continue;
                        }
                    }
                    //END PATCH

                    if (effectAndCondition.condition)
                    {
                        newCondition = RulesetCondition.CreateActiveCondition(
                            rulesetCharacter.Guid,
                            effectAndCondition.condition,
                            DurationType.Irrelevant,
                            0,
                            TurnOccurenceType.StartOfTurn,
                            __instance.Guid,
                            rulesetCharacter.CurrentFaction.Name);

                        rulesetCharacter.AddConditionOfCategory(
                            AttributeDefinitions.TagLightSensitivity, newCondition, false);
                    }

                    if (effectAndCondition.effect != null)
                    {
                        var implementationService =
                            ServiceRepository.GetService<IRulesetImplementationService>();

                        __instance.affectingLightEffects.Add(implementationService
                            .InstantiateEffectEnvironment(
                                rulesetCharacter, effectAndCondition.effect, -1, 0, false, new BoxInt(),
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

            rulesetCharacter.RefreshAll();

            return false;
        }
    }

    //PATCH: fix vanilla issues that removes hero off stealth if within enemy perceived range on a surprise attack
    [HarmonyPatch(typeof(GameLocationCharacter), nameof(GameLocationCharacter.UpdateStealthStatus))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UpdateStealthStatus_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameLocationCharacter __instance)
        {
            if (!Main.Settings.KeepStealthOnHeroIfPerceivedDuringSurpriseAttack)
            {
                return true;
            }

            var service = ServiceRepository.GetService<IGameLocationBattleService>();

            if (service.HasBattleStarted)
            {
                return true;
            }

            __instance.wasPerceivedByFoes = __instance.isPerceivedByFoes;

            return false;
        }
    }
}
