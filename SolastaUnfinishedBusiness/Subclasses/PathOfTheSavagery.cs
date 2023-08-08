using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static FeatureDefinitionAttributeModifier;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class PathOfTheSavagery : AbstractSubclass
{
    private const string Name = "PathOfTheSavagery";

    internal static readonly FeatureDefinitionPower PowerPrimalInstinct = FeatureDefinitionPowerBuilder
        .Create(PowerBarbarianRageStart, $"Power{Name}PrimalInstinct")
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.RagePoints)
        .SetOverriddenPower(PowerBarbarianRageStart)
        .AddToDB();

    public PathOfTheSavagery()
    {
        // LEVEL 03

        // Savage Strength

        var attackModifierSavageStrength = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{Name}SavageStrength")
            .SetGuiPresentation(Category.Feature)
            .SetDualWield(true, true)
            .AddToDB();

        // Primal Instinct

        var actionAffinityCombatRageStart = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}PrimalInstinct")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.CombatRageStart)
            .SetForbiddenActions(ActionDefinitions.Id.RageStart)
            .AddToDB();

        var featureSetPrimalInstinct = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}PrimalInstinct")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(actionAffinityCombatRageStart, PowerPrimalInstinct)
            .AddToDB();

        // LEVEL 06

        // Wrath and Fury

#if false
        var conditionGrievousWound = ConditionDefinitionBuilder
            .Create($"Condition{Name}GrievousWound")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBleeding)
            .SetPossessive()
            .CopyParticleReferences(ConditionDefinitions.ConditionBleeding)
            .SetConditionType(ConditionType.Detrimental)
            .CopyParticleReferences(ConditionDefinitions.ConditionStunned)
            .SetParentCondition(ConditionDefinitions.ConditionIncapacitated)
            .AddFeatures(ConditionDefinitions.ConditionIncapacitated.Features.ToArray())
            .AddToDB();

        var powerGrievousWound = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}GrievousWound")
            .SetGuiPresentation($"Condition{Name}GrievousWound", Category.Condition)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Strength)
                    .SetParticleEffectParameters(SpellDefinitions.VampiricTouch)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionGrievousWound, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        powerGrievousWound.EffectDescription.EffectParticleParameters.conditionParticleReference =
            ConditionDefinitions.ConditionStunned.conditionParticleReference;
        powerGrievousWound.EffectDescription.EffectParticleParameters.conditionEndParticleReference =
            ConditionDefinitions.ConditionStunned.conditionEndParticleReference;
        powerGrievousWound.EffectDescription.EffectParticleParameters.conditionStartParticleReference =
            ConditionDefinitions.ConditionStunned.conditionStartParticleReference;
#endif

        var featureWrathAndFury = FeatureDefinitionBuilder
            .Create($"Feature{Name}WrathAndFury")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                //new AttackEffectAfterDamageWrathAndFury(powerGrievousWound),
                new UpgradeWeaponDice(GeUpgradedDice, ValidatorsWeapon.AlwaysValid,
                    ValidatorsCharacter.HasMeleeWeaponInMainAndOffhand))
            .AddToDB();

        // LEVEL 10

        // Unbridled Ferocity

        var conditionUnbridledFerocity = ConditionDefinitionBuilder
            .Create($"Condition{Name}UnbridledFerocity")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionSorcererChildRiftDeflection)
            .SetSilent(Silent.WhenRemoved)
            .SetSpecialDuration(DurationType.Permanent)
            .SetSpecialInterruptions(ConditionInterruption.RageStop)
            .SetPossessive()
            .AllowMultipleInstances()
            .AddFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}UnbridledFerocity")
                    .SetGuiPresentation($"Condition{Name}UnbridledFerocity", Category.Condition)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.CriticalThreshold, -1)
                    .AddToDB())
            .AddToDB();

        var featureUnbridledFerocity = FeatureDefinitionBuilder
            .Create($"Feature{Name}UnbridledFerocity")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new PhysicalAttackAfterDamageUnbridledFerocity(conditionUnbridledFerocity))
            .AddToDB();

        // LEVEL 14

        // Furious Defense

        var featureFuriousDefense = FeatureDefinitionAttributeModifierBuilder
            .Create($"Feature{Name}FuriousDefense")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
            .SetSituationalContext(ExtraSituationalContext.IsRagingAndDualWielding)
            .AddToDB();

        featureFuriousDefense.SetCustomSubFeatures(new ModifySavingThrowAttributeFuriousDefense(featureFuriousDefense));

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheSavagery, 256))
            .AddFeaturesAtLevel(3, attackModifierSavageStrength, featureSetPrimalInstinct)
            .AddFeaturesAtLevel(6, featureWrathAndFury)
            .AddFeaturesAtLevel(10, featureUnbridledFerocity)
            .AddFeaturesAtLevel(14, featureFuriousDefense)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Barbarian;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static (int number, DieType dieType, DieType versatileDieType) GeUpgradedDice(
        RulesetCharacter rulesetCharacter, DamageForm damageForm)
    {
        var upgradeDiceMap = new Dictionary<DieType, DieType>
        {
            { DieType.D1, DieType.D2 },
            { DieType.D2, DieType.D3 },
            { DieType.D3, DieType.D4 },
            { DieType.D4, DieType.D6 },
            { DieType.D6, DieType.D8 },
            { DieType.D8, DieType.D10 },
            { DieType.D10, DieType.D12 },
            { DieType.D12, DieType.D12 },
            { DieType.D20, DieType.D20 }
        };

        var dieType = damageForm.dieType;
        var versatileDieType = damageForm.VersatileDieType;
        var diceNumber = damageForm.DiceNumber;

        return (diceNumber, upgradeDiceMap[dieType], upgradeDiceMap[versatileDieType]);
    }

