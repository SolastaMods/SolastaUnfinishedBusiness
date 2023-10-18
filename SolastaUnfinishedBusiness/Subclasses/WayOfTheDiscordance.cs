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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttackModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfTheDiscordance : AbstractSubclass
{
    private const string Name = "WayOfTheDiscordance";
    private const int TurmoilLevel = 6;
    private const int EntropicStrikesLevel = 11;
    private static CharacterAction CurrentAttackAction => Global.CurrentAttackAction;

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

        var additionalDamageDiscordance = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}Discordance")
            .SetGuiPresentationNoContent(true)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionDefinition = conditionDiscordance
                })
            .AddCustomSubFeatures(
                new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, _, mode, _) =>
                    (OperationType.Set,
                        (ValidatorsWeapon.IsUnarmed(character, mode)
                         && CurrentAttackAction != null
                         && !CurrentAttackAction.ActionParams.TargetCharacters[0].RulesetCharacter
                             .HasConditionOfType(conditionHadDiscordanceDamageThisTurn))
                        || character.GetClassLevel(CharacterClassDefinitions.Monk) >= EntropicStrikesLevel)))
            .AddToDB();

        var powerDiscordanceDamage = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Discordance")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
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

        // when powers are checked for validation, the additional condition from Discordance additional damage hasn't been added already
        powerDiscordanceDamage.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(
                character =>
                    CurrentAttackAction != null
                    && ValidatorsWeapon.IsUnarmed(character, CurrentAttackAction.ActionParams.AttackMode)
                    && CurrentAttackAction.ActionParams.TargetCharacters[0].RulesetCharacter
                        .HasConditionOfType(conditionDiscordance)),
            new ModifyEffectDescriptionDiscordance(powerDiscordanceDamage));

        var featureSetDiscordance = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Discordance")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(additionalDamageDiscordance, powerDiscordanceDamage)
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
            new ValidatorsValidatePowerUse(character =>
                UsablePowersProvider.Get(powerChaosChanneling, character).RemainingUses > 0));

        powerChaosChannelingPoints.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(character =>
                UsablePowersProvider.Get(powerChaosChanneling, character).RemainingUses == 0));

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
            .AddCustomSubFeatures(new ForceConditionCategory(TagCombat))
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
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
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
            .AddToDB();

        // when powers are checked for validation, the additional condition from Discordance additional damage hasn't been added already
        // it also hasn't been removed so far by the Discordance damage power
        powerTurmoil.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(character =>
                CurrentAttackAction != null
                && ValidatorsWeapon.IsUnarmed(character, CurrentAttackAction.ActionParams.AttackMode)
                && !CurrentAttackAction.ActionParams.TargetCharacters[0].RulesetCharacter
                    .HasConditionOfType(conditionHadTurmoil)
                && CurrentAttackAction.ActionParams.TargetCharacters[0].RulesetCharacter
                    .HasConditionOfType(conditionDiscordance)));

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
                            Constitution,
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
                PowerVisibilityModifier.Hidden,
                new MagicEffectFinishedByMeBurstOfDisharmony(
                    conditionDiscordance, powerDiscordanceDamage, conditionHadTurmoil, powerTurmoil),
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
                            .SetTempHpForm(0, DieType.D4, 1, true)
                            .Build())
                    .SetParticleEffectParameters(PowerPactChainPseudodragon)
                    .Build())
            .AddToDB();

        powerTidesOfChaos.AddCustomSubFeatures(
            PowerVisibilityModifier.Hidden,
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
    // Discordance
    //

    private sealed class ModifyEffectDescriptionDiscordance : IModifyEffectDescription
    {
        private readonly BaseDefinition _powerDefinition;

        public ModifyEffectDescriptionDiscordance(BaseDefinition powerDefinition)
        {
            _powerDefinition = powerDefinition;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == _powerDefinition;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var monkLevel = character.GetClassLevel(CharacterClassDefinitions.Monk);
            var damageForm = effectDescription.FindFirstDamageForm();

            damageForm.BonusDamage = ComputeAbilityScoreModifier(character.TryGetAttributeValue(Wisdom));
            damageForm.DieType = AttackModifierMonkMartialArtsImprovedDamage.DieTypeByRankTable
                .Find(x => x.Rank == monkLevel).DieType;

            return effectDescription;
        }
    }

    //
    // Chaos Channeling
    //

    private sealed class ModifyWeaponAttackModeChaosChanneling : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidatorsWeapon.IsUnarmed(character, attackMode))
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
    // Burst of Disharmony
    //

    private sealed class MagicEffectFinishedByMeBurstOfDisharmony : IMagicEffectFinishedByMe
    {
        private readonly ConditionDefinition _conditionDiscordance;
        private readonly ConditionDefinition _conditionHadTurmoil;
        private readonly FeatureDefinitionPower _powerDiscordance;
        private readonly FeatureDefinitionPower _powerTurmoil;

        public MagicEffectFinishedByMeBurstOfDisharmony(
            ConditionDefinition conditionDiscordance,
            FeatureDefinitionPower powerDiscordance,
            ConditionDefinition conditionHadTurmoil,
            FeatureDefinitionPower powerTurmoil)
        {
            _conditionDiscordance = conditionDiscordance;
            _powerDiscordance = powerDiscordance;
            _conditionHadTurmoil = conditionHadTurmoil;
            _powerTurmoil = powerTurmoil;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            // Discordance Damage
            var targets = new List<GameLocationCharacter>();

            targets.SetRange(action.actionParams.TargetCharacters
                .Where(x => x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }
                            && x.RulesetCharacter.AllConditions.Count(y =>
                                y.ConditionDefinition == _conditionDiscordance) > 1));

            if (targets.Empty())
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();

            var actionParamsDiscordance = action.ActionParams.Clone();
            var usablePowerDiscordance = UsablePowersProvider.Get(_powerDiscordance, rulesetCharacter);

            actionParamsDiscordance.ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower;
            actionParamsDiscordance.RulesetEffect = rulesetImplementationService
                .InstantiateEffectPower(rulesetCharacter, usablePowerDiscordance, false)
                .AddAsActivePowerToSource();
            actionParamsDiscordance.TargetCharacters.SetRange(targets);

            // special case don't ExecuteAction here
            action.ResultingActions.Add(new CharacterActionSpendPower(actionParamsDiscordance));

            // Turmoil
            var monkLevel = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Monk);

            if (monkLevel < TurmoilLevel)
            {
                yield break;
            }

            targets.RemoveAll(x =>
                x.RulesetCharacter is not { IsDeadOrDyingOrUnconscious: false }
                || x.RulesetCharacter.HasConditionOfType(_conditionHadTurmoil));

            if (targets.Empty())
            {
                yield break;
            }

            var actionParamsTurmoil = action.ActionParams.Clone();
            var usablePowerTurmoil = UsablePowersProvider.Get(_powerTurmoil, rulesetCharacter);

            actionParamsTurmoil.ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower;
            actionParamsTurmoil.RulesetEffect = rulesetImplementationService
                .InstantiateEffectPower(rulesetCharacter, usablePowerTurmoil, false)
                .AddAsActivePowerToSource();
            actionParamsTurmoil.TargetCharacters.SetRange(targets);

            // special case don't ExecuteAction here
            action.ResultingActions.Add(new CharacterActionSpendPower(actionParamsTurmoil));
        }
    }

    //
    // Tides of Chaos
    //

    private sealed class OnReducedToZeroHpByMeOrAllyTidesOfChaos :
        IOnReducedToZeroHpByMeOrAlly, IModifyEffectDescription
    {
        private readonly ConditionDefinition _conditionTurmoil;
        private readonly FeatureDefinitionPower _powerTidesOfChaos;

        public OnReducedToZeroHpByMeOrAllyTidesOfChaos(
            ConditionDefinition conditionTurmoil,
            FeatureDefinitionPower powerTidesOfChaos)
        {
            _conditionTurmoil = conditionTurmoil;
            _powerTidesOfChaos = powerTidesOfChaos;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == _powerTidesOfChaos;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var temporaryHitPointsForm = effectDescription.EffectForms[0].TemporaryHitPointsForm;
            var levels = character.GetClassLevel(CharacterClassDefinitions.Monk);
            var dieType = levels switch
            {
                >= 17 => DieType.D10,
                >= 11 => DieType.D8,
                >= 5 => DieType.D6,
                _ => DieType.D4
            };

            temporaryHitPointsForm.dieType = dieType;
            temporaryHitPointsForm.bonusHitPoints = (levels + 1) / 2;

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
            var markOnceInAnyTurn = _powerTidesOfChaos.Name + attacker.Name;

            if (rulesetAlly is not { IsDeadOrDyingOrUnconscious: false }
                || !ally.OncePerTurnIsValid(markOnceInAnyTurn))
            {
                yield break;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var rulesetDowned = downedCreature.RulesetCharacter;

            if (!rulesetDowned.HasConditionOfType(_conditionTurmoil)
                || !battleService.IsWithinXCells(ally, downedCreature, 6))
            {
                yield break;
            }

            ally.UsedSpecialFeatures.TryAdd(markOnceInAnyTurn, 0);

            // regain Ki Point
            rulesetAlly.ForceKiPointConsumption(-1);
            rulesetAlly.KiPointsAltered?.Invoke(rulesetAlly, rulesetAlly.RemainingKiPoints);

            // temporarily heal
            var usablePower = UsablePowersProvider.Get(_powerTidesOfChaos, rulesetAlly);
            var actionParams = new CharacterActionParams(ally, ActionDefinitions.Id.SpendPower)
            {
                ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower,
                RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                    .InstantiateEffectPower(rulesetAlly, usablePower, false)
                    .AddAsActivePowerToSource(),
                targetCharacters = { ally }
            };

            // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(actionParams, null, true);
        }
    }
}
