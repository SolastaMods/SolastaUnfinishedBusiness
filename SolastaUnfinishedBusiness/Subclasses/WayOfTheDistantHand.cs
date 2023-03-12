using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WayOfTheDistantHand : AbstractSubclass
{
    private const string ZenArrowTag = "ZenArrow";

    // Zen Archer's Monk weapons are bows and darts ranged weapons.
    private static readonly List<WeaponTypeDefinition> ZenArcherWeapons = new()
    {
        WeaponTypeDefinitions.ShortbowType, WeaponTypeDefinitions.LongbowType
    };

    internal WayOfTheDistantHand()
    {
        var zenArrow = Sprites.GetSprite("ZenArrow", Resources.ZenArrow, 128, 64);

        //
        // LEVEL 03
        //

        var proficiencyWayOfTheDistantHandCombat = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyWayOfTheDistantHandCombat")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                ProficiencyType.Weapon,
                WeaponTypeDefinitions.LongbowType.Name,
                WeaponTypeDefinitions.ShortbowType.Name)
            .SetCustomSubFeatures(
                new ZenArcherMarker(),
                new RangedAttackInMeleeDisadvantageRemover(
                    IsMonkWeapon, ValidatorsCharacter.HasNoArmor, ValidatorsCharacter.HasNoShield),
                new AddTagToWeaponAttack(ZenArrowTag, IsZenArrowAttack))
            .AddToDB();

        // ZEN ARROW

        var powerWayOfTheDistantHandZenArrowTechnique = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowTechnique")
            .SetGuiPresentation(Category.Feature, zenArrow)
            .SetUsesFixed(ActivationTime.OnAttackHit, RechargeRate.KiPoints)
            .SetCustomSubFeatures(
                new RestrictReactionAttackMode((mode, _, _) =>
                    mode != null && mode.AttackTags.Contains(ZenArrowTag)))
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
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
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
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
            .AddToDB();

        var powerWayOfTheDistantHandZenArrowPush = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowPush")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
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
            .AddToDB();

        var powerWayOfTheDistantHandZenArrowDistract = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowDistract")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
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
                            .SetConditionForm(ConditionDefinitionBuilder
                                    .Create("ConditionWayOfTheDistantHandDistract")
                                    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled)
                                    .SetConditionType(ConditionType.Detrimental)
                                    .SetSpecialDuration(DurationType.Round, 1)
                                    .SetSpecialInterruptions(ConditionInterruption.Attacks)
                                    .SetFeatures(FeatureDefinitionCombatAffinityBuilder
                                        .Create("CombatAffinityWayOfTheDistantHandDistract")
                                        .SetGuiPresentationNoContent(true)
                                        .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                                        .AddToDB())
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
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

        //ideally this should be simple feature, but leaving as Power for compatibility
        var powerWayOfTheDistantHandZenArcherFlurryOfArrows = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArcherFlurryOfArrows")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints, 2)
            .SetCustomSubFeatures(
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
                        .SetFeatures(
                            FeatureDefinitionBuilder
                                .Create("FeatureWayOfTheDistantHandFlurry")
                                .SetGuiPresentationNoContent(true)
                                .SetCustomSubFeatures(AddFlurryOfArrowsAttacks.Mark)
                                .AddToDB())
                        .AddToDB()))
            .SetShowCasting(false)
            .AddToDB();

        var wayOfDistantHandsKiPoweredArrows = FeatureDefinitionBuilder
            .Create("FeatureWayOfTheDistantHandKiPoweredArrows")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new AddTagToWeaponAttack(TagsDefinitions.Magical, (mode, _, character) =>
                    IsZenArcherWeapon(character, mode.SourceDefinition as ItemDefinition)))
            .AddToDB();

        //
        // LEVEL 11
        //

        var wayOfDistantHandsZenArcherStunningArrows = FeatureDefinitionBuilder
            .Create("FeatureWayOfTheDistantHandZenArcherStunningArrows")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ZenArcherStunningArrows())
            .AddToDB();

        // UPGRADE ZEN ARROW

        var powerWayOfTheDistantHandZenArrowUpgradedTechnique = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowUpgradedTechnique")
            .SetGuiPresentation(Category.Feature, zenArrow)
            .SetUsesFixed(ActivationTime.OnAttackHit, RechargeRate.KiPoints)
            .SetOverriddenPower(powerWayOfTheDistantHandZenArrowTechnique)
            .SetCustomSubFeatures(
                new RestrictReactionAttackMode((mode, _, _) =>
                    mode != null && mode.AttackTags.Contains(ZenArrowTag)))
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
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
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
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
                            .SetConditionForm(ConditionDefinitionBuilder
                                .Create("ConditionWayOfTheDistantHandZenArrowUpgradedSlow")
                                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionEncumbered)
                                .SetConditionType(ConditionType.Detrimental)
                                .SetSpecialDuration(DurationType.Round, 1)
                                .SetFeatures(FeatureDefinitionMovementAffinityBuilder
                                    .Create("MovementAffinityWayOfTheDistantHandUpgradedSlow")
                                    .SetGuiPresentationNoContent(true)
                                    .SetBaseSpeedMultiplicativeModifier(0)
                                    .AddToDB())
                                .AddToDB(), ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var powerWayOfTheDistantHandUpgradedPush = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandUpgradedPush")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetSavingThrowData(
                        true,
                        AttributeDefinitions.Strength,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(EffectFormBuilder.Create()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 4)
                        .Build())
                    .Build())
            .AddToDB();

        var powerWayOfTheDistantHandUpgradedDistract = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandUpgradedDistract")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
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
                            .SetConditionForm(ConditionDefinitionBuilder
                                .Create("ConditionWayOfTheDistantHandUpgradedDistract")
                                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled)
                                .SetConditionType(ConditionType.Detrimental)
                                .SetSpecialDuration(DurationType.Round, 1)
                                .SetFeatures(FeatureDefinitionCombatAffinityBuilder
                                    .Create("CombatAffinityWayOfTheDistantHandUpgradedDistract")
                                    .SetGuiPresentationNoContent(true)
                                    .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                                    .AddToDB())
                                .AddToDB(), ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        PowerBundle.RegisterPowerBundle(
            powerWayOfTheDistantHandZenArrowUpgradedTechnique,
            true,
            powerWayOfTheDistantHandZenArrowUpgradedProne,
            powerWayOfTheDistantHandUpgradedPush,
            powerWayOfTheDistantHandUpgradedDistract);

        //
        // PROGRESSION
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WayOfTheDistantHand")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("WayOfTheDistantHand", Resources.WayOfTheDistantHand, 256))
            .AddFeaturesAtLevel(3,
                proficiencyWayOfTheDistantHandCombat,
                powerWayOfTheDistantHandZenArrowTechnique)
            .AddFeaturesAtLevel(6,
                wayOfDistantHandsKiPoweredArrows,
                powerWayOfTheDistantHandZenArcherFlurryOfArrows)
            .AddFeaturesAtLevel(11,
                wayOfDistantHandsZenArcherStunningArrows,
                powerWayOfTheDistantHandZenArrowUpgradedTechnique)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    // private class ExtendWeaponRange : IModifyAttackModeForWeapon
    // {
    //     internal void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon)
    //     {
    //         if (attackMode == null || attackMode.Magical || (!attackMode.Ranged && !attackMode.Thrown))
    //         {
    //             return;
    //         }
    //
    //         if (!Monk.IsMonkWeapon(character, attackMode))
    //         {
    //             return;
    //         }
    //
    //         attackMode.CloseRange = Math.Min(16, attackMode.CloseRange * 2);
    //         attackMode.MaxRange = Math.Min(32, attackMode.MaxRange * 2);
    //     }
    // }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    internal override DeityDefinition DeityDefinition => null;

    private static bool IsMonkWeapon(RulesetAttackMode attackMode, RulesetItem weapon, RulesetCharacter character)
    {
        return IsMonkWeapon(character, attackMode) || IsMonkWeapon(character, weapon);
    }

    private static bool IsMonkWeapon(RulesetActor character, RulesetAttackMode attackMode)
    {
        return attackMode is { SourceDefinition: ItemDefinition item } && IsMonkWeapon(character, item);
    }

    internal static bool IsMonkWeapon(RulesetActor actor, RulesetItem weapon)
    {
        return weapon == null || IsMonkWeapon(actor, weapon.ItemDefinition);
    }

    private static bool IsMonkWeapon(RulesetActor actor, ItemDefinition weapon)
    {
        return weapon != null && IsMonkWeapon(actor, weapon.WeaponDescription);
    }

    internal static bool IsMonkWeapon(RulesetActor actor, WeaponDescription weapon)
    {
        if (weapon == null)
        {
            return false;
        }

        return weapon.IsMonkWeaponOrUnarmed() || IsZenArcherWeapon(actor, weapon);
    }

    private static bool IsZenArcherWeapon(RulesetActor actor, ItemDefinition item)
    {
        return IsZenArcherWeapon(actor, item.WeaponDescription);
    }

    private static bool IsZenArcherWeapon(RulesetActor actor, WeaponDescription weapon)
    {
        if (actor == null || weapon == null)
        {
            return false;
        }

        var typeDefinition = weapon.WeaponTypeDefinition;

        if (typeDefinition == null)
        {
            return false;
        }

        return actor.HasSubFeatureOfType<ZenArcherMarker>() && ZenArcherWeapons.Contains(typeDefinition);
    }

    private static bool IsZenArrowAttack(RulesetAttackMode mode, RulesetItem weapon, RulesetCharacter character)
    {
        return mode != null
               && (mode.Ranged || mode.Thrown)
               && IsZenArcherWeapon(character, mode.SourceDefinition as ItemDefinition);
    }

    private static bool WieldsZenArcherWeapon(RulesetCharacter character)
    {
        var mainHandItem = character.GetMainWeapon();

        return IsZenArcherWeapon(character, mainHandItem?.ItemDefinition);
    }

    private sealed class ZenArcherMarker
    {
        // used for easier detection of Zen Archer characters to extend their Monk weapon list
    }

    private sealed class ZenArcherStunningArrows
    {
        // used for easier detection of Zen Archer characters to allow stunning strike on arrows
    }

    internal sealed class ZenArcherDiceUpgrade : IRestrictedContextValidator
    {
        private ZenArcherDiceUpgrade()
        {
        }

        public static IRestrictedContextValidator Marker { get; } = new ZenArcherDiceUpgrade();


        public (OperationType, bool) ValidateContext(BaseDefinition definition, IRestrictedContextProvider provider,
            RulesetCharacter character, ItemDefinition itemDefinition, bool rangedAttack, RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            return (OperationType.Or, IsZenArcherWeapon(character, attackMode.sourceDefinition as ItemDefinition));
        }
    }

    private sealed class UpgradeFlurry : IOnAfterActionFeature
    {
        private readonly ConditionDefinition condition;

        public UpgradeFlurry(ConditionDefinition condition)
        {
            this.condition = condition;
        }

        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionFlurryOfBlows)
            {
                return;
            }

            var character = action.ActingCharacter?.RulesetCharacter;
            if (character == null)
            {
                return;
            }

            var rulesetCondition = RulesetCondition.CreateActiveCondition(character.Guid,
                condition,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                character.Guid,
                string.Empty
            );

            character.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }

    private sealed class AddFlurryOfArrowsAttacks : AddExtraAttackBase
    {
        private AddFlurryOfArrowsAttacks() : base(ActionDefinitions.ActionType.Bonus, ValidatorsCharacter.HasNoArmor,
            ValidatorsCharacter.HasNoShield, WieldsZenArcherWeapon)
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
            var strikeDefinition = mainHandItem.itemDefinition;
            var attackModifiers = hero.attackModifiers;
            var attackMode = hero.RefreshAttackMode(
                ActionType,
                strikeDefinition,
                strikeDefinition.WeaponDescription,
                ValidatorsCharacter.IsFreeOffhandForUnarmedTa(hero),
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
