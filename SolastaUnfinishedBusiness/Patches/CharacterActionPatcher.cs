using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionPatcher
{
    [HarmonyPatch(typeof(CharacterAction), nameof(CharacterAction.InstantiateAction))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class InstantiateAction_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CharacterActionParams actionParams, ref CharacterAction __result)
        {
            //PATCH: creates action objects for actions defined in mod

            // required when interacting with some game inanimate objects (like minor gates)
            if (actionParams == null)
            {
                return true;
            }

            var name = CharacterAction.GetTypeName(actionParams);

            //Actions defined in mod will be non-null, actions from base game will be null
            var type = Type.GetType(name);

            if (type == null)
            {
                return true;
            }

            __result = Activator.CreateInstance(type, actionParams) as CharacterAction;

            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterAction), nameof(CharacterAction.Execute))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Execute_Patch
    {
        private static bool ActionShouldKeepConcentration(CharacterAction action)
        {
            var isProtectedPower =
                action is CharacterActionUsePower or CharacterActionSpendPower or CharacterActionDoNothing &&
                action.ActionParams is { UsablePower: not null } &&
                action.ActionParams.UsablePower.PowerDefinition
                    .HasSubFeatureOfType<IPreventRemoveConcentrationOnPowerUse>();

            return isProtectedPower;
        }

        private static void FixAlwaysConsumeMainActionOnBattleSurprise(CharacterAction action)
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (action.ActionId)
            {
                case ActionDefinitions.Id.PowerMain when
                    action.ActionParams.activeEffect is RulesetEffectPower effectPower &&
                    effectPower.PowerDefinition.ActivationTime != ActivationTime.Action:
                {
                    var actionType = effectPower.ActionType;
                    var allActionDefinitions = ServiceRepository
                        .GetService<IGameLocationActionService>().AllActionDefinitions;

                    action.ActionParams.actionDefinition = actionType switch
                    {
                        ActionDefinitions.ActionType.Bonus =>
                            allActionDefinitions[ActionDefinitions.Id.PowerBonus],
                        ActionDefinitions.ActionType.NoCost =>
                            allActionDefinitions[ActionDefinitions.Id.PowerNoCost],
                        _ => action.ActionParams.actionDefinition
                    };

                    break;
                }
                case ActionDefinitions.Id.CastMain when
                    action.ActionParams.activeEffect is RulesetEffectSpell effectSpell &&
                    effectSpell.SpellDefinition.ActivationTime != ActivationTime.Action:
                {
                    var actionType = effectSpell.ActionType;
                    var allActionDefinitions = ServiceRepository
                        .GetService<IGameLocationActionService>().AllActionDefinitions;

                    action.ActionParams.actionDefinition = actionType switch
                    {
                        ActionDefinitions.ActionType.Bonus =>
                            allActionDefinitions[ActionDefinitions.Id.CastBonus],
                        ActionDefinitions.ActionType.NoCost =>
                            allActionDefinitions[ActionDefinitions.Id.CastNoCost],
                        _ => action.ActionParams.actionDefinition
                    };

                    break;
                }
            }
        }

        [UsedImplicitly]
        public static void Prefix(CharacterAction __instance)
        {
            var actingCharacter = __instance.ActingCharacter;

            //BUGFIX: vanilla always consume a main action on battle surprise phase even if a bonus power or spell
            if (Gui.Battle != null &&
                Gui.Battle.CurrentRound == 1 &&
                Gui.Battle.InitiativeSortedContenders.Count > 0 &&
                Gui.Battle.ActiveContender == Gui.Battle.InitiativeSortedContenders[0])
            {
                FixAlwaysConsumeMainActionOnBattleSurprise(__instance);
            }

            //PATCH: support `IPreventRemoveConcentrationOnPowerUse`
            if (ActionShouldKeepConcentration(__instance))
            {
                actingCharacter.UsedSpecialFeatures.TryAdd(CharacterActionExtensions.ShouldKeepConcentration, 0);
            }
            else
            {
                actingCharacter.UsedSpecialFeatures.Remove(CharacterActionExtensions.ShouldKeepConcentration);
            }

            switch (__instance)
            {
                case CharacterActionReady:
                    CustomReactionsContext.ReadReadyActionPreferredCantrip(__instance.actionParams);
                    break;

                case CharacterActionMoveStepBase characterActionMoveStepBase:
                    OtherFeats.NotifyFeatStealth(characterActionMoveStepBase);
                    break;
            }
        }

        [UsedImplicitly]
        public static IEnumerator Postfix(IEnumerator values, CharacterAction __instance)
        {
            while (values.MoveNext())
            {
                yield return values.Current;
            }

            //PATCH: support for `IActionFinishedByMe`
            var actingCharacter = __instance.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            foreach (var actionFinished in rulesetCharacter
                         .GetEffectControllerOrSelf()
                         .GetSubFeaturesByType<IActionFinishedByMe>())
            {
                yield return actionFinished.OnActionFinishedByMe(__instance);
            }

            switch (__instance)
            {
                case CharacterActionMoveStepBase:
                case CharacterActionMagicEffect { isPostSpecialMove: true }:
                {
                    //PATCH: support for Polearm Expert AoO. processes saved movement to trigger AoO when appropriate
                    var extraAoOEvents = AttacksOfOpportunity.ProcessOnCharacterMoveEnd(actingCharacter);

                    while (extraAoOEvents.MoveNext())
                    {
                        yield return extraAoOEvents.Current;
                    }

                    //PATCH: support for MovementTracker
                    MovementTracker.CleanMovementCache();

                    //PATCH: set cursor to dirty and reprocess valid positions if ally was moved by Gambit or Warlord, or enemy moved by other means
                    if (!actingCharacter.IsMyTurn())
                    {
                        var cursorService = ServiceRepository.GetService<ICursorService>();
                        var cursorLocationBattleFriendlyTurn =
                            cursorService.AllCursors.OfType<CursorLocationBattleFriendlyTurn>().First();

                        if (!cursorLocationBattleFriendlyTurn.Active)
                        {
                            yield break;
                        }

                        cursorLocationBattleFriendlyTurn.dirty = true;
                        cursorLocationBattleFriendlyTurn.ComputeValidDestinations();
                    }

                    break;
                }

                //PATCH: support for Circle of the Wildfire cauterizing flames, and grapple scenarios
                case CharacterActionPushed:
                case CharacterActionPushedCustom:
                {
                    yield return CircleOfTheWildfire.HandleCauterizingFlamesBehavior(actingCharacter);

                    GrappleContext.ValidateGrappleAfterForcedMove(actingCharacter);
                    break;
                }
                case CharacterActionShove:
                {
                    var target = __instance.ActionParams.TargetCharacters[0];

                    yield return CircleOfTheWildfire.HandleCauterizingFlamesBehavior(target);

                    GrappleContext.ValidateGrappleAfterForcedMove(target);
                    break;
                }
            }

            //PATCH: support for Old Tactics feat
            yield return MeleeCombatFeats.HandleFeatOldTactics(__instance);

            //PATCH: support for Official Flanking Rules
            if (Main.Settings.UseOfficialFlankingRules)
            {
                FlankingAndHigherGround.ClearFlankingDeterminationCache();
            }

            //PATCH: support for `ExtraConditionInterruption.UsesBonusAction`
            if (__instance.ActionType == ActionDefinitions.ActionType.Bonus)
            {
                rulesetCharacter.ProcessConditionsMatchingInterruption(
                    (ConditionInterruption)ExtraConditionInterruption.UsesBonusAction);
            }
        }
    }

    [HarmonyPatch(typeof(CharacterAction), nameof(CharacterAction.ApplyStealthBreakerBehavior))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ApplyStealthBreakerBehavior_Patch
    {
        internal static bool ShouldBanter;

        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var computeStealthBreakValueMethod = typeof(GameLocationCharacter).GetMethod("ComputeStealthBreak");
            var myComputeStealthBreakValueMethod =
                new Func<GameLocationCharacter, bool, ActionModifier, List<GameLocationCharacter>, CharacterAction,
                    bool>(
                    ComputeStealthBreak).Method;

            return instructions
                .ReplaceCall(computeStealthBreakValueMethod,
                    1, "CharacterAction.ApplyStealthBreakerBehavior",
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, myComputeStealthBreakValueMethod));
        }

        private static bool ComputeStealthBreak(
            GameLocationCharacter __instance,
            bool roll,
            ActionModifier actionModifier,
            List<GameLocationCharacter> detectorsWithAdvantage,
            CharacterAction action)
        {
            //PATCH: fix vanilla issues that removes hero off stealth if within enemy perceived range on a surprise attack
            if (Main.Settings.KeepStealthOnHeroIfPerceivedDuringSurpriseAttack &&
                Gui.Battle != null &&
                Gui.Battle.CurrentRound == 1 &&
                Gui.Battle.InitiativeSortedContenders.Count > 0 &&
                __instance == Gui.Battle.InitiativeSortedContenders[0])
            {
                __instance.wasPerceivedByFoes = false; // this is key to force below to recalculate
                __instance.UpdateStealthStatus();
            }
            //END PATCH

            ShouldBanter = true;

            switch (action)
            {
                case CharacterActionAttack:
                {
                    if ((action.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess
                         && Main.Settings.StealthBreaksWhenAttackHits)
                        || (action.AttackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure
                            && Main.Settings.StealthBreaksWhenAttackMisses))
                    {
                        ShouldBanter = false;
                        roll = false;
                    }

                    break;
                }
                case CharacterActionCastSpell actionCastSpell:
                {
                    var activeSpell = actionCastSpell.ActiveSpell;
                    var spell = activeSpell.SpellDefinition;

                    if (spell.EffectDescription.RangeType
                        is RangeType.Touch
                        or RangeType.MeleeHit
                        or RangeType.RangeHit)
                    {
                        if ((action.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess
                             && Main.Settings.StealthBreaksWhenAttackHits)
                            || (action.AttackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure
                                && Main.Settings.StealthBreaksWhenAttackMisses))
                        {
                            ShouldBanter = false;
                            roll = false;
                        }
                    }
                    else if (spell.EffectDescription.TargetSide != Side.Ally)
                    {
                        var isSubtle = activeSpell.MetamagicOption ==
                                       DatabaseHelper.MetamagicOptionDefinitions.MetamagicSubtleSpell;

                        if (Main.Settings.StealthDoesNotBreakWithSubtle
                            && isSubtle
                            && spell.MaterialComponentType == MaterialComponentType.None)
                        {
                            return false;
                        }

                        if ((spell.MaterialComponentType != MaterialComponentType.None &&
                             Main.Settings.StealthBreaksWhenCastingMaterial)
                            || (spell.SomaticComponent && Main.Settings.StealthBreaksWhenCastingSomatic && !isSubtle)
                            || (spell.VerboseComponent && Main.Settings.StealthBreaksWhenCastingVerbose && !isSubtle))
                        {
                            ShouldBanter = false;
                            roll = false;
                        }
                    }

                    break;
                }
                case CharacterActionSpendPower:
                case CharacterActionUsePower:
                {
                    if (action.ActionParams.RulesetEffect.EffectDescription.RangeType
                        is RangeType.Touch
                        or RangeType.MeleeHit
                        or RangeType.RangeHit)
                    {
                        if ((action.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess
                             && Main.Settings.StealthBreaksWhenAttackHits)
                            || (action.AttackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure
                                && Main.Settings.StealthBreaksWhenAttackMisses))
                        {
                            ShouldBanter = false;
                            roll = false;
                        }
                    }

                    break;
                }
            }

            return __instance.ComputeStealthBreak(roll, actionModifier, detectorsWithAdvantage);
        }
    }
}
