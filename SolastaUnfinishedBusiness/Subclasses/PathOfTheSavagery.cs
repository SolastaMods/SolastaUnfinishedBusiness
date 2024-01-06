using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

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

        // Furious Defense

        var featureFuriousDefense = FeatureDefinitionAttributeModifierBuilder
            .Create($"Feature{Name}FuriousDefense")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
            .SetSituationalContext(ExtraSituationalContext.IsRagingAndDualWielding)
            .AddToDB();

        // LEVEL 10

        // Wrath and Fury

        var featureWrathAndFury = FeatureDefinitionBuilder
            .Create($"Feature{Name}WrathAndFury")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(
                //new AttackEffectAfterDamageWrathAndFury(powerGrievousWound),
                new UpgradeWeaponDice(GeUpgradedDice, ValidatorsWeapon.AlwaysValid,
                    ValidatorsCharacter.HasMeleeWeaponInMainAndOffhand),
                new ActionFinishedByMeWrathAndFury())
            .AddToDB();

        // LEVEL 14

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
            .AddCustomSubFeatures(new PhysicalAttackAfterDamageUnbridledFerocity(conditionUnbridledFerocity))
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheSavagery, 256))
            .AddFeaturesAtLevel(3, attackModifierSavageStrength, featureSetPrimalInstinct)
            .AddFeaturesAtLevel(6, featureFuriousDefense)
            .AddFeaturesAtLevel(10, featureWrathAndFury)
            .AddFeaturesAtLevel(14, featureUnbridledFerocity)
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


    internal static void OnRollSavingThrowFuriousDefense(RulesetActor defender, ref string abilityScoreName)
    {
        if (abilityScoreName != AttributeDefinitions.Dexterity ||
            !defender.HasAnyConditionOfType(ConditionRaging))
        {
            return;
        }

        var dexSavingThrowBonus =
            AttributeDefinitions.ComputeAbilityScoreModifier(
                defender.TryGetAttributeValue(AttributeDefinitions.Dexterity)) +
            defender.ComputeBaseSavingThrowBonus(AttributeDefinitions.Dexterity, []);

        var strSavingThrowBonus =
            AttributeDefinitions.ComputeAbilityScoreModifier(
                defender.TryGetAttributeValue(AttributeDefinitions.Strength)) +
            defender.ComputeBaseSavingThrowBonus(AttributeDefinitions.Strength, []);

        if (dexSavingThrowBonus >= strSavingThrowBonus)
        {
            return;
        }

        abilityScoreName = AttributeDefinitions.Strength;
    }

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
                    AttributeDefinitions.TagEffect, _conditionUnbridledFerocity.Name);
            }
            else if (outcome == RollOutcome.Success && rulesetAttacker.HasAnyConditionOfType(ConditionRaging))
            {
                rulesetAttacker.InflictCondition(
                    _conditionUnbridledFerocity.Name,
                    _conditionUnbridledFerocity.DurationType,
                    _conditionUnbridledFerocity.DurationParameter,
                    _conditionUnbridledFerocity.turnOccurence,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    _conditionUnbridledFerocity.Name,
                    0,
                    0,
                    0);
            }
        }
    }


    private sealed class ActionFinishedByMeWrathAndFury : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            if (characterAction is not CharacterActionRecklessAttack)
            {
                yield break;
            }

            var rulesetCharacter = characterAction.ActingCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetCharacter.HasAnyConditionOfType(ConditionReckless))
            {
                yield break;
            }

            var classLevel = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Barbarian);
            var temporaryHitPoints = (classLevel + 1) / 2;

            rulesetCharacter.ReceiveTemporaryHitPoints(
                temporaryHitPoints, DurationType.Minute, 1, TurnOccurenceType.EndOfTurn, rulesetCharacter.Guid);
        }
    }
}
