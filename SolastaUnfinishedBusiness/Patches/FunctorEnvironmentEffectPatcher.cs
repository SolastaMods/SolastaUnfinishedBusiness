using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using TA;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public class FunctorEnvironmentEffectPatcher
{
    //BUGFIX: vanilla only offers Bardic Inspiration during combat. This fixes that.
    //code is vanilla, cleaned up by Rider, except for BEGIN / END patch block
    [HarmonyPatch(typeof(FunctorEnvironmentEffect), nameof(FunctorEnvironmentEffect.Execute))]
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

        private static IEnumerator TriggerEnvironmentEffectPosition(
            GameLocationEnvironmentManager environmentManager,
            GameLocationCharacter triggerer,
            EnvironmentEffectDefinition environmentEffectDefinition,
            int3 position,
            int savingThrowDCOverride,
            int addDice,
            GameGadget sourceGadget,
            bool isGlobalEnvironmentEffect)
        {
            var effectEnvironment = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectEnvironment(triggerer?.RulesetCharacter, environmentEffectDefinition,
                    savingThrowDCOverride, addDice, false, new BoxInt(),
                    triggerer?.LocationPosition ?? position, sourceGadget.UniqueNameId,
                    isGlobalEnvironmentEffect);
            if (effectEnvironment.EffectDescription.TargetType == RuleDefinitions.TargetType.KindredSpirit)
            {
                Trace.LogError("Trying to trigger an effect targeting only the kindred spirit, not yet implemented!");
            }
            else
            {
                var origin = new Vector3();

                environmentManager.affectedCharacters.Clear();

                if (effectEnvironment.EffectDescription.TargetType == RuleDefinitions.TargetType.Self)
                {
                    environmentManager.affectedCharacters.Add(triggerer);
                    origin = ServiceRepository.GetService<IGameLocationPositioningService>()
                        .GetWorldPositionFromGridPosition(position);
                }
                else if (effectEnvironment.EffectDescription.IsAoE)
                {
                    var fromGridPosition = ServiceRepository.GetService<IGameLocationPositioningService>()
                        .GetWorldPositionFromGridPosition(triggerer?.LocationPosition ?? position);
                    var direction = new Vector3();
                    var shapeType = effectEnvironment.EffectDescription.ShapeType;
                    var service = ServiceRepository.GetService<IGameLocationTargetingService>();

                    service.ComputeTargetingParameters(
                        fromGridPosition,
                        null,
                        new int3(), shapeType,
                        effectEnvironment.EffectDescription.RangeType,
                        ref origin, ref direction);
                    service.ComputeTargetsOfAreaOfEffect(
                        origin,
                        direction,
                        new Vector3(),
                        shapeType,
                        RuleDefinitions.Side.Neutral,
                        environmentEffectDefinition.EffectDescription,
                        effectEnvironment.ComputeTargetParameter(),
                        effectEnvironment.ComputeTargetParameter2(),
                        environmentManager.affectedCharacters,
                        false,
                        groundOnly: effectEnvironment.EffectDescription.AffectOnlyGround);
                }

                environmentManager.EnvironmentEffectTriggered?.Invoke(effectEnvironment, origin);

                yield return TriggerEnvironmentEffect(environmentManager, effectEnvironment);
            }
        }

        private static IEnumerator TriggerEnvironmentEffectBox(
            GameLocationEnvironmentManager environmentManager,
            GameLocationCharacter triggerer,
            EnvironmentEffectDefinition environmentEffectDefinition,
            BoxInt boxTargetArea,
            int savingThrowOverride,
            int addDice,
            GameGadget sourceGadget,
            bool isGlobalEnvironmentEffect)
        {
            var effectEnvironment = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectEnvironment(triggerer?.RulesetCharacter, environmentEffectDefinition,
                    savingThrowOverride, addDice, true, boxTargetArea, new int3(), sourceGadget.UniqueNameId,
                    isGlobalEnvironmentEffect);

            environmentManager.affectedCharacters.Clear();

            ServiceRepository.GetService<IGameLocationTargetingService>().ComputeTargetsOfAreaOfEffect(
                boxTargetArea,
                RuleDefinitions.Side.Neutral,
                environmentEffectDefinition.EffectDescription,
                effectEnvironment.ComputeTargetParameter(),
                effectEnvironment.ComputeTargetParameter2(),
                environmentManager.affectedCharacters);

            var origin = boxTargetArea.Center - new Vector3(0.0f, boxTargetArea.Size.y / 2f, 0.0f);

            environmentManager.EnvironmentEffectTriggered?.Invoke(effectEnvironment, origin);

            yield return TriggerEnvironmentEffect(environmentManager, effectEnvironment);
        }

        private static IEnumerator TriggerEnvironmentEffect(
            GameLocationEnvironmentManager environmentManager,
            RulesetEffectEnvironment activeEnvironmentEffect)
        {
            var service = ServiceRepository.GetService<IRulesetImplementationService>();

            service.ClearDamageFormsByIndex();

            for (var index = 0; index < environmentManager.affectedCharacters.Count; ++index)
            {
                var affectedCharacter = environmentManager.affectedCharacters[index];
                var recurrentEffect = activeEnvironmentEffect.EffectDescription.RecurrentEffect;

                if (recurrentEffect == RuleDefinitions.RecurrentEffect.No ||
                    (recurrentEffect & RuleDefinitions.RecurrentEffect.OnActivation) !=
                    RuleDefinitions.RecurrentEffect.No)
                {
                    var actionModifier = new ActionModifier();
                    var rolledSaveThrow = activeEnvironmentEffect.TryRollSavingThrow(
                        null,
                        RuleDefinitions.Side.Neutral,
                        affectedCharacter.RulesetActor,
                        actionModifier,
                        activeEnvironmentEffect.EffectDescription.EffectForms,
                        true,
                        out var saveOutcome,
                        out var saveOutcomeDelta);

                    if (rolledSaveThrow)
                    {
                        var battleManager = ServiceRepository.GetService<IGameLocationBattleService>()
                            as GameLocationBattleManager;
                        var hasBorrowedLuck = affectedCharacter.RulesetActor.HasConditionOfTypeOrSubType(
                            RuleDefinitions.ConditionBorrowedLuck);
                        var savingThrowData = new SavingThrowData
                        {
                            SaveActionModifier = actionModifier,
                            SaveOutcome = saveOutcome,
                            SaveOutcomeDelta = saveOutcomeDelta,
                            SaveDC = RulesetActorExtensions.SaveDC,
                            SaveBonusAndRollModifier = RulesetActorExtensions.SaveBonusAndRollModifier,
                            SavingThrowAbility = RulesetActorExtensions.SavingThrowAbility,
                            SourceDefinition = activeEnvironmentEffect.SourceDefinition,
                            EffectDescription = activeEnvironmentEffect.EffectDescription,
                            Title = activeEnvironmentEffect.SourceDefinition.FormatTitle(),
                            Action = null
                        };

                        yield return TryAlterOutcomeSavingThrow.Handler(
                            battleManager,
                            null,
                            affectedCharacter,
                            savingThrowData,
                            hasBorrowedLuck,
                            activeEnvironmentEffect.EffectDescription);
                    }

                    var formsParams = new RulesetImplementationDefinitions.ApplyFormsParams();

                    formsParams.FillSourceAndTarget(null, affectedCharacter.RulesetActor);
                    formsParams.FillFromActiveEffect(activeEnvironmentEffect);
                    formsParams.FillSpecialParameters(
                        rolledSaveThrow, activeEnvironmentEffect.AddDice, 0, 0, 0,
                        actionModifier, saveOutcome, saveOutcomeDelta, false, index,
                        environmentManager.affectedCharacters.Count, null);
                    formsParams.effectSourceType = RuleDefinitions.EffectSourceType.Environment;
                    service.ApplyEffectForms(
                        activeEnvironmentEffect.EffectDescription.EffectForms,
                        formsParams,
                        null,
                        out _,
                        out _,
                        effectApplication: activeEnvironmentEffect.EffectDescription.EffectApplication,
                        filters: activeEnvironmentEffect.EffectDescription.EffectFormFilters);
                }

                environmentManager.EnvironmentEffectImpactTriggered?.Invoke(activeEnvironmentEffect, affectedCharacter);
            }

            if (activeEnvironmentEffect.IsOnGoing())
            {
                activeEnvironmentEffect.RemainingRounds = RuleDefinitions.ComputeRoundsDuration(
                    activeEnvironmentEffect.EffectDescription.DurationType,
                    activeEnvironmentEffect.EffectDescription.DurationParameter);
                activeEnvironmentEffect.TerminatedSelf += environmentManager.ActiveEnvironmentEffectTerminatedSelf;
                environmentManager.activeEnvironmentEffects.Add(activeEnvironmentEffect);
                environmentManager.activeEnvironmentEffects.Sort((effectA, effectB) =>
                    -(effectA.SourceDefinition is EnvironmentEffectDefinition sourceDefinition1
                        ? sourceDefinition1.Priority
                        : 0).CompareTo(effectB.SourceDefinition is EnvironmentEffectDefinition sourceDefinition2
                        ? sourceDefinition2.Priority
                        : 0));
                environmentManager.RegisterGlobalActiveEffect(activeEnvironmentEffect);
            }
            else
            {
                activeEnvironmentEffect.Terminate(false);
            }

            service.ClearDamageFormsByIndex();
        }

        private static IEnumerator Execute(
            FunctorParametersDescription functorParameters)
        {
            GameLocationCharacter actingCharacter = null;

            if (functorParameters.ActingCharacters.Count > 0)
            {
                actingCharacter = functorParameters.ActingCharacters[0];
            }

            var environmentManager =
                ServiceRepository.GetService<IGameLocationEnvironmentService>() as GameLocationEnvironmentManager;

            if (!environmentManager)
            {
                Trace.LogError("Missing environmentService in FunctorEnvironmentEffect");
            }
            else
            {
                var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
                var rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();

                while (rulesetImplementationService.IsApplyingEffects())
                {
                    yield return null;
                }

                if (functorParameters.IsGlobalEnvironmentEffect)
                {
                    yield return TriggerEnvironmentEffectPosition(
                        environmentManager,
                        actingCharacter,
                        functorParameters.EnvironmentEffectDefinition,
                        int3.zero,
                        functorParameters.SavingThrowOverride,
                        functorParameters.AddDice,
                        functorParameters.SourceGadget.GameGadget,
                        functorParameters.IsGlobalEnvironmentEffect);
                }

                if (functorParameters.BoxColliders.Count > 0)
                {
                    foreach (var t in functorParameters.BoxColliders)
                    {
                        if (!t)
                        {
                            continue;
                        }

                        var min = t.bounds.min;
                        var max = t.bounds.max;

                        yield return TriggerEnvironmentEffectBox(
                            environmentManager,
                            actingCharacter,
                            functorParameters.EnvironmentEffectDefinition,
                            new BoxInt(
                                new int3(
                                    Mathf.RoundToInt(min.x),
                                    Mathf.RoundToInt(min.y),
                                    Mathf.RoundToInt(min.z)),
                                new int3(
                                    Mathf.RoundToInt(max.x) - 1,
                                    Mathf.RoundToInt(max.y) - 1,
                                    Mathf.RoundToInt(max.z) - 1)),
                            functorParameters.SavingThrowOverride,
                            functorParameters.AddDice,
                            functorParameters.SourceGadget.GameGadget,
                            functorParameters.IsGlobalEnvironmentEffect);
                    }
                }

                if (functorParameters.Nodes.Count > 0)
                {
                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    foreach (var t in functorParameters.Nodes)
                    {
                        if (t)
                        {
                            yield return TriggerEnvironmentEffectPosition(
                                environmentManager,
                                actingCharacter,
                                functorParameters.EnvironmentEffectDefinition,
                                positioningService.GetGridPositionFromWorldPosition(t.transform),
                                functorParameters.SavingThrowOverride,
                                functorParameters.AddDice,
                                functorParameters.SourceGadget.GameGadget,
                                functorParameters.IsGlobalEnvironmentEffect);
                        }
                    }
                }

                if (functorParameters.Nodes.Empty() && functorParameters.BoxColliders.Empty())
                {
                    yield return TriggerEnvironmentEffectPosition(
                        environmentManager,
                        actingCharacter,
                        functorParameters.EnvironmentEffectDefinition,
                        positioningService.GetGridPositionFromWorldPosition(functorParameters.SourceGadget.transform),
                        functorParameters.SavingThrowOverride,
                        functorParameters.AddDice,
                        functorParameters.SourceGadget.GameGadget,
                        functorParameters.IsGlobalEnvironmentEffect);
                }
            }
        }
    }
}