#if false
    private sealed class AttackEffectAfterDamageWrathAndFury : IPhysicalAttackAfterDamage
    {
        private readonly FeatureDefinitionPower _powerGrievousWound;

        public AttackEffectAfterDamageWrathAndFury(
            FeatureDefinitionPower powerGrievousWound)
        {
            _powerGrievousWound = powerGrievousWound;
        }

        public void OnPhysicalAttackAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var rulesetDefender = defender.RulesetCharacter;

            // only on critical hits
            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                outcome is not RollOutcome.CriticalSuccess)
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not {IsDeadOrDyingOrUnconscious:false})
            {
                return;
            }

            // only if dual wielding melee
            if (!ValidatorsCharacter.HasMeleeWeaponInMainAndOffhand(attacker.RulesetCharacter))
            {
                return;
            }

            var usablePower = UsablePowersProvider.Get(_powerGrievousWound, rulesetAttacker);
            var effectPower = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectPower(rulesetAttacker, usablePower, false)
                .AddAsActivePowerToSource();

            GameConsoleHelper.LogCharacterUsedPower(rulesetAttacker, _powerGrievousWound);
            effectPower.ApplyEffectOnCharacter(rulesetDefender, true, defender.LocationPosition);
        }
    }
#endif

    private sealed class PhysicalAttackAfterDamageUnbridledFerocity : IPhysicalAttackAfterDamage
    {
        private readonly ConditionDefinition _conditionUnbridledFerocity;

        public PhysicalAttackAfterDamageUnbridledFerocity(ConditionDefinition conditionUnbridledFerocity)
        {
            _conditionUnbridledFerocity = conditionUnbridledFerocity;
        }

        public void OnPhysicalAttackAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (outcome == RollOutcome.CriticalSuccess)
            {
                rulesetAttacker.RemoveAllConditionsOfCategoryAndType(
                    AttributeDefinitions.TagCombat, _conditionUnbridledFerocity.Name);
            }
            else if (outcome == RollOutcome.Success && rulesetAttacker.HasAnyConditionOfType(ConditionRaging))
            {
                rulesetAttacker.InflictCondition(
                    _conditionUnbridledFerocity.Name,
                    _conditionUnbridledFerocity.DurationType,
                    _conditionUnbridledFerocity.DurationParameter,
                    _conditionUnbridledFerocity.turnOccurence,
                    AttributeDefinitions.TagCombat,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    null,
                    0,
                    0,
                    0);
            }
        }
    }

    private sealed class ModifySavingThrowAttributeFuriousDefense : IModifySavingThrowAttribute
    {
        private readonly FeatureDefinition _featureDefinition;

        public ModifySavingThrowAttributeFuriousDefense(FeatureDefinition featureDefinition)
        {
            _featureDefinition = featureDefinition;
        }

        public bool IsValid(RulesetActor rulesetActor, string attributeScore)
        {
            return attributeScore == AttributeDefinitions.Dexterity &&
                   rulesetActor.HasAnyConditionOfType(ConditionRaging);
        }

        public string SavingThrowAttribute(RulesetActor rulesetActor)
        {
            (rulesetActor as RulesetCharacter)!.LogCharacterUsedFeature(_featureDefinition);

            return AttributeDefinitions.Strength;
        }
    }
}
