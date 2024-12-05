using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.Infrastructure;
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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class SorcerousWildMagic : AbstractSubclass
{
    private const string Name = "SorcerousWildMagic";
    private const ActionDefinitions.Id TidesOfChaosRecharge = (ActionDefinitions.Id)ExtraActionId.TidesOfChaosRecharge;

    //
    // subclass features
    //

    private static readonly FeatureDefinitionPower PowerTidesOfChaos = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}TidesOfChaos")
        .SetGuiPresentation(Category.Feature)
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
        .SetShowCasting(false)
        .AddCustomSubFeatures(new CustomBehaviorTidesOfChaos(), ModifyPowerVisibility.Hidden)
        .AddToDB();

    // used power for a better combat log tooltip
    private static readonly FeatureDefinitionPower PowerWildMagicSurge = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}WildMagicSurge")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new CustomBehaviorWildMagicSurge(), ModifyPowerVisibility.Hidden)
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
        .AddCustomSubFeatures(new CustomBehaviorSpellBombardment())
        .AddToDB();

    private static readonly ConditionDefinition ConditionTidesOfChaosRechargeMark = ConditionDefinitionBuilder
        .Create($"Condition{Name}TidesOfChaosRechargeMark")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
        .AddToDB();

    private static readonly ConditionDefinition ConditionWildSurgeMark = ConditionDefinitionBuilder
        .Create($"Condition{Name}WildSurgeMark")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
        .AddToDB();

    //
    // Wild Surge Powers
    //

    private static readonly List<FeatureDefinitionPower> WildSurgePowers = [];

    private static readonly FeatureDefinitionPower PowerFireball = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}Fireball")
        .SetGuiPresentation($"Power{Name}D02", Category.Feature)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetShowCasting(false)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create(Fireball)
                .UseQuickAnimations()
                .Build())
        .AddCustomSubFeatures(TidesOfChaosRepayTides.Marker)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerGrease = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}Grease")
        .SetGuiPresentation($"Power{Name}D12", Category.Feature)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetShowCasting(false)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create(Grease)
                .UseQuickAnimations()
                .Build())
        .AddCustomSubFeatures(TidesOfChaosRepayTides.Marker)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerLightningStrike = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}LightningStrike")
        .SetGuiPresentation("PowerSorcerousWildMagicD19", Category.Feature, LightningBolt)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique, 3)
                .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeLightning, 4, DieType.D10))
                .SetParticleEffectParameters(LightningBolt)
                .Build())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerWildHealing = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}WildHealing")
        .SetGuiPresentation($"Power{Name}D14", Category.Feature)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetShowCasting(false)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeNecrotic, 1, DieType.D10))
                .SetParticleEffectParameters(PowerWightLordRetaliate)
                .Build())
        .AddCustomSubFeatures( /*new CustomBehaviorWildHealing(),*/ TidesOfChaosRepayTides.Marker)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerTeleport = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}Teleport")
        .SetGuiPresentation("PowerSorcerousWildMagicD05", Category.Feature, MistyStep)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetShowCasting(false)
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
        .AddCustomSubFeatures(new PowerOrSpellInitiatedByMeTeleport(), TidesOfChaosRepayTides.Marker)
        .AddToDB();

    private static readonly ConditionDefinition ConditionChaos = ConditionDefinitionBuilder
        .Create($"Condition{Name}Chaos")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionConjuredCreature)
        .SetPossessive()
        .SetConditionType(ConditionType.Neutral)
        .AddToDB();

    private static readonly ConditionDefinition ConditionDamageResistance = ConditionDefinitionBuilder
        .Create($"Condition{Name}DamageResistance")
        .SetGuiPresentation("PowerSorcerousWildMagicD18", Category.Feature,
            ConditionDefinitions.ConditionMagicallyArmored)
        .SetPossessive()
        .SetFeatures(
            DamageAffinityAcidResistance,
            DamageAffinityBludgeoningResistanceTrue,
            DamageAffinityColdResistance,
            DamageAffinityFireResistance,
            DamageAffinityForceDamageResistance,
            DamageAffinityLightningResistance,
            DamageAffinityNecroticResistance,
            DamageAffinityPiercingResistanceTrue,
            DamageAffinityPoisonResistance,
            DamageAffinityPsychicResistance,
            DamageAffinityRadiantResistance,
            DamageAffinitySlashingResistanceTrue,
            DamageAffinityThunderResistance)
        .SetConditionParticleReference(ConjureCelestialCouatl)
        .AddToDB();

    private static readonly ConditionDefinition ConditionLightningStrike = ConditionDefinitionBuilder
        .Create($"Condition{Name}LightningStrike")
        .SetGuiPresentation("PowerSorcerousWildMagicD19", Category.Feature, ConditionGuided)
        .SetPossessive()
        .SetFeatures(PowerLightningStrike)
        .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
        .AddToDB();

    private static readonly ConditionDefinition ConditionPiercingVulnerability = ConditionDefinitionBuilder
        .Create($"Condition{Name}PiercingVulnerability")
        .SetGuiPresentation("PowerSorcerousWildMagicD17", Category.Feature, ConditionTargetedByGuidingBolt)
        .SetPossessive()
        .SetConditionType(ConditionType.Detrimental)
        .SetFeatures(DamageAffinityPiercingVulnerability)
        .SetConditionParticleReference(Condition_MummyLord_ChannelNegativeEnergy)
        .AddToDB();

    private static readonly ConditionDefinition ConditionMaxDamageRolls = ConditionDefinitionBuilder
        .Create($"Condition{Name}MaxDamageRolls")
        .SetGuiPresentation("PowerSorcerousWildMagicD10", Category.Feature, ConditionAuraOfCourage)
        .SetPossessive()
        .AddCustomSubFeatures(new CustomBehaviorMaxDamageRolls())
        .AddToDB();

    public SorcerousWildMagic()
    {
        // LEVEL 01

        // Wild Magic Surge

        ConditionDamageResistance.GuiPresentation.description = Gui.EmptyContent;
        ConditionPiercingVulnerability.GuiPresentation.description = Gui.EmptyContent;
        ConditionLightningStrike.GuiPresentation.description = Gui.EmptyContent;

        PowerLightningStrike.EffectDescription.EffectForms.Add(
            EffectFormBuilder.ConditionForm(
                ConditionLightningStrike, ConditionForm.ConditionOperation.Remove, true, true));

        PowerWildHealing
            .EffectDescription.EffectForms[0].DamageForm.healFromInflictedDamage = HealFromInflictedDamage.Full;

        // Tides of Chaos

        _ = ActionDefinitionBuilder
            .Create("TidesOfChaosRecharge")
            .SetGuiPresentation(Category.Action, ReapplyEffect)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.TidesOfChaosRecharge)
            .SetActionType(ActionDefinitions.ActionType.NoCost)
            .SetFormType(ActionDefinitions.ActionFormType.Large)
            .AddToDB();

        var actionAffinityTidesOfChaosRecharge = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityTidesOfChaosRecharge")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(TidesOfChaosRecharge)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(
                    c =>
                        !c.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, ConditionChaos.Name) &&
                        !c.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect,
                            ConditionTidesOfChaosRechargeMark.Name),
                    ValidatorsCharacter.HasNotAvailablePowerUsage(PowerTidesOfChaos)))
            .AddToDB();

        var featureSetTidesOfChaos = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}TidesOfChaos")
            .SetGuiPresentation(PowerTidesOfChaos.GuiPresentation)
            .AddFeatureSet(actionAffinityTidesOfChaosRecharge, PowerTidesOfChaos)
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

        // these powers should not be added to progression as they get added dynamically
        // before reaction modal and get removed right after
        for (var i = 1; i <= 20; i++)
        {
            WildSurgePowers.Add(
                FeatureDefinitionPowerSharedPoolBuilder
                    .Create($"Power{Name}D{i:D02}")
                    .SetGuiPresentation(Category.Feature, hidden: true)
                    .SetSharedPool(ActivationTime.NoCost, PowerControlledChaos)
                    .SetShowCasting(false)
                    .AddToDB());
        }

        PowerBundle.RegisterPowerBundle(PowerControlledChaos, false, WildSurgePowers);

        // LEVEL 18

        // Spell Bombardment

        // Main

        SwitchWildSurgeChanceDieThreshold();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.SorcererWildMagic, 256))
            .AddFeaturesAtLevel(1, PowerWildMagicSurge, featureSetTidesOfChaos)
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
    // Tides of Chaos
    //

    internal static IEnumerator HandleRechargeTidesOfChaos(RulesetCharacter rulesetCharacter)
    {
        var character = GameLocationCharacter.GetFromActor(rulesetCharacter);
        var hasChaos = rulesetCharacter.HasConditionOfCategoryAndType(
            AttributeDefinitions.TagEffect, ConditionChaos.Name);

        rulesetCharacter.LogCharacterActivatesAbility(
            PowerTidesOfChaos.GuiPresentation.Title,
            "Feedback/&TidesOfChaosForcedSurge",
            tooltipContent: PowerTidesOfChaos.Name,
            tooltipClass: "PowerDefinition");

        rulesetCharacter.InflictCondition(
            ConditionTidesOfChaosRechargeMark.Name,
            DurationType.Round,
            0,
            TurnOccurenceType.EndOfTurn,
            AttributeDefinitions.TagEffect,
            rulesetCharacter.guid,
            rulesetCharacter.CurrentFaction.Name,
            1,
            ConditionTidesOfChaosRechargeMark.Name,
            0,
            0,
            0);

        yield return HandleWildSurge(character, hasChaos);
    }

    private static void RepayTidesOfChaosIfRequired(RulesetCharacter rulesetCharacter)
    {
        if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, ConditionTidesOfChaosRechargeMark.Name, out var activeCondition) ||
            activeCondition.Amount != 0)
        {
            return;
        }

        var usablePower = PowerProvider.Get(PowerTidesOfChaos, rulesetCharacter);

        usablePower.RepayUse();
        activeCondition.Amount = 1;
    }

    //
    // Wild Surge
    //

    internal static void SwitchWildSurgeChanceDieThreshold()
    {
        PowerWildMagicSurge.GuiPresentation.description = $"{PowerWildMagicSurge.Name}Description"
            .Formatted(Category.Feature, Main.Settings.WildSurgeDieRollThreshold.ToString());
    }

    private static IEnumerator HandleWildSurge(GameLocationCharacter attacker, bool avoidOnes = false)
    {
        var rulesetAttacker = attacker.RulesetCharacter;
        var levels = rulesetAttacker.GetSubclassLevel(CharacterClassDefinitions.Sorcerer, Name);

        if (levels >= 14)
        {
            yield return HandleControlledChaos(attacker, avoidOnes);
            yield break;
        }

        var wildSurgeDie1 = 0;
        var threshold = avoidOnes ? 2 : 1;

        while (wildSurgeDie1 < threshold)
        {
            wildSurgeDie1 = RollDie(DieType.D20, AdvantageType.None, out _, out _);
        }

        var selectedPower = WildSurgePowers[wildSurgeDie1 - 1];

        rulesetAttacker.ShowDieRoll(DieType.D20, wildSurgeDie1, title: PowerWildMagicSurge.GuiPresentation.Title);

        ApplyWildSurge(attacker, wildSurgeDie1, PowerWildMagicSurge, selectedPower, "Feedback/&WidSurgeDieRoll");
    }

    //
    // Controlled Chaos
    //

    private static IEnumerator HandleControlledChaos(GameLocationCharacter attacker, bool avoidOnes = false)
    {
        var threshold = avoidOnes ? 2 : 1;

        var wildSurgeDie1 = 0;

        while (wildSurgeDie1 < threshold)
        {
            wildSurgeDie1 = RollDie(DieType.D20, AdvantageType.None, out _, out _);
        }

        var wildSurgeDie2 = 0;

        while (wildSurgeDie2 < threshold || wildSurgeDie2 == wildSurgeDie1)
        {
            wildSurgeDie2 = RollDie(DieType.D20, AdvantageType.None, out _, out _);
        }

        if (wildSurgeDie1 > wildSurgeDie2)
        {
            (wildSurgeDie1, wildSurgeDie2) = (wildSurgeDie2, wildSurgeDie1);
        }

        var rulesetAttacker = attacker.RulesetCharacter;

        rulesetAttacker.ShowDieRoll(DieType.D20, wildSurgeDie1, title: PowerWildMagicSurge.GuiPresentation.Title);
        rulesetAttacker.ShowDieRoll(DieType.D20, wildSurgeDie2, title: PowerWildMagicSurge.GuiPresentation.Title);
        rulesetAttacker.LogCharacterActivatesAbility(
            PowerControlledChaos.FormatTitle(),
            "Feedback/&ControlledChaosDieRoll",
            tooltipContent: PowerControlledChaos.Name,
            tooltipClass: "PowerDefinition",
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

        yield return attacker.MyReactToSpendPowerBundle(
            usablePowerPool,
            [attacker],
            attacker,
            "ControlledChaos",
            reactionValidated: ReactionValidated,
            reactionNotValidated: ReactionNotValidated);

        rulesetAttacker.UsablePowers.Remove(usablePowerFirst);
        rulesetAttacker.UsablePowers.Remove(usablePowerSecond);

        yield break;

        void ReactionValidated(ReactionRequestSpendBundlePower reactionRequest)
        {
            var selectedRoll = reactionRequest.SelectedSubOption + 1;
            var selectedPower = WildSurgePowers.ElementAt(reactionRequest.SelectedSubOption);

            ApplyWildSurge(
                attacker, selectedRoll, PowerControlledChaos, selectedPower, "Feedback/&ControlledChaosDieChoice");
        }

        void ReactionNotValidated(ReactionRequestSpendBundlePower reactionRequest)
        {
            var choiceRoll = RollDie(DieType.D2, AdvantageType.None, out _, out _);
            var selectedRoll = choiceRoll == 1 ? wildSurgeDie1 : wildSurgeDie2;
            var selectedPower = WildSurgePowers.ElementAt(selectedRoll - 1);

            ApplyWildSurge(
                attacker, selectedRoll, PowerControlledChaos, selectedPower, "Feedback/&ControlledChaosDieChoice");
        }
    }

    //
    // Spell Bombardment 
    //

    internal static void HandleSpellBombardment(
        RulesetCharacter rulesetCharacter, DamageForm damageForm, List<int> rolledValues, ref int damage)
    {
        var dieType = damageForm.DieType;
        var maxDie = DiceMaxValue[(int)dieType];
        var character = GameLocationCharacter.GetFromActor(rulesetCharacter);

        if (!rolledValues.Contains(maxDie) ||
            !character.UsedSpecialFeatures.TryGetValue(FeatureSpellBombardment.Name, out var value) ||
            value > 0)
        {
            return;
        }

        character.UsedSpecialFeatures[FeatureSpellBombardment.Name] = 1;
        rulesetCharacter.LogCharacterUsedFeature(FeatureSpellBombardment);

        rolledValues.Add(maxDie);
        damage += maxDie;
    }


    //
    // Tides of Chaos
    //

    private sealed class TidesOfChaosRepayTides : IPowerOrSpellFinishedByMe
    {
        internal static readonly TidesOfChaosRepayTides Marker = new();

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var character = action.ActingCharacter;
            var rulesetCharacter = character.RulesetCharacter;

            RepayTidesOfChaosIfRequired(rulesetCharacter);

            yield break;
        }
    }

    //
    // Wild Magic Surge
    //

    private sealed class CustomBehaviorWildMagicSurge : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var hasUsedWildMarkThisTurn = rulesetAttacker.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, ConditionWildSurgeMark.Name);
            var hasChaos = rulesetAttacker.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, ConditionChaos.Name);

            if (hasUsedWildMarkThisTurn ||
                action is not CharacterActionCastSpell actionCastSpell ||
                actionCastSpell.Countered ||
                actionCastSpell.ExecutionFailed ||
                (actionCastSpell.ActiveSpell.SpellDefinition.SpellLevel == 0 && !hasChaos) ||
                (actionCastSpell.ActiveSpell.SpellRepertoire != null && // casting from a scroll so let wild surge
                 actionCastSpell.ActiveSpell.SpellRepertoire.SpellCastingClass != CharacterClassDefinitions.Sorcerer))
            {
                yield break;
            }

            if (!hasChaos)
            {
                var chanceDie = RollDie(DieType.D20, AdvantageType.None, out _, out _);
                var shouldRollWildSurge = chanceDie <= Main.Settings.WildSurgeDieRollThreshold;

                rulesetAttacker.ShowDieRoll(DieType.D20, chanceDie, title: PowerWildMagicSurge.GuiPresentation.Title);
                rulesetAttacker.LogCharacterActivatesAbility(
                    PowerWildMagicSurge.GuiPresentation.Title,
                    "Feedback/&WidSurgeChanceDieRoll",
                    tooltipContent: PowerWildMagicSurge.Name,
                    tooltipClass: "PowerDefinition",
                    extra:
                    [
                        (shouldRollWildSurge
                                ? ConsoleStyleDuplet.ParameterType.Negative
                                : ConsoleStyleDuplet.ParameterType.Positive,
                            chanceDie.ToString())
                    ]);

                if (!shouldRollWildSurge)
                {
                    yield break;
                }
            }

            rulesetAttacker.InflictCondition(
                ConditionWildSurgeMark.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionWildSurgeMark.Name,
                0,
                0,
                0);

            yield return HandleWildSurge(attacker, hasChaos);
        }
    }

    private sealed class CustomBehaviorTidesOfChaos
        : ITryAlterOutcomeAttack, ITryAlterOutcomeAttributeCheck, ITryAlterOutcomeSavingThrow
    {
        public int HandlerPriority => -5; // ensure it triggers after bend luck

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(PowerTidesOfChaos, rulesetHelper);

            if (action.AttackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure) ||
                helper != attacker ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            // any reaction within an attack flow must use the attacker as waiter
            yield return helper.MyReactToSpendPower(
                usablePower,
                attacker,
                "TidesOfChaosAttack",
                "SpendPowerTidesOfChaosAttackDescription"
                    .Formatted(Category.Reaction, defender.Name, action.FormatTitle()),
                ReactionValidated,
                battleManager);

            yield break;

            void ReactionValidated()
            {
                // this is an exception to rule and only happens
                // as powers added at 1st level from subclasses won't have a class assigned
                usablePower.Consume();

                List<TrendInfo> advantageTrends =
                    [new(1, FeatureSourceType.CharacterFeature, PowerTidesOfChaos.Name, PowerTidesOfChaos)];

                actionModifier.AttackAdvantageTrends.SetRange(advantageTrends);

                RollOutcome outcome;
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
                        attackMode.Ranged,
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
                    return;
                }

                action.AttackRollOutcome = outcome;
                action.AttackSuccessDelta = successDelta;
                action.AttackRoll = roll;

                var sign = toHitBonus > 0 ? "+" : string.Empty;

                rulesetHelper.LogCharacterUsedFeature(
                    PowerTidesOfChaos,
                    "Feedback/&TriggerRerollLine",
                    false,
                    (ConsoleStyleDuplet.ParameterType.Base, $"{attackRoll}{sign}{toHitBonus}"),
                    (ConsoleStyleDuplet.ParameterType.FailedRoll,
                        Gui.Format("Feedback/&RollAttackFailureTitle", $"{attackRoll + toHitBonus}")));
            }
        }

        public IEnumerator OnTryAlterAttributeCheck(
            GameLocationBattleManager battleManager,
            AbilityCheckData abilityCheckData,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(PowerTidesOfChaos, rulesetHelper);

            if (abilityCheckData.AbilityCheckRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure) ||
                helper != defender ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            // any reaction within a saving flow must use the yielder as waiter
            yield return helper.MyReactToSpendPower(
                usablePower,
                helper,
                "TidesOfChaosCheck",
                "SpendPowerTidesOfChaosCheckDescription".Localized(Category.Reaction),
                ReactionValidated,
                battleManager);

            yield break;

            void ReactionValidated()
            {
                List<TrendInfo> advantageTrends =
                    [new(1, FeatureSourceType.CharacterFeature, PowerTidesOfChaos.Name, PowerTidesOfChaos)];

                abilityCheckData.AbilityCheckActionModifier.AbilityCheckAdvantageTrends.SetRange(advantageTrends);

                var dieRoll = rulesetHelper.RollDie(
                    DieType.D20, RollContext.None, false, AdvantageType.Advantage, out _, out _);

                abilityCheckData.AbilityCheckSuccessDelta += dieRoll - abilityCheckData.AbilityCheckRoll;
                abilityCheckData.AbilityCheckRoll = dieRoll;
                abilityCheckData.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckSuccessDelta >= 0
                    ? RollOutcome.Success
                    : RollOutcome.Failure;

                rulesetHelper.LogCharacterActivatesAbility(
                    PowerTidesOfChaos.GuiPresentation.Title,
                    "Feedback/&TidesOfChaosAdvantageCheck",
                    tooltipContent: PowerTidesOfChaos.Name,
                    tooltipClass: "PowerDefinition");
            }
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            SavingThrowData savingThrowData,
            bool hasHitVisual)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(PowerTidesOfChaos, rulesetHelper);

            if (savingThrowData.SaveOutcome is not RollOutcome.Failure ||
                helper != defender ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            // any reaction within a saving flow must use the yielder as waiter
            yield return helper.MyReactToSpendPower(
                usablePower,
                helper,
                "TidesOfChaosSave",
                "SpendPowerTidesOfChaosSaveDescription"
                    .Formatted(Category.Reaction, attacker?.Name ?? ReactionRequestCustom.EnvTitle,
                        savingThrowData.Title),
                ReactionValidated,
                battleManager);

            yield break;

            void ReactionValidated()
            {
                List<TrendInfo> advantageTrends =
                    [new(1, FeatureSourceType.CharacterFeature, PowerTidesOfChaos.Name, PowerTidesOfChaos)];

                savingThrowData.SaveActionModifier.SavingThrowAdvantageTrends.SetRange(advantageTrends);

                TryAlterOutcomeSavingThrow.TryRerollSavingThrow(attacker, defender, savingThrowData, hasHitVisual);

                rulesetHelper.LogCharacterActivatesAbility(
                    PowerTidesOfChaos.GuiPresentation.Title,
                    "Feedback/&TidesOfChaosAdvantageSavingThrow",
                    tooltipContent: PowerTidesOfChaos.Name,
                    tooltipClass: "PowerDefinition");
            }
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

            if (helper == attacker ||
                !helper.CanReact() ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0 ||
                Math.Abs(action.AttackSuccessDelta) > 4)
            {
                yield break;
            }

            string stringParameter;

            if (helper.Side == attacker.Side &&
                action.AttackRollOutcome is RollOutcome.Failure)
            {
                stringParameter = "BendLuckAttack";
            }
            else if (helper.Side != attacker.Side &&
                     action.AttackRollOutcome is RollOutcome.Success)
            {
                stringParameter = "BendLuckEnemyAttack";
            }
            else
            {
                yield break;
            }

            // any reaction within an attack flow must use the attacker as waiter
            yield return helper.MyReactToSpendPower(
                usablePower,
                attacker,
                stringParameter,
                $"SpendPower{stringParameter}Description".Formatted(Category.Reaction, defender.Name),
                ReactionValidated,
                battleManager);

            yield break;

            void ReactionValidated()
            {
                helper.SpendActionType(ActionDefinitions.ActionType.Reaction);

                EffectHelpers.StartVisualEffect(helper, attacker,
                    PowerDomainLawHolyRetribution, EffectHelpers.EffectType.Caster);

                var dieRoll = rulesetHelper.RollDie(
                    DieType.D4, RollContext.None, false, AdvantageType.None, out _, out _);

                if (helper.Side == attacker.Side)
                {
                    attackModifier.AttacktoHitTrends.Add(
                        new TrendInfo(dieRoll, FeatureSourceType.Power, powerBendLuck.Name, powerBendLuck)
                        {
                            dieType = DieType.D4, dieFlag = TrendInfoDieFlag.None
                        });

                    action.AttackSuccessDelta += dieRoll;
                    attackModifier.AttackRollModifier += dieRoll;
                    action.AttackRollOutcome =
                        action.AttackSuccessDelta >= 0 ? RollOutcome.Success : RollOutcome.Failure;

                    rulesetHelper.LogCharacterActivatesAbility(
                        powerBendLuck.GuiPresentation.Title,
                        "Feedback/&BendLuckAttackToHitRoll",
                        tooltipContent: powerBendLuck.Name,
                        tooltipClass: "PowerDefinition",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D4)),
                            (action.AttackSuccessDelta >= 0
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
                    action.AttackRollOutcome =
                        action.AttackSuccessDelta >= 0 ? RollOutcome.Success : RollOutcome.Failure;

                    rulesetHelper.LogCharacterActivatesAbility(
                        powerBendLuck.GuiPresentation.Title,
                        "Feedback/&BendLuckEnemyAttackToHitRoll",
                        tooltipContent: powerBendLuck.Name,
                        tooltipClass: "PowerDefinition",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D4)),
                            (action.AttackSuccessDelta >= 0
                                ? ConsoleStyleDuplet.ParameterType.Negative
                                : ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString())
                        ]);
                }
            }
        }

        public IEnumerator OnTryAlterAttributeCheck(
            GameLocationBattleManager battleManager,
            AbilityCheckData abilityCheckData,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerBendLuck, rulesetHelper);

            if (helper == defender ||
                !helper.CanReact() ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0 ||
                Math.Abs(abilityCheckData.AbilityCheckSuccessDelta) > 4)
            {
                yield break;
            }

            string stringParameter;

            if (helper.Side == defender.Side &&
                abilityCheckData.AbilityCheckRoll > 0 &&
                abilityCheckData.AbilityCheckRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                stringParameter = "BendLuckCheck";
            }
            else if (helper.Side != defender.Side &&
                     abilityCheckData.AbilityCheckRoll > 0 &&
                     abilityCheckData.AbilityCheckRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                stringParameter = "BendLuckEnemyCheck";
            }
            else
            {
                yield break;
            }

            // any reaction within an attribute check flow must use the yielder as waiter
            yield return helper.MyReactToSpendPower(
                usablePower,
                helper,
                stringParameter,
                $"SpendPower{stringParameter}Description".Formatted(Category.Reaction, defender.Name),
                ReactionValidated,
                battleManager);

            yield break;

            void ReactionValidated()
            {
                helper.SpendActionType(ActionDefinitions.ActionType.Reaction);

                EffectHelpers.StartVisualEffect(helper, defender,
                    PowerDomainLawHolyRetribution, EffectHelpers.EffectType.Caster);

                var dieRoll = rulesetHelper.RollDie(
                    DieType.D4, RollContext.None, false, AdvantageType.None, out _, out _);
                var abilityCheckModifier = abilityCheckData.AbilityCheckActionModifier;

                if (helper.Side == defender.Side)
                {
                    abilityCheckModifier.AbilityCheckModifierTrends.Add(
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
                        tooltipContent: powerBendLuck.Name,
                        tooltipClass: "PowerDefinition",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D4)),
                            (abilityCheckData.AbilityCheckSuccessDelta >= 0
                                ? ConsoleStyleDuplet.ParameterType.Positive
                                : ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString())
                        ]);
                }
                else
                {
                    abilityCheckModifier.AbilityCheckModifierTrends.Add(
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
                        tooltipContent: powerBendLuck.Name,
                        tooltipClass: "PowerDefinition",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D4)),
                            (abilityCheckData.AbilityCheckSuccessDelta >= 0
                                ? ConsoleStyleDuplet.ParameterType.Negative
                                : ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString())
                        ]);
                }
            }
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            SavingThrowData savingThrowData,
            bool hasHitVisual)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerBendLuck, rulesetHelper);

            if (helper == defender ||
                !helper.CanReact() ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0 ||
                Math.Abs(savingThrowData.SaveOutcomeDelta) > 4)
            {
                yield break;
            }

            string stringParameter;

            if (helper.Side == defender.Side &&
                savingThrowData.SaveOutcome == RollOutcome.Failure)
            {
                stringParameter = "BendLuckSaving";
            }
            else if (helper.Side != defender.Side &&
                     savingThrowData.SaveOutcome == RollOutcome.Success)
            {
                stringParameter = "BendLuckEnemySaving";
            }
            else
            {
                yield break;
            }


            // any reaction within a saving flow must use the yielder as waiter
            yield return helper.MyReactToSpendPower(
                usablePower,
                helper,
                stringParameter,
                $"SpendPower{stringParameter}Description".Formatted(Category.Reaction,
                    defender.Name, attacker?.Name ?? ReactionRequestCustom.EnvTitle, savingThrowData.Title),
                ReactionValidated,
                battleManager);

            yield break;

            void ReactionValidated()
            {
                helper.SpendActionType(ActionDefinitions.ActionType.Reaction);

                EffectHelpers.StartVisualEffect(helper, defender,
                    PowerDomainLawHolyRetribution, EffectHelpers.EffectType.Caster);

                var dieRoll = rulesetHelper.RollDie(
                    DieType.D4, RollContext.None, false, AdvantageType.None, out _, out _);

                if (helper.Side == defender.Side)
                {
                    savingThrowData.SaveOutcomeDelta += dieRoll;
                    savingThrowData.SaveOutcome =
                        savingThrowData.SaveOutcomeDelta >= 0 ? RollOutcome.Success : RollOutcome.Failure;

                    rulesetHelper.LogCharacterActivatesAbility(
                        powerBendLuck.GuiPresentation.Title,
                        "Feedback/&BendLuckSavingToHitRoll",
                        tooltipContent: powerBendLuck.Name,
                        tooltipClass: "PowerDefinition",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D4)),
                            (savingThrowData.SaveOutcomeDelta >= 0
                                ? ConsoleStyleDuplet.ParameterType.Positive
                                : ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString())
                        ]);
                }
                else
                {
                    savingThrowData.SaveOutcomeDelta -= dieRoll;
                    savingThrowData.SaveOutcome =
                        savingThrowData.SaveOutcomeDelta >= 0 ? RollOutcome.Success : RollOutcome.Failure;

                    rulesetHelper.LogCharacterActivatesAbility(
                        powerBendLuck.GuiPresentation.Title,
                        "Feedback/&BendLuckEnemySavingToHitRoll",
                        tooltipContent: powerBendLuck.Name,
                        tooltipClass: "PowerDefinition",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D4)),
                            (savingThrowData.SaveOutcomeDelta >= 0
                                ? ConsoleStyleDuplet.ParameterType.Negative
                                : ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString())
                        ]);
                }
            }
        }
    }

    //
    // Spell Bombardment
    //

    private sealed class CustomBehaviorSpellBombardment : IMagicEffectInitiatedByMe, IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            attacker.UsedSpecialFeatures.Remove(FeatureSpellBombardment.Name);

            yield break;
        }

        public IEnumerator OnMagicEffectInitiatedByMe(
            CharacterAction action,
            RulesetEffect activeEffect,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var levels = rulesetAttacker.GetSubclassLevel(CharacterClassDefinitions.Sorcerer, Name);

            attacker.UsedSpecialFeatures.Remove(FeatureSpellBombardment.Name);

            if (levels < 18 ||
                ((action.Countered ||
                  action is CharacterActionCastSpell { ExecutionFailed: true } ||
                  activeEffect is not RulesetEffectSpell rulesetEffectSpell ||
                  rulesetEffectSpell.SpellRepertoire?.SpellCastingClass != CharacterClassDefinitions.Sorcerer) &&
                 activeEffect.SourceDefinition != PowerFireball))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.Add(FeatureSpellBombardment.Name, 0);
        }
    }

    #region Wild Surge Handlers

    private static void ApplyWildSurge(
        GameLocationCharacter caster,
        int roll,
        FeatureDefinitionPower sourcePower,
        FeatureDefinitionPower selectedPower,
        string feedback)
    {
#if DEBUG
        if (Main.Settings.WildSurgeEffectDie > 0)
        {
            roll = Main.Settings.WildSurgeEffectDie;
        }
#endif

        var rulesetCaster = caster.RulesetCharacter;

        rulesetCaster.LogCharacterActivatesPower(
            sourcePower.FormatTitle(),
            feedback,
            tooltipContent: sourcePower.Name,
            tooltipClass: "PowerDefinition",
            extra:
            [
                (ConsoleStyleDuplet.ParameterType.Positive, roll.ToString(), string.Empty, string.Empty),
                (ConsoleStyleDuplet.ParameterType.AttackSpellPower, selectedPower.FormatTitle(), selectedPower.Name,
                    "PowerDefinition")
            ]);

        switch (roll)
        {
            // trigger a random Wild Surge effect (except this one) at the start of each of your turns for one minute
            case 1:
                EffectHelpers.StartVisualEffect(
                    caster, caster, PowerSessrothTeleport, EffectHelpers.EffectType.Caster);
                InflictConditionOnCreaturesWithinRange(caster, ConditionChaos.Name, DurationType.Minute);
                break;

            // you cast Fireball centered on self
            case 2:
                ExecutePowerNoCostOnCasterLocation(caster, PowerFireball);
                break;

            // you regain 2D10 hit points
            case 3:
                EffectHelpers.StartVisualEffect(
                    caster, caster, CureWounds, EffectHelpers.EffectType.Effect);
                rulesetCaster.ReceiveHealing(
                    rulesetCaster.RollDiceAndSum(DieType.D10, RollContext.HealValueRoll, 2, [], false),
                    true, rulesetCaster.Guid);
                break;

            // each creature within 20 feet of you (including you) catches on fire
            case 4:
                InflictConditionOnCreaturesWithinRange(
                    caster, ConditionOnFire1D4.Name,
                    DurationType.Minute, 1, TurnOccurenceType.EndOfTurn, 4);
                break;

            // you teleport up to 60 feet to an unoccupied space of your choice that you can see
            case 5:
                EffectHelpers.StartVisualEffect(
                    caster, caster, PowerPactChainQuasit, EffectHelpers.EffectType.Caster);
                ExecutePowerNoCostOnCasterLocation(caster, PowerTeleport);
                break;

            // you become frightened until the end of your next turn
            case 6:
                EffectHelpers.StartVisualEffect(
                    caster, caster, PowerSorcererHauntedSoulSpiritVisage, EffectHelpers.EffectType.Caster);
                InflictConditionOnCreaturesWithinRange(caster, RuleDefinitions.ConditionFrightened, DurationType.Round);
                break;

            // you cast invisibility on self
            case 7:
                EffectHelpers.StartVisualEffect(
                    caster, caster, PowerDefilerDarkness, EffectHelpers.EffectType.Caster);
                InflictConditionOnCreaturesWithinRange(
                    caster, ConditionDefinitions.ConditionInvisible.Name, DurationType.Minute);
                break;

            // a random creature within 60 feet of you (other than you) becomes poisoned for 1 hour
            case 8:
                InflictConditionOnRandomCreatureWithinRange(
                    caster, RuleDefinitions.ConditionPoisoned, DurationType.Hour, 1, TurnOccurenceType.EndOfTurn,
                    PowerDomainOblivionMarkOfFate, 12);
                break;

            // you gain a [class level / 4 (rounded up)] level spell slot
            case 9:
                var levels = caster.RulesetCharacter.GetSubclassLevel(CharacterClassDefinitions.Sorcerer, Name);
                var slotLevel = (levels + 3) / 4;

                EffectHelpers.StartVisualEffect(
                    caster, caster, PowerMagebaneSpellCrusher, EffectHelpers.EffectType.Effect);
                rulesetCaster.LogCharacterActivatesAbility(string.Empty, "Feedback/&RecoverSpellSlotOfLevel",
                    extra: [(ConsoleStyleDuplet.ParameterType.Base, slotLevel.ToString())]);
                InflictConditionOnCreaturesWithinRange(
                    caster, $"ConditionAdditionalSpellSlot{slotLevel}", DurationType.UntilLongRest);
                break;

            // maximize the damage of the next damaging spell you cast within the next minute
            case 10:
                EffectHelpers.StartVisualEffect(
                    caster, caster, PowerPatronFiendDarkOnesBlessing, EffectHelpers.EffectType.Effect);
                InflictConditionOnCreaturesWithinRange(caster, ConditionMaxDamageRolls.Name, DurationType.Minute);
                break;

            // A random creature within 60 feet of you (other than you) can fly for one minute.
            case 11:
                InflictConditionOnRandomCreatureWithinRange(
                    caster, RuleDefinitions.ConditionFlying,
                    DurationType.Minute, 1, TurnOccurenceType.EndOfTurn, PowerSorcererManaPainterDrain, 12);
                break;

            // you cast grease centered on self
            case 12:
                ExecutePowerNoCostOnCasterLocation(caster, PowerGrease, 1);
                break;

            // you cast mirror image
            case 13:
                EffectHelpers.StartVisualEffect(
                    caster, caster, SpellsContext.MirrorImage, EffectHelpers.EffectType.Caster);
                InflictConditionOnCreaturesWithinRange(
                    caster, SpellBuilders.ConditionMirrorImageMark.Name, DurationType.Minute);
                break;

            // each creature within 30 feet of you (other than you) takes 1d10 necrotic damage. You regain hit points equal to every necrotic damage amount dealt
            case 14:
                ExecutePowerNoCostOnCasterLocation(caster, PowerWildHealing, 6, false);
                break;

            // each creature within 30 feet of you (other than you) becomes invisible for one minute
            case 15:
                InflictConditionOnCreaturesWithinRange(
                    caster, ConditionDefinitions.ConditionInvisible.Name,
                    DurationType.Minute, 1, TurnOccurenceType.EndOfTurn, 6, PowerPhaseMarilithTeleport);
                break;

            // you can take one additional action immediately
            case 16:
                EffectHelpers.StartVisualEffect(
                    caster, caster, PowerDomainBattleDecisiveStrike, EffectHelpers.EffectType.Caster);
                InflictConditionOnCreaturesWithinRange(
                    caster, ConditionDefinitions.ConditionSurged.Name, DurationType.Round, 0);
                break;

            // each creature within 30 feet of you (including you) gain vulnerability to piercing damage for one minute
            case 17:
                InflictConditionOnCreaturesWithinRange(
                    caster, ConditionPiercingVulnerability.Name,
                    DurationType.Minute, 1, TurnOccurenceType.EndOfTurn, 6, PowerIncubus_Drain);
                break;

            // you gain resistance to all damage for one minute
            case 18:
                EffectHelpers.StartVisualEffect(
                    caster, caster, PowerOathOfTirmarGoldenSpeech, EffectHelpers.EffectType.Caster);
                InflictConditionOnCreaturesWithinRange(caster, ConditionDamageResistance.Name, DurationType.Minute);
                break;

            // up to three creatures you choose within 30 feet of you take 4d10 lightning damage as a free action before your turn ends
            case 19:
                EffectHelpers.StartVisualEffect(
                    caster, caster, PowerDomainElementalLightningBlade, EffectHelpers.EffectType.Effect);
                InflictConditionOnCreaturesWithinRange(caster, ConditionLightningStrike.Name, DurationType.Round, 0);
                break;

            // you gain all expended sorcery points
            case 20:
                EffectHelpers.StartVisualEffect(
                    caster, caster, Resurrection, EffectHelpers.EffectType.Effect);
                rulesetCaster.LogCharacterActivatesAbility(string.Empty, "Screen/&SorceryPointsRecoveredDescription");
                rulesetCaster.SpendSorceryPoints(-rulesetCaster.UsedSorceryPoints);
                break;
        }

        //
        // repay Tides of Chaos if required but only for effects that don't execute power
        // effects that execute power repay tides after the effect finishes as these are queued
        //

        List<int> wildPowers = [2, 5, 12, 14];

        if (!wildPowers.Contains(roll))
        {
            RepayTidesOfChaosIfRequired(rulesetCaster);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ExecutePowerNoCostOnCasterLocation(
        GameLocationCharacter caster, FeatureDefinitionPower power, int range = 0, bool includeCaster = true)
    {
        var rulesetCaster = caster.RulesetCharacter;
        var usablePower = PowerProvider.Get(power, rulesetCaster);
        var targets = new List<GameLocationCharacter>();

        EnumerateTargetsWithinRange(caster, range, targets, includeCaster);
        caster.MyExecuteActionPowerNoCost(usablePower, [.. targets]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InflictConditionOnRandomCreatureWithinRange(
        GameLocationCharacter caster,
        string conditionName,
        DurationType durationType,
        int durationParameter,
        TurnOccurenceType turnOccurenceType,
        IMagicEffect magicEffect,
        int range)
    {
        var rulesetCaster = caster.RulesetCharacter;
        var targets = new List<GameLocationCharacter>();

        EnumerateTargetsWithinRange(caster, range, targets, false);

        var random = new PcgRandom((ulong)DateTime.Now.Ticks);
        var index = random.Next(targets.Count);
        var target = targets.ElementAt(index);
        var rulesetTarget = target.RulesetCharacter;

        EffectHelpers.StartVisualEffect(caster, target, magicEffect, EffectHelpers.EffectType.Effect);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InflictConditionOnCreaturesWithinRange(
        GameLocationCharacter caster,
        string conditionName,
        DurationType durationType,
        int durationParameter = 1,
        TurnOccurenceType turnOccurenceType = TurnOccurenceType.EndOfTurn,
        int range = 0,
        IMagicEffect magicEffect = null)
    {
        var rulesetCaster = caster.RulesetCharacter;

        if (range == 0)
        {
            rulesetCaster.InflictCondition(
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

            return;
        }

        var targets = new List<GameLocationCharacter>();

        EnumerateTargetsWithinRange(caster, range, targets);

        foreach (var target in targets)
        {
            var rulesetTarget = target.RulesetActor;

            if (magicEffect != null)
            {
                EffectHelpers.StartVisualEffect(caster, target, magicEffect, EffectHelpers.EffectType.Effect);
            }

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

    private static void EnumerateTargetsWithinRange(
        GameLocationCharacter caster, int range, List<GameLocationCharacter> targets, bool includeCaster = true)
    {
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);

        targets.SetRange(contenders.Where(x =>
            x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
            x.IsWithinRange(caster, range) &&
            (includeCaster || x != caster)));
    }

    //
    // Max Damage Rolls
    //

    private sealed class CustomBehaviorMaxDamageRolls
        : IMagicEffectInitiatedByMe, IMagicEffectFinishedByMe, IForceMaxDamageTypeDependent
    {
        private const string Tag = "WildSurgeMaxDamageRolls";

        public bool IsValid(RulesetActor rulesetActor, DamageForm damageForm)
        {
            var actor = GameLocationCharacter.GetFromActor(rulesetActor);

            return actor != null && actor.UsedSpecialFeatures.ContainsKey(Tag);
        }

        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (attacker.UsedSpecialFeatures.Remove(Tag) &&
                rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionMaxDamageRolls.Name, out var activeCondition))
            {
                rulesetAttacker.RemoveCondition(activeCondition);
            }

            yield break;
        }

        public IEnumerator OnMagicEffectInitiatedByMe(
            CharacterAction action,
            RulesetEffect activeEffect,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            attacker.UsedSpecialFeatures.Remove(Tag);

            if (activeEffect is not RulesetEffectSpell ||
                activeEffect.EffectDescription.FindFirstDamageForm() == null)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var powerMaxDamageRolls = WildSurgePowers[9];

            rulesetAttacker.LogCharacterUsedPower(powerMaxDamageRolls);
            attacker.UsedSpecialFeatures.Add(Tag, 0);
        }
    }

    //
    // Teleport
    //

    private sealed class PowerOrSpellInitiatedByMeTeleport
        : IPowerOrSpellInitiatedByMe, IIgnoreInvisibilityInterruptionCheck
    {
        public IEnumerator OnPowerOrSpellInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            yield return CampaignsContext.SelectPosition(action, PowerTeleport);
        }
    }

#if false
    //
    // Wild Healing
    //

    private sealed class CustomBehaviorWildHealing : IPowerOrSpellInitiatedByMe, IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var character = action.ActingCharacter;
            var rulesetCharacter = character.RulesetCharacter;

            if (character.UsedSpecialFeatures.Remove(PowerWildHealing.Name))
            {
                EffectHelpers.StartVisualEffect(character, character, Heal, EffectHelpers.EffectType.Effect);
            }

            rulesetCharacter.HealingReceived -= HealingReceived;

            yield break;
        }

        public IEnumerator OnPowerOrSpellInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var character = action.ActingCharacter;
            var rulesetCharacter = character.RulesetCharacter;

            character.UsedSpecialFeatures.Remove(PowerWildHealing.Name);
            rulesetCharacter.HealingReceived += HealingReceived;

            yield break;
        }

        private static void HealingReceived(
            RulesetCharacter rulesetCharacter,
            int healing,
            ulong sourceGuid,
            HealingCap healingCaps,
            IHealingModificationProvider healingModificationProvider)
        {
            var character = GameLocationCharacter.GetFromActor(rulesetCharacter);

            character?.UsedSpecialFeatures.Add(PowerWildHealing.Name, 0);
        }
    }
#endif

    #endregion
}
