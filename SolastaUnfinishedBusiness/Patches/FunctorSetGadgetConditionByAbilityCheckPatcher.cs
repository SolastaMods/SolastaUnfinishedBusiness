using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FunctorSetGadgetConditionByAbilityCheckPatcher
{
    //BUGFIX: vanilla only offers Bardic Inspiration during combat. This fixes that.
    //code is vanilla, cleaned up by Rider, except for BEGIN / END patch block
    [HarmonyPatch(typeof(FunctorSetGadgetConditionByAbilityCheck),
        nameof(FunctorSetGadgetConditionByAbilityCheck.Execute))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Execute_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref IEnumerator __result, FunctorParametersDescription functorParameters)
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

            if (functorParameters.AbilityCheck.ProficiencyName == SkillDefinitions.Perception)
            {
                foreach (var actingCharacter in functorParameters.ActingCharacters.Distinct().ToList())
                {
                    if (actingCharacter.RulesetCharacter.CanRevealHiddenObjects())
                    {
                        var worldGadget = !functorParameters.BoolParameter
                            ? functorParameters.TargetGadget
                            : functorParameters.SourceGadget;
                        var conditionIndex = Array.IndexOf(worldGadget.ConditionChoices(),
                            functorParameters.TargetConditionState.name);
                        worldGadget.GameGadget.SetCondition(conditionIndex,
                            functorParameters.TargetConditionState.state, functorParameters.ActingCharacters);
                        break;
                    }

                    yield return ExecuteCheckOnCharacter(functorParameters, actingCharacter);
                }
            }
            else
            {
                var actingCharacter = functorParameters.ActingCharacters[0];

                yield return ExecuteCheckOnCharacter(functorParameters, actingCharacter);
            }
        }

        private static IEnumerator ExecuteCheckOnCharacter(
            FunctorParametersDescription functorParameters,
            GameLocationCharacter actingCharacter)
        {
            var isPerceptionCheck = functorParameters.AbilityCheck.ProficiencyName == SkillDefinitions.Perception;
            var ulongList = !isPerceptionCheck || !actingCharacter.AlertPerception
                ? functorParameters.FailedCharacters
                : functorParameters.FailedCharactersAlternate;
            var service = ServiceRepository.GetService<IGameSettingsService>();

            if (ulongList.Contains(actingCharacter.Guid) && service is not { AuthorizeRetryOnGadgets: true })
            {
                yield break;
            }

            var checkDC = functorParameters.AbilityCheck.DifficultyClass;

            if (functorParameters.VariableIntParameter1.behaviour != GadgetDefinitions.GadgetVariableBehaviour.Disabled)
            {
                checkDC = functorParameters.SourceGadget
                    .GetVariableOverride(functorParameters.VariableIntParameter1)
                    .valueInt;
            }

            var minRoll = functorParameters.AbilityCheck.MinRoll;

            if (functorParameters.VariableIntParameter2.behaviour != GadgetDefinitions.GadgetVariableBehaviour.Disabled)
            {
                minRoll = functorParameters.SourceGadget
                    .GetVariableOverride(functorParameters.VariableIntParameter2)
                    .valueInt;
            }

            var actionModifier = new ActionModifier();
            var passive = false;

            if (isPerceptionCheck && !functorParameters.AbilityCheck.IgnorePassive)
            {
                passive = !actingCharacter.AlertPerception;

                if (functorParameters.SourceGadget.GameGadget.LightingInformationPerFlow.TryGetValue(
                        functorParameters.ListenerIndex, out var lightingInformation))
                {
                    var num = float.MaxValue;

                    foreach (var int3 in lightingInformation.perceptionBox.EnumerateAllPositionsWithin())
                    {
                        num = Mathf.Min(num, (actingCharacter.LocationPosition - int3).magnitude);
                    }

                    actingCharacter.ComputeLightingModifierForLightingState(
                        num, lightingInformation.lightingState, actionModifier);
                }
            }

            var rollOutcome = RuleDefinitions.RollOutcome.Neutral;

            if (isPerceptionCheck && service is { AutoDetectTraps: true })
            {
                rollOutcome = RuleDefinitions.RollOutcome.Success;
            }

            if (((passive ? 0 : !functorParameters.AbilityCheck.IgnorePassive ? 1 : 0) & (isPerceptionCheck ? 1 : 0)) !=
                0 &&
                rollOutcome == RuleDefinitions.RollOutcome.Neutral)
            {
                var num = 10 + actingCharacter.RulesetCharacter.ComputeBaseAbilityCheckBonus(
                    functorParameters.AbilityCheck.AbilityScoreName, null,
                    functorParameters.AbilityCheck.ProficiencyName);
                actingCharacter.PrepareActionModifier(functorParameters.AbilityCheck.AbilityScoreName,
                    functorParameters.AbilityCheck.ProficiencyName, functorParameters.AbilityCheck.Affinity,
                    actionModifier, 8);

                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (RuleDefinitions.ComputeAdvantage(actionModifier.AbilityCheckAdvantageTrends))
                {
                    case RuleDefinitions.AdvantageType.Advantage:
                        num += 5;
                        break;
                    case RuleDefinitions.AdvantageType.Disadvantage:
                        num -= 5;
                        break;
                }

                if (num >= checkDC)
                {
                    rollOutcome = RuleDefinitions.RollOutcome.Success;
                }
            }

            if (rollOutcome == RuleDefinitions.RollOutcome.Neutral)
            {
                var abilityCheckRoll = actingCharacter.RollAbilityCheck(
                    functorParameters.AbilityCheck.AbilityScoreName,
                    functorParameters.AbilityCheck.ProficiencyName,
                    checkDC,
                    functorParameters.AbilityCheck.Affinity,
                    actionModifier,
                    passive,
                    minRoll,
                    out rollOutcome,
                    out var successDelta,
                    !functorParameters.AbilityCheck.Silent,
                    !functorParameters.AbilityCheck.Silent);

                //PATCH: support for Bardic Inspiration roll off battle and ITryAlterOutcomeAttributeCheck
                var abilityCheckData = new AbilityCheckData
                {
                    AbilityCheckRoll = abilityCheckRoll,
                    AbilityCheckRollOutcome = rollOutcome,
                    AbilityCheckSuccessDelta = successDelta,
                    AbilityCheckActionModifier = actionModifier
                };

                yield return TryAlterOutcomeAttributeCheck
                    .HandleITryAlterOutcomeAttributeCheck(actingCharacter, abilityCheckData);

                rollOutcome = abilityCheckData.AbilityCheckRollOutcome;
            }

            var worldGadget = !functorParameters.BoolParameter
                ? functorParameters.TargetGadget
                : functorParameters.SourceGadget;

            if (rollOutcome is RuleDefinitions.RollOutcome.Success or RuleDefinitions.RollOutcome.CriticalSuccess)
            {
                var conditionIndex = Array.IndexOf(
                    worldGadget.ConditionChoices(), functorParameters.TargetConditionState.name);

                worldGadget.GameGadget.SetCondition(
                    conditionIndex,
                    functorParameters.TargetConditionState.state, functorParameters.ActingCharacters);

                yield break;
            }

            // ReSharper disable once InvertIf
            if (!ulongList.Contains(actingCharacter.Guid))
            {
                ulongList.Add(actingCharacter.Guid);

                if (!functorParameters.HasAlternateTargetConditionState)
                {
                    yield break;
                }

                var conditionIndex = Array.IndexOf(
                    worldGadget.ConditionChoices(), functorParameters.AlternateTargetConditionState.name);

                worldGadget.GameGadget.SetCondition(
                    conditionIndex,
                    functorParameters.AlternateTargetConditionState.state, functorParameters.ActingCharacters);
            }
        }
    }
}
