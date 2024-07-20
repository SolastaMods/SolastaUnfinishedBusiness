using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Races;

internal static class RaceImpBuilder
{
    private const string Name = "Imp";
    private const ActionDefinitions.Id ImpishWrathToggle = (ActionDefinitions.Id)ExtraActionId.ImpishWrathToggle;

    private const string ConditionImpAssistedAllyName = "ConditionImpAssistedAlly";
    private const string ConditionImpAssistedEnemyName = "ConditionImpAssistedEnemy";
    private const string ConditionImpPassageName = "ConditionImpPassage";
    private const string ConditionImpSpiteName = "ConditionImpSpite";
    private const string ConditionImpSpiteMarkerName = "ConditionImpSpiteMarker";

    internal static CharacterRaceDefinition RaceImp { get; } = BuildImp();

    [NotNull]
    private static CharacterRaceDefinition BuildImp()
    {
        var raceImp = CharacterRaceDefinitionBuilder
            .Create(CharacterRaceDefinitions.Tiefling, $"Race{Name}")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove6,
                FeatureDefinitionSenses.SenseNormalVision,
                FeatureDefinitionSenses.SenseDarkvision,
                FlexibleRacesContext.FeatureSetLanguageCommonPlusOne)
            .AddToDB();

        RacesContext.RaceScaleMap[raceImp] = 7f / 9.4f;

