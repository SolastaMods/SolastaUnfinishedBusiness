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
                Sprites.GetSprite("PowerLightningLure", Resources.PowerLightningLure, 128))
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
                            .SetDamageForm(DamageTypeLightning, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(LightningBolt)
                    .SetCasterEffectParameters(PowerDomainElementalLightningBlade)
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
            .SetGuiPresentation(Category.Feature, PowerOathOfDevotionTurnUnholy)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetCasterEffectParameters(ShockingGrasp)
                    .Build())
            .AddCustomSubFeatures(
                ValidatorsValidatePowerUse.HasBonusAttackAvailable,
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasAnyOfConditions(ConditionFlurryOfBlows)),
                new MagicEffectFinishedByMeTempestFury())
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
            .SetGuiPresentation($"FeatureSet{Name}EyeOfTheStorm", Category.Feature,
                Sprites.GetSprite(Name, Resources.PowerEyeOfTheStorm, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 3)
            .SetShowCasting(false)
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
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.DomainElementalLighting)
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
        ConditionDefinition conditionEyeOfTheStorm) : IModifyEffectDescription, IMagicEffectFinishedByMe
    {
        private readonly EffectForm _effectFormEyeOfTheStorm = EffectFormBuilder
            .Create()
            .HasSavingThrow(EffectSavingThrowType.Negates)
            .SetConditionForm(conditionEyeOfTheStorm, ConditionForm.ConditionOperation.Add)
            .Build();

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;

            actingCharacter.BurnOneMainAttack();

            yield break;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerLightningLure;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (character.GetClassLevel(CharacterClassDefinitions.Monk) >= 17)
            {
                effectDescription.EffectForms.Add(_effectFormEyeOfTheStorm);
            }

            effectDescription.EffectForms[1].DamageForm.DieType = character.GetMonkDieType();
            effectDescription.EffectForms[1].DamageForm.BonusDamage = AttributeDefinitions.ComputeAbilityScoreModifier(
                character.TryGetAttributeValue(AttributeDefinitions.Dexterity));

            return effectDescription;
        }
    }

    //
    // Tempest Fury
    //

    internal sealed class MagicEffectFinishedByMeTempestFury : IMagicEffectFinishedByMe, IValidatePowerUse
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var targets = Gui.Battle.GetContenders(actingCharacter, withinRange: 1);

            if (targets.Count == 0)
            {
                yield break;
            }

            var attackModeOff = actingCharacter.FindActionAttackMode(ActionDefinitions.Id.AttackOff);

            if (attackModeOff == null)
            {
                yield break;
            }

            //get copy to be sure we don't break existing mode
            var attackMode = RulesetAttackMode.AttackModesPool.Get();

            attackMode.Copy(attackModeOff);
            attackMode.ActionType = ActionDefinitions.ActionType.NoCost;

            actingCharacter.BurnOneBonusAttack();
            actingCharacter.UsedSpecialFeatures.TryAdd("PowerTempestFury", 0);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var target in targets)
            {
                var attackModifier = new ActionModifier();
                var actionParams = new CharacterActionParams(actingCharacter, ActionDefinitions.Id.AttackFree)
                {
                    AttackMode = attackMode, TargetCharacters = { target }, ActionModifiers = { attackModifier }
                };

                ServiceRepository.GetService<IGameLocationActionService>()?
                    .ExecuteAction(actionParams, null, true);
            }
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            var glc = GameLocationCharacter.GetFromActor(character);

            return glc != null && glc.OncePerTurnIsValid("PowerTempestFury");
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
