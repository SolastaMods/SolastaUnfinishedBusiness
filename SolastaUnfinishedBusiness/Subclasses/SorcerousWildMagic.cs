using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class SorcerousWildMagic : AbstractSubclass
{
    private const string Name = "SorcerousWildMagic";

    public SorcerousWildMagic()
    {
        // LEVEL 01

        // Wild Magic Surge

        var conditionChaos = ConditionDefinitionBuilder
            .Create($"Condition{Name}Chaos")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionConjuredCreature)
            .SetConditionType(ConditionType.Neutral)
            .AddToDB();

        var featureWildMagicSurge = FeatureDefinitionBuilder
            .Create($"Feature{Name}WildMagicSurge")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // Tides of Chaos

        var combatAffinityTidesOfChaos = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}TidesOfChaos")
            .SetGuiPresentation($"Power{Name}TidesOfChaos", Category.Feature, Gui.NoLocalization)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .AddToDB();

        var savingThrowAffinityTidesOfChaos = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}TidesOfChaos")
            .SetGuiPresentation($"Power{Name}TidesOfChaos", Category.Feature, Gui.NoLocalization)
            .SetAffinities(CharacterSavingThrowAffinity.Advantage, false,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddToDB();

        var abilityCheckAffinityTidesOfChaos = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheckAffinity{Name}TidesOfChaos")
            .SetGuiPresentation($"Power{Name}TidesOfChaos", Category.Feature, Gui.NoLocalization)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage, abilityProficiencyPairs:
                (AttributeDefinitions.Strength, string.Empty))
            .AddToDB();

        var conditionTidesOfChaos = ConditionDefinitionBuilder
            .Create($"Condition{Name}TidesOfChaos")
            .SetGuiPresentation($"Power{Name}TidesOfChaos", Category.Feature,
                ConditionDefinitions.ConditionBearsEndurance)
            .SetPossessive()
            .SetFeatures(combatAffinityTidesOfChaos, savingThrowAffinityTidesOfChaos, abilityCheckAffinityTidesOfChaos)
            .SetSpecialInterruptions(
                ConditionInterruption.Attacks,
                ConditionInterruption.AbilityCheck,
                ConditionInterruption.SavingThrow)
            .AddToDB();

        conditionTidesOfChaos.GuiPresentation.description = Gui.NoLocalization;

        var powerTidesOfChaos = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TidesOfChaos")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite(Name, Resources.PowerTidesOfChaos, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionTidesOfChaos))
                    .Build())
            .AddToDB();

        // LEVEL 06

        // Bend Luck

        var powerBendLuck = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BendLuck")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.SorceryPoints, 2, 0)
            .SetShowCasting(false)
            .AddToDB();

        powerBendLuck.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new CustomBehaviorBendLuck(powerBendLuck));

        // LEVEL 14

        // Controlled Chaos

        var powerControlledChaos = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ControlledChaos")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        // these powers should not be added to progression
        var powers = new List<FeatureDefinitionPower>();

        for (var i = 1; i <= 20; i++)
        {
            powers.Add(
                FeatureDefinitionPowerSharedPoolBuilder
                    .Create($"Power{Name}{i:D02}Reaction")
                    .SetGuiPresentation($"Power{Name}D{i:D02}", Category.Feature, hidden: true)
                    .SetSharedPool(ActivationTime.NoCost, powerControlledChaos)
                    .AddToDB());
        }

        PowerBundle.RegisterPowerBundle(powerControlledChaos, false, powers);

        featureWildMagicSurge.AddCustomSubFeatures(
            new CustomBehaviorWildMagicSurge(featureWildMagicSurge, conditionChaos, powerControlledChaos, [.. powers]));

        // LEVEL 18

        // Spell Bombardment

        var featureSpellBombardment = FeatureDefinitionBuilder
            .Create($"Feature{Name}SpellBombardment")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.SorcererWildMagic, 256))
            .AddFeaturesAtLevel(1, featureWildMagicSurge, powerTidesOfChaos)
            .AddFeaturesAtLevel(6, powerBendLuck)
            .AddFeaturesAtLevel(14, powerControlledChaos)
            .AddFeaturesAtLevel(18, featureSpellBombardment)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Sorcerer;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceSorcerousOrigin;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class CustomBehaviorWildMagicSurge(
        FeatureDefinition featureWildSurge,
        ConditionDefinition conditionChaos,
        FeatureDefinitionPower powerControlledChaos,
        params FeatureDefinitionPower[] powers) : IMagicEffectFinishedByMe
    {
        private static FeatureDefinitionPower _selectedPower;

        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterActionMagicEffect action, GameLocationCharacter attacker, List<GameLocationCharacter> targets)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (action is not CharacterActionCastSpell actionCastSell ||
                actionCastSell.ActiveSpell.SpellDefinition.SpellLevel == 0 ||
                rulesetAttacker.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, conditionChaos.Name))
            {
                yield break;
            }

            var chanceDie = RollDie(DieType.D20, AdvantageType.None, out _, out _);

            rulesetAttacker.ShowDieRoll(DieType.D20, chanceDie, title: featureWildSurge.GuiPresentation.Title);
            rulesetAttacker.LogCharacterActivatesAbility(
                featureWildSurge.GuiPresentation.Title,
                "Feedback/&WidSurgeChanceDieRoll",
                extra:
                [
                    (chanceDie > 1 ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                        chanceDie.ToString())
                ]);

            if (chanceDie > 1)
            {
                yield break;
            }

            var levels = rulesetAttacker.GetSubclassLevel(CharacterClassDefinitions.Sorcerer, Name);

            if (levels >= 14)
            {
                var wildSurgeDie1 = RollDie(DieType.D20, AdvantageType.None, out _, out _);
                var wildSurgeDie2 = RollDie(DieType.D20, AdvantageType.None, out _, out _);

                rulesetAttacker.ShowDieRoll(DieType.D20, chanceDie, title: featureWildSurge.GuiPresentation.Title);
                rulesetAttacker.LogCharacterActivatesAbility(
                    featureWildSurge.GuiPresentation.Title,
                    "Feedback/&ControlledChaosDieRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Positive, wildSurgeDie1.ToString()),
                        (ConsoleStyleDuplet.ParameterType.Positive, wildSurgeDie2.ToString())
                    ]);

                yield return HandleControlledChaos(attacker, wildSurgeDie1, wildSurgeDie2);
            }
            else
            {
                var wildSurgeDie = RollDie(DieType.D20, AdvantageType.None, out _, out _);

                rulesetAttacker.ShowDieRoll(DieType.D20, chanceDie, title: featureWildSurge.GuiPresentation.Title);
                rulesetAttacker.LogCharacterActivatesAbility(
                    featureWildSurge.GuiPresentation.Title,
                    "Feedback/&WidSurgeDieRoll",
                    extra: [(ConsoleStyleDuplet.ParameterType.Positive, wildSurgeDie.ToString())]);

                _selectedPower = powers[wildSurgeDie - 1];
            }

            //TODO: implement wild surge powers
            if (_selectedPower)
            {
                rulesetAttacker.LogCharacterUsedPower(_selectedPower);
            }
        }

        private IEnumerator HandleControlledChaos(GameLocationCharacter attacker, int firstRoll, int secondRoll)
        {
            var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!actionManager || !battleManager)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePowerPool = PowerProvider.Get(powerControlledChaos, rulesetAttacker);
            var usablePowerFirst = PowerProvider.Get(powers[firstRoll - 1], rulesetAttacker);
            var usablePowerSecond = PowerProvider.Get(powers[secondRoll - 1], rulesetAttacker);

            rulesetAttacker.UsablePowers.Add(usablePowerFirst);
            rulesetAttacker.UsablePowers.Add(usablePowerSecond);

            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
            {
                StringParameter = "ControlledChaos",
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetAttacker, usablePowerPool, false),
                UsablePower = usablePowerPool,
                TargetCharacters = { attacker }
            };

            var count = actionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestSpendBundlePower(actionParams);

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, actionManager, count);

            rulesetAttacker.UsablePowers.Remove(usablePowerFirst);
            rulesetAttacker.UsablePowers.Remove(usablePowerSecond);

            if (reactionRequest.Validated)
            {
                _selectedPower = powers.ElementAt(reactionRequest.SelectedSubOption);
            }
            else
            {
                var choiceRoll = RollDie(DieType.D2, AdvantageType.None, out _, out _);
                var choice = choiceRoll == 1 ? firstRoll : secondRoll;

                _selectedPower = powers.ElementAt(choice);
            }
        }
    }

    //
    // Bend Luck
    //
    
    private sealed class CustomBehaviorBendLuck(FeatureDefinitionPower powerBendLuck)
        : ITryAlterOutcomeAttack, ITryAlterOutcomeAttributeCheck, ITryAlterOutcomeSavingThrow
    {
        public int HandlerPriority => 30;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerBendLuck, rulesetHelper);

            if (rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0 ||
                Math.Abs(action.AttackSuccessDelta) > 4)
            {
                yield break;
            }

            string stringParameter;

            if (helper.Side == attacker.Side &&
                action.AttackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                stringParameter = "BendLuckAttack";
            }
            else if (helper.Side != attacker.Side &&
                     action.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                stringParameter = "BendLuckEnemyAttack";
            }
            else
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var reactionParams =
                new CharacterActionParams(helper, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = stringParameter,
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { helper }
                };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(reactionParams, "UsePower", helper);

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var dieRoll = rulesetHelper.RollDie(DieType.D4, RollContext.None, false, AdvantageType.None, out _, out _);

            if (helper.Side == attacker.Side)
            {
                attackModifier.AttacktoHitTrends.Add(
                    new TrendInfo(dieRoll, FeatureSourceType.Power, powerBendLuck.Name, powerBendLuck)
                    {
                        dieType = DieType.D4, dieFlag = TrendInfoDieFlag.None
                    });

                action.AttackSuccessDelta += dieRoll;
                attackModifier.AttackRollModifier += dieRoll;
                action.AttackRollOutcome = action.AttackSuccessDelta >= 0 ? RollOutcome.Success : RollOutcome.Failure;

                rulesetHelper.LogCharacterActivatesAbility(
                    powerBendLuck.GuiPresentation.Title,
                    "Feedback/&BendLuckAttackToHitRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D4)),
                        (action.AttackRollOutcome > 0
                            ? ConsoleStyleDuplet.ParameterType.Positive
                            : ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString())
                    ]);
            }
            else
            {
                attackModifier.AttacktoHitTrends.Add(
                    new TrendInfo(-dieRoll, FeatureSourceType.Power, powerBendLuck.Name, powerBendLuck)
                    {
                        dieType = DieType.D4, dieFlag = TrendInfoDieFlag.None
                    });

                action.AttackSuccessDelta -= dieRoll;
                attackModifier.AttackRollModifier -= dieRoll;
                action.AttackRollOutcome = action.AttackSuccessDelta >= 0 ? RollOutcome.Success : RollOutcome.Failure;

                rulesetHelper.LogCharacterActivatesAbility(
                    powerBendLuck.GuiPresentation.Title,
                    "Feedback/&BendLuckEnemyAttackToHitRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D4)),
                        (action.AttackRollOutcome > 0
                            ? ConsoleStyleDuplet.ParameterType.Positive
                            : ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString())
                    ]);
            }
        }

        public IEnumerator OnTryAlterAttributeCheck(
            GameLocationBattleManager battleManager,
            AbilityCheckData abilityCheckData,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier abilityCheckModifier)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerBendLuck, rulesetHelper);

            if (rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0 ||
                Math.Abs(abilityCheckData.AbilityCheckSuccessDelta) > 4)
            {
                yield break;
            }

            string stringParameter;

            if (helper.Side == defender.Side &&
                abilityCheckData.AbilityCheckRoll > 0 &&
                abilityCheckData.AbilityCheckRollOutcome == RollOutcome.Failure)
            {
                stringParameter = "BendLuckCheck";
            }
            else if (helper.Side != defender.Side &&
                     abilityCheckData.AbilityCheckRoll > 0 &&
                     abilityCheckData.AbilityCheckRollOutcome == RollOutcome.Success)
            {
                stringParameter = "BendLuckEnemyCheck";
            }
            else
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var reactionParams =
                new CharacterActionParams(helper, ActionDefinitions.Id.PowerNoCost)
                {
                    StringParameter = stringParameter,
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { helper }
                };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(reactionParams, "UsePower", helper);

            yield return battleManager.WaitForReactions(defender, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var dieRoll = rulesetHelper.RollDie(DieType.D4, RollContext.None, false, AdvantageType.None, out _, out _);

            if (helper.Side == defender.Side)
            {
                abilityCheckModifier.AbilityCheckAdvantageTrends.Add(
                    new TrendInfo(dieRoll, FeatureSourceType.Power, powerBendLuck.Name, powerBendLuck)
                    {
                        dieType = DieType.D4, dieFlag = TrendInfoDieFlag.None
                    });

                abilityCheckModifier.AbilityCheckModifier += dieRoll;
                abilityCheckData.AbilityCheckSuccessDelta += dieRoll;
                abilityCheckData.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckSuccessDelta >= 0
                    ? RollOutcome.Success
                    : RollOutcome.Failure;

                rulesetHelper.LogCharacterActivatesAbility(
                    powerBendLuck.GuiPresentation.Title,
                    "Feedback/&BendLuckCheckToHitRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D4)),
                        (abilityCheckData.AbilityCheckRollOutcome > 0
                            ? ConsoleStyleDuplet.ParameterType.Positive
                            : ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString())
                    ]);
            }
            else
            {
                abilityCheckModifier.AbilityCheckAdvantageTrends.Add(
                    new TrendInfo(-dieRoll, FeatureSourceType.Power, powerBendLuck.Name, powerBendLuck)
                    {
                        dieType = DieType.D4, dieFlag = TrendInfoDieFlag.None
                    });

                abilityCheckModifier.AbilityCheckModifier -= dieRoll;
                abilityCheckData.AbilityCheckSuccessDelta -= dieRoll;
                abilityCheckData.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckSuccessDelta >= 0
                    ? RollOutcome.Success
                    : RollOutcome.Failure;

                rulesetHelper.LogCharacterActivatesAbility(
                    powerBendLuck.GuiPresentation.Title,
                    "Feedback/&BendLuckEnemyCheckToHitRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D4)),
                        (abilityCheckData.AbilityCheckRollOutcome > 0
                            ? ConsoleStyleDuplet.ParameterType.Positive
                            : ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString())
                    ]);
            }
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier saveModifier,
            bool hasHitVisual,
            bool hasBorrowedLuck)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerBendLuck, rulesetHelper);

            if (rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0 ||
                Math.Abs(action.SaveOutcomeDelta) > 4)
            {
                yield break;
            }

            string stringParameter;

            if (helper.Side == defender.Side &&
                action.RolledSaveThrow &&
                action.SaveOutcome == RollOutcome.Failure)
            {
                stringParameter = "BendLuckSaving";
            }
            else if (helper.Side != defender.Side &&
                     action.RolledSaveThrow &&
                     action.SaveOutcome == RollOutcome.Success)
            {
                stringParameter = "BendLuckEnemySaving";
            }
            else
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var reactionParams =
                new CharacterActionParams(helper, ActionDefinitions.Id.PowerNoCost)
                {
                    StringParameter = stringParameter,
                    StringParameter2 = "UseLuckySavingDescription".Formatted(
                        Category.Reaction, defender.Name, attacker.Name, helper.Name),
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { attacker }
                };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(reactionParams, "UsePower", attacker);

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var dieRoll = rulesetHelper.RollDie(DieType.D4, RollContext.None, false, AdvantageType.None, out _, out _);

            if (helper.Side == attacker.Side)
            {
                saveModifier.SavingThrowAdvantageTrends.Add(
                    new TrendInfo(dieRoll, FeatureSourceType.Power, powerBendLuck.Name, powerBendLuck)
                    {
                        dieType = DieType.D4, dieFlag = TrendInfoDieFlag.None
                    });

                action.SaveOutcomeDelta += dieRoll;
                saveModifier.SavingThrowModifier += dieRoll;
                action.SaveOutcome = action.SaveOutcomeDelta >= 0 ? RollOutcome.Success : RollOutcome.Failure;

                rulesetHelper.LogCharacterActivatesAbility(
                    powerBendLuck.GuiPresentation.Title,
                    "Feedback/&BendLuckSavingToHitRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D4)),
                        (action.SaveOutcome > 0
                            ? ConsoleStyleDuplet.ParameterType.Positive
                            : ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString())
                    ]);
            }
            else
            {
                saveModifier.SavingThrowAdvantageTrends.Add(
                    new TrendInfo(-dieRoll, FeatureSourceType.Power, powerBendLuck.Name, powerBendLuck)
                    {
                        dieType = DieType.D4, dieFlag = TrendInfoDieFlag.None
                    });

                action.SaveOutcomeDelta -= dieRoll;
                saveModifier.SavingThrowModifier -= dieRoll;
                action.SaveOutcome = action.SaveOutcomeDelta >= 0 ? RollOutcome.Success : RollOutcome.Failure;

                rulesetHelper.LogCharacterActivatesAbility(
                    powerBendLuck.GuiPresentation.Title,
                    "Feedback/&BendLuckEnemySavingToHitRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D4)),
                        (action.SaveOutcome > 0
                            ? ConsoleStyleDuplet.ParameterType.Positive
                            : ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString())
                    ]);
            }
        }
    }
}