        raceImp.subRaces = [BuildImpInfernal(raceImp), BuildImpForest(raceImp), BuildImpBadland(raceImp)];
        return raceImp;
    }

    #region Infernal Imp

    private static CharacterRaceDefinition BuildImpInfernal(CharacterRaceDefinition raceImp)
    {
        const string NAME = "ImpInfernal";

        var spriteReference = Sprites.GetSprite(NAME, Resources.ImpInfernal, 1024, 512);

        var featureSetImpInfernalFiendishResistance = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}FiendishResistance")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance)
            .AddToDB();

        var featureSetImpInfernalAbilityScoreIncrease = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}AbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease,
                FeatureDefinitionAttributeModifiers.AttributeModifierDragonbornAbilityScoreIncreaseCha)
            .AddToDB();

        var castSpellImpInfernal = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellTiefling, $"CastSpell{NAME}")
            .SetGuiPresentation(Category.Feature)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(
                SpellListDefinitionBuilder
                    .Create($"SpellList{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .ClearSpells()
                    .SetSpellsAtLevel(0, SpellDefinitions.ViciousMockery)
                    .SetSpellsAtLevel(1, SpellDefinitions.Invisibility)
                    .FinalizeSpells(true, 1)
                    .AddToDB())
            .AddToDB();

        var raceImpInfernal = CharacterRaceDefinitionBuilder
            .Create(raceImp, $"Race{NAME}")
            .SetBaseHeight(42)
            .SetGuiPresentation(Category.Race, spriteReference)
            .SetFeaturesAtLevel(1,
                featureSetImpInfernalAbilityScoreIncrease,
                featureSetImpInfernalFiendishResistance,
                castSpellImpInfernal)
            .AddToDB();

        raceImpInfernal.racePresentation.preferedSkinColors = new RangedInt(15, 19);

        return raceImpInfernal;
    }

    #endregion

    #region Badland Imp

    private static CharacterRaceDefinition BuildImpBadland(CharacterRaceDefinition raceImp)
    {
        const string NAME = "ImpBadland";

        var spriteReference = Sprites.GetSprite(NAME, Resources.ImpBadland, 1024, 512);

        var featureSetImpBadlandAbilityScoreIncrease = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}AbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionAttributeModifiers.AttributeModifierDwarfAbilityScoreIncrease,
                FeatureDefinitionAttributeModifiers.AttributeModifierElfHighAbilityScoreIncrease)
            .AddToDB();

        var powerImpBadlandDrawInspiration = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DrawInspiration")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .AddToDB();

        powerImpBadlandDrawInspiration.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new DrawInspirationAlterOutcome(powerImpBadlandDrawInspiration));

        var powerImpBadlandAssistPool = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}AssistPool")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .AddToDB();

        var powerImpBadlandAssist = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{NAME}Assist")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite("PowerHelp", Resources.PowerHelp, 256, 128))
            .SetUniqueInstance()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique, 2)
                    .SetDurationData(DurationType.Round)
                    .ExcludeCaster()
                    .Build())
            .SetSharedPool(ActivationTime.BonusAction, powerImpBadlandAssistPool)
            .AddToDB();

        var conditionAssistedAlly = ConditionDefinitionBuilder.Create(ConditionImpAssistedAllyName)
            .SetConditionType(ConditionType.Beneficial)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
            .AddToDB();

        conditionAssistedAlly.AddCustomSubFeatures(new CustomBehaviorAssistAlly(conditionAssistedAlly));

        var conditionSpite = ConditionDefinitionBuilder.Create(ConditionImpSpiteName)
            .SetConditionType(ConditionType.Detrimental)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBaned)
            .AddToDB();

        ConditionDefinitionBuilder
            .Create(ConditionImpSpiteMarkerName)
            .AddCustomSubFeatures(new ImpSpiteAttackOnHit())
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var movementAffinityImpPassage = FeatureDefinitionMovementAffinityBuilder
            .Create("MovementAffinityImpPassage")
            .SetGuiPresentationNoContent(true)
            .SetBaseSpeedAdditiveModifier(2)
            .AddToDB();

        ConditionDefinitionBuilder.Create(ConditionImpPassageName)
            .SetConditionType(ConditionType.Beneficial)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHasted)
            .SetFeatures(movementAffinityImpPassage)
            .AddToDB();

        ConditionDefinitionBuilder.Create(ConditionImpAssistedEnemyName)
            .SetConditionType(ConditionType.Detrimental)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDistracted)
            .AddToDB();

        var powerImpAssistMagicEffect = new PowerOrSpellFinishedByMePowerImpAssist(ConditionImpAssistedEnemyName);

        powerImpBadlandAssist.AddCustomSubFeatures(
            new PowerImpAssistTargetFilter(true), powerImpAssistMagicEffect);

        var powerImpBadlandHospitality = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{NAME}Hospitality")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerImpHospitality", Resources.PowerImpHospitality, 256, 128))
            .SetUniqueInstance()
            .SetOverriddenPower(powerImpBadlandAssist)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.IndividualsUnique, 2)
                    .SetDurationData(DurationType.Round)
                    .ExcludeCaster()
                    .Build())
            .SetSharedPool(ActivationTime.BonusAction, powerImpBadlandAssistPool)
            .AddToDB();

        powerImpBadlandHospitality.AddCustomSubFeatures(
            new PowerImpAssistTargetFilter(false),
            new PowerImpHospitalityMagic(powerImpAssistMagicEffect));

        var powerImpBadlandPassage = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{NAME}Passage")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerImpPassage", Resources.PowerImpPassage, 256, 128))
            .SetUniqueInstance()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique, 2)
                    .SetDurationData(DurationType.Round)
                    .ExcludeCaster()
                    .Build())
            .SetSharedPool(ActivationTime.BonusAction, powerImpBadlandAssistPool)
            .AddToDB();

        powerImpBadlandPassage.AddCustomSubFeatures(
            new PowerImpAssistTargetFilter(false),
            new PowerImpPassageMagic(powerImpAssistMagicEffect));

        var powerImpBadlandSpite = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{NAME}Spite")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite("PowerImpSpite", Resources.PowerImpSpite, 256, 128))
            .SetUniqueInstance()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique, 2)
                    .SetDurationData(DurationType.Round)
                    .ExcludeCaster()
                    .Build())
            .SetSharedPool(ActivationTime.BonusAction, powerImpBadlandAssistPool)
            .AddToDB();

        powerImpBadlandSpite.AddCustomSubFeatures(
            new PowerImpAssistTargetFilter(true),
            new PowerOrSpellFinishedByMePowerImpAssist(conditionSpite.name));

        var raceImpBadland = CharacterRaceDefinitionBuilder
            .Create(raceImp, $"Race{NAME}")
            .SetGuiPresentation(Category.Race, spriteReference)
            .SetFeaturesAtLevel(1,
                featureSetImpBadlandAbilityScoreIncrease,
                powerImpBadlandAssistPool,
                powerImpBadlandAssist,
                powerImpBadlandDrawInspiration,
                FeatureDefinitionFeatureSets.FeatureSetElfFeyAncestry)
            .AddFeaturesAtLevel(3,
                powerImpBadlandHospitality,
                powerImpBadlandPassage,
                powerImpBadlandSpite)
            .AddToDB();

        raceImpBadland.racePresentation.preferedSkinColors = new RangedInt(1, 5);

        return raceImpBadland;
    }

    private class CustomBehaviorAssistAlly(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition condition)
        : IPhysicalAttackInitiatedByMe, IMagicEffectInitiatedByMe, IModifyAttackActionModifier
    {
        public IEnumerator OnMagicEffectInitiatedByMe(
            CharacterActionMagicEffect action,
            RulesetEffect activeEffect,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (activeEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit) ||
                targets.Count == 0)
            {
                yield break;
            }

            yield return HandleAssist(attacker, targets[0]);
        }

        public void OnAttackComputeModifier(RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            if (defender.HasConditionOfType(ConditionImpAssistedEnemyName) ||
                defender.HasConditionOfType(ConditionImpSpiteName))
            {
                attackModifier.attackAdvantageTrends.Add(
                    new TrendInfo(1, FeatureSourceType.Condition, condition.name, condition));
            }
        }

        public IEnumerator OnPhysicalAttackInitiatedByMe(GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            yield return HandleAssist(attacker, defender);
        }

        private static IEnumerator HandleAssist(
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter attacker,
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter defender)
        {
            // Only remove assisted condition if attacking assisted enemy
            if (defender.RulesetActor.HasConditionOfType(ConditionImpAssistedEnemyName) ||
                defender.RulesetActor.HasConditionOfType(ConditionImpSpiteName))
            {
                var isSpite = defender.RulesetActor.HasConditionOfType(ConditionImpSpiteName);

                defender.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionImpAssistedEnemyName);
                defender.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionImpSpiteName);

                if (isSpite)
                {
                    defender.RulesetActor.InflictCondition(
                        ConditionImpSpiteMarkerName,
                        DurationType.Round,
                        0,
                        TurnOccurenceType.EndOfTurn,
                        AttributeDefinitions.TagEffect,
                        attacker.RulesetCharacter.guid,
                        attacker.RulesetCharacter.CurrentFaction.name,
                        1,
                        ConditionImpSpiteMarkerName,
                        0,
                        0,
                        0);
                }
            }
            else
            {
                yield break;
            }
        }
    }

    private class PowerOrSpellFinishedByMePowerImpAssist(string enemyCondition) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var targetCharacters = action.ActionParams.TargetCharacters;

            switch (targetCharacters.Count)
            {
                case 0:
                    yield break;
                case 2:
                {
                    var ally = targetCharacters[0];
                    var enemy = targetCharacters[1];

                    ally.RulesetCharacter.InflictCondition(
                        ConditionImpAssistedAllyName,
                        DurationType.Round,
                        0,
                        TurnOccurenceType.EndOfTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetCharacter.guid,
                        rulesetCharacter.CurrentFaction.name,
                        1,
                        ConditionImpAssistedAllyName,
                        0,
                        0,
                        0);

                    enemy.RulesetCharacter.InflictCondition(
                        enemyCondition,
                        DurationType.Round,
                        1,
                        (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetCharacter.guid,
                        rulesetCharacter.CurrentFaction.name,
                        1,
                        enemyCondition,
                        0,
                        0,
                        0);
                    break;
                }
            }
        }
    }

    private class PowerImpPassageMagic(IPowerOrSpellFinishedByMe powerImpAssist) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            yield return powerImpAssist.OnPowerOrSpellFinishedByMe(action, baseDefinition);

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var targetCharacters = action.ActionParams.TargetCharacters;

            actingCharacter.RulesetCharacter.InflictCondition(
                ConditionImpPassageName,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.name,
                1,
                ConditionImpPassageName,
                0,
                0,
                0);

            foreach (var target in targetCharacters.Where(target => target.Side is Side.Ally))
            {
                target.RulesetCharacter.InflictCondition(
                    ConditionImpPassageName,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetCharacter.guid,
                    rulesetCharacter.CurrentFaction.name,
                    1,
                    ConditionImpPassageName,
                    0,
                    0,
                    0);
            }
        }
    }

    private class PowerImpAssistTargetFilter(bool requireEnemy) : IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => requireEnemy;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var selectedTargets = __instance.SelectionService.SelectedTargets;

            switch (selectedTargets.Count)
            {
                // select Ally first
                case 0 when target.Side != Side.Ally:
                    __instance.actionModifier.FailureFlags.Add("Tooltip/&SelectAnAlly");
                    return false;
                case > 0 when target.Side != Side.Enemy:
                    __instance.actionModifier.FailureFlags.Add("Tooltip/&AlreadySelectedAnAlly");
                    return false;
            }

            if (target.Side == Side.Ally)
            {
                if (!target.RulesetActor.HasConditionOfType(ConditionImpAssistedAllyName) &&
                    !target.RulesetActor.HasConditionOfType(ConditionImpSpiteName))
                {
                    return true;
                }

                __instance.actionModifier.FailureFlags.Add("Tooltip/&TargetAlreadyAssisted");
                return false;
            }

            // only allow enemy within reach
            if (!__instance.ActionParams.actingCharacter.IsWithinRange(target, 1))
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&TargetOutOfRange");
                return false;
            }

            if (!target.RulesetActor.HasConditionOfType(ConditionImpAssistedEnemyName))
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add("Tooltip/&TargetAlreadyAssisted");

            return false;
        }
    }

    private class PowerImpHospitalityMagic(IPowerOrSpellFinishedByMe parent) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            yield return parent.OnPowerOrSpellFinishedByMe(action, baseDefinition);

            var actingCharacter = action.ActingCharacter;
            var actingRulesetCharacter = actingCharacter.RulesetCharacter;
            var targetCharacters = action.ActionParams.TargetCharacters;

            var dieRoll =
                actingRulesetCharacter.RollDie(DieType.D6, RollContext.None, false, AdvantageType.None, out _, out _);
            var healingReceived =
                actingRulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus) + dieRoll;

            actingRulesetCharacter.ReceiveTemporaryHitPoints(
                healingReceived, DurationType.UntilLongRest, 0, TurnOccurenceType.StartOfTurn,
                actingRulesetCharacter.guid);
            EffectHelpers.StartVisualEffect(actingCharacter, actingCharacter, SpellDefinitions.CureWounds,
                EffectHelpers.EffectType.Effect);

            foreach (var target in targetCharacters.Where(target => target.Side == Side.Ally))
            {
                target.RulesetCharacter.ReceiveTemporaryHitPoints(
                    healingReceived, DurationType.UntilLongRest, 0, TurnOccurenceType.StartOfTurn,
                    actingRulesetCharacter.guid);
                EffectHelpers.StartVisualEffect(target, target, SpellDefinitions.CureWounds,
                    EffectHelpers.EffectType.Effect);
            }
        }
    }

    private class ImpSpiteAttackOnHit : ITryAlterOutcomeAttack
    {
        public int HandlerPriority => -10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            if (action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess) ||
                helper != defender)
            {
                yield break;
            }

            defender.RulesetActor.InflictCondition(
                "ConditionMocked",
                DurationType.Minute,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                attacker.RulesetCharacter.guid,
                attacker.RulesetCharacter.CurrentFaction.name,
                1,
                "ConditionMocked",
                0,
                0,
                0);

            EffectHelpers.StartVisualEffect(
                defender, defender, SpellDefinitions.ViciousMockery, EffectHelpers.EffectType.Effect);
        }
    }

    private class DrawInspirationAlterOutcome(FeatureDefinitionPower powerImpBadlandDrawInspiration) :
        ITryAlterOutcomeAttack,
        ITryAlterOutcomeSavingThrow
    {
        private const int InspirationValue = 3;

        public int HandlerPriority => -10;

        public IEnumerator OnTryAlterOutcomeAttack(GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var rulesetHelper = attacker.RulesetCharacter;

            if (action.AttackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure) ||
                helper != attacker ||
                action.AttackSuccessDelta < -InspirationValue ||
                rulesetHelper.GetRemainingPowerUses(powerImpBadlandDrawInspiration) == 0)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerImpBadlandDrawInspiration, rulesetHelper);
            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
            {
                StringParameter = "DrawInspiration",
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                UsablePower = usablePower
            };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToSpendPower(actionParams);

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!actionParams.ReactionValidated)
            {
                yield break;
            }

            action.AttackSuccessDelta += InspirationValue;
            action.AttackRollOutcome = RollOutcome.Success;
            actionModifier.AttackRollModifier += InspirationValue;
            actionModifier.AttacktoHitTrends.Add(new TrendInfo(
                InspirationValue, FeatureSourceType.Power,
                powerImpBadlandDrawInspiration.Name, powerImpBadlandDrawInspiration));
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            bool hasHitVisual, [UsedImplicitly] bool hasBorrowedLuck)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (helper != defender ||
                !action.RolledSaveThrow ||
                action.SaveOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure) ||
                rulesetDefender.GetRemainingPowerUses(powerImpBadlandDrawInspiration) == 0 ||
                action.SaveOutcomeDelta < -InspirationValue)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerImpBadlandDrawInspiration, rulesetDefender);
            var actionParams = new CharacterActionParams(defender, ActionDefinitions.Id.SpendPower)
            {
                StringParameter = "DrawInspiration",
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetDefender, usablePower, false),
                UsablePower = usablePower
            };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToSpendPower(actionParams);
            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!actionParams.ReactionValidated)
            {
                yield break;
            }

            action.RolledSaveThrow = true;
            action.SaveOutcomeDelta = 0;
            action.SaveOutcome = RollOutcome.Success;
        }
    }

    #endregion

    #region Forest Imp

    private static CharacterRaceDefinition BuildImpForest(CharacterRaceDefinition raceImp)
    {
        const string NAME = "ImpForest";

        var spriteReference = Sprites.GetSprite(NAME, Resources.ImpForest, 1024, 512);

        var featureSetImpForestAbilityScoreIncrease = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}AbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease,
                FeatureDefinitionAttributeModifiers.AttributeModifierHalfOrcAbilityScoreIncreaseCon)
            .AddToDB();

        var actionAffinityImpForestInnateCunning = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{NAME}InnateCunning")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions(
                ActionDefinitions.Id.DisengageBonus,
                ActionDefinitions.Id.HideBonus)
            .AddToDB();

        var conditionImpishWrathMark = ConditionDefinitionBuilder
            .Create($"Condition{Name}ImpishWrathMark")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(
                ConditionInterruption.Attacks,
                ConditionInterruption.CastSpellExecuted,
                ConditionInterruption.UsePowerExecuted)
            .AddToDB();

        var powerImpForestImpishWrath = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}ImpishWrath")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .DelegatedToAction()
            .AddToDB();

        powerImpForestImpishWrath.AddCustomSubFeatures(
            new CustomBehaviorImpishWrath(powerImpForestImpishWrath, conditionImpishWrathMark));

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.MetamagicToggle, "ImpishWrathToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.ImpishWrathToggle)
            .SetActivatedPower(powerImpForestImpishWrath)
            .AddToDB();

        var actionAffinityImpishWrathToggle = FeatureDefinitionActionAffinityBuilder
            .Create(FeatureDefinitionActionAffinitys.ActionAffinitySorcererMetamagicToggle,
                "ActionAffinityImpishWrathToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(ImpishWrathToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(
                    ValidatorsCharacter.HasAvailablePowerUsage(powerImpForestImpishWrath)))
            .AddToDB();

        var featureSetImpForestImpishWrath = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}ImpishWrath")
            .SetGuiPresentation($"Power{NAME}ImpishWrath", Category.Feature)
            .SetFeatureSet(powerImpForestImpishWrath, actionAffinityImpishWrathToggle)
            .AddToDB();

        var raceImpForest = CharacterRaceDefinitionBuilder
            .Create(raceImp, $"Race{NAME}")
            .SetGuiPresentation(Category.Race, spriteReference)
            .SetFeaturesAtLevel(1,
                featureSetImpForestAbilityScoreIncrease,
                actionAffinityImpForestInnateCunning,
                featureSetImpForestImpishWrath,
                FeatureDefinitionFeatureSets.FeatureSetElfFeyAncestry)
            .AddToDB();

        raceImpForest.racePresentation.preferedSkinColors = new RangedInt(28, 37);

        return raceImpForest;
    }

    private class CustomBehaviorImpishWrath(
        FeatureDefinitionPower powerImpForestImpishWrath,
        ConditionDefinition conditionImpForestImpishWrathMark)
        : IMagicEffectBeforeHitConfirmedOnEnemy, IPhysicalAttackBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            HandleImpishWrath(attacker, actualEffectForms);

            yield break;
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            HandleImpishWrath(attacker, actualEffectForms);

            yield break;
        }

        private void HandleImpishWrath(GameLocationCharacter attacker, List<EffectForm> actualEffectForms)
        {
            var damageForm =
                actualEffectForms.FirstOrDefault(x => x.FormType == EffectForm.EffectFormType.Damage);

            if (damageForm == null)
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var alreadyTriggered = rulesetAttacker.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, conditionImpForestImpishWrathMark.Name);
            var shouldTrigger =
                rulesetAttacker.IsToggleEnabled(ImpishWrathToggle) &&
                rulesetAttacker.GetRemainingPowerUses(powerImpForestImpishWrath) > 0;

            if (shouldTrigger && !alreadyTriggered)
            {
                var usablePower = PowerProvider.Get(powerImpForestImpishWrath, rulesetAttacker);

                rulesetAttacker.UsePower(usablePower);
                rulesetAttacker.InflictCondition(
                    conditionImpForestImpishWrathMark.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfSourceTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    conditionImpForestImpishWrathMark.Name,
                    0,
                    0,
                    0);
            }

            switch (alreadyTriggered)
            {
                case false when !shouldTrigger:
                    return;
                case true:
                    rulesetAttacker.LogCharacterUsedPower(powerImpForestImpishWrath);
                    break;
            }

            var index = actualEffectForms.IndexOf(damageForm);
            var bonusDamage = attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var effectForm =
                EffectFormBuilder.DamageForm(damageForm.DamageForm.DamageType, 0, DieType.D1, bonusDamage);

            actualEffectForms.Insert(index + 1, effectForm);
        }
    }

    #endregion
}
