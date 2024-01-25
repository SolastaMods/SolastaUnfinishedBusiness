using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.BehaviorsGeneric;
using SolastaUnfinishedBusiness.BehaviorsSpecific;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfTheDistantHand : AbstractSubclass
{
    internal const string Name = "WayOfTheDistantHand";
    internal const int StunningStrikeWithBowAllowedLevel = 11;

    public WayOfTheDistantHand()
    {
        var zenArrow = Sprites.GetSprite("ZenArrow", Resources.ZenArrow, 128, 64);

        //
        // LEVEL 03
        //

        var featureWayOfTheDistantHandCombat =
            FeatureDefinitionBuilder
                .Create($"Feature{Name}Combat")
                .SetGuiPresentationNoContent(true)
                .AddCustomSubFeatures(
                    new CustomLevelUpLogicWayOfTheDistantHandCombat(),
                    new RemoveRangedAttackInMeleeDisadvantage(ValidatorsCharacter.HasBowWithoutArmor))
                .AddToDB();

        // ZEN ARROW

        var powerWayOfTheDistantHandZenArrowTechnique = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ZenArrowTechnique")
            .SetGuiPresentation(Category.Feature, zenArrow)
            .SetUsesFixed(ActivationTime.OnAttackHit, RechargeRate.KiPoints)
            .AddCustomSubFeatures(
                IsModifyPowerPool.Marker,
                new RestrictReactionAttackMode((_, attacker, _, _, _) =>
                    attacker.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                    ValidatorsCharacter.HasBowWithoutArmor(attacker.RulesetCharacter) &&
                    attacker.RulesetCharacter.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.MonkKiPointsToggle)))
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .Build())
            .AddToDB();

        var powerWayOfTheDistantHandZenArrowProne = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ZenArrowProne")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetSavingThrowData(
                        true,
                        AttributeDefinitions.Dexterity,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerWayOfTheDistantHandZenArrowPush = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ZenArrowPush")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetSavingThrowData(
                        true,
                        AttributeDefinitions.Strength,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerWayOfTheDistantHandZenArrowDistract = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ZenArrowDistract")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetSavingThrowData(
                        true,
                        AttributeDefinitions.Wisdom,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create($"Condition{Name}Distract")
                                    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled)
                                    .SetConditionType(ConditionType.Detrimental)
                                    .SetSpecialDuration(DurationType.Round, 1)
                                    .SetSpecialInterruptions(ConditionInterruption.Attacks)
                                    .SetFeatures(
                                        FeatureDefinitionCombatAffinityBuilder
                                            .Create($"CombatAffinity{Name}Distract")
                                            .SetGuiPresentation($"Power{Name}ZenArrowDistract",
                                                Category.Feature, Gui.NoLocalization)
                                            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                                            .AddToDB())
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(
            powerWayOfTheDistantHandZenArrowTechnique,
            true,
            powerWayOfTheDistantHandZenArrowProne,
            powerWayOfTheDistantHandZenArrowPush,
            powerWayOfTheDistantHandZenArrowDistract);

        //
        // LEVEL 06
        //

        var powerWayOfTheDistantHandZenArcherFlurryOfArrows = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ZenArcherFlurryOfArrows")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints, 2, 2)
            .AddCustomSubFeatures(
                ModifyPowerVisibility.Hidden,
                new UpgradeFlurry(
                    ConditionDefinitionBuilder
                        .Create($"Condition{Name}AttackedWithMonkWeapon")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                        .SetSpecialInterruptions(
                            ConditionInterruption.BattleEnd,
                            ConditionInterruption.AnyBattleTurnEnd)
                        .AddCustomSubFeatures(new AddExtraFlurryOfArrowsAttack())
                        .AddToDB()))
            .SetShowCasting(false)
            .AddToDB();

        var wayOfDistantHandsKiPoweredArrows = FeatureDefinitionBuilder
            .Create($"Feature{Name}KiPoweredArrows")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new AddTagToWeaponWeaponAttack(
                TagsDefinitions.MagicalWeapon, ValidatorsWeapon.AlwaysValid, ValidatorsCharacter.HasBowWithoutArmor))
            .AddToDB();

        //
        // LEVEL 11
        //

        var wayOfDistantHandsZenArcherStunningArrows = FeatureDefinitionBuilder
            .Create($"Feature{Name}ZenArcherStunningArrows")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // UPGRADE ZEN ARROW

        var powerWayOfTheDistantHandZenArrowUpgradedTechnique = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ZenArrowUpgradedTechnique")
            .SetGuiPresentation(Category.Feature, zenArrow)
            .SetUsesFixed(ActivationTime.OnAttackHit, RechargeRate.KiPoints)
            .SetOverriddenPower(powerWayOfTheDistantHandZenArrowTechnique)
            .AddCustomSubFeatures(
                IsModifyPowerPool.Marker,
                new RestrictReactionAttackMode((_, attacker, _, _, _) =>
                    attacker.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                    ValidatorsCharacter.HasBowWithoutArmor(attacker.RulesetCharacter) &&
                    attacker.RulesetCharacter.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.MonkKiPointsToggle)))
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .Build())
            .AddToDB();

        var powerWayOfTheDistantHandZenArrowUpgradedProne = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ZenArrowUpgradedProne")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetSavingThrowData(
                        true,
                        AttributeDefinitions.Dexterity,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create($"Condition{Name}ZenArrowUpgradedSlow")
                                    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionEncumbered)
                                    .SetConditionType(ConditionType.Detrimental)
                                    .SetSpecialDuration(DurationType.Round, 1)
                                    .SetFeatures(
                                        FeatureDefinitionMovementAffinityBuilder
                                            .Create($"MovementAffinity{Name}UpgradedSlow")
                                            .SetGuiPresentation($"Condition{Name}ZenArrowUpgradedSlow",
                                                Category.Condition, Gui.NoLocalization)
                                            .SetBaseSpeedMultiplicativeModifier(0)
                                            .AddToDB())
                                    .AddToDB(), ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerWayOfTheDistantHandUpgradedPush = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}UpgradedPush")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetSavingThrowData(
                        true,
                        AttributeDefinitions.Strength,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 4)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerWayOfTheDistantHandUpgradedDistract = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}UpgradedDistract")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetSavingThrowData(
                        true,
                        AttributeDefinitions.Wisdom,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create($"Condition{Name}UpgradedDistract")
                                    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled)
                                    .SetConditionType(ConditionType.Detrimental)
                                    .SetSpecialDuration(DurationType.Round, 1)
                                    .SetFeatures(
                                        FeatureDefinitionCombatAffinityBuilder
                                            .Create($"CombatAffinity{Name}UpgradedDistract")
                                            .SetGuiPresentation($"Power{Name}UpgradedDistract",
                                                Category.Feature, Gui.NoLocalization)
                                            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                                            .AddToDB())
                                    .AddToDB(), ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(
            powerWayOfTheDistantHandZenArrowUpgradedTechnique,
            true,
            powerWayOfTheDistantHandZenArrowUpgradedProne,
            powerWayOfTheDistantHandUpgradedPush,
            powerWayOfTheDistantHandUpgradedDistract);

        //
        // LEVEL 17
        //

        var attackModifierWayOfTheDistantHandUnseenEyes = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{Name}UnseenEyes")
            .SetGuiPresentation(Category.Feature)
            .SetDamageRollModifier(0, AttackModifierMethod.AddProficiencyBonus, AttributeDefinitions.Wisdom)
            .AddCustomSubFeatures(
                ValidatorsRestrictedContext.IsZenArrowAttack,
                new CustomLevelUpLogicUnseenEyes())
            .AddToDB();

        //
        // PROGRESSION
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheDistantHand, 256))
            .AddFeaturesAtLevel(3,
                GameUiContext.ActionAffinityMonkKiPointsToggle,
                featureWayOfTheDistantHandCombat,
                powerWayOfTheDistantHandZenArrowTechnique,
                powerWayOfTheDistantHandZenArrowDistract,
                powerWayOfTheDistantHandZenArrowProne,
                powerWayOfTheDistantHandZenArrowPush)
            .AddFeaturesAtLevel(6,
                wayOfDistantHandsKiPoweredArrows,
                powerWayOfTheDistantHandZenArcherFlurryOfArrows)
            .AddFeaturesAtLevel(11,
                wayOfDistantHandsZenArcherStunningArrows,
                powerWayOfTheDistantHandZenArrowUpgradedTechnique,
                powerWayOfTheDistantHandZenArrowUpgradedProne,
                powerWayOfTheDistantHandUpgradedPush,
                powerWayOfTheDistantHandUpgradedDistract)
            .AddFeaturesAtLevel(17,
                attackModifierWayOfTheDistantHandUnseenEyes)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Monk;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class CustomLevelUpLogicWayOfTheDistantHandCombat : ICustomLevelUpLogic
    {
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            hero.TrainedInvocations.TryAdd(GetDefinition<InvocationDefinition>(
                "CustomInvocationMonkWeaponSpecializationShortbowType"));
            hero.TrainedInvocations.TryAdd(GetDefinition<InvocationDefinition>(
                "CustomInvocationMonkWeaponSpecializationLongbowType"));
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // empty
        }
    }

    private sealed class CustomLevelUpLogicUnseenEyes : ICustomLevelUpLogic
    {
        public void ApplyFeature([NotNull] RulesetCharacterHero hero, string tag)
        {
            ModifyAttributeAndMax(hero, AttributeDefinitions.Wisdom, 2);

            hero.RefreshAll();
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            ModifyAttributeAndMax(hero, AttributeDefinitions.Wisdom, -2);
        }

        private static void ModifyAttributeAndMax([NotNull] RulesetActor hero, string attributeName, int amount)
        {
            var attribute = hero.GetAttribute(attributeName);

            attribute.BaseValue += amount;
            attribute.MaxValue += amount;
            attribute.MaxEditableValue += amount;
            attribute.Refresh();

            hero.AbilityScoreIncreased?.Invoke(hero, attributeName, amount, amount);
        }
    }

    private sealed class UpgradeFlurry(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition condition) : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not CharacterActionFlurryOfBlows)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                condition.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                condition.Name,
                0,
                0,
                0);
        }
    }
}
