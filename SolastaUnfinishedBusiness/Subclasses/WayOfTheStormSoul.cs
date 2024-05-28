using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfTheStormSoul : AbstractSubclass
{
    private const string Name = "WayOfTheStormSoul";

    public WayOfTheStormSoul()
    {
        // LEVEL 03

        // Disciple of Storms

        var additionalDamageDiscipleOfStorms = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DiscipleOfStorms")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("DiscipleOfStorms")
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.FlurryOfBlows)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.SameAsBaseWeaponDie)
            .SetSpecificDamageType(DamageTypeLightning)
            .SetImpactParticleReference(LightningBolt)
            .AddCustomSubFeatures(new MagicEffectFinishedByMeAnyDiscipleOfStorms())
            .AddToDB();

        // LEVEL 06

        // Lightning Warrior

        var powerLightningLure = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}LightningLure")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("LightningLure", Resources.LightningLure, 128))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 3, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.DragToOrigin, 2)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeLightning, 1, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (5, 1), (11, 2), (17, 3))
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(LightningBolt)
                    .Build())
            .AddToDB();

        var featureSetLightningWarrior = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}LightningWarrior")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerLightningLure, FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance)
            .AddToDB();

        // LEVEL 11

        // Tempest's Fury

        var powerTempestFury = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TempestFury")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetParticleEffectParameters(ShockingGrasp)
                    .Build())
            .AddCustomSubFeatures(
                ValidatorsValidatePowerUse.HasBonusAttackAvailable,
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasAnyOfConditions(ConditionFlurryOfBlows)),
                new AttackAfterMagicEffectTempestFury())
            .AddToDB();

        // LEVEL 17

        // Eye of The Storm

        var conditionEyeOfTheStorm = ConditionDefinitionBuilder
            .Create($"Condition{Name}EyeOfTheStorm")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionShocked)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .CopyParticleReferences(ConditionDefinitions.ConditionShocked)
            .SetSpecialDuration(DurationType.Minute, 1, TurnOccurenceType.EndOfSourceTurn)
            .AddToDB();

        var additionalDamageEyeOfTheStorm = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}EyeOfTheStorm")
            .SetGuiPresentationNoContent(true)
            .SetRequiredProperty(RestrictedContextRequiredProperty.UnarmedOrMonkWeapon)
            .SetImpactParticleReference(ConditionDefinitions.ConditionShocked.conditionParticleReference)
            .AddConditionOperation(ConditionOperationDescription.ConditionOperation.Add, conditionEyeOfTheStorm)
            .AddToDB();

        var powerEyeOfTheStormLeap = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EyeOfTheStormLeap")
            .SetGuiPresentation($"FeatureSet{Name}EyeOfTheStorm", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 0, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeLightning, 5, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionEyeOfTheStorm, ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .SetParticleEffectParameters(PowerDomainElementalLightningBlade)
                    .Build())
            .AddCustomSubFeatures(ValidatorsValidatePowerUse.InCombat)
            .AddToDB();

        powerEyeOfTheStormLeap.EffectDescription.EffectParticleParameters.impactParticleReference =
            powerEyeOfTheStormLeap.EffectDescription.EffectParticleParameters.effectParticleReference;

        var powerEyeOfTheStorm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EyeOfTheStorm")
            .SetGuiPresentation($"FeatureSet{Name}EyeOfTheStorm", Category.Feature, PowerOathOfDevotionTurnUnholy)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 3)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(ShockingGrasp)
                    .Build())
            .AddCustomSubFeatures(
                ValidatorsValidatePowerUse.InCombat,
                new MagicEffectFinishedByMeEyeOfTheStorm(powerEyeOfTheStormLeap, conditionEyeOfTheStorm))
            .AddToDB();

        var featureSetEyeOfTheStorm = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}EyeOfTheStorm")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(additionalDamageEyeOfTheStorm, powerEyeOfTheStorm, powerEyeOfTheStormLeap)
            .AddToDB();

        powerLightningLure.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new CustomBehaviorLightningLure(powerLightningLure, conditionEyeOfTheStorm));

        //
        // MAIN
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheTempest, 256))
            .AddFeaturesAtLevel(3, additionalDamageDiscipleOfStorms)
            .AddFeaturesAtLevel(6, featureSetLightningWarrior)
            .AddFeaturesAtLevel(11, powerTempestFury)
            .AddFeaturesAtLevel(17, featureSetEyeOfTheStorm)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Monk;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Disciple of Storms
    //

    private sealed class MagicEffectFinishedByMeAnyDiscipleOfStorms : IMagicEffectFinishedByMeAny
    {
        public IEnumerator OnMagicEffectFinishedByMeAny(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != PowerMonkFlurryOfBlows)
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                ConditionDisengaging,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                // all disengaging in game is set under TagCombat (why?)
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                ConditionDisengaging,
                0,
                0,
                0);
        }
    }

    //
    // Lightning Lure
    //

    private sealed class CustomBehaviorLightningLure(
        FeatureDefinitionPower powerLightningLure,
        ConditionDefinition conditionEyeOfTheStorm)
        : IModifyEffectDescription, IMagicEffectFinishedByMe, IValidatePowerUse
    {
        private const string Tag = "LightningLure";

        private readonly EffectForm _effectFormEyeOfTheStorm = EffectFormBuilder
            .Create()
            .HasSavingThrow(EffectSavingThrowType.Negates)
            .SetConditionForm(conditionEyeOfTheStorm, ConditionForm.ConditionOperation.Add)
            .Build();

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;

            actingCharacter.BurnOneMainAttack();
            actingCharacter.UsedSpecialFeatures.TryAdd(Tag, 0);

            yield break;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerLightningLure &&
                   character.GetClassLevel(CharacterClassDefinitions.Monk) >= 17;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            effectDescription.EffectForms.Add(_effectFormEyeOfTheStorm);

            return effectDescription;
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            var glc = GameLocationCharacter.GetFromActor(character);

            return glc != null && glc.OnceInMyTurnIsValid(Tag);
        }
    }

    //
    // Tempest Fury
    //

    internal sealed class AttackAfterMagicEffectTempestFury : IAttackAfterMagicEffect
    {
        public IAttackAfterMagicEffect.CanAttackHandler CanAttack { get; } =
            CanMeleeAttack;

        public IAttackAfterMagicEffect.GetAttackAfterUseHandler PerformAttackAfterUse { get; } =
            DefaultAttackHandler;

        public IAttackAfterMagicEffect.CanUseHandler CanBeUsedToAttack { get; } =
            DefaultCanUseHandler;

        private static bool CanMeleeAttack([NotNull] GameLocationCharacter caster, GameLocationCharacter target)
        {
            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackOff);

            if (attackMode == null)
            {
                return false;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var attackModifier = new ActionModifier();
            var evalParams = new BattleDefinitions.AttackEvaluationParams();

            evalParams.FillForPhysicalReachAttack(
                caster, caster.LocationPosition, attackMode, target, target.LocationPosition, attackModifier);

            return battleService.CanAttack(evalParams);
        }

        [CanBeNull]
        private static IEnumerable<CharacterActionParams> DefaultAttackHandler(
            [CanBeNull] CharacterActionMagicEffect effect)
        {
            var actionParams = effect?.ActionParams;

            if (actionParams == null)
            {
                return null;
            }

            if (Gui.Battle == null)
            {
                return null;
            }

            var caster = actionParams.ActingCharacter;
            var targets = Gui.Battle
                .GetContenders(caster, withinRange: 1);

            if (targets.Count == 0)
            {
                return null;
            }

            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackOff);

            if (attackMode == null)
            {
                return null;
            }

            //get copy to be sure we don't break existing mode
            var rulesetAttackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            rulesetAttackModeCopy.Copy(attackMode);

            attackMode = rulesetAttackModeCopy;

            //set action type to be same as the one used for the magic effect
            attackMode.ActionType = effect.ActionType;

            var attackModifier = new ActionModifier();

            return targets
                .Where(t => CanMeleeAttack(caster, t))
                .Select(target =>
                    new CharacterActionParams(caster, ActionDefinitions.Id.AttackFree)
                    {
                        AttackMode = attackMode, TargetCharacters = { target }, ActionModifiers = { attackModifier }
                    });
        }

        private static bool DefaultCanUseHandler(
            [NotNull] CursorLocationSelectTarget targeting,
            GameLocationCharacter caster,
            GameLocationCharacter target, [NotNull] out string failure)
        {
            failure = string.Empty;

            return true;
        }
    }

    //
    // Eye of The Storm
    //

    internal sealed class MagicEffectFinishedByMeEyeOfTheStorm(
        FeatureDefinitionPower powerEyeOfTheStormLeap,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionEyeOfTheStorm) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            var attacker = action.ActingCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;

            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerEyeOfTheStormLeap, rulesetAttacker);
            var targets = Gui.Battle.GetContenders(attacker)
                .Where(x =>
                    x.RulesetActor.AllConditions
                        .Any(y => y.ConditionDefinition == conditionEyeOfTheStorm &&
                                  y.SourceGuid == rulesetAttacker.Guid))
                .ToList();
            var actionModifiers = new List<ActionModifier>();

            for (var i = 0; i < targets.Count; i++)
            {
                actionModifiers.Add(new ActionModifier());
            }

            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = actionModifiers,
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                UsablePower = usablePower,
                targetCharacters = targets
            };

            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);
        }
    }
}
