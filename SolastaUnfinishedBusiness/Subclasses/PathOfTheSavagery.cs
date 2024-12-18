using System.Collections;
using System.Collections.Generic;
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
        .AddCustomSubFeatures(new ModifyEffectDescriptionPrimalInstinct())
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
                new UpgradeWeaponDice(GeUpgradedDice, (_, _, c) =>
                    ValidatorsCharacter.HasMeleeWeaponInMainAndOffhand(c)),
                new ActionFinishedByMeWrathAndFury())
            .AddToDB();

        // LEVEL 14

        // Unbridled Ferocity

        var conditionUnbridledFerocity = ConditionDefinitionBuilder
            .Create($"Condition{Name}UnbridledFerocity")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionSorcererChildRiftDeflection)
            .SetSilent(Silent.WhenRemoved)
            // don't use vanilla RageStop with permanent conditions
            .SetSpecialInterruptions(ExtraConditionInterruption.SourceRageStop)
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
            .AddCustomSubFeatures(new PhysicalAttackFinishedByMeUnbridledFerocity(conditionUnbridledFerocity))
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

    internal static void OnRollSavingThrowFuriousDefense(RulesetCharacter defender, ref string abilityScoreName)
    {
        if (abilityScoreName != AttributeDefinitions.Dexterity ||
            !defender.HasConditionOfTypeOrSubType(ConditionRaging) ||
            defender.GetSubclassLevel(CharacterClassDefinitions.Barbarian, Name) < 6)
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

    private sealed class ModifyEffectDescriptionPrimalInstinct : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == PowerPrimalInstinct;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (!Main.Settings.EnableBarbarianPersistentRage2024)
            {
                return effectDescription;
            }

            var classLevel = character.GetClassLevel(CharacterClassDefinitions.Barbarian);

            // persistent rage is only granted at 15
            if (classLevel < 15)
            {
                return effectDescription;
            }

            effectDescription.EffectForms[0].ConditionForm.ConditionDefinition =
                ConditionDefinitions.ConditionRagingPersistent;

            return effectDescription;
        }
    }

    private sealed class PhysicalAttackFinishedByMeUnbridledFerocity(ConditionDefinition conditionUnbridledFerocity)
        : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (rollOutcome == RollOutcome.CriticalSuccess)
            {
                rulesetAttacker.RemoveAllConditionsOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionUnbridledFerocity.Name);
            }
            else if (rollOutcome == RollOutcome.Success && rulesetAttacker.HasConditionOfTypeOrSubType(ConditionRaging))
            {
                rulesetAttacker.InflictCondition(
                    conditionUnbridledFerocity.Name,
                    DurationType.Permanent,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    conditionUnbridledFerocity.Name,
                    0,
                    0,
                    0);
            }
        }
    }

    private sealed class ActionFinishedByMeWrathAndFury : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not CharacterActionRecklessAttack)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetCharacter.HasConditionOfCategoryAndType(AttributeDefinitions.TagCombat, ConditionReckless))
            {
                yield break;
            }

            var classLevel = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Barbarian);
            var temporaryHitPoints = (classLevel + 1) / 2;

            rulesetCharacter.ReceiveTemporaryHitPoints(
                temporaryHitPoints, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);
        }
    }
}
