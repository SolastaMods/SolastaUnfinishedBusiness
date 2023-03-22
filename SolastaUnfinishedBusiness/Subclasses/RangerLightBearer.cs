using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RangerLightBearer : AbstractSubclass
{
    private const string Name = "RangerLightBearer";

    internal RangerLightBearer()
    {
        // LEVEL 03

        // Light Bearer Magic

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("Ranger")
            .SetSpellcastingClass(CharacterClassDefinitions.Ranger)
            .SetPreparedSpellGroups(
                BuildSpellGroup(3, Bless),
                BuildSpellGroup(5, PassWithoutTrace),
                BuildSpellGroup(9, SpellsContext.BlindingSmite),
                BuildSpellGroup(13, SpellsContext.StaggeringSmite),
                BuildSpellGroup(17, SpellsContext.BanishingSmite))
            .AddToDB();

        var powerLight = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Light")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(Light.EffectDescription)
            .AddToDB();

        var featureSetLight = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Light")
            .SetGuiPresentationNoContent(true)
            .AddFeatureSet(powerLight)
            .AddToDB();

        // Blessed Warrior

        var conditionBlessedWarrior = ConditionDefinitionBuilder
            .Create($"Condition{Name}BlessedWarrior")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMarkedByBrandingSmite)
            .SetConditionType(ConditionType.Detrimental)
            .AddToDB();

        var powerBlessedWarrior = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BlessedWarrior")
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerBlessedWarrior", Resources.PowerBlessedWarrior, 256, 128))
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionBlessedWarrior, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(new ModifyAttackModeForWeaponBlessedWarrior(conditionBlessedWarrior))
            .AddToDB();

        // Lifebringer

        var attributeModifierLifeBringerBase = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}LifeBringerBase")
            .SetGuiPresentationNoContent(true)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.Set,
                AttributeDefinitions.HealingPool, 0)
            .AddToDB();

        var attributeModifierLifeBringerAdditive = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}LifeBringerAdditive")
            .SetGuiPresentationNoContent(true)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.HealingPool, 1)
            .AddToDB();

        var powerLifeBringer = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerPaladinLayOnHands, $"Power{Name}LifeBringer")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerLifeBringer", Resources.PowerLifeBringer, 256, 128))
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(FeatureDefinitionPowers.PowerPaladinLayOnHands)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.Individuals)
                    .SetRestrictedCreatureFamilies(
                        CharacterFamilyDefinitions.Construct, CharacterFamilyDefinitions.Undead)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetHealingForm(HealingComputation.Pool, 0, DieType.D1, 0, true,
                                HealingCap.HalfMaximumHitPoints)
                            .Build())
                    .Build())
            .AddToDB();

        // LEVEL 07

        // Blessed Glow

        /*
        
        You have the ability to conjure holy light. When you cast Light, you can force each creature of your choice within 20 ft of the light source to succeed on a constitution saving thrown against your spell saving DC or be blinded for 1 minute. An undead or fiend automatically fail this saving throw. 
         
        */

        // LEVEL 11

        // Angelic Form

        var conditionAngelicForm = ConditionDefinitionBuilder
            .Create($"Condition{Name}AngelicForm")
            .AddFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionFlyingAdaptive,
                FeatureDefinitionAttackModifierBuilder
                    .Create($"AttackModifier{Name}AngelicForm")
                    .AddToDB())
            .AddToDB();

        var powerAngelicFormSprout = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AngelicFormSprout")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionAngelicForm,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        powerAngelicFormSprout.SetCustomSubFeatures(new OnAfterActionFeatureAngelicForm(powerAngelicFormSprout));

        var powerAngelicFormDismiss = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AngelicFormDismiss")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.AtWill)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionAngelicForm,
                                ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(
                new ValidatorsPowerUse(ValidatorsCharacter.HasAnyOfConditions(conditionAngelicForm.Name)))
            .AddToDB();

        var featureSetAngelicForm = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AngelicForm")
            .SetGuiPresentation($"Power{Name}AngelicFormSprout", Category.Feature)
            .AddFeatureSet(powerAngelicFormSprout, powerAngelicFormDismiss)
            .AddToDB();

        // LEVEL 15

        // Warding Light

        /*
        
        You can use your holy light to temporarily blind assailants. When a creature within 60 ft makes an attack roll against a target and doesn't have advantage on the roll, you can use your reaction to impose disadvantage on it.
        
        */

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RangerLightBearer, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells,
                featureSetLight,
                powerBlessedWarrior,
                attributeModifierLifeBringerBase,
                attributeModifierLifeBringerAdditive,
                powerLifeBringer)
            .AddFeaturesAtLevel(7)
            .AddFeaturesAtLevel(11,
                featureSetAngelicForm)
            .AddFeaturesAtLevel(15)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Blessed Warrior
    //

    private sealed class ModifyAttackModeForWeaponBlessedWarrior : IBeforeAttackEffect
    {
        private readonly ConditionDefinition _conditionDefinition;

        public ModifyAttackModeForWeaponBlessedWarrior(ConditionDefinition conditionDefinition)
        {
            _conditionDefinition = conditionDefinition;
        }

        public void BeforeOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (attackMode == null || outcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                return;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (!rulesetDefender.HasAnyConditionOfType(_conditionDefinition.Name))
            {
                return;
            }

            var effectDescription = attackMode.EffectDescription;

            foreach (var damageForm in effectDescription.EffectForms
                         .Where(x => x.FormType == EffectForm.EffectFormType.Damage))
            {
                damageForm.DamageForm.damageType = DamageTypeRadiant;
            }

            var damage = effectDescription.FindFirstDamageForm();
            var k = effectDescription.EffectForms.FindIndex(form => form.damageForm == damage);

            // add additional radiant dice
            var classLevel = attacker.RulesetCharacter.GetClassLevel(CharacterClassDefinitions.Ranger);
            var diceNumber = classLevel < 11 ? 1 : 2;
            var additionalDice = EffectFormBuilder
                .Create()
                .SetDamageForm(DamageTypeRadiant, diceNumber, DieType.D8)
                .Build();

            effectDescription.EffectForms.Insert(k + 1, additionalDice);

            // remove condition on successful attack
            var rulesetCondition =
                rulesetDefender.AllConditions.FirstOrDefault(x => x.ConditionDefinition == _conditionDefinition);

            if (rulesetCondition != null)
            {
                rulesetDefender.RemoveCondition(rulesetCondition);
            }
        }
    }

    //
    // Angelic Form
    //

    private sealed class OnAfterActionFeatureAngelicForm : IOnAfterActionFeature
    {
        private static FeatureDefinitionPower _featureDefinitionPower;

        public OnAfterActionFeatureAngelicForm(FeatureDefinitionPower featureDefinitionPower)
        {
            _featureDefinitionPower = featureDefinitionPower;
        }

        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != _featureDefinitionPower)
            {
                return;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var classLevel = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Ranger);

            rulesetCharacter.ReceiveTemporaryHitPoints(
                classLevel, DurationType.Minute, 1, TurnOccurenceType.EndOfTurn, rulesetCharacter.Guid);
        }
    }
}
