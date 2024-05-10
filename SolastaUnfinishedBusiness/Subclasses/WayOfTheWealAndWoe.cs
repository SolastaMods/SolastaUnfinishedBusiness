using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfTheWealAndWoe : AbstractSubclass
{
    private const string Name = "WayOfWealAndWoe";

    public WayOfTheWealAndWoe()
    {
        var attributeModifierWeal = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}Weal")
            .SetGuiPresentationNoContent(true)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.CriticalThreshold, -1)
            .AddToDB();

        var conditionWeal = ConditionDefinitionBuilder
            .Create($"Condition{Name}Weal")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AllowMultipleInstances()
            .SetFeatures(attributeModifierWeal)
            .AddToDB();

        var featureWeal = FeatureDefinitionBuilder
            .Create($"Feature{Name}Weal")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var featureWoe = FeatureDefinitionBuilder
            .Create($"Feature{Name}Woe")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var featureSelfPropelledWeal = FeatureDefinitionBuilder
            .Create($"Feature{Name}SelfPropelledWeal")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var featureBrutalWeal = FeatureDefinitionBuilder
            .Create($"Feature{Name}BrutalWeal")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var featureTheirWoe = FeatureDefinitionBuilder
            .Create($"Feature{Name}TheirWoe")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureWeal.AddCustomSubFeatures(new CustomBehaviorWealAndWoe(
            conditionWeal,
            featureWeal,
            featureWoe,
            featureSelfPropelledWeal,
            featureBrutalWeal,
            featureTheirWoe));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.OathOfDemonHunter, 256))
            .AddFeaturesAtLevel(3, featureWeal, featureWoe)
            .AddFeaturesAtLevel(6, featureSelfPropelledWeal)
            .AddFeaturesAtLevel(11, featureBrutalWeal)
            .AddFeaturesAtLevel(17, featureTheirWoe)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Monk;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class CustomBehaviorWealAndWoe : IPhysicalAttackFinishedByMe, IModifyDiceRoll
    {
        private readonly ConditionDefinition _conditionWeal;
        private readonly FeatureDefinition _featureBrutalWeal;
        private readonly FeatureDefinition _featureSelfPropelledWeal;
        private readonly FeatureDefinition _featureTheirWoe;
        private readonly FeatureDefinition _featureWeal;
        private readonly FeatureDefinition _featureWoe;

        internal CustomBehaviorWealAndWoe(
            ConditionDefinition conditionWeal,
            FeatureDefinition featureWeal,
            FeatureDefinition featureWoe,
            FeatureDefinition featureSelfPropelledWeal,
            FeatureDefinition featureBrutalWeal,
            FeatureDefinition featureTheirWoe)
        {
            _conditionWeal = conditionWeal;
            _featureWeal = featureWeal;
            _featureWoe = featureWoe;
            _featureSelfPropelledWeal = featureSelfPropelledWeal;
            _featureBrutalWeal = featureBrutalWeal;
            _featureTheirWoe = featureTheirWoe;
        }

        public void BeforeRoll(
            RollContext rollContext,
            RulesetCharacter rulesetCharacter,
            ref DieType dieType,
            ref AdvantageType advantageType)
        {
            // Empty
        }

        public void AfterRoll(
            DieType dieType,
            AdvantageType advantageType,
            RollContext rollContext,
            RulesetCharacter rulesetCharacter,
            ref int firstRoll,
            ref int secondRoll,
            ref int result)
        {
            if (rollContext != RollContext.AttackRoll)
            {
                return;
            }

            var conditionWealCount =
                rulesetCharacter.AllConditions.Count(x => x.ConditionDefinition == _conditionWeal);

            if (result == 1 || result - conditionWealCount > 1)
            {
                return;
            }

            rulesetCharacter.LogCharacterUsedFeature(_featureWeal, "Feedback/&WoeReroll", false,
                (ConsoleStyleDuplet.ParameterType.Player, rulesetCharacter.Name),
                (ConsoleStyleDuplet.ParameterType.SuccessfulRoll, result.ToString()),
                (ConsoleStyleDuplet.ParameterType.FailedRoll, 1.ToString()));

            firstRoll = 1;
            secondRoll = 1;
            result = 1;
        }

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
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!ValidatorsWeapon.IsUnarmed(attackMode) &&
                !rulesetAttacker.IsMonkWeapon(attackMode?.SourceDefinition as ItemDefinition))
            {
                yield break;
            }

            var level = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Monk);

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (rollOutcome)
            {
                case RollOutcome.Failure:
                case RollOutcome.Neutral:
                case RollOutcome.Success:
                    // Weal
                    rulesetAttacker.InflictCondition(
                        _conditionWeal.Name,
                        DurationType.UntilAnyRest,
                        0,
                        TurnOccurenceType.EndOfTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        _conditionWeal.Name,
                        0,
                        0,
                        0);
                    break;

                case RollOutcome.CriticalFailure:
                    // Their Woe / Woe
                    if (level >= 17)
                    {
                        if (rulesetDefender is { IsDeadOrDyingOrUnconscious: false })
                        {
                            rulesetAttacker.LogCharacterUsedFeature(_featureTheirWoe);
                            InflictMartialArtDieDamage(attacker, defender, attackMode, rollOutcome);
                        }
                    }
                    else
                    {
                        rulesetAttacker.LogCharacterUsedFeature(_featureWoe);
                        InflictMartialArtDieDamage(attacker, attacker, attackMode, rollOutcome);
                    }

                    // Weal (RESET)
                    rulesetAttacker.RemoveAllConditionsOfCategoryAndType(
                        AttributeDefinitions.TagEffect, _conditionWeal.Name);
                    break;

                case RollOutcome.CriticalSuccess:
                    // Self Propelled Weal
                    if (level >= 6 && rulesetAttacker.UsedKiPoints > 0)
                    {
                        rulesetAttacker.LogCharacterUsedFeature(_featureSelfPropelledWeal);
                        rulesetAttacker.ForceKiPointConsumption(-1);
                        rulesetAttacker.KiPointsAltered?.Invoke(rulesetAttacker, rulesetAttacker.RemainingKiPoints);
                    }

                    // Brutal Weal
                    if (level >= 11 && rulesetDefender is { IsDeadOrDyingOrUnconscious: false })
                    {
                        rulesetAttacker.LogCharacterUsedFeature(_featureBrutalWeal);
                        InflictMartialArtDieDamage(attacker, defender, attackMode, rollOutcome);
                    }

                    // Weal (RESET)
                    rulesetAttacker.RemoveAllConditionsOfCategoryAndType(
                        AttributeDefinitions.TagEffect, _conditionWeal.Name);
                    break;
            }
        }

        private static void InflictMartialArtDieDamage(
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome outcome)
        {
            var originalDamageForm = attackMode.EffectDescription.FindFirstDamageForm();

            if (originalDamageForm == null)
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;
            var criticalSuccess = outcome == RollOutcome.CriticalSuccess;
            var monkLevel = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Monk);
            var dieType = FeatureDefinitionAttackModifiers.AttackModifierMonkMartialArtsImprovedDamage
                .DieTypeByRankTable
                .Find(x => x.Rank == monkLevel).DieType;
            var rolls = new List<int>();
            var damageForm = new DamageForm
            {
                DamageType = originalDamageForm.DamageType, DieType = dieType, DiceNumber = 1, BonusDamage = 0
            };
            var damageRoll =
                rulesetAttacker.RollDamage(damageForm, 0, criticalSuccess, 0, 0, 1, false, false, false, rolls);
            var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
            {
                sourceCharacter = rulesetAttacker,
                targetCharacter = rulesetDefender,
                position = defender.LocationPosition
            };

            RulesetActor.InflictDamage(
                damageRoll,
                damageForm,
                damageForm.DamageType,
                applyFormsParams,
                rulesetDefender,
                false,
                rulesetAttacker.Guid,
                false,
                attackMode.AttackTags,
                new RollInfo(damageForm.DieType, rolls, 0),
                false,
                out _);
        }
    }
}
