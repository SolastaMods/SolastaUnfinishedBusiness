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
using SolastaUnfinishedBusiness.Spells;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class SorcerousWildMagic : AbstractSubclass
{
    private const string Name = "SorcerousWildMagic";
    private const ActionDefinitions.Id TidesOfChaosToggle = (ActionDefinitions.Id)ExtraActionId.TidesOfChaosToggle;

    //TODO: remove before release
    internal static bool ForceWildSurge = true;

    //TODO: remove before release
    internal static int ForceWildSurgeRoll = 2;

    private static readonly List<FeatureDefinitionPower> WildSurgePowers = [];

    private static readonly FeatureDefinition FeatureWildMagicSurge = FeatureDefinitionBuilder
        .Create($"Feature{Name}WildMagicSurge")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new CustomBehaviorWildMagicSurge())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerTidesOfChaos = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}TidesOfChaos")
        .SetGuiPresentation(Category.Feature)
        .SetUsesProficiencyBonus(ActivationTime.NoCost)
        .AddCustomSubFeatures(new CustomBehaviorTidesOfChaos(), ModifyPowerVisibility.Hidden)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerControlledChaos = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}ControlledChaos")
        .SetGuiPresentation(Category.Feature)
        .SetUsesFixed(ActivationTime.NoCost)
        .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
        .AddToDB();

    private static readonly FeatureDefinition FeatureSpellBombardment = FeatureDefinitionBuilder
        .Create($"Feature{Name}SpellBombardment")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerGrease = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}Grease")
        .SetGuiPresentation($"Power{Name}D12", Category.Feature)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create(Grease)
                .Build())
        .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerNecroticDamage = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}NecroticDamage")
        .SetGuiPresentation($"Power{Name}D14", Category.Feature)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeNecrotic, 1, DieType.D10))
                .Build())
        .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
        .AddToDB();

    private static readonly ConditionDefinition ConditionTidesOfChaos = ConditionDefinitionBuilder
        .Create($"Condition{Name}TidesOfChaos")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .AddToDB();

    private static readonly ConditionDefinition ConditionChaos = ConditionDefinitionBuilder
        .Create($"Condition{Name}Chaos")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionConjuredCreature)
        .SetPossessive()
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
        .SetPossessive()
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
        .SetPossessive()
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

    private static readonly ConditionDefinition ConditionMaxDamageRolls = ConditionDefinitionBuilder
        .Create($"Condition{Name}MaxDamageRolls")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .AddToDB();

    public SorcerousWildMagic()
    {
        // LEVEL 01

        // Wild Magic Surge

        PowerNecroticDamage.EffectDescription.EffectForms[0].DamageForm.healFromInflictedDamage =
            HealFromInflictedDamage.Full;

        // Tides of Chaos

        _ = ActionDefinitionBuilder
            .Create(MetamagicToggle, "TidesOfChaosToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.TidesOfChaosToggle)
            .SetActivatedPower(PowerTidesOfChaos)
            .AddToDB();

        var actionAffinityImpishWrathToggle = FeatureDefinitionActionAffinityBuilder
            .Create(FeatureDefinitionActionAffinitys.ActionAffinitySorcererMetamagicToggle,
                "ActionAffinityTidesOfChaosToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(TidesOfChaosToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(PowerTidesOfChaos)))
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

        // these powers should not be added to progression
        for (var i = 1; i <= 20; i++)
        {
            WildSurgePowers.Add(
                FeatureDefinitionPowerSharedPoolBuilder
                    .Create($"Power{Name}D{i:D02}")
                    .SetGuiPresentation(Category.Feature, hidden: true)
                    .SetSharedPool(ActivationTime.NoCost, PowerControlledChaos)
                    .AddToDB());
        }

        PowerBundle.RegisterPowerBundle(PowerControlledChaos, false, WildSurgePowers);

        // LEVEL 18

        // Spell Bombardment

        // Main

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.SorcererWildMagic, 256))
            .AddFeaturesAtLevel(1,
                FeatureWildMagicSurge, PowerTidesOfChaos, actionAffinityImpishWrathToggle)
            .AddFeaturesAtLevel(6, powerBendLuck)
            .AddFeaturesAtLevel(14, PowerControlledChaos)
            .AddFeaturesAtLevel(18, FeatureSpellBombardment)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Sorcerer;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceSorcerousOrigin;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Spell Bombardment (also handles HandleWildSurgeD10)
    //

    internal static void HandleSpellBombardment(
        RulesetCharacter rulesetCharacter, DamageForm damageForm, List<int> rolledValues, ref int damage)
    {
        var character = GameLocationCharacter.GetFromActor(rulesetCharacter);
        var levels = rulesetCharacter.GetSubclassLevel(CharacterClassDefinitions.Sorcerer, Name);
        var dieType = damageForm.DieType;
        var maxDie = DiceMaxValue[(int)dieType];
        var tag = FeatureSpellBombardment.Name;

        if (levels < 18 ||
            !rolledValues.Contains(maxDie) ||
            !character.OncePerTurnIsValid(tag))
        {
            HandleWildSurgeD10(rulesetCharacter, rolledValues, maxDie, ref damage);

            return;
        }

        character.UsedSpecialFeatures.TryAdd(tag, 0);
        rulesetCharacter.LogCharacterUsedFeature(FeatureSpellBombardment);

        var roll = RollDie(dieType, AdvantageType.None, out _, out _);

        rolledValues.Add(roll);
        damage += roll;

        HandleWildSurgeD10(rulesetCharacter, rolledValues, maxDie, ref damage);
    }

    //
    // Wild Surge
    //

    private static IEnumerator HandleWildSurge(GameLocationCharacter attacker)
    {
        var rulesetAttacker = attacker.RulesetCharacter;
        var levels = rulesetAttacker.GetSubclassLevel(CharacterClassDefinitions.Sorcerer, Name);

        if (levels >= 14)
        {
            yield return HandleControlledChaos(attacker);
            yield break;
        }

        var selectedRoll = RollDie(DieType.D20, AdvantageType.None, out _, out _);

        if (Gui.Battle == null)
        {
            while (selectedRoll == 1)
            {
                selectedRoll = RollDie(DieType.D20, AdvantageType.None, out _, out _);
            }
        }

        //TODO: remove before release
        if (ForceWildSurgeRoll > 0)
        {
            selectedRoll = ForceWildSurgeRoll;
        }

        var selectedPower = WildSurgePowers[selectedRoll - 1];

        rulesetAttacker.ShowDieRoll(DieType.D20, selectedRoll, title: FeatureWildMagicSurge.GuiPresentation.Title);

        ApplyWildSurge(attacker, selectedRoll,
            FeatureWildMagicSurge.FormatTitle(), selectedPower.FormatTitle(), "Feedback/&WidSurgeDieRoll");
    }

    //
    // Controlled Chaos
    //

    private static IEnumerator HandleControlledChaos(GameLocationCharacter attacker)
    {
        var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
        var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

        if (!actionManager || !battleManager)
        {
            yield break;
        }

        var wildSurgeDie1 = RollDie(DieType.D20, AdvantageType.None, out _, out _);
        var wildSurgeDie2 = RollDie(DieType.D20, AdvantageType.None, out _, out _);

        if (Gui.Battle == null)
        {
            while (wildSurgeDie1 == 1)
            {
                wildSurgeDie1 = RollDie(DieType.D20, AdvantageType.None, out _, out _);
            }

            while (wildSurgeDie2 == 1)
            {
                wildSurgeDie2 = RollDie(DieType.D20, AdvantageType.None, out _, out _);
            }
        }

        //TODO: remove before release
        if (ForceWildSurgeRoll > 0)
        {
            wildSurgeDie1 = ForceWildSurgeRoll;
        }

        var rulesetAttacker = attacker.RulesetCharacter;

        rulesetAttacker.ShowDieRoll(DieType.D20, wildSurgeDie1, title: FeatureWildMagicSurge.GuiPresentation.Title);
        rulesetAttacker.ShowDieRoll(DieType.D20, wildSurgeDie2, title: FeatureWildMagicSurge.GuiPresentation.Title);

        rulesetAttacker.LogCharacterActivatesAbility(
            PowerControlledChaos.FormatTitle(),
            "Feedback/&ControlledChaosDieRoll",
            extra:
            [
                (ConsoleStyleDuplet.ParameterType.Positive, wildSurgeDie1.ToString()),
                (ConsoleStyleDuplet.ParameterType.Positive, wildSurgeDie2.ToString())
            ]);

        var usablePowerPool = PowerProvider.Get(PowerControlledChaos, rulesetAttacker);
        var usablePowerFirst = PowerProvider.Get(WildSurgePowers[wildSurgeDie1 - 1], rulesetAttacker);
        var usablePowerSecond = PowerProvider.Get(WildSurgePowers[wildSurgeDie2 - 1], rulesetAttacker);

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
            selectedPower = WildSurgePowers.ElementAt(reactionRequest.SelectedSubOption);
            selectedRoll = reactionRequest.SelectedSubOption + 1;
        }
        else
        {
            var choiceRoll = RollDie(DieType.D2, AdvantageType.None, out _, out _);
            var choice = choiceRoll == 1 ? wildSurgeDie1 : wildSurgeDie2;

            selectedPower = WildSurgePowers.ElementAt(choice - 1);
            selectedRoll = choice;
        }

        ApplyWildSurge(attacker, selectedRoll,
            FeatureWildMagicSurge.FormatTitle(), selectedPower.FormatTitle(), "Feedback/&ControlledChaosDieChoice");
    }

    //
    // Wild Magic Surge
    //

    private sealed class CustomBehaviorWildMagicSurge : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterActionMagicEffect action, GameLocationCharacter attacker, List<GameLocationCharacter> targets)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (action is not CharacterActionCastSpell actionCastSell ||
                actionCastSell.ActiveSpell.SpellDefinition.SpellLevel == 0 ||
                rulesetAttacker.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, ConditionChaos.Name))
            {
                yield break;
            }

            bool shouldRollWildSurge;

            // force wild surge roll if tides of chaos was used
            if (rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionTidesOfChaos.Name, out var activeConditionSource))
            {
                rulesetAttacker.RemoveCondition(activeConditionSource);
                rulesetAttacker.LogCharacterActivatesAbility(
                    PowerTidesOfChaos.GuiPresentation.Title, "Feedback/&TidesOfChaosForcedSurge");

                shouldRollWildSurge = true;
            }
            // otherwise roll chance die
            else
            {
                var chanceDie = RollDie(DieType.D20, AdvantageType.None, out _, out _);

                //TODO: remove before release
                if (ForceWildSurge)
                {
                    chanceDie = 1;
                }

                shouldRollWildSurge = chanceDie <= ForceWildSurgeRoll;

                rulesetAttacker.ShowDieRoll(DieType.D20, chanceDie, title: FeatureWildMagicSurge.GuiPresentation.Title);
                rulesetAttacker.LogCharacterActivatesAbility(
                    FeatureWildMagicSurge.GuiPresentation.Title,
                    "Feedback/&WidSurgeChanceDieRoll",
                    extra:
                    [
                        (shouldRollWildSurge ? ConsoleStyleDuplet.ParameterType.Negative : ConsoleStyleDuplet.ParameterType.Positive,
                            chanceDie.ToString())
                    ]);
            }

            if (shouldRollWildSurge)
            {
                yield return HandleWildSurge(attacker);
            }
        }
    }

    //
    // Tides of Chaos
    //

    private sealed class CustomBehaviorTidesOfChaos : ITryAlterOutcomeAttack, ITryAlterOutcomeSavingThrow
    {
        public int HandlerPriority => -9; // ensure it triggers after bend of luck

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
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!battleManager)
            {
                yield break;
            }

            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(PowerTidesOfChaos, rulesetHelper);

            if (action.AttackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure) ||
                helper != attacker ||
                !helper.OncePerTurnIsValid(PowerTidesOfChaos.Name) ||
                !rulesetHelper.IsToggleEnabled(TidesOfChaosToggle) ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            helper.UsedSpecialFeatures.TryAdd(PowerTidesOfChaos.Name, 0);
            rulesetHelper.UsePower(usablePower);

            var advantageTrends =
                new List<TrendInfo>
                {
                    new(1, FeatureSourceType.CharacterFeature, PowerTidesOfChaos.Name, PowerTidesOfChaos)
                };

            actionModifier.AttackAdvantageTrends.SetRange(advantageTrends);

            var outcome = action.AttackRollOutcome;
            var rollCaption = outcome == RollOutcome.CriticalFailure
                ? "Feedback/&RollAttackCriticalFailureTitle"
                : "Feedback/&RollAttackFailureTitle";
            var attackRoll = action.AttackRoll;
            int roll;
            int toHitBonus;
            int successDelta;

            if (attackMode != null)
            {
                toHitBonus = attackMode.ToHitBonus + actionModifier.AttackRollModifier;
                roll = rulesetHelper.RollAttack(
                    attackMode.ToHitBonus,
                    defender.RulesetActor,
                    attackMode.SourceDefinition,
                    attackMode.ToHitBonusTrends,
                    false,
                    actionModifier.AttackAdvantageTrends,
                    attackMode.ranged,
                    false,
                    actionModifier.AttackRollModifier,
                    out outcome,
                    out successDelta,
                    -1,
                    true);
            }
            else if (rulesetEffect != null)
            {
                toHitBonus = rulesetEffect.MagicAttackBonus + actionModifier.AttackRollModifier;
                roll = rulesetHelper.RollMagicAttack(
                    rulesetEffect,
                    defender.RulesetActor,
                    rulesetEffect.GetEffectSource(),
                    actionModifier.AttacktoHitTrends,
                    actionModifier.AttackAdvantageTrends,
                    false,
                    actionModifier.AttackRollModifier,
                    out outcome,
                    out successDelta,
                    -1,
                    true);
            }
            // should never happen
            else
            {
                yield break;
            }

            var sign = toHitBonus > 0 ? "+" : string.Empty;

            rulesetHelper.LogCharacterUsedFeature(
                PowerTidesOfChaos,
                "Feedback/&TriggerRerollLine",
                false,
                (ConsoleStyleDuplet.ParameterType.Base, $"{attackRoll}{sign}{toHitBonus}"),
                (ConsoleStyleDuplet.ParameterType.FailedRoll,
                    Gui.Format(rollCaption, $"{attackRoll + toHitBonus}")));

            action.AttackRollOutcome = outcome;
            action.AttackSuccessDelta = successDelta;
            action.AttackRoll = roll;

            InflictConditionOnCreaturesWithinRange(attacker, ConditionTidesOfChaos.Name, DurationType.UntilLongRest);
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            bool hasHitVisual,
            bool hasBorrowedLuck)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(PowerTidesOfChaos, rulesetHelper);

            if (!action.RolledSaveThrow ||
                action.SaveOutcome != RollOutcome.Failure ||
                helper != defender ||
                !helper.OncePerTurnIsValid(PowerTidesOfChaos.Name) ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var reactionParams = new CharacterActionParams(helper, ActionDefinitions.Id.SpendPower)
            {
                StringParameter = "TidesOfChaos",
                StringParameter2 = "SpendPowerTidesOfChaosDescription"
                    .Formatted(Category.Reaction, defender.Name, attacker.Name, action.FormatTitle()),
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                UsablePower = usablePower
            };

            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToSpendPower(reactionParams);

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            helper.UsedSpecialFeatures.TryAdd(PowerTidesOfChaos.Name, 0);
            rulesetHelper.UsePower(usablePower);

            rulesetHelper.LogCharacterActivatesAbility(
                PowerTidesOfChaos.GuiPresentation.Title, "Feedback/&TidesOfChaosSavingDieRoll");

            var advantageTrends =
                new List<TrendInfo>
                {
                    new(1, FeatureSourceType.CharacterFeature, PowerTidesOfChaos.Name, PowerTidesOfChaos)
                };

            actionModifier.SavingThrowAdvantageTrends.SetRange(advantageTrends);

            action.RolledSaveThrow = action.ActionParams.RulesetEffect == null
                ? action.ActionParams.AttackMode.TryRollSavingThrow(
                    attacker.RulesetCharacter,
                    defender.RulesetActor,
                    actionModifier, action.ActionParams.AttackMode.EffectDescription.EffectForms,
                    out var saveOutcome, out var saveOutcomeDelta)
                : action.ActionParams.RulesetEffect.TryRollSavingThrow(
                    attacker.RulesetCharacter,
                    attacker.Side,
                    defender.RulesetActor,
                    actionModifier, action.ActionParams.RulesetEffect.EffectDescription.EffectForms, hasHitVisual,
                    out saveOutcome, out saveOutcomeDelta);

            action.SaveOutcome = saveOutcome;
            action.SaveOutcomeDelta = saveOutcomeDelta;

            InflictConditionOnCreaturesWithinRange(attacker, ConditionTidesOfChaos.Name, DurationType.UntilLongRest);
        }
    }

    //
    // Bend Luck
    //

    private sealed class CustomBehaviorBendLuck(FeatureDefinitionPower powerBendLuck)
        : ITryAlterOutcomeAttack, ITryAlterOutcomeAttributeCheck, ITryAlterOutcomeSavingThrow
    {
        public int HandlerPriority => -10;

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

    private static void ApplyWildSurge(
        GameLocationCharacter caster, int roll, string featureTitle, string selectedPowerTitle, string feedback)
    {
        var rulesetCaster = caster.RulesetCharacter;

        rulesetCaster.LogCharacterActivatesAbility(
            featureTitle,
            feedback,
            extra:
            [
                (ConsoleStyleDuplet.ParameterType.Positive, roll.ToString()),
                (ConsoleStyleDuplet.ParameterType.AttackSpellPower, selectedPowerTitle)
            ]);

        switch (roll)
        {
            // trigger a random Wild Surge effect (except this one) at the start of each of your turns for the next minute
            case 1:
                InflictConditionOnCreaturesWithinRange(caster, ConditionChaos.Name, DurationType.Minute);
                break;

            // you cast Fireball centered on self
            case 2:
                CastSpellWithoutConsumption(caster, Fireball, 3);
                break;

            // you regain 2D10 hit points
            case 3:
                var healing = rulesetCaster.RollDiceAndSum(DieType.D10, RollContext.HealValueRoll, 2, [], false);

                rulesetCaster.ReceiveHealing(healing, true, rulesetCaster.Guid);
                break;

            // each creature within 20 feet of you (including you) catches on fire
            case 4:
                InflictConditionOnCreaturesWithinRange(
                    caster, ConditionDefinitions.ConditionOnFire1D4.Name,
                    DurationType.Minute, 1, TurnOccurenceType.EndOfTurn, 4);
                break;

            // You can teleport up to 60 feet to an unoccupied space of your choice that you can see as a free action before your turn ends
            case 5:
                InflictConditionOnCreaturesWithinRange(caster, ConditionTeleport.Name, DurationType.Round, 0);
                break;

            // you become frightened until the end of your next turn
            case 6:
                InflictConditionOnCreaturesWithinRange(caster, ConditionFrightened, DurationType.Round);
                break;

            // you cast invisibility on self
            case 7:
                InflictConditionOnCreaturesWithinRange(
                    caster, ConditionDefinitions.ConditionInvisible.Name, DurationType.Minute);
                break;

            // a random creature within 60 feet of you becomes poisoned for 1 hour
            case 8:
                InflictConditionOnRandomCreatureWithinRange(
                    caster, ConditionPoisoned, DurationType.Hour, 1, TurnOccurenceType.EndOfTurn, 12);
                break;

            // you regain your lowest-level expended spell slot
            case 9:
                var levels = caster.RulesetCharacter.GetSubclassLevel(CharacterClassDefinitions.Sorcerer, Name);
                var slotLevel = (levels + 3) / 4;

                InflictConditionOnCreaturesWithinRange(
                    caster, $"ConditionAdditionalSpellSlot{slotLevel}", DurationType.UntilLongRest);
                break;

            // maximize the damage of the next damaging spell you cast within the next minute
            case 10:
                InflictConditionOnCreaturesWithinRange(caster, ConditionMaxDamageRolls.Name, DurationType.Minute);
                break;

            // A random creature within 60 feet of you can fly for a minute
            case 11:
                InflictConditionOnRandomCreatureWithinRange(
                    caster, ConditionFlying, DurationType.Minute, 1, TurnOccurenceType.EndOfTurn, 12);
                break;

            // you cast grease centered on self
            case 12:
                HandleWildSurgeD12(caster);
                break;

            // you cast mirror image on self
            case 13:
                InflictConditionOnCreaturesWithinRange(
                    caster, SpellBuilders.ConditionMirrorImageMark.Name, DurationType.Minute);
                break;

            case 14:
                HandleWildSurgeD14(caster);
                break;

            // you cast invisibility on self and each creature within 30 ft
            case 15:
                InflictConditionOnCreaturesWithinRange(
                    caster, ConditionDefinitions.ConditionInvisible.Name,
                    DurationType.Minute, 1, TurnOccurenceType.EndOfTurn, 6);
                break;

            case 16:
                HandleWildSurgeD16(caster);
                break;

            // each creature within 30 feet of you (including you) gain vulnerability to piercing damage for the next minute
            case 17:
                InflictConditionOnCreaturesWithinRange(
                    caster, ConditionPiercingVulnerability.Name,
                    DurationType.Minute, 1, TurnOccurenceType.EndOfTurn, 6);
                break;

            // you gain resistance to all damage for the next minute
            case 18:
                InflictConditionOnCreaturesWithinRange(caster, ConditionDamageResistance.Name, DurationType.Minute);
                break;

            // up to three creatures you choose within 30 feet of you take 4d10 lightning damage as a free action before your turn ends
            case 19:
                InflictConditionOnCreaturesWithinRange(caster, ConditionLightningStrike.Name, DurationType.Round, 0);
                break;

            // you gain all expended sorcery points
            case 20:
                rulesetCaster.UsedSorceryPoints = 0;
                rulesetCaster.SorceryPointsAltered?.Invoke(rulesetCaster, rulesetCaster.RemainingSorceryPoints);
                break;
        }
    }

    private static void InflictConditionOnRandomCreatureWithinRange(
        GameLocationCharacter caster,
        string conditionName,
        DurationType durationType,
        int durationParameter,
        TurnOccurenceType turnOccurenceType,
        int range)
    {
        var rulesetCaster = caster.RulesetCharacter;
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);
        var targets = contenders
            .Where(x => x.IsWithinRange(caster, range) && x != caster)
            .ToList();

        var random = new Random();
        var index = random.Next(targets.Count);
        var target = targets.ElementAt(index);
        var rulesetTarget = target.RulesetCharacter;

        rulesetTarget.InflictCondition(
            conditionName,
            durationType,
            durationParameter,
            turnOccurenceType,
            AttributeDefinitions.TagEffect,
            rulesetCaster.guid,
            rulesetCaster.CurrentFaction.Name,
            1,
            conditionName,
            0,
            0,
            0);
    }

    private static void InflictConditionOnCreaturesWithinRange(
        GameLocationCharacter caster,
        string conditionName,
        DurationType durationType,
        int durationParameter = 1,
        TurnOccurenceType turnOccurenceType = TurnOccurenceType.EndOfTurn,
        int range = 0)
    {
        var rulesetCaster = caster.RulesetCharacter;
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);
        var targets = contenders
            .Where(x => x.IsWithinRange(caster, range))
            .ToList();

        foreach (var rulesetTarget in targets.Select(target => target.RulesetCharacter))
        {
            rulesetTarget.InflictCondition(
                conditionName,
                durationType,
                durationParameter,
                turnOccurenceType,
                AttributeDefinitions.TagEffect,
                rulesetCaster.guid,
                rulesetCaster.CurrentFaction.Name,
                1,
                conditionName,
                0,
                0,
                0);
        }
    }

    private static void CastSpellWithoutConsumption(
        GameLocationCharacter caster, SpellDefinition spell, int slotLevel)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var rulesetCaster = caster.RulesetCharacter;
        var spellRepertoire =
            rulesetCaster.SpellRepertoires.FirstOrDefault(
                x => x.SpellCastingClass == CharacterClassDefinitions.Sorcerer);
        var effectSpell = ServiceRepository.GetService<IRulesetImplementationService>()
            .InstantiateEffectSpell(rulesetCaster, spellRepertoire, spell, slotLevel, false);

        var actionParams = new CharacterActionParams(caster, ActionDefinitions.Id.CastNoCost)
        {
            ActionModifiers = { new ActionModifier() },
            RulesetEffect = effectSpell,
            SpellRepertoire = spellRepertoire,
            TargetCharacters = { caster },
            Positions = { caster.LocationPosition }
        };

        if (Main.Settings.UseAlternateSpellPointsSystem)
        {
            SpellPointsContext.GrantPoints(rulesetCaster, slotLevel);
        }
        else
        {
            foreach (var repertoire in rulesetCaster.SpellRepertoires.Where(x =>
                         x.SpellCastingClass != CharacterClassDefinitions.Warlock &&
                         x.SpellCastingFeature.SpellCastingOrigin
                             is FeatureDefinitionCastSpell.CastingOrigin.Class
                             or FeatureDefinitionCastSpell.CastingOrigin.Subclass))
            {
                repertoire.RecoverMissingSlots(new Dictionary<int, int> { { slotLevel, 1 } });
            }
        }

        actionService.ExecuteAction(actionParams, null, true);
    }

    private static void HandleWildSurgeD10(
        RulesetCharacter rulesetCharacter, List<int> rolledValues, int maxDie, ref int damage)
    {
        if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, $"Condition{Name}MaxDamageRolls", out var activeCondition))
        {
            return;
        }

        rulesetCharacter.RemoveCondition(activeCondition);

        for (var i = 0; i < rolledValues.Count; i++)
        {
            rolledValues[i] = maxDie;
        }

        damage = rolledValues.Count * maxDie;
    }

    // you cast grease centered on self
    private static void HandleWildSurgeD12(GameLocationCharacter caster)
    {
        var rulesetCaster = caster.RulesetCharacter;
        var implementationManager =
            ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

        var usablePower = PowerProvider.Get(PowerGrease, rulesetCaster);
        var actionParams = new CharacterActionParams(caster, ActionDefinitions.Id.SpendPower)
        {
            ActionModifiers = { new ActionModifier() },
            RulesetEffect = implementationManager
                .MyInstantiateEffectPower(rulesetCaster, usablePower, false),
            UsablePower = usablePower,
            Positions = { caster.LocationPosition }
        };

        ServiceRepository.GetService<IGameLocationActionService>()?
            .ExecuteAction(actionParams, null, true);
    }

    // each creature within 30 feet of you (other than you) takes 1d10 necrotic damage. You regain hit points equal to the sum of the necrotic damage dealt
    private static void HandleWildSurgeD14(GameLocationCharacter caster)
    {
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);
        var rulesetCaster = caster.RulesetCharacter;

        var targets = contenders
            .Where(x => x.IsWithinRange(caster, 6) && x != caster)
            .ToList();

        var actionModifiers = new List<ActionModifier>();

        for (var i = 0; i < targets.Count; i++)
        {
            actionModifiers.Add(new ActionModifier());
        }

        var implementationManager =
            ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

        var usablePower = PowerProvider.Get(PowerNecroticDamage, rulesetCaster);

        var actionParams = new CharacterActionParams(caster, ActionDefinitions.Id.SpendPower)
        {
            ActionModifiers = actionModifiers,
            RulesetEffect = implementationManager
                .MyInstantiateEffectPower(rulesetCaster, usablePower, false),
            UsablePower = usablePower,
            targetCharacters = targets
        };

        ServiceRepository.GetService<IGameLocationActionService>()?
            .ExecuteAction(actionParams, null, true);
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

    private sealed class CharacterTurnStartListenerChaos(
        FeatureDefinition featureWildSurge) : ICharacterTurnStartListener
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

    #endregion
}
