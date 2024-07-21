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
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class SorcerousWildMagic : AbstractSubclass
{
    private const string Name = "SorcerousWildMagic";
    internal static bool ForceWildSurge;
    internal static int ForceWildSurgeRoll;

    private static readonly FeatureDefinition FeatureWildMagicSurge = FeatureDefinitionBuilder
        .Create($"Feature{Name}WildMagicSurge")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    private static readonly FeatureDefinition FeatureSpellBombardment = FeatureDefinitionBuilder
        .Create($"Feature{Name}SpellBombardment")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    private static readonly ConditionDefinition ConditionChaos = ConditionDefinitionBuilder
        .Create($"Condition{Name}Chaos")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionConjuredCreature)
        .SetConditionType(ConditionType.Neutral)
        .AddCustomSubFeatures(new CharacterTurnStartListenerChaos(FeatureWildMagicSurge))
        .AddToDB();

    private static readonly ConditionDefinition ConditionDamageResistance = ConditionDefinitionBuilder
        .Create($"Condition{Name}DamageResistance")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMagicallyArmored)
        .SetPossessive()
        .SetFeatures(
            DamageAffinityAcidResistance,
            DamageAffinityBludgeoningResistance,
            DamageAffinityColdResistance,
            DamageAffinityFireResistance,
            DamageAffinityForceDamageResistance,
            DamageAffinityLightningResistance,
            DamageAffinityNecroticResistance,
            DamageAffinityPiercingResistance,
            DamageAffinityPoisonResistance,
            DamageAffinityPsychicResistance,
            DamageAffinityRadiantResistance,
            DamageAffinitySlashingResistance,
            DamageAffinityThunderResistance)
        .AddToDB();

    private static readonly ConditionDefinition ConditionPiercingVulnerability = ConditionDefinitionBuilder
        .Create($"Condition{Name}PiercingVulnerability")
        .SetGuiPresentation(Category.Condition, Gui.NoLocalization, ConditionDefinitions.ConditionTargetedByGuidingBolt)
        .SetPossessive()
        .SetConditionType(ConditionType.Detrimental)
        .SetFeatures(
            DamageAffinityPiercingVulnerability)
        .AddToDB();

    private static readonly ConditionDefinition ConditionTeleport = ConditionDefinitionBuilder
        .Create($"Condition{Name}Teleport")
        .SetGuiPresentation(Category.Condition, Gui.NoLocalization, ConditionDefinitions.ConditionConjuredCreature)
        .SetFeatures(
            FeatureDefinitionPowerBuilder
                .Create($"Power{Name}Teleport")
                .SetGuiPresentation("PowerSorcerousWildMagicD05", Category.Feature, MistyStep)
                .SetUsesFixed(ActivationTime.NoCost, RechargeRate.TurnStart)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Position)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                                .Build())
                        .SetParticleEffectParameters(MistyStep)
                        .Build())
                .AddToDB())
        .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
        .AddToDB();

    private static readonly ConditionDefinition ConditionLightningStrike = ConditionDefinitionBuilder
        .Create($"Condition{Name}LightningStrike")
        .SetGuiPresentation(Category.Condition, Gui.NoLocalization, ConditionDefinitions.ConditionGuided)
        .SetFeatures(
            FeatureDefinitionPowerBuilder
                .Create($"Power{Name}LightningStrike")
                .SetGuiPresentation("PowerSorcerousWildMagicD19", Category.Feature, LightningBolt)
                .SetUsesFixed(ActivationTime.NoCost, RechargeRate.TurnStart)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                        .SetEffectForms(
                            EffectFormBuilder.DamageForm(DamageTypeLightning, 4, DieType.D10))
                        .SetParticleEffectParameters(LightningBolt)
                        .Build())
                .AddToDB())
        .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
        .AddToDB();

    public SorcerousWildMagic()
    {
        // LEVEL 01

        // Wild Magic Surge

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
            [
                (AttributeDefinitions.Strength, string.Empty),
                (AttributeDefinitions.Dexterity, string.Empty),
                (AttributeDefinitions.Constitution, string.Empty),
                (AttributeDefinitions.Intelligence, string.Empty),
                (AttributeDefinitions.Wisdom, string.Empty),
                (AttributeDefinitions.Charisma, string.Empty)
            ])
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

        powerBendLuck.AddCustomSubFeatures(ModifyPowerVisibility.Hidden, new CustomBehaviorBendLuck(powerBendLuck));

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
            var prefix = $"Power{Name}D{i:D02}";

            powers.Add(
                FeatureDefinitionPowerSharedPoolBuilder
                    .Create(prefix + "Reaction")
                    .SetGuiPresentation(prefix, Category.Feature, hidden: true)
                    .SetSharedPool(ActivationTime.NoCost, powerControlledChaos)
                    .AddToDB());
        }

        PowerBundle.RegisterPowerBundle(powerControlledChaos, false, powers);

        FeatureWildMagicSurge.AddCustomSubFeatures(
            new CustomBehaviorWildMagicSurge(FeatureWildMagicSurge, powerControlledChaos, [.. powers]));

        // LEVEL 18

        // Spell Bombardment

        // Main

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.SorcererWildMagic, 256))
            .AddFeaturesAtLevel(1, FeatureWildMagicSurge, powerTidesOfChaos)
            .AddFeaturesAtLevel(6, powerBendLuck)
            .AddFeaturesAtLevel(14, powerControlledChaos)
            .AddFeaturesAtLevel(18, FeatureSpellBombardment)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Sorcerer;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceSorcerousOrigin;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static void HandleSpellBombardment(
        RulesetCharacter rulesetCharacter, DamageForm damageForm, List<int> rolledValues, ref int damage)
    {
        var character = GameLocationCharacter.GetFromActor(rulesetCharacter);
        var levels = rulesetCharacter.GetSubclassLevel(CharacterClassDefinitions.Sorcerer, Name);

        if (levels < 18)
        {
            return;
        }

        var dieType = damageForm.DieType;
        var maxDie = DiceMaxValue[(int)dieType];
        var tag = FeatureSpellBombardment.Name;

        if (!rolledValues.Contains(maxDie) ||
            !character.OncePerTurnIsValid(tag))
        {
            return;
        }

        character.UsedSpecialFeatures.TryAdd(tag, 0);
        rulesetCharacter.LogCharacterUsedFeature(FeatureSpellBombardment);

        var roll = RollDie(dieType, AdvantageType.None, out _, out _);

        rolledValues.Add(roll);
        damage += roll;
    }

    //
    // Chaos
    //

    private sealed class CharacterTurnStartListenerChaos(
        FeatureDefinition featureWildSurge)
        : ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;
            var wildSurgeDie = 1;

            while (wildSurgeDie == 1)
            {
                wildSurgeDie = RollDie(DieType.D20, AdvantageType.None, out _, out _);
            }

            rulesetCharacter.ShowDieRoll(DieType.D20, wildSurgeDie, title: featureWildSurge.GuiPresentation.Title);
            rulesetCharacter.LogCharacterActivatesAbility(
                featureWildSurge.GuiPresentation.Title,
                "Feedback/&WidSurgeDieRoll",
                extra: [(ConsoleStyleDuplet.ParameterType.Positive, wildSurgeDie.ToString())]);
        }
    }

    //
    // Wild Magic Surge
    //

    private sealed class CustomBehaviorWildMagicSurge(
        FeatureDefinition featureWildSurge,
        FeatureDefinitionPower powerControlledChaos,
        params FeatureDefinitionPower[] powers) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterActionMagicEffect action, GameLocationCharacter attacker, List<GameLocationCharacter> targets)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (action is not CharacterActionCastSpell actionCastSell ||
                actionCastSell.ActiveSpell.SpellDefinition.SpellLevel == 0 ||
                actionCastSell.ActionId == ActionDefinitions.Id.CastNoCost ||
                rulesetAttacker.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, ConditionChaos.Name))
            {
                yield break;
            }

            var chanceDie = RollDie(DieType.D20, AdvantageType.None, out _, out _);

            if (ForceWildSurge)
            {
                chanceDie = 1;
            }

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

                if (ForceWildSurgeRoll > 0)
                {
                    wildSurgeDie1 = ForceWildSurgeRoll;
                }

                rulesetAttacker.ShowDieRoll(DieType.D20, wildSurgeDie1, title: featureWildSurge.GuiPresentation.Title);
                rulesetAttacker.ShowDieRoll(DieType.D20, wildSurgeDie2, title: featureWildSurge.GuiPresentation.Title);

                rulesetAttacker.LogCharacterActivatesAbility(
                    powerControlledChaos.FormatTitle(),
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
                var selectedRoll = RollDie(DieType.D20, AdvantageType.None, out _, out _);

                if (ForceWildSurgeRoll > 0)
                {
                    selectedRoll = ForceWildSurgeRoll;
                }

                var selectedPower = powers[selectedRoll - 1];

                rulesetAttacker.ShowDieRoll(DieType.D20, selectedRoll, title: featureWildSurge.GuiPresentation.Title);

                HandleWildSurge(attacker, selectedRoll,
                    powerControlledChaos.FormatTitle(), selectedPower.FormatTitle(), "Feedback/&WidSurgeDieRoll");
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

            int selectedRoll;
            FeatureDefinitionPower selectedPower;

            if (reactionRequest.Validated)
            {
                selectedPower = powers.ElementAt(reactionRequest.SelectedSubOption);
                selectedRoll = reactionRequest.SelectedSubOption + 1;
            }
            else
            {
                var choiceRoll = RollDie(DieType.D2, AdvantageType.None, out _, out _);
                var choice = choiceRoll == 1 ? firstRoll : secondRoll;

                selectedPower = powers.ElementAt(choice - 1);
                selectedRoll = choice;
            }

            HandleWildSurge(attacker, selectedRoll,
                FeatureWildMagicSurge.FormatTitle(), selectedPower.FormatTitle(), "Feedback/&ControlledChaosDieChoice");
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

    #region Wild Surge Handlers

    private static void HandleWildSurge(
        GameLocationCharacter caster, int roll, string featureTitle, string selectedPowerTitle, string feedback)
    {
        var rulesetCaster = caster.RulesetCharacter;

        rulesetCaster.LogCharacterActivatesAbility(
            featureTitle,
            feedback,
            extra:
            [
                (ConsoleStyleDuplet.ParameterType.Positive, roll.ToString()),
                (ConsoleStyleDuplet.ParameterType.Base, selectedPowerTitle)
            ]);

        switch (roll)
        {
            case 1:
                HandleWildSurgeD01(caster);
                break;
            case 2:
                HandleWildSurgeD02(caster);
                break;
            case 3:
                HandleWildSurgeD03(caster);
                break;
            case 4:
                HandleWildSurgeD04(caster);
                break;
            case 5:
                HandleWildSurgeD05(caster);
                break;
            case 6:
                HandleWildSurgeD06(caster);
                break;
            case 7:
                HandleWildSurgeD07(caster);
                break;
            case 8:
                HandleWildSurgeD08(caster);
                break;
            case 9:
                HandleWildSurgeD09(caster);
                break;
            case 10:
                HandleWildSurgeD10(caster);
                break;
            case 11:
                HandleWildSurgeD11(caster);
                break;
            case 12:
                HandleWildSurgeD12(caster);
                break;
            case 13:
                HandleWildSurgeD13(caster);
                break;
            case 14:
                HandleWildSurgeD14(caster);
                break;
            case 15:
                HandleWildSurgeD15(caster);
                break;
            case 16:
                HandleWildSurgeD16(caster);
                break;
            case 17:
                HandleWildSurgeD17(caster);
                break;
            case 18:
                HandleWildSurgeD18(caster);
                break;
            case 19:
                HandleWildSurgeD19(caster);
                break;
            case 20:
                HandleWildSurgeD20(caster);
                break;
        }
    }

    // trigger a random Wild Surge effect (except this one) at the start of each of your turns for the next minute
    private static void HandleWildSurgeD01(GameLocationCharacter caster)
    {
        var rulesetCaster = caster.RulesetCharacter;

        rulesetCaster.InflictCondition(
            ConditionChaos.Name,
            DurationType.Minute,
            1,
            TurnOccurenceType.EndOfTurn,
            AttributeDefinitions.TagEffect,
            rulesetCaster.guid,
            rulesetCaster.CurrentFaction.Name,
            1,
            ConditionChaos.Name,
            0,
            0,
            0);
    }

    // you cast Fireball centered on self
    private static void HandleWildSurgeD02(GameLocationCharacter caster)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var rulesetCaster = caster.RulesetCharacter;
        var spellRepertoire =
            rulesetCaster.SpellRepertoires.FirstOrDefault(
                x => x.SpellCastingClass == CharacterClassDefinitions.Sorcerer);
        var effectSpell = ServiceRepository.GetService<IRulesetImplementationService>()
            .InstantiateEffectSpell(rulesetCaster, spellRepertoire, Fireball, 3, false);


        var actionParams = new CharacterActionParams(caster, ActionDefinitions.Id.CastNoCost)
        {
            ActionModifiers = { new ActionModifier() },
            RulesetEffect = effectSpell,
            SpellRepertoire = spellRepertoire,
            Positions = { caster.LocationPosition }
        };

        rulesetCaster.SpellsCastByMe.TryAdd(effectSpell);
        actionService.ExecuteAction(actionParams, null, true);
    }

    // you regain 2D10 hit points
    private static void HandleWildSurgeD03(GameLocationCharacter caster)
    {
        var rulesetCaster = caster.RulesetCharacter;

        var healing = rulesetCaster.RollDiceAndSum(DieType.D10, RollContext.HealValueRoll, 2, [], false);

        rulesetCaster.ReceiveHealing(healing, true, rulesetCaster.Guid);
    }

    // each creature within 20 feet of you (including you) catches on fire
    private static void HandleWildSurgeD04(GameLocationCharacter caster)
    {
        var rulesetCaster = caster.RulesetCharacter;
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);
        var targets = contenders.Where(x => x.IsWithinRange(caster, 4)).ToList();

        foreach (var rulesetTarget in targets.Select(target => target.RulesetCharacter))
        {
            rulesetTarget.InflictCondition(
                ConditionDefinitions.ConditionOnFire1D4.Name,
                DurationType.Minute,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCaster.guid,
                rulesetCaster.CurrentFaction.Name,
                1,
                ConditionDefinitions.ConditionOnFire1D4.Name,
                0,
                0,
                0);
        }
    }

    // You can teleport up to 60 feet to an unoccupied space of your choice that you can see as a free action before your turn ends
    private static void HandleWildSurgeD05(GameLocationCharacter caster)
    {
        var rulesetCaster = caster.RulesetCharacter;

        rulesetCaster.InflictCondition(
            ConditionTeleport.Name,
            DurationType.Round,
            0,
            TurnOccurenceType.EndOfTurn,
            AttributeDefinitions.TagEffect,
            rulesetCaster.guid,
            rulesetCaster.CurrentFaction.Name,
            1,
            ConditionTeleport.Name,
            0,
            0,
            0);
    }

    // you become frightened until the end of your next turn
    private static void HandleWildSurgeD06(GameLocationCharacter caster)
    {
        var rulesetCaster = caster.RulesetCharacter;

        rulesetCaster.InflictCondition(
            ConditionFrightened,
            DurationType.Round,
            1,
            TurnOccurenceType.EndOfTurn,
            AttributeDefinitions.TagEffect,
            rulesetCaster.guid,
            rulesetCaster.CurrentFaction.Name,
            1,
            ConditionFrightened,
            0,
            0,
            0);
    }

    // you cast invisibility on self
    private static void HandleWildSurgeD07(GameLocationCharacter caster)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var rulesetCaster = caster.RulesetCharacter;
        var spellRepertoire =
            rulesetCaster.SpellRepertoires.FirstOrDefault(
                x => x.SpellCastingClass == CharacterClassDefinitions.Sorcerer);
        var effectSpell = ServiceRepository.GetService<IRulesetImplementationService>()
            .InstantiateEffectSpell(rulesetCaster, spellRepertoire, Invisibility, 2, false);

        var actionParams = new CharacterActionParams(caster, ActionDefinitions.Id.CastNoCost)
        {
            ActionModifiers = { new ActionModifier() },
            RulesetEffect = effectSpell,
            SpellRepertoire = spellRepertoire,
            TargetCharacters = { caster }
        };

        rulesetCaster.SpellsCastByMe.TryAdd(effectSpell);
        actionService.ExecuteAction(actionParams, null, true);
    }

    // a random creature within 60 feet of you becomes poisoned for 1 hour
    private static void HandleWildSurgeD08(GameLocationCharacter caster)
    {
        var rulesetCaster = caster.RulesetCharacter;
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);
        var targets = contenders
            .Where(x => x.IsWithinRange(caster, 12) && x != caster)
            .ToList();

        var random = new Random();
        var index = random.Next(targets.Count);
        var target = targets.ElementAt(index);
        var rulesetTarget = target.RulesetCharacter;

        rulesetTarget.InflictCondition(
            ConditionPoisoned,
            DurationType.Hour,
            1,
            TurnOccurenceType.EndOfTurn,
            AttributeDefinitions.TagEffect,
            rulesetCaster.guid,
            rulesetCaster.CurrentFaction.Name,
            1,
            ConditionPoisoned,
            0,
            0,
            0);
    }

    // you regain your lowest-level expended spell slot
    private static void HandleWildSurgeD09(GameLocationCharacter caster)
    {
        // empty
    }


    // maximize the damage of the next damaging spell you cast within the next minute
    private static void HandleWildSurgeD10(GameLocationCharacter caster)
    {
        // empty   
    }

    // A random creature within 60 feet of you can fly for a minute
    private static void HandleWildSurgeD11(GameLocationCharacter caster)
    {
        var rulesetCaster = caster.RulesetCharacter;
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);
        var targets = contenders
            .Where(x => x.IsWithinRange(caster, 12) && x != caster)
            .ToList();

        var random = new Random();
        var index = random.Next(targets.Count);
        var target = targets.ElementAt(index);
        var rulesetTarget = target.RulesetCharacter;

        rulesetTarget.InflictCondition(
            ConditionFlying,
            DurationType.Minute,
            1,
            TurnOccurenceType.EndOfTurn,
            AttributeDefinitions.TagEffect,
            rulesetCaster.guid,
            rulesetCaster.CurrentFaction.Name,
            1,
            ConditionFlying,
            0,
            0,
            0);
    }

    // you cast Grease centered on self
    private static void HandleWildSurgeD12(GameLocationCharacter caster)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var rulesetCaster = caster.RulesetCharacter;
        var spellRepertoire =
            rulesetCaster.SpellRepertoires.FirstOrDefault(
                x => x.SpellCastingClass == CharacterClassDefinitions.Sorcerer);
        var effectSpell = ServiceRepository.GetService<IRulesetImplementationService>()
            .InstantiateEffectSpell(rulesetCaster, spellRepertoire, Grease, 1, false);

        var actionParams = new CharacterActionParams(caster, ActionDefinitions.Id.CastNoCost)
        {
            ActionModifiers = { new ActionModifier() },
            RulesetEffect = effectSpell,
            SpellRepertoire = spellRepertoire,
            Positions = { caster.LocationPosition }
        };

        rulesetCaster.SpellsCastByMe.TryAdd(effectSpell);
        actionService.ExecuteAction(actionParams, null, true);
    }

    // you cast Mirror Image
    private static void HandleWildSurgeD13(GameLocationCharacter caster)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var rulesetCaster = caster.RulesetCharacter;
        var spellRepertoire =
            rulesetCaster.SpellRepertoires.FirstOrDefault(
                x => x.SpellCastingClass == CharacterClassDefinitions.Sorcerer);
        var effectSpell = ServiceRepository.GetService<IRulesetImplementationService>()
            .InstantiateEffectSpell(rulesetCaster, spellRepertoire, SpellsContext.MirrorImage, 2, false);

        var actionParams = new CharacterActionParams(caster, ActionDefinitions.Id.CastNoCost)
        {
            ActionModifiers = { new ActionModifier() },
            RulesetEffect = effectSpell,
            SpellRepertoire = spellRepertoire,
            Positions = { caster.LocationPosition }
        };

        rulesetCaster.SpellsCastByMe.TryAdd(effectSpell);
        actionService.ExecuteAction(actionParams, null, true);
    }

    // each creature within 30 feet of you (other than you) takes 1d10 necrotic damage. You regain hit points equal to the sum of the necrotic damage dealt
    private static void HandleWildSurgeD14(GameLocationCharacter caster)
    {
        // empty
    }

    // you cast invisibility on self and each creature within 30 ft
    private static void HandleWildSurgeD15(GameLocationCharacter caster)
    {
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var rulesetCaster = caster.RulesetCharacter;
        var spellRepertoire =
            rulesetCaster.SpellRepertoires.FirstOrDefault(
                x => x.SpellCastingClass == CharacterClassDefinitions.Sorcerer);
        var effectSpell = ServiceRepository.GetService<IRulesetImplementationService>()
            .InstantiateEffectSpell(rulesetCaster, spellRepertoire, Invisibility, 2, false);

        var targets = contenders.Where(x => x.IsWithinRange(caster, 6)).ToList();
        var actionModifiers = new List<ActionModifier>();

        for (var i = 0; i < targets.Count; i++)
        {
            actionModifiers.Add(new ActionModifier());
        }

        var actionParams = new CharacterActionParams(caster, ActionDefinitions.Id.CastNoCost)
        {
            ActionModifiers = actionModifiers,
            RulesetEffect = effectSpell,
            SpellRepertoire = spellRepertoire,
            targetCharacters = targets
        };

        rulesetCaster.SpellsCastByMe.TryAdd(effectSpell);
        actionService.ExecuteAction(actionParams, null, true);
    }

    // you can take one additional action immediately
    private static void HandleWildSurgeD16(GameLocationCharacter caster)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var actionParams = new CharacterActionParams(caster, ActionDefinitions.Id.ActionSurge)
        {
            ActionModifiers = { new ActionModifier() }, TargetCharacters = { caster }
        };

        actionService.ExecuteAction(actionParams, null, true);
    }

    // you and all creatures within 30 feet of you gain vulnerability to piercing damage for the next minute
    private static void HandleWildSurgeD17(GameLocationCharacter caster)
    {
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);
        var rulesetCaster = caster.RulesetCharacter;

        var targets = contenders.Where(x => x.IsWithinRange(caster, 6)).ToList();

        foreach (var rulesetTarget in targets.Select(target => target.RulesetCharacter))
        {
            rulesetTarget.InflictCondition(
                ConditionPiercingVulnerability.Name,
                DurationType.Minute,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCaster.guid,
                rulesetCaster.CurrentFaction.Name,
                1,
                ConditionPiercingVulnerability.Name,
                0,
                0,
                0);
        }
    }

    // you gain resistance to all damage for the next minute
    private static void HandleWildSurgeD18(GameLocationCharacter caster)
    {
        var rulesetCaster = caster.RulesetCharacter;

        rulesetCaster.InflictCondition(
            ConditionDamageResistance.Name,
            DurationType.Minute,
            1,
            TurnOccurenceType.EndOfTurn,
            AttributeDefinitions.TagEffect,
            rulesetCaster.guid,
            rulesetCaster.CurrentFaction.Name,
            1,
            ConditionDamageResistance.Name,
            0,
            0,
            0);
    }

    // up to three creatures you choose within 30 feet of you take 4d10 lightning damage as a free action before your turn ends
    private static void HandleWildSurgeD19(GameLocationCharacter caster)
    {
        var rulesetCaster = caster.RulesetCharacter;

        rulesetCaster.InflictCondition(
            ConditionLightningStrike.Name,
            DurationType.Round,
            0,
            TurnOccurenceType.EndOfTurn,
            AttributeDefinitions.TagEffect,
            rulesetCaster.guid,
            rulesetCaster.CurrentFaction.Name,
            1,
            ConditionLightningStrike.Name,
            0,
            0,
            0);
    }

    // you regain all expended sorcery points
    private static void HandleWildSurgeD20(GameLocationCharacter caster)
    {
        var rulesetCaster = caster.RulesetCharacter;

        rulesetCaster.UsedSorceryPoints = 0;
        rulesetCaster.SorceryPointsAltered?.Invoke(rulesetCaster, rulesetCaster.RemainingSorceryPoints);
    }

    #endregion
}
