using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static AttributeDefinitions;
using static ConditionForm;
using static FeatureDefinitionSavingThrowAffinity;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfTheDiscordance : AbstractSubclass
{
    private const string Name = "WayOfTheDiscordance";
    private const int TurmoilLevel = 6;
    private const int EntropicStrikesLevel = 11;

    public WayOfTheDiscordance()
    {
        // LEVEL 03

        // Discordance

        var conditionHadDiscordanceDamageThisTurn = ConditionDefinitionBuilder
            .Create($"Condition{Name}HadDiscordanceDamageThisTurn")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration()
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var conditionDiscordance = ConditionDefinitionBuilder
            .Create($"Condition{Name}Discordance")
            .SetGuiPresentation(Category.Condition, ConditionRestrictedInsideMagicCircle)
            .SetSilent(Silent.WhenRemoved)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetSpecialDuration(DurationType.Minute, 1)
            .AllowMultipleInstances()
            .AddToDB();

        var powerDiscordance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Discordance")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionDiscordance, ConditionOperation.Remove),
                        EffectFormBuilder.ConditionForm(conditionDiscordance, ConditionOperation.Remove),
                        EffectFormBuilder.DamageForm(DamageTypeNecrotic, 1, DieType.D4),
                        EffectFormBuilder.ConditionForm(conditionHadDiscordanceDamageThisTurn))
                    .SetParticleEffectParameters(PowerSorakDreadLaughter)
                    .Build())
            .AddToDB();

        var featureSetDiscordance = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Discordance")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerDiscordance)
            .AddToDB();

        // Chaos Channeling

        var conditionChaosChanneling = ConditionDefinitionBuilder
            .Create($"Condition{Name}ChaosChanneling")
            .SetGuiPresentation(Category.Condition, ConditionSpiritGuardiansSelf)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(new ModifyWeaponAttackModeChaosChanneling())
            .AddToDB();

        var powerChaosChanneling = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ChaosChanneling")
            .SetGuiPresentation($"FeatureSet{Name}ChaosChanneling", Category.Feature, PowerOathOfDevotionTurnUnholy)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionChaosChanneling))
                    .SetParticleEffectParameters(PowerSorcererChildRiftDeflection)
                    .Build())
            .AddToDB();

        var powerChaosChannelingPoints = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ChaosChannelingPoints")
            .SetGuiPresentation($"FeatureSet{Name}ChaosChanneling", Category.Feature, PowerOathOfDevotionTurnUnholy)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 0)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionChaosChanneling))
                    .SetParticleEffectParameters(PowerSorcererChildRiftDeflection)
                    .Build())
            .AddToDB();

        powerChaosChanneling.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(character => character.GetRemainingPowerUses(powerChaosChanneling) > 0));

        powerChaosChannelingPoints.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(character => character.GetRemainingPowerUses(powerChaosChanneling) == 0));

        var featureSetChaosChanneling = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ChaosChanneling")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerChaosChanneling, powerChaosChannelingPoints)
            .AddToDB();

        // LEVEL 06

        // Turmoil

        var combatAffinityTurmoil = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}Turmoil")
            .SetGuiPresentation($"Condition{Name}TurmoilTitle".Formatted(Category.Condition), Gui.NoLocalization)
            .SetMyAttackModifierSign(AttackModifierSign.Substract)
            .SetMyAttackModifierDieType(DieType.D4)
            .AddToDB();

        var savingThrowAffinityTurmoil = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}Turmoil")
            .SetGuiPresentation($"Condition{Name}TurmoilTitle".Formatted(Category.Condition), Gui.NoLocalization)
            .SetModifiers(ModifierType.RemoveDice, DieType.D4, 1, false,
                Charisma,
                Constitution,
                Dexterity,
                Intelligence,
                Strength,
                Wisdom)
            .AddToDB();

        var conditionTurmoil = ConditionDefinitionBuilder
            .Create($"Condition{Name}Turmoil")
            .SetGuiPresentation(Category.Condition, ConditionDoomLaughter)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .CopyParticleReferences(ConditionStrikeOfChaosAttackAdvantage)
            .AddFeatures(combatAffinityTurmoil, savingThrowAffinityTurmoil)
            // required by Tides of Chaos to properly identify turmoil on death
            .AddCustomSubFeatures(
                new ForceConditionCategory(TagCombat))
            .AddToDB();

        var conditionHadTurmoil = ConditionDefinitionBuilder
            .Create($"Condition{Name}HadTurmoil")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Day, 1)
            .AddToDB();

        var powerTurmoil = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Turmoil")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, Charisma, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, Wisdom, 8)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionHadTurmoil),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionTurmoil, ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .Build())
                    .SetParticleEffectParameters(PowerSorakDreadLaughter)
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        //
        // FINALIZE DISCORDANCE BEHAVIOR
        //

        powerDiscordance.AddCustomSubFeatures(new CustomBehaviorDiscordance(
            powerDiscordance, conditionDiscordance, conditionHadDiscordanceDamageThisTurn,
            powerTurmoil, conditionHadTurmoil));

        // LEVEL 11

        // Burst of Disharmony

        var powerBurstOfDisharmonyPool = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BurstOfDisharmony")
            .SetGuiPresentation($"FeatureSet{Name}BurstOfDisharmony", Category.Feature,
                Sprites.GetSprite("PowerBurstOfDisharmony", Resources.PowerBurstOfDisharmony, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints, 0)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Cube, 3)
                    .Build())
            .AddToDB();

        var powerBurstOfDisharmonyList = new List<FeatureDefinitionPower>();

        for (var i = 10; i >= 2; i--)
        {
            var kiNumber = i; // closure
            var diceNumber = 1 + kiNumber;
            var minimumClassLevelAllowed = (kiNumber * 2) - 1;

            var powerBurstOfDisharmony = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{Name}BurstOfDisharmony{kiNumber}")
                .SetGuiPresentation(
                    $"Power{Name}BurstOfDisharmonyTitle".Formatted(Category.Feature, kiNumber),
                    $"Power{Name}BurstOfDisharmonyDescription".Formatted(Category.Feature, kiNumber, diceNumber))
                .SetSharedPool(ActivationTime.BonusAction, powerBurstOfDisharmonyPool, kiNumber)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Minute, 1)
                        .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Cube, 3)
                        .SetSavingThrowData(
                            false,
                            Dexterity,
                            true,
                            EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                                .SetDamageForm(DamageTypeNecrotic, diceNumber, DieType.D6)
                                .Build(),
                            EffectFormBuilder.ConditionForm(conditionDiscordance))
                        .SetParticleEffectParameters(PowerSorcererChildRiftRiftwalk)
                        .Build())
                .AddToDB();

            powerBurstOfDisharmony.AddCustomSubFeatures(
                ModifyPowerVisibility.Hidden,
                new MagicEffectFinishedByMeBurstOfDisharmony(
                    conditionDiscordance, powerDiscordance, conditionHadTurmoil, powerTurmoil),
                new ValidatorsValidatePowerUse(
                    c => c.RemainingKiPoints >= kiNumber &&
                         c.GetClassLevel(CharacterClassDefinitions.Monk) >= minimumClassLevelAllowed));

            powerBurstOfDisharmonyList.Add(powerBurstOfDisharmony);
        }

        PowerBundle.RegisterPowerBundle(powerBurstOfDisharmonyPool, false, powerBurstOfDisharmonyList);

        var featureSetBurstOfDisharmony = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BurstOfDisharmony")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(powerBurstOfDisharmonyPool)
            .AddFeatureSet(powerBurstOfDisharmonyList.OfType<FeatureDefinition>().ToArray())
            .AddToDB();

        // Entropic Strikes

        var featureEntropicStrikes = FeatureDefinitionBuilder
            .Create($"Feature{Name}EntropicStrikes")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // LEVEL 17

        // Tides of Chaos

        var powerTidesOfChaos = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TidesOfChaos")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(0, DieType.D4, 0, true)
                            .Build())
                    .SetParticleEffectParameters(PowerPactChainPseudodragon)
                    .Build())
            .AddToDB();

        powerTidesOfChaos.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new OnReducedToZeroHpByMeOrAllyTidesOfChaos(conditionTurmoil, powerTidesOfChaos));

        //
        // MAIN
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheDiscordance, 256))
            .AddFeaturesAtLevel(3, featureSetChaosChanneling, featureSetDiscordance)
            .AddFeaturesAtLevel(6, powerTurmoil)
            .AddFeaturesAtLevel(11, featureSetBurstOfDisharmony, featureEntropicStrikes)
            .AddFeaturesAtLevel(17, powerTidesOfChaos)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Monk;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Discordance [Also handles Turmoil]
    //

    private sealed class CustomBehaviorDiscordance(
        FeatureDefinitionPower powerDiscordance,
        ConditionDefinition conditionDiscordance,
        ConditionDefinition conditionHadDiscordanceDamageThisTurn,
        FeatureDefinitionPower powerTurmoil,
        ConditionDefinition conditionHadTurmoil)
        : IPhysicalAttackFinishedByMe, IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerDiscordance;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var damageForm = effectDescription.FindFirstDamageForm();

            if (damageForm == null)
            {
                return effectDescription;
            }

            damageForm.BonusDamage = ComputeAbilityScoreModifier(character.TryGetAttributeValue(Wisdom));
            damageForm.DieType = character.GetMonkDieType();

            return effectDescription;
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (attackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;

            if (!ValidatorsWeapon.IsUnarmed(attackMode))
            {
                yield break;
            }

            var monkLevel = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Monk);
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender.HasConditionOfType(conditionHadDiscordanceDamageThisTurn) &&
                monkLevel < EntropicStrikesLevel)
            {
                yield break;
            }

            if (rulesetDefender.AllConditions.All(x => x.ConditionDefinition != conditionDiscordance))
            {
                rulesetDefender.InflictCondition(
                    conditionDiscordance.Name,
                    conditionDiscordance.DurationType,
                    conditionDiscordance.DurationParameter,
                    conditionDiscordance.TurnOccurence,
                    TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    conditionDiscordance.Name,
                    0,
                    0,
                    0);

                yield break;
            }

            SpendPower(action, rulesetAttacker, powerDiscordance);

            if (monkLevel >= TurmoilLevel &&
                defender.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                !defender.RulesetCharacter.HasConditionOfType(conditionHadTurmoil))
            {
                SpendPower(action, rulesetAttacker, powerTurmoil);
            }
        }

        private static void SpendPower(
            CharacterAction action,
            RulesetCharacter rulesetAttacker,
            FeatureDefinitionPower featureDefinitionPower)
        {
            var actionParams = action.ActionParams.Clone();
            var usablePower = PowerProvider.Get(featureDefinitionPower, rulesetAttacker);

            actionParams.ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower;
            actionParams.RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                //CHECK: no need for AddAsActivePowerToSource
                .InstantiateEffectPower(rulesetAttacker, usablePower, false);

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
            actionService.ExecuteAction(actionParams, null, true);
        }
    }

    //
    // Chaos Channeling
    //

    private sealed class ModifyWeaponAttackModeChaosChanneling : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidatorsWeapon.IsUnarmed(attackMode))
            {
                return;
            }

            attackMode.reachRange += 6;
            attackMode.reach = true;

            var damageForm = attackMode.EffectDescription.FindFirstDamageForm();

            damageForm.DamageType = DamageTypeNecrotic;
        }
    }

    //
    // Burst of Disharmony [also handles discordance and turmoil]
    //

    private sealed class MagicEffectFinishedByMeBurstOfDisharmony(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDiscordance,
        FeatureDefinitionPower powerDiscordance,
        ConditionDefinition conditionHadTurmoil,
        FeatureDefinitionPower powerTurmoil)
        : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            // Discordance Damage
            var targets = new List<GameLocationCharacter>();

            targets.SetRange(action.actionParams.TargetCharacters
                .Where(x =>
                    x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }
                    && x.RulesetCharacter.AllConditions.Count(y =>
                        y.ConditionDefinition == conditionDiscordance) > 1));

            if (targets.Empty())
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();

            var actionParamsDiscordance = action.ActionParams.Clone();
            var usablePowerDiscordance = PowerProvider.Get(powerDiscordance, rulesetCharacter);

            actionParamsDiscordance.ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower;
            actionParamsDiscordance.RulesetEffect = rulesetImplementationService
                //CHECK: no need for AddAsActivePowerToSource
                .InstantiateEffectPower(rulesetCharacter, usablePowerDiscordance, false);
            actionParamsDiscordance.TargetCharacters.SetRange(targets);

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            // must enqueue actions
            actionService.ExecuteAction(actionParamsDiscordance, null, true);

            // Turmoil
            var monkLevel = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Monk);

            if (monkLevel < TurmoilLevel)
            {
                yield break;
            }

            targets.RemoveAll(x =>
                x.RulesetCharacter is not { IsDeadOrDyingOrUnconscious: false }
                || x.RulesetCharacter.HasConditionOfType(conditionHadTurmoil));

            if (targets.Empty())
            {
                yield break;
            }

            var actionParamsTurmoil = action.ActionParams.Clone();
            var usablePowerTurmoil = PowerProvider.Get(powerTurmoil, rulesetCharacter);

            actionParamsTurmoil.ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower;
            actionParamsTurmoil.RulesetEffect = rulesetImplementationService
                //CHECK: no need for AddAsActivePowerToSource
                .InstantiateEffectPower(rulesetCharacter, usablePowerTurmoil, false);
            actionParamsTurmoil.TargetCharacters.SetRange(targets);

            // must enqueue actions
            actionService.ExecuteAction(actionParamsTurmoil, null, true);
        }
    }

    //
    // Tides of Chaos
    //

    private sealed class OnReducedToZeroHpByMeOrAllyTidesOfChaos(
        ConditionDefinition conditionTurmoil,
        FeatureDefinitionPower powerTidesOfChaos)
        :
            IOnReducedToZeroHpByMeOrAlly, IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerTidesOfChaos;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var temporaryHitPointsForm = effectDescription.EffectForms[0].TemporaryHitPointsForm;
            var monkLevel = character.GetClassLevel(CharacterClassDefinitions.Monk);

            temporaryHitPointsForm.BonusHitPoints = monkLevel;

            return effectDescription;
        }

        public IEnumerator HandleReducedToZeroHpByMeOrAlly(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            GameLocationCharacter ally,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var rulesetAlly = ally.RulesetCharacter;
            var markOnceInAnyTurn = powerTidesOfChaos.Name + attacker.Name;

            if (rulesetAlly is not { IsDeadOrDyingOrUnconscious: false }
                || !ally.OncePerTurnIsValid(markOnceInAnyTurn))
            {
                yield break;
            }

            var rulesetDowned = downedCreature.RulesetCharacter;

            if (!rulesetDowned.HasConditionOfType(conditionTurmoil) || !ally.IsWithinRange(downedCreature, 6))
            {
                yield break;
            }

            ally.UsedSpecialFeatures.TryAdd(markOnceInAnyTurn, 0);

            // regain Ki Point
            rulesetAlly.ForceKiPointConsumption(-1);
            rulesetAlly.KiPointsAltered?.Invoke(rulesetAlly, rulesetAlly.RemainingKiPoints);

            // temporarily heal
            var usablePower = PowerProvider.Get(powerTidesOfChaos, rulesetAlly);
            var actionParams = new CharacterActionParams(ally, ActionDefinitions.Id.SpendPower)
            {
                ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower,
                RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                    //CHECK: no need for AddAsActivePowerToSource
                    .InstantiateEffectPower(rulesetAlly, usablePower, false),
                targetCharacters = { ally }
            };

            // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(actionParams, null, true);
        }
    }
}
