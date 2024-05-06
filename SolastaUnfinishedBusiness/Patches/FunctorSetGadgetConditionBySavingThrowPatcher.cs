#if false
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FunctorSetGadgetConditionBySavingThrowPatcher
{
    //BUGFIX: vanilla only offers Bardic Inspiration during combat. This fixes that.
    //code is vanilla, cleaned up by Rider, except for BEGIN / END patch block
    [HarmonyPatch(typeof(FunctorSetGadgetConditionBySavingThrow),
        nameof(FunctorSetGadgetConditionBySavingThrow.Execute))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SelectCharacters_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            out IEnumerator __result,
            FunctorParametersDescription functorParameters)
        {
            __result = Execute(functorParameters);

            return false;
        }

        private static IEnumerator Execute(FunctorParametersDescription functorParameters)
        {
            if (functorParameters.ActingCharacters.Count == 0)
            {
                yield break;
            }

            var actingCharacter = functorParameters.ActingCharacters[0];

            yield return ExecuteSaveOnCharacter(functorParameters, actingCharacter);
        }

        private static IEnumerator ExecuteSaveOnCharacter(
            FunctorParametersDescription functorParameters,
            GameLocationCharacter actingCharacter)
        {
            var saveDC = functorParameters.AbilityCheck.DifficultyClass;

            if (functorParameters.VariableIntParameter1.behaviour != GadgetDefinitions.GadgetVariableBehaviour.Disabled)
            {
                saveDC = functorParameters.SourceGadget
                    .GetVariableOverride(functorParameters.VariableIntParameter1)
                    .valueInt;
            }

            var actionModifier = new ActionModifier();
            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
            var effectFormList = new List<EffectForm>();
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var abilityScoreName = functorParameters.AbilityCheck.AbilityScoreName;
            var gadgetDefinition = functorParameters.GadgetDefinition;

            implementationService.TryRollSavingThrow(
                null,
                RuleDefinitions.Side.Enemy,
                rulesetCharacter,
                actionModifier,
                false,
                true,
                abilityScoreName,
                saveDC,
                false,
                false,
                false,
                RuleDefinitions.FeatureSourceType.Base,
                effectFormList,
                null,
                null,
                string.Empty,
                gadgetDefinition,
                string.Empty,
                null,
                out var outcome,
                out var saveOutcomeDelta);

            var worldGadget = !functorParameters.BoolParameter
                ? functorParameters.TargetGadget
                : functorParameters.SourceGadget;

            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>()
                as GameLocationBattleManager;

            if (outcome == RuleDefinitions.RollOutcome.Failure)
            {
                battleManager!.GetBestParametersForBardicDieRoll(
                    actingCharacter,
                    out var bestDie,
                    out _,
                    out var sourceCondition,
                    out var forceMaxRoll,
                    out var advantage);

                if (bestDie > RuleDefinitions.DieType.D1 &&
                    actingCharacter.RulesetCharacter != null)
                {
                    // Is the die enough to overcome the failure?
                    if (RuleDefinitions.DiceMaxValue[(int)bestDie] >= Mathf.Abs(saveOutcomeDelta))
                    {
                        var reactionParams =
                            new CharacterActionParams(actingCharacter,
                                ActionDefinitions.Id.UseBardicInspiration)
                            {
                                IntParameter = (int)bestDie,
                                IntParameter2 = (int)RuleDefinitions.BardicInspirationUsageType.SavingThrow
                            };

                        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
                        var previousReactionCount = actionService.PendingReactionRequestGroups.Count;

                        actionService.ReactToUseBardicInspiration(reactionParams);

                        yield return battleManager.WaitForReactions(actingCharacter, actionService,
                            previousReactionCount);

                        if (reactionParams.ReactionValidated)
                        {
                            // Now we have a shot at succeeding on the ability check
                            var roll = actingCharacter.RulesetCharacter.RollBardicInspirationDie(
                                sourceCondition, saveOutcomeDelta, forceMaxRoll, advantage);

                            if (roll >= Mathf.Abs(saveOutcomeDelta))
                            {
                                // The roll is now a success!
                                outcome = RuleDefinitions.RollOutcome.Success;
                            }
                        }
                    }
                }
            }

            //PATCH: support for `ITryAlterOutcomeAttributeCheck`
            // foreach (var tryAlterOutcomeSavingThrow in TryAlterOutcomeSavingThrow.Handler(
            //              battleManager, null, actingCharacter, null, new ActionModifier(), true, false))
            // {
            //     yield return tryAlterOutcomeSavingThrow;
            // }
                
            //END PATCH
            
            if (outcome is RuleDefinitions.RollOutcome.Success or RuleDefinitions.RollOutcome.CriticalSuccess)
            {
                var conditionIndex = Array.IndexOf(worldGadget.ConditionChoices(),
                    functorParameters.TargetConditionState.name);
                worldGadget.GameGadget.SetCondition(conditionIndex, functorParameters.TargetConditionState.state,
                    functorParameters.ActingCharacters);
                yield break;
            }

            // ReSharper disable once InvertIf
            if (functorParameters.HasAlternateTargetConditionState)
            {
                var conditionIndex = Array.IndexOf(worldGadget.ConditionChoices(),
                    functorParameters.AlternateTargetConditionState.name);
                worldGadget.GameGadget.SetCondition(conditionIndex,
                    functorParameters.AlternateTargetConditionState.state,
                    functorParameters.ActingCharacters);
            }
        }
    }
}
#endif
