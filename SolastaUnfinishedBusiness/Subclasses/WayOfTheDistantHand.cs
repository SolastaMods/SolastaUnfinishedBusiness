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
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfTheDistantHand : AbstractSubclass
{
    private const string ZenArrowTag = "ZenArrow";

    public WayOfTheDistantHand()
    {
        var zenArrow = Sprites.GetSprite("ZenArrow", Resources.ZenArrow, 128, 64);

        //
        // LEVEL 03
        //

        var featureWayOfTheDistantHandCombat =
            FeatureDefinitionBuilder
                .Create("FeatureWayOfTheDistantHandCombat")
                .SetGuiPresentationNoContent(true)
                .AddCustomSubFeatures(
                    new CustomLevelUpLogicWayOfTheDistantHandCombat(),
                    new RangedAttackInMeleeDisadvantageRemover(
                        ValidatorsWeapon.IsZenArrowAttack,
                        ValidatorsCharacter.HasNoArmor, ValidatorsCharacter.HasNoShield),
                    new AddTagToWeaponWeaponAttack(ZenArrowTag, ValidatorsWeapon.AlwaysValid))
                .AddToDB();

        // ZEN ARROW

        var powerWayOfTheDistantHandZenArrowTechnique = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowTechnique")
            .SetGuiPresentation(Category.Feature, zenArrow)
            .SetUsesFixed(ActivationTime.OnAttackHit, RechargeRate.KiPoints)
            .AddCustomSubFeatures(
                IsPowerPool.Marker,
                new RestrictReactionAttackMode((_, attacker, _, attackMode, _) =>
                    attackMode != null &&
                    attackMode.AttackTags.Contains(ZenArrowTag) &&
                    attacker.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                    attacker.RulesetCharacter.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.MonkKiPointsToggle)))
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .Build())
            .AddToDB();

        var powerWayOfTheDistantHandZenArrowProne = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowProne")
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
            .AddCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        var powerWayOfTheDistantHandZenArrowPush = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowPush")
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
            .AddCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        var powerWayOfTheDistantHandZenArrowDistract = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowDistract")
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
                                    .Create("ConditionWayOfTheDistantHandDistract")
                                    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled)
                                    .SetConditionType(ConditionType.Detrimental)
                                    .SetSpecialDuration(DurationType.Round, 1)
                                    .SetSpecialInterruptions(ConditionInterruption.Attacks)
                                    .SetFeatures(
                                        FeatureDefinitionCombatAffinityBuilder
                                            .Create("CombatAffinityWayOfTheDistantHandDistract")
                                            .SetGuiPresentation("PowerWayOfTheDistantHandZenArrowDistract",
                                                Category.Feature, Gui.NoLocalization)
                                            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                                            .AddToDB())
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(PowerVisibilityModifier.Hidden)
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
            .Create("PowerWayOfTheDistantHandZenArcherFlurryOfArrows")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints, 2, 2)
            .AddCustomSubFeatures(
                PowerVisibilityModifier.Hidden,
                new UpgradeFlurry(
                    ConditionDefinitionBuilder
                        .Create("ConditionWayOfTheDistantHandAttackedWithMonkWeapon")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                        .SetSpecialInterruptions(
                            ConditionInterruption.BattleEnd,
                            ConditionInterruption.AnyBattleTurnEnd)
                        .AddCustomSubFeatures(AddFlurryOfArrowsAttacks.Mark)
                        .AddToDB()))
            .SetShowCasting(false)
            .AddToDB();

        var wayOfDistantHandsKiPoweredArrows = FeatureDefinitionBuilder
            .Create("FeatureWayOfTheDistantHandKiPoweredArrows")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, ValidatorsWeapon.IsZenArrowAttack))
            .AddToDB();

        //
        // LEVEL 11
        //

        var wayOfDistantHandsZenArcherStunningArrows = FeatureDefinitionBuilder
            .Create("FeatureWayOfTheDistantHandZenArcherStunningArrows")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // UPGRADE ZEN ARROW

        var powerWayOfTheDistantHandZenArrowUpgradedTechnique = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowUpgradedTechnique")
            .SetGuiPresentation(Category.Feature, zenArrow)
            .SetUsesFixed(ActivationTime.OnAttackHit, RechargeRate.KiPoints)
            .SetOverriddenPower(powerWayOfTheDistantHandZenArrowTechnique)
            .AddCustomSubFeatures(
                IsPowerPool.Marker,
                new RestrictReactionAttackMode((_, attacker, _, attackMode, _) =>
                    attackMode != null &&
                    attackMode.AttackTags.Contains(ZenArrowTag) &&
                    attacker.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                    attacker.RulesetCharacter.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.MonkKiPointsToggle)))
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .Build())
            .AddToDB();

        var powerWayOfTheDistantHandZenArrowUpgradedProne = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowUpgradedProne")
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
                                    .Create("ConditionWayOfTheDistantHandZenArrowUpgradedSlow")
                                    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionEncumbered)
                                    .SetConditionType(ConditionType.Detrimental)
                                    .SetSpecialDuration(DurationType.Round, 1)
                                    .SetFeatures(
                                        FeatureDefinitionMovementAffinityBuilder
                                            .Create("MovementAffinityWayOfTheDistantHandUpgradedSlow")
                                            .SetGuiPresentation("ConditionWayOfTheDistantHandZenArrowUpgradedSlow",
                                                Category.Condition, Gui.NoLocalization)
                                            .SetBaseSpeedMultiplicativeModifier(0)
                                            .AddToDB())
                                    .AddToDB(), ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        var powerWayOfTheDistantHandUpgradedPush = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandUpgradedPush")
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
            .AddCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        var powerWayOfTheDistantHandUpgradedDistract = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandUpgradedDistract")
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
                                    .Create("ConditionWayOfTheDistantHandUpgradedDistract")
                                    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled)
                                    .SetConditionType(ConditionType.Detrimental)
                                    .SetSpecialDuration(DurationType.Round, 1)
                                    .SetFeatures(
                                        FeatureDefinitionCombatAffinityBuilder
                                            .Create("CombatAffinityWayOfTheDistantHandUpgradedDistract")
                                            .SetGuiPresentation("PowerWayOfTheDistantHandUpgradedDistract",
                                                Category.Feature, Gui.NoLocalization)
                                            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                                            .AddToDB())
                                    .AddToDB(), ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(PowerVisibilityModifier.Hidden)
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
            .Create("AttackModifierWayOfTheDistantHandUnseenEyes")
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
            .Create("WayOfTheDistantHand")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("WayOfTheDistantHand", Resources.WayOfTheDistantHand, 256))
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
            var alreadyThere = !hero.TrainedInvocations.TryAdd(GetDefinition<InvocationDefinition>(
                "CustomInvocationMonkWeaponSpecializationShortbowType"));

            // don't invert the order because of short circuit evaluation
            alreadyThere = !hero.TrainedInvocations.TryAdd(GetDefinition<InvocationDefinition>(
                               "CustomInvocationMonkWeaponSpecializationLongbowType"))
                           || alreadyThere;
            alreadyThere = !hero.TrainedInvocations.TryAdd(GetDefinition<InvocationDefinition>(
                               "CustomInvocationMonkWeaponSpecializationCEHandXbowType"))
                           || alreadyThere;

            // grant a rapier if by any chance one of above weapons were already specialized in
            if (alreadyThere)
            {
                hero.TrainedInvocations.TryAdd(
                    GetDefinition<InvocationDefinition>("CustomInvocationMonkWeaponSpecializationRapierType"));
            }
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

    private sealed class UpgradeFlurry : IActionFinishedByMe
    {
        private readonly ConditionDefinition _condition;

        public UpgradeFlurry(ConditionDefinition condition)
        {
            _condition = condition;
        }

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not CharacterActionFlurryOfBlows)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                _condition.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                _condition.Name,
                0,
                0,
                0);
        }
    }

    private sealed class AddFlurryOfArrowsAttacks : AddExtraAttackBase
    {
        private AddFlurryOfArrowsAttacks() :
            base(ActionDefinitions.ActionType.Bonus, ValidatorsCharacter.HasNoArmor, ValidatorsCharacter.HasNoShield)
        {
        }

        public static AddFlurryOfArrowsAttacks Mark { get; } = new();

        protected override AttackModeOrder GetOrder(RulesetCharacter character)
        {
            return AttackModeOrder.Start;
        }

        protected override List<RulesetAttackMode> GetAttackModes(RulesetCharacter character)
        {
            if (character is not RulesetCharacterHero hero)
            {
                return null;
            }

            var mainHandItem = hero.GetMainWeapon();

            // don't use ?? on Unity Objects as it bypasses the lifetime check on the underlying object
            var strikeDefinition = mainHandItem?.ItemDefinition;

#pragma warning disable IDE0270
            if (strikeDefinition == null)
#pragma warning restore IDE0270
            {
                strikeDefinition = hero.UnarmedStrikeDefinition;
            }

            if (!hero.IsMonkWeapon(strikeDefinition))
            {
                return null;
            }

            var attackModifiers = hero.attackModifiers;
            var attackMode = hero.RefreshAttackMode(
                ActionType,
                strikeDefinition,
                strikeDefinition.WeaponDescription,
                ValidatorsCharacter.IsFreeOffhandVanilla(hero),
                true,
                EquipmentDefinitions.SlotTypeMainHand,
                attackModifiers,
                hero.FeaturesOrigin,
                mainHandItem
            );

            attackMode.attacksNumber = 2;

            return new List<RulesetAttackMode> { attackMode };
        }
    }
}
