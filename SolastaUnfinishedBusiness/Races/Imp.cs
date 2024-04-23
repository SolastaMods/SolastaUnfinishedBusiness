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
            .AddCustomSubFeatures(HasModifiedUses.Marker, IsModifyPowerPool.Marker)
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
                    .SetNoSavingThrow()
                    .ExcludeCaster()
                    .Build())
            .SetSharedPool(ActivationTime.BonusAction, powerImpBadlandAssistPool)
            .AddToDB();

        var conditionAssistedAlly = ConditionDefinitionBuilder.Create(ConditionImpAssistedAllyName)
            .SetConditionType(ConditionType.Beneficial)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
            .AddToDB();

        conditionAssistedAlly.AddCustomSubFeatures(new ImpAssistedAllyAttackInitiatedByMe(conditionAssistedAlly));

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

        var powerImpAssistMagicEffect = new PowerImpAssistMagicEffectFinishedByMe(ConditionImpAssistedEnemyName);

        powerImpBadlandAssist.AddCustomSubFeatures(
            new PowerImpAssistTargetFilter(powerImpBadlandAssist, true), powerImpAssistMagicEffect);

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
                    .SetNoSavingThrow()
                    .ExcludeCaster()
                    .Build())
            .SetSharedPool(ActivationTime.BonusAction, powerImpBadlandAssistPool)
            .AddToDB();

        powerImpBadlandHospitality.AddCustomSubFeatures(
            new PowerImpAssistTargetFilter(powerImpBadlandHospitality, false),
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
                    .SetNoSavingThrow()
                    .ExcludeCaster()
                    .Build())
            .SetSharedPool(ActivationTime.BonusAction, powerImpBadlandAssistPool)
            .AddToDB();

        powerImpBadlandPassage.AddCustomSubFeatures(
            new PowerImpAssistTargetFilter(powerImpBadlandPassage, false),
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
                    .SetNoSavingThrow()
                    .ExcludeCaster()
                    .Build())
            .SetSharedPool(ActivationTime.BonusAction, powerImpBadlandAssistPool)
            .AddToDB();

        powerImpBadlandSpite.AddCustomSubFeatures(
            new PowerImpAssistTargetFilter(powerImpBadlandSpite, true),
            new PowerImpAssistMagicEffectFinishedByMe(conditionSpite.name));

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

    private class ImpAssistedAllyAttackInitiatedByMe(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition condition)
        : IPhysicalAttackInitiatedByMe, IMagicEffectAttackInitiatedByMe, IModifyAttackActionModifier
    {
        public IEnumerator OnMagicEffectAttackInitiatedByMe(
            CharacterActionMagicEffect action,
            RulesetEffect activeEffect,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool checkMagicalAttackDamage)
        {
            yield return HandleAssist(attacker, defender);
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
            if (defender.RulesetCharacter.HasConditionOfType(ConditionImpAssistedEnemyName) ||
                defender.RulesetCharacter.HasConditionOfType(ConditionImpSpiteName))
            {
                var isSpite = defender.RulesetCharacter.HasConditionOfType(ConditionImpSpiteName);

                defender.RulesetCharacter.RemoveAllConditionsOfType(ConditionImpAssistedEnemyName);
                defender.RulesetCharacter.RemoveAllConditionsOfType(ConditionImpSpiteName);

                if (isSpite)
                {
                    defender.RulesetCharacter.InflictCondition(
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

    private class PowerImpAssistMagicEffectFinishedByMe(string enemyCondition) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
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

    private class PowerImpPassageMagic(IMagicEffectFinishedByMe powerImpAssist) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            yield return powerImpAssist.OnMagicEffectFinishedByMe(action, baseDefinition);

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

    private class PowerImpAssistTargetFilter(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerImpBadlandAssist,
        bool requireEnemy) : IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => requireEnemy;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower ||
                rulesetEffectPower.PowerDefinition != powerImpBadlandAssist)
            {
                return true;
            }

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
                if (!target.RulesetCharacter.HasConditionOfType(ConditionImpAssistedAllyName) &&
                    !target.RulesetCharacter.HasConditionOfType(ConditionImpSpiteName))
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

            if (!target.RulesetCharacter.HasConditionOfType(ConditionImpAssistedEnemyName))
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add("Tooltip/&TargetAlreadyAssisted");

            return false;
        }
    }

    private class PowerImpHospitalityMagic(IMagicEffectFinishedByMe parent) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            yield return parent.OnMagicEffectFinishedByMe(action, baseDefinition);

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

    private class ImpSpiteAttackOnHit : IAttackBeforeHitPossibleOnMeOrAlly
    {
        public IEnumerator OnAttackBeforeHitPossibleOnMeOrAlly(GameLocationBattleManager battleManager,
            [UsedImplicitly] GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect,
            int attackRoll)
        {
            if (defender != helper)
            {
                yield break;
            }

            if (defender.RulesetCharacter.IsDeadOrDying)
            {
                yield break;
            }

            defender.RulesetCharacter.InflictCondition(
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

        public IEnumerator OnTryAlterOutcomeAttack(GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (attacker != helper ||
                action.AttackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure) ||
                action.AttackSuccessDelta < -InspirationValue ||
                rulesetAttacker.GetRemainingPowerUses(powerImpBadlandDrawInspiration) == 0)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerImpBadlandDrawInspiration, rulesetAttacker);
            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
            {
                StringParameter = "DrawInspiration",
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
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
            actionModifier.AttackRollModifier += InspirationValue;
            actionModifier.AttacktoHitTrends?.Add(new TrendInfo(
                InspirationValue,
                FeatureSourceType.Power,
                powerImpBadlandDrawInspiration.Name,
                powerImpBadlandDrawInspiration));
            action.AttackRollOutcome = RollOutcome.Success;
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
            if (defender != helper ||
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
            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
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

            rulesetDefender.UsePower(usablePower);
            action.RolledSaveThrow = true;
            action.saveOutcomeDelta = 0;
            action.saveOutcome = RollOutcome.Success;
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

        var powerImpForestImpishWrath = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}ImpishWrath")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .DelegatedToAction()
            .AddToDB();

        powerImpForestImpishWrath.AddCustomSubFeatures(new CustomBehaviorImpishWrath(powerImpForestImpishWrath));

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

    private class CustomBehaviorImpishWrath(FeatureDefinitionPower powerImpForestImpishWrath)
        : IPhysicalAttackFinishedByMe, IMagicEffectFinishedByMeAny
    {
        public IEnumerator OnMagicEffectFinishedByMeAny(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            var rulesetEffect = action.actionParams.RulesetEffect;

            if (!rulesetEffect.EffectDescription.HasFormOfType(EffectForm.EffectFormType.Damage))
            {
                yield break;
            }

            if ((action.RolledSaveThrow &&
                 action.SaveOutcome == RollOutcome.Failure) ||
                (action.AttackRoll != 0 &&
                 action.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield return HandleImpishWrath(attacker,
                    defender,
                    rulesetEffect.SourceTags,
                    rulesetEffect.EffectDescription.FindFirstDamageForm()?.damageType);
            }
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (!attackMode.EffectDescription.HasFormOfType(EffectForm.EffectFormType.Damage))
            {
                yield break;
            }

            if (action.AttackRoll != 0 &&
                action.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                yield return HandleImpishWrath(
                    attacker,
                    defender,
                    attackMode.AttackTags,
                    attackMode.EffectDescription.FindFirstDamageForm()?.damageType);
            }
        }

        private IEnumerator HandleImpishWrath(
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            List<string> attackTags,
            string damageType = DamageTypeBludgeoning)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.IsToggleEnabled(ImpishWrathToggle))
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrUnconscious: false })
            {
                yield break;
            }

            if (rulesetAttacker.GetRemainingPowerUses(powerImpForestImpishWrath) == 0)
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(powerImpForestImpishWrath, rulesetAttacker);

            rulesetAttacker.UsePower(usablePower);

            var bonusDamage = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var damageForm = new DamageForm
            {
                DamageType = damageType, DieType = DieType.D1, DiceNumber = 0, BonusDamage = bonusDamage
            };
            var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
            {
                sourceCharacter = rulesetAttacker,
                targetCharacter = rulesetDefender,
                position = defender.LocationPosition
            };

            RulesetActor.InflictDamage(
                bonusDamage,
                damageForm,
                damageType,
                applyFormsParams,
                rulesetDefender,
                false,
                rulesetAttacker.Guid,
                false,
                attackTags,
                new RollInfo(DieType.D1, [], bonusDamage),
                false,
                out _);
        }
    }

    #endregion
}
