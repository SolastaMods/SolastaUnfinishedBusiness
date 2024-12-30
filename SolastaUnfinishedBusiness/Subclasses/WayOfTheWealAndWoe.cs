using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
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

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfTheWealAndWoe // : AbstractSubclass
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

        // kept name for backward compatibility
        var featureWoe = FeatureDefinitionPowerBuilder
            .Create($"Feature{Name}Woe")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.Position)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeBludgeoning, 1))
                    .Build())
            .AddToDB();

        featureWoe.AddCustomSubFeatures(
            new ModifyEffectDescription(featureWoe),
            ModifyPowerVisibility.Hidden);

        var featureSelfPropelledWeal = FeatureDefinitionBuilder
            .Create($"Feature{Name}SelfPropelledWeal")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // kept name for backward compatibility
        var featureBrutalWeal = FeatureDefinitionPowerBuilder
            .Create($"Feature{Name}BrutalWeal")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.Position)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeBludgeoning, 1))
                    .Build())
            .AddToDB();

        featureBrutalWeal.AddCustomSubFeatures(
            new ModifyEffectDescription(featureBrutalWeal),
            ModifyPowerVisibility.Hidden);

        // kept name for backward compatibility
        var featureTheirWoe = FeatureDefinitionPowerBuilder
            .Create($"Feature{Name}TheirWoe")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.Position)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeBludgeoning, 1))
                    .Build())
            .AddToDB();

        featureTheirWoe.AddCustomSubFeatures(
            new ModifyEffectDescription(featureTheirWoe),
            ModifyPowerVisibility.Hidden);

        featureWeal.AddCustomSubFeatures(new CustomBehaviorWealAndWoe(
            conditionWeal,
            featureWeal,
            featureSelfPropelledWeal,
            featureBrutalWeal,
            featureWoe,
            featureTheirWoe));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheWealAndWoe, 256))
            .AddFeaturesAtLevel(3, featureWeal, featureWoe)
            .AddFeaturesAtLevel(6, featureSelfPropelledWeal)
            .AddFeaturesAtLevel(11, featureBrutalWeal)
            .AddFeaturesAtLevel(17, featureTheirWoe)
            .AddToDB();
    }

    // internal CharacterClassDefinition Klass => CharacterClassDefinitions.Monk;

    internal CharacterSubclassDefinition Subclass { get; }

    // internal FeatureDefinitionSubclassChoice SubclassChoice =>
    //     FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    // internal DeityDefinition DeityDefinition { get; }

    private sealed class CustomBehaviorWealAndWoe(
        ConditionDefinition conditionWeal,
        FeatureDefinition featureWeal,
        FeatureDefinition featureSelfPropelledWeal,
        FeatureDefinitionPower featureBrutalWeal,
        FeatureDefinitionPower featureWoe,
        FeatureDefinitionPower featureTheirWoe) : IPhysicalAttackFinishedByMe, IModifyDiceRoll
    {
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
                rulesetCharacter.ConditionsByCategory
                    .SelectMany(x => x.Value)
                    .Count(x => x.ConditionDefinition == conditionWeal);

            if (result == 1 || result - conditionWealCount > 1)
            {
                return;
            }

            rulesetCharacter.LogCharacterUsedFeature(featureWeal, "Feedback/&WoeReroll", false,
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
            var rulesetDefender = defender.RulesetActor;

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
                        conditionWeal.Name,
                        DurationType.UntilAnyRest,
                        0,
                        TurnOccurenceType.EndOfTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        conditionWeal.Name,
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
                            InflictMartialArtDieDamage(featureTheirWoe, attacker, defender);
                        }
                    }
                    else
                    {
                        InflictMartialArtDieDamage(featureWoe, attacker, attacker);
                    }

                    // Weal (RESET)
                    rulesetAttacker.RemoveAllConditionsOfCategoryAndType(
                        AttributeDefinitions.TagEffect, conditionWeal.Name);
                    break;

                case RollOutcome.CriticalSuccess:
                    // Self Propelled Weal
                    if (level >= 6 && rulesetAttacker.UsedKiPoints > 0)
                    {
                        rulesetAttacker.LogCharacterUsedFeature(featureSelfPropelledWeal);
                        rulesetAttacker.ForceKiPointConsumption(-1);
                        rulesetAttacker.KiPointsAltered?.Invoke(rulesetAttacker, rulesetAttacker.RemainingKiPoints);
                    }

                    // Brutal Weal
                    if (level >= 11 && rulesetDefender is { IsDeadOrDyingOrUnconscious: false })
                    {
                        InflictMartialArtDieDamage(featureBrutalWeal, attacker, defender);
                    }

                    // Weal (RESET)
                    rulesetAttacker.RemoveAllConditionsOfCategoryAndType(
                        AttributeDefinitions.TagEffect, conditionWeal.Name);
                    break;
            }
        }

        private static void InflictMartialArtDieDamage(
            FeatureDefinitionPower power,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(power, rulesetAttacker);

            // woe, their woe, and brutal weal are use at will power
            attacker.MyExecuteActionSpendPower(usablePower, defender);
        }
    }

    private sealed class ModifyEffectDescription(FeatureDefinitionPower power) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == power;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var dieType = character.GetMonkDieType();

            effectDescription.FindFirstDamageForm().DieType = dieType;

            return effectDescription;
        }
    }
}
