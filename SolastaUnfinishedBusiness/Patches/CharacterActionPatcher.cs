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

        [UsedImplicitly]
        public static void Prefix(CharacterAction __instance)
        {
            //BUGFIX: vanilla always consume a main action on battle surprise phase even if a bonus power or spell
            if (Gui.Battle != null &&
                Gui.Battle.CurrentRound == 1 &&
                Gui.Battle.InitiativeSortedContenders.Count > 0 &&
                Gui.Battle.ActiveContender == Gui.Battle.InitiativeSortedContenders[0])
            {
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (__instance.ActionId)
                {
                    case ActionDefinitions.Id.PowerMain when
                        __instance.ActionParams.activeEffect is RulesetEffectPower effectPower &&
                        effectPower.PowerDefinition.ActivationTime != ActivationTime.Action:
                    {
                        var actionType = effectPower.ActionType;
                        var allActionDefinitions = ServiceRepository
                            .GetService<IGameLocationActionService>().AllActionDefinitions;

                        __instance.ActionParams.actionDefinition = actionType switch
                        {
                            ActionDefinitions.ActionType.Bonus =>
                                allActionDefinitions[ActionDefinitions.Id.PowerBonus],
                            ActionDefinitions.ActionType.NoCost =>
                                allActionDefinitions[ActionDefinitions.Id.PowerNoCost],
                            _ => __instance.ActionParams.actionDefinition
                        };

                        break;
                    }
                    case ActionDefinitions.Id.CastMain when
                        __instance.ActionParams.activeEffect is RulesetEffectSpell effectSpell &&
                        effectSpell.SpellDefinition.ActivationTime != ActivationTime.Action:
                    {
                        var actionType = effectSpell.ActionType;
                        var allActionDefinitions = ServiceRepository
                            .GetService<IGameLocationActionService>().AllActionDefinitions;

                        __instance.ActionParams.actionDefinition = actionType switch
                        {
                            ActionDefinitions.ActionType.Bonus =>
                                allActionDefinitions[ActionDefinitions.Id.CastBonus],
                            ActionDefinitions.ActionType.NoCost =>
                                allActionDefinitions[ActionDefinitions.Id.CastNoCost],
                            _ => __instance.ActionParams.actionDefinition
                        };

                        break;
                    }
                }
            }

            //PATCH: support `IPreventRemoveConcentrationOnPowerUse`
            if (ActionShouldKeepConcentration(__instance))
            {
                __instance.ActingCharacter.UsedSpecialFeatures.TryAdd(
                    CharacterActionExtensions.ShouldKeepConcentration, 0);
            }
            else
            {
                __instance.ActingCharacter.UsedSpecialFeatures.Remove(
                    CharacterActionExtensions.ShouldKeepConcentration);
            }

            switch (__instance)
            {
#if false
                case CharacterActionCastSpell or CharacterActionSpendSpellSlot:
                    //PATCH: Hold the state of the SHIFT key on bool 5 to determine which slot to use on MC Warlock
                    var isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                    __instance.actionParams.BoolParameter5 = isShiftPressed;
                    break;
#endif
                case CharacterActionReady:
                    CustomReactionsContext.ReadReadyActionPreferredCantrip(__instance.actionParams);
                    break;

                case CharacterActionSpendPower spendPower:
                    PowerBundle.SpendBundledPowerIfNeeded(spendPower);
                    break;

                case CharacterActionAttack characterActionAttack:
                    Global.CurrentAttackAction.Push(characterActionAttack);
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

            //PATCH: clear flanking rules determination cache on every action end
            if (Main.Settings.UseOfficialFlankingRules && Gui.Battle != null)
            {
                FlankingAndHigherGround.ClearFlankingDeterminationCache();
            }

            var actingCharacter = __instance.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            if (rulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
            {
                //PATCH: support for `IActionFinishedByMe`
                foreach (var actionFinished in rulesetCharacter
                             .GetEffectControllerOrSelf()
                             .GetSubFeaturesByType<IActionFinishedByMe>())
                {
                    yield return actionFinished.OnActionFinishedByMe(__instance);
                }
            }

            //PATCH: support for `IActionFinishedByContender`
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var contenders =
                (Gui.Battle?.AllContenders ??
                 locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters))
                .ToList();
            foreach (var target in contenders)
            {
                var rulesetTarget = target.RulesetCharacter;

                foreach (var actionFinishedByContender in rulesetTarget
                                .GetSubFeaturesByType<IActionFinishedByContender>())
                {
                    yield return actionFinishedByContender.OnActionFinishedByContender(__instance, target);
                }
            }


            if (__instance.ActionType == ActionDefinitions.ActionType.Bonus)
            {
                rulesetCharacter.ProcessConditionsMatchingInterruption(
                    (ConditionInterruption)ExtraConditionInterruption.UsesBonusAction);
            }

            // ReSharper disable once InvertIf
            if (__instance is CharacterActionAttack actionAttack)
            {
                Global.CurrentAttackAction.Pop();

                if (actionAttack.ActionParams.AttackMode != null)
                {
                    rulesetCharacter.ProcessConditionsMatchingInterruption(
                        (ConditionInterruption)ExtraConditionInterruption.AttacksWithWeaponOrUnarmed);
                }
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
                case CharacterActionAttack actionAttack:
                {
                    if ((actionAttack.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess
                         && Main.Settings.StealthBreaksWhenAttackHits)
                        || (actionAttack.AttackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure
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
                        if ((actionCastSpell.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess
                             && Main.Settings.StealthBreaksWhenAttackHits)
                            || (actionCastSpell.AttackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure
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
                case CharacterActionSpendPower actionSpendPower:
                {
                    var activePower = actionSpendPower.activePower;
                    var power = activePower.PowerDefinition;

                    if (power.EffectDescription.RangeType
                        is RangeType.Touch
                        or RangeType.MeleeHit
                        or RangeType.RangeHit)
                    {
                        if ((actionSpendPower.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess
                             && Main.Settings.StealthBreaksWhenAttackHits)
                            || (actionSpendPower.AttackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure
                                && Main.Settings.StealthBreaksWhenAttackMisses))
                        {
                            ShouldBanter = false;
                            roll = false;
                        }
                    }

                    break;
                }

                case CharacterActionUsePower actionUsePower:
                {
                    var activePower = actionUsePower.activePower;
                    var power = activePower.PowerDefinition;

                    if (power.EffectDescription.RangeType
                        is RangeType.Touch
                        or RangeType.MeleeHit
                        or RangeType.RangeHit)
                    {
                        if ((actionUsePower.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess
                             && Main.Settings.StealthBreaksWhenAttackHits)
                            || (actionUsePower.AttackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure
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
