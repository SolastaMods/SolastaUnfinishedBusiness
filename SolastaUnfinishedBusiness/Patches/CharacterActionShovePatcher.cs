using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionShovePatcher
{
    [HarmonyPatch(typeof(CharacterActionShove), nameof(CharacterActionShove.ExecuteImpl))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref IEnumerator __result, CharacterActionShove __instance)
        {
            __result = Execute(__instance);

            return false;
        }

        private static IEnumerator Execute(CharacterActionShove characterActionShove)
        {
            var actingCharacter = characterActionShove.ActingCharacter;
            var target = characterActionShove.ActionParams.TargetCharacters[0];
            var isTopple = characterActionShove.ActionParams.BoolParameter;
            var isSameSide = actingCharacter.Side == target.Side;
            var isIncapacitated = target.RulesetCharacter.IsIncapacitated;
            var abilityCheckData = new AbilityCheckData
            {
                AbilityCheckActionModifier = new ActionModifier(), Action = characterActionShove
            };
            var opponentAbilityCheckData = new AbilityCheckData
            {
                AbilityCheckActionModifier = new ActionModifier(), Action = characterActionShove
            };

            //BEGIN PATCH
            // original code
#if false
            bool success =
 isSameSide || isIncapacitated || CharacterActionShove.ResolveRolls(characterActionShove.ActingCharacter, target, characterActionShove.ActionId);
#endif
            yield return TryAlterOutcomeAttributeCheck.ResolveRolls(
                actingCharacter, target, characterActionShove.ActionId, abilityCheckData, opponentAbilityCheckData);

            var success =
                isSameSide ||
                isIncapacitated ||
                abilityCheckData.AbilityCheckRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess;
            //END PATCH

            if (isIncapacitated)
            {
                target.RulesetCharacter.ShoveAutomaticIncapacitated(
                    characterActionShove.ActionDefinition, target.RulesetCharacter);
            }

            actingCharacter.TurnTowards(target, false);

            yield return actingCharacter.EventSystem.UpdateMotionsAndWaitForEvent(
                GameLocationCharacterEventSystem.Event.RotationEnd);
            yield return actingCharacter.WaitForHitAnimation();

            actingCharacter.Shove(target, success);

            yield return actingCharacter.EventSystem.WaitForEvent(
                GameLocationCharacterEventSystem.Event.ShoveImpact);

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var shoveActionUsed = actionService.ShoveActionUsed;

            shoveActionUsed?.Invoke(
                actingCharacter, target, characterActionShove.ActionDefinition, success);

            if (success)
            {
                if (isTopple)
                {
                    if (target.SetProne(true))
                    {
                        yield return target.EventSystem.WaitForEvent(
                            GameLocationCharacterEventSystem.Event.ProneInAnimationEnd);
                    }
                }
                else
                {
                    var position = characterActionShove.ActionParams.Positions[0];

                    actionService.StopCharacterActions(target, CharacterAction.InterruptionType.ForcedMovement);

                    var actionParams = new CharacterActionParams(target, ActionDefinitions.Id.Pushed, position)
                    {
                        BoolParameter = false, BoolParameter4 = false
                    };

                    if (actionParams.TargetCharacters.Count <= 0)
                    {
                        actionParams.TargetCharacters.Add(actingCharacter);
                    }
                    else
                    {
                        actionParams.TargetCharacters[0] = actingCharacter;
                    }

                    actionParams.CanBeCancelled = false;
                    actionService.ExecuteAction(actionParams, null, false);
                }

                var rulesetCharacter = actingCharacter.RulesetCharacter;

                if (rulesetCharacter.UsablePowers.Count > 0)
                {
                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    foreach (var usablePower in rulesetCharacter.UsablePowers)
                    {
                        if (!rulesetCharacter.IsPowerOverriden(usablePower) &&
                            rulesetCharacter.GetRemainingUsesOfPower(usablePower) > 0 &&
                            usablePower.PowerDefinition.ActivationTime == ActivationTime.OnSuccessfulShoveAuto)
                        {
                            actingCharacter.MyExecuteActionSpendPower(usablePower, target);
                        }
                    }
                }
            }

            yield return actingCharacter.EventSystem.WaitForEvent(
                GameLocationCharacterEventSystem.Event.ShoveAnimationEnd);

            actingCharacter.TurnTowards(target);

            yield return actingCharacter.EventSystem.UpdateMotionsAndWaitForEvent(
                GameLocationCharacterEventSystem.Event.RotationEnd);

            //PATCH: support for Poisonous feat
            //Poison attacker if feat owner gets shoved
            var rulesetTargetHero = target.RulesetCharacter.GetOriginalHero();

            if (rulesetTargetHero != null &&
                rulesetTargetHero.TrainedFeats.Contains(OtherFeats.FeatPoisonousSkin))
            {
                yield return OtherFeats.PoisonTarget(target, actingCharacter);
            }

            //END PATCH

            var guard = 1000;

            while (actionService.IsActionInCurrentChains(ActionDefinitions.Id.FreeFall) && guard > 0)
            {
                --guard;
                yield return null;
            }

            if (guard <= 0)
            {
                Trace.LogError("Shove waited too long for FreeFall Action.");
            }
        }
    }
}
