//TODO: need support to fully integrate this sub with official monk...

using System;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class TraditionZenArcher : AbstractSubclass
{
    private const string FlurryTag = "MonkFlurryAttack";
    private const string ZenArrowTag = "ZenArrow";
    private static IsCharacterValidHandler _attackedWithMonkWeapon;
    private static AssetReferenceSprite _flurryOfArrows, _zenArrows;

    // Zen Archer's Monk weapons are bows and darts ranged weapons.
    private static readonly List<WeaponTypeDefinition> MonkWeapons = new()
    {
        WeaponTypeDefinitions.ShortbowType, WeaponTypeDefinitions.LongbowType
    };

    private static FeatureDefinitionPower _distantHandTechnique;
    private static ConditionDefinition _distractedCondition;

    // ReSharper disable once InconsistentNaming
    private readonly CharacterSubclassDefinition Subclass;

    public TraditionZenArcher()
    {
        Subclass = CharacterSubclassDefinitionBuilder
            .Create("TraditionZenArcher")
            .SetOrUpdateGuiPresentation(Category.Subclass,
                CharacterSubclassDefinitions.RangerMarksman.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3, BuildLevel03Features())
            .AddFeaturesAtLevel(6, BuildLevel06Features())
            .AddFeaturesAtLevel(11, BuildLevel11Features())
            .AddFeaturesAtLevel(17, BuildLevel17Features())
            .AddToDB();
    }

    private static AssetReferenceSprite FlurryOfArrows =>
        _flurryOfArrows ??=
            null; // CustomIcons.CreateAssetReferenceSprite("FlurryOfArrows", Resources.FlurryOfArrows, 128, 64);

    private static AssetReferenceSprite ZenArrows =>
        _zenArrows ??= null; // CustomIcons.CreateAssetReferenceSprite("ZenArrow", Resources.ZenArrow, 128, 64);

    private static FeatureDefinitionPower DistantHandTechnique => _distantHandTechnique ??= BuildZenArrow();
    private static ConditionDefinition DistractedCondition => _distractedCondition ??= BuildDistractedCondition();

    private static bool IsMonkWeapon2(RulesetAttackMode attackMode, RulesetItem weapon, RulesetCharacter character)
    {
        return IsMonkWeapon2(character, attackMode) || IsMonkWeapon2(character, weapon);
    }

    private static bool IsMonkWeapon2(RulesetCharacter character, RulesetAttackMode attackMode)
    {
        return attackMode is { SourceDefinition: ItemDefinition item } && IsMonkWeapon2(character, item);
    }

    private static bool IsMonkWeapon2(RulesetCharacter character, RulesetItem weapon)
    {
        //fists
        return weapon == null || IsMonkWeapon2(character, weapon.ItemDefinition);
    }

    private static bool IsMonkWeapon2(RulesetCharacter character, ItemDefinition weapon)
    {
        if (weapon == null)
        {
            return false;
        }

        var typeDefinition = weapon.WeaponDescription?.WeaponTypeDefinition;

        if (typeDefinition == null)
        {
            return false;
        }

        return MonkWeapons.Contains(typeDefinition)
               || IsMonkWeapon(character, weapon);
    }

    private static bool IsMonkWeapon(RulesetCharacter character, ItemDefinition item)
    {
        if (character == null || item == null)
        {
            return false;
        }

        var typeDefinition = item.WeaponDescription?.WeaponTypeDefinition;

        if (typeDefinition == null)
        {
            return false;
        }

        return character.HasSubFeatureOfType<ZenArcherMarker>()
               && MonkWeapons.Contains(typeDefinition);
    }

    private static FeatureDefinition[] BuildLevel03Features()
    {
        return new FeatureDefinition[]
        {
            FeatureDefinitionProficiencyBuilder
                .Create("ClassMonkZenArcherCombat")
                .SetGuiPresentation(Category.Feature)
                .SetProficiencies(ProficiencyType.Weapon,
                    WeaponTypeDefinitions.LongbowType.Name,
                    WeaponTypeDefinitions.ShortbowType.Name)
                .SetCustomSubFeatures(
                    new ZenArcherMarker(),
                    new RangedAttackInMeleeDisadvantageRemover(IsMonkWeapon2,
                        ValidatorsCharacter.NoArmor, ValidatorsCharacter.NoShield),
                    new AddTagToWeaponAttack(ZenArrowTag, IsZenArrowAttack)
                )
                .AddToDB(),
            DistantHandTechnique
        };
    }

    private static FeatureDefinitionPower BuildZenArrow()
    {
        var technique = FeatureDefinitionPowerBuilder
            .Create("ClassMonkZenArrowTechnique")
            .SetGuiPresentation(Category.Feature, ZenArrows)
            // .SetShortTitle("Power/&ClassMonkZenArrowTechniqueShortTitle")
            // .SetSharedPool(Monk.KiPool)
            .SetActivationTime(ActivationTime.OnAttackHit)
            .SetRechargeRate(RechargeRate.KiPoints)
            .SetCostPerUse(1)
            .SetCustomSubFeatures(new ReactionAttackModeRestriction(
                (mode, _, _, _) => mode != null && mode.AttackTags.Contains(ZenArrowTag)
            ))
            .AddToDB();

        var prone = FeatureDefinitionPowerBuilder
            .Create("ClassMonkZenArrowProne")
            .SetGuiPresentation(Category.Feature)
            // .SetSharedPool(Monk.KiPool)
            .SetActivationTime(ActivationTime.NoCost)
            .SetRechargeRate(RechargeRate.KiPoints)
            .SetCostPerUse(1)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Dexterity,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency
                )
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                    .SetMotionForm(MotionForm.MotionType.FallProne, 0)
                    .Build())
                .Build())
            .AddToDB();

        var push = FeatureDefinitionPowerBuilder
            .Create("ClassMonkZenArrowPush")
            .SetGuiPresentation(Category.Feature)
            // .SetSharedPool(Monk.KiPool)
            .SetActivationTime(ActivationTime.NoCost)
            .SetRechargeRate(RechargeRate.KiPoints)
            .SetCostPerUse(1)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Strength,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency
                )
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                    .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
                    .Build())
                .Build())
            .AddToDB();

        var distract = FeatureDefinitionPowerBuilder
            .Create("ClassMonkZenArrowDistract")
            .SetGuiPresentation(Category.Feature)
            // .SetSharedPool(Monk.KiPool)
            .SetActivationTime(ActivationTime.NoCost)
            .SetRechargeRate(RechargeRate.KiPoints)
            .SetCostPerUse(1)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Wisdom,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency
                )
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .SetConditionForm(DistractedCondition, ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        PowersBundleContext.RegisterPowerBundle(technique, true, prone, push, distract);

        return technique;
    }

    private static ConditionDefinition BuildDistractedCondition()
    {
        return ConditionDefinitionBuilder
            .Create("ClassMonkZenArrowDistractCondition")
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionDazzled.GuiPresentation.SpriteReference)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .SetConditionType(ConditionType.Detrimental)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .SetFeatures(FeatureDefinitionCombatAffinityBuilder
                .Create("ClassMonkZenArrowDistractFeature")
                .SetGuiPresentationNoContent(true)
                .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                .AddToDB())
            .AddToDB();
    }

    private static FeatureDefinition[] BuildLevel06Features()
    {
        var extraFlurryAttack1 = FeatureDefinitionAdditionalActionBuilder
            .Create("ClassMonkZenArcherFlurryOfArrowsExtraAttacks1")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new AddExtraMainHandAttack(ActionDefinitions.ActionType.Bonus, true,
                    ValidatorsCharacter.NoArmor, ValidatorsCharacter.NoShield, WieldsZenArcherWeapon)
                .SetTags(FlurryTag)) //TODO: do we need flurry tag here?
            .SetActionType(ActionDefinitions.ActionType.Bonus)
            .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
            .AddToDB();

        var extraFlurryAttack2 = FeatureDefinitionAdditionalActionBuilder
            .Create("ClassMonkZenArcherFlurryOfArrowsExtraAttacks2")
            .SetGuiPresentationNoContent(true)
            .SetActionType(ActionDefinitions.ActionType.Bonus)
            .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
            .AddToDB();

        var attackedWithMonkWeaponCondition = ConditionDefinitionBuilder
            .Create("ClassMonkAttackedWithMonkWeapon")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        _attackedWithMonkWeapon = ValidatorsCharacter.HasAnyOfConditions(attackedWithMonkWeaponCondition);

        var flurryOfArrows = FeatureDefinitionPowerBuilder
            .Create("ClassMonkZenArcherFlurryOfArrows")
            .SetGuiPresentation(Category.Feature, FlurryOfArrows)
            // .SetSharedPool(Monk.KiPool)
            .SetActivationTime(ActivationTime.BonusAction)
            .SetCostPerUse(2)
            .SetRechargeRate(RechargeRate.KiPoints)
            .SetShowCasting(false)
            //TODO: check this...
            // .SetCustomSubFeatures(new PowerUseValidity(_attackedWithMonkWeapon, ValidatorsCharacter.NoShield, ValidatorsCharacter.NoArmor))
            .SetEffectDescription(new EffectDescriptionBuilder()
                .AddEffectForm(new EffectFormBuilder()
                    .SetConditionForm(ConditionDefinitionBuilder
                            .Create("ClassMonkZenArcherFlurryOfArrowsCondition")
                            .SetGuiPresentationNoContent(true)
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            .SetDuration(DurationType.Round, 0, false)
                            .SetSpecialDuration(true)
                            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                            .SetSpecialInterruptions(ConditionInterruption.BattleEnd,
                                ConditionInterruption.AnyBattleTurnEnd)
                            .SetFeatures(extraFlurryAttack1, extraFlurryAttack2)
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add, true, true)
                    .Build())
                .Build())
            .AddToDB();

        var kiPoweredArrows = FeatureDefinitionBuilder
            .Create("ClassMonkZenArcherKiPoweredArows")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new AddTagToWeaponAttack(TagsDefinitions.Magical,
                (mode, _, character) => IsMonkWeapon(character, mode.SourceDefinition as ItemDefinition)))
            .AddToDB();

        return new[] { kiPoweredArrows, flurryOfArrows };
    }

    private static FeatureDefinition[] BuildLevel11Features()
    {
        var stunningArrows = FeatureDefinitionBuilder
            .Create("ClassMonkZenArcherStunningArrows")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ZenArcherStunningArrows())
            .AddToDB();

        return new[] { stunningArrows, BuildUpgradedZenArrow() };
    }

    private static FeatureDefinition BuildUpgradedZenArrow()
    {
        var technique = FeatureDefinitionPowerBuilder
            .Create("ClassMonkZenArrowUpgradedTechnique")
            .SetGuiPresentation(Category.Feature, ZenArrows)
            // .SetShortTitle("Power/&ClassMonkZenArrowUpgradedTechniqueShortTitle")
            // .SetSharedPool(Monk.KiPool)
            .SetActivationTime(ActivationTime.OnAttackHit)
            .SetRechargeRate(RechargeRate.ShortRest)
            .SetCostPerUse(1)
            .SetOverriddenPower(DistantHandTechnique)
            .SetCustomSubFeatures(new ReactionAttackModeRestriction(
                (mode, _, _, _) => mode != null && mode.AttackTags.Contains(ZenArrowTag)
            ))
            .AddToDB();

        var prone = FeatureDefinitionPowerBuilder
            .Create("ClassMonkZenArrowUpgradedProne")
            .SetGuiPresentation(Category.Feature)
            // .SetSharedPool(Monk.KiPool)
            .SetActivationTime(ActivationTime.NoCost)
            .SetRechargeRate(RechargeRate.KiPoints)
            .SetCostPerUse(1)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Dexterity,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency
                )
                .SetEffectForms(new EffectFormBuilder()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                        .SetMotionForm(MotionForm.MotionType.FallProne, 0)
                        .Build(),
                    new EffectFormBuilder()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetConditionForm(ConditionDefinitionBuilder
                            .Create("ClassMonkZenArrowUpgradedSlowCondition")
                            .SetGuiPresentation(Category.Condition,
                                ConditionDefinitions.ConditionEncumbered.GuiPresentation.SpriteReference)
                            .SetDuration(DurationType.Round, 1)
                            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                            .SetConditionType(ConditionType.Detrimental)
                            .SetFeatures(FeatureDefinitionMovementAffinityBuilder
                                .Create("ClassMonkZenArrowUpgradedSlowFeature")
                                .SetGuiPresentationNoContent(true)
                                //TODO: check if zero on diags... .SetBaseSpeedMultiplicativeModifier(0)
                                .AddToDB())
                            .AddToDB(), ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();

        var push = FeatureDefinitionPowerBuilder
            .Create("ClassMonkZenArrowUpgradedPush")
            .SetGuiPresentation(Category.Feature)
            // .SetSharedPool(Monk.KiPool)
            .SetActivationTime(ActivationTime.NoCost)
            .SetRechargeRate(RechargeRate.ShortRest)
            .SetCostPerUse(1)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Strength,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency
                )
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                    .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 4)
                    .Build())
                .Build())
            .AddToDB();

        var distract = FeatureDefinitionPowerBuilder
            .Create("ClassMonkZenArrowUpgradedDistract")
            .SetGuiPresentation(Category.Feature)
            // .SetSharedPool(Monk.KiPool)
            .SetRechargeRate(RechargeRate.KiPoints)
            .SetActivationTime(ActivationTime.NoCost)
            .SetRechargeRate(RechargeRate.ShortRest)
            .SetCostPerUse(1)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Wisdom,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency
                )
                .SetEffectForms(
                    new EffectFormBuilder()
                        .HasSavingThrow(EffectSavingThrowType.None)
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetConditionForm(ConditionDefinitionBuilder
                            .Create("ClassMonkZenArrowUpgradedDistractCondition")
                            .SetGuiPresentation(Category.Condition,
                                ConditionDefinitions.ConditionDazzled.GuiPresentation.SpriteReference)
                            .SetDuration(DurationType.Round, 1)
                            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                            .SetConditionType(ConditionType.Detrimental)
                            .SetFeatures(FeatureDefinitionCombatAffinityBuilder
                                .Create("ClassMonkZenArrowUpgradedDistractFeature")
                                .SetGuiPresentationNoContent(true)
                                .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                                .AddToDB())
                            .AddToDB(), ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();

        PowersBundleContext.RegisterPowerBundle(technique, true, prone, push, distract);

        return technique;
    }

    private static FeatureDefinition[] BuildLevel17Features()
    {
        return Array.Empty<FeatureDefinition>();
    }

    private static bool IsZenArrowAttack(RulesetAttackMode mode, RulesetItem weapon, RulesetCharacter character)
    {
        return mode != null
               && (mode.Ranged || mode.Thrown)
               && IsMonkWeapon(character, mode.SourceDefinition as ItemDefinition);
    }

    private static bool WieldsZenArcherWeapon(RulesetCharacter character)
    {
        var mainHandItem = character.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand]
            .EquipedItem;

        return IsMonkWeapon(character, mainHandItem?.ItemDefinition);
    }

    // private class ExtendWeaponRange : IModifyAttackModeForWeapon
    // {
    //     public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon)
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

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }

    private sealed class ZenArcherMarker
    {
        //Used for easier detection of Zen Archer characters to extend their Monk weapon list
    }

    public sealed class ZenArcherStunningArrows
    {
        //Used for easier detection of Zen Archer characters to allow stunning strike on arrows
    }
}
