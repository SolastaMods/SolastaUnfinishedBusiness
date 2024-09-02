using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Interfaces;

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
        private static readonly EffectDescription EmptyEffectDescription = EffectDescriptionBuilder
            .Create()
            .Build();

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
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var abilityScoreName = functorParameters.AbilityCheck.AbilityScoreName;
            var gadgetDefinition = functorParameters.GadgetDefinition;
            var rolledSavingThrow = implementationService.TryRollSavingThrow(
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
                EmptyEffectDescription.EffectForms,
                null,
                null,
                string.Empty,
                gadgetDefinition,
                string.Empty,
                null,
                out var saveOutcome,
                out var saveOutcomeDelta);

            var worldGadget = !functorParameters.BoolParameter
                ? functorParameters.TargetGadget
                : functorParameters.SourceGadget;

            if (rolledSavingThrow)
            {
                var battleManager = ServiceRepository.GetService<IGameLocationBattleService>()
                    as GameLocationBattleManager;
                var hasBorrowedLuck =
                    rulesetCharacter.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionBorrowedLuck);
                var savingThrowData = new SavingThrowData
                {
                    SaveActionModifier = actionModifier,
                    SaveOutcome = saveOutcome,
                    SaveOutcomeDelta = saveOutcomeDelta,
                    SaveDC = RulesetActorExtensions.SaveDC,
                    SaveBonusAndRollModifier = RulesetActorExtensions.SaveBonusAndRollModifier,
                    SavingThrowAbility = RulesetActorExtensions.SavingThrowAbility,
                    SourceDefinition = gadgetDefinition,
                    EffectDescription = EmptyEffectDescription,
                    Title = gadgetDefinition.FormatTitle(),
                    Action = null
                };

                yield return TryAlterOutcomeSavingThrow.Handler(
                    battleManager,
                    null,
                    actingCharacter,
                    savingThrowData,
                    hasBorrowedLuck,
                    EmptyEffectDescription);
            }

            if (saveOutcome == RuleDefinitions.RollOutcome.Success)
            {
                var conditionIndex = Array.IndexOf(
                    worldGadget.ConditionChoices(), functorParameters.TargetConditionState.name);

                worldGadget.GameGadget.SetCondition(
                    conditionIndex,
                    functorParameters.TargetConditionState.state,
                    functorParameters.ActingCharacters);
            }
            else if (functorParameters.HasAlternateTargetConditionState)
            {
                var conditionIndex = Array.IndexOf(
                    worldGadget.ConditionChoices(), functorParameters.AlternateTargetConditionState.name);

                worldGadget.GameGadget.SetCondition(
                    conditionIndex,
                    functorParameters.AlternateTargetConditionState.state,
                    functorParameters.ActingCharacters);
            }
        }
    }
}
