using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Classes.Monk.Subclasses
{
    public static class ZenArcher
    {
        private const string ZenArrowTag = "ZenArrow";

        private static AssetReferenceSprite _flurryOfArrows, _zenArrows;

        private static AssetReferenceSprite FlurryOfArrows => _flurryOfArrows ??=
            CustomIcons.CreateAssetReferenceSprite("FlurryOfArrows", Resources.FlurryOfArrows, 128, 64);

        private static AssetReferenceSprite ZenArrows => _zenArrows ??=
            CustomIcons.CreateAssetReferenceSprite("ZenArrow", Resources.ZenArrow, 128, 64);

        // Zen Archer's Monk weapons are bows and darts ranged weapons.
        private static readonly List<WeaponTypeDefinition> MonkWeapons = new()
        {
            WeaponTypeDefinitions.ShortbowType,
            WeaponTypeDefinitions.LongbowType,
        };

        public static CharacterSubclassDefinition Build()
        {
            return CharacterSubclassDefinitionBuilder
                .Create("ClassMonkTraditionZenArcher", DefinitionBuilder.CENamespaceGuid)
                .SetOrUpdateGuiPresentation(Category.Subclass,
                    CharacterSubclassDefinitions.RangerMarksman.GuiPresentation.SpriteReference)
                .AddFeaturesAtLevel(3, BuildLevel03Features())
                .AddFeaturesAtLevel(6, BuildLevel06Features())
                .AddFeaturesAtLevel(11, BuildLevel11Features())
                .AddFeaturesAtLevel(17, BuildLevel17Features())
                .AddToDB();
        }

        public static bool IsMonkWeapon(RulesetCharacter character, ItemDefinition item)
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
            return new[]
            {
                FeatureDefinitionProficiencyBuilder
                    .Create("ClassMonkZenArcherCombat", Monk.GUID)
                    .SetGuiPresentation(Category.Feature)
                    .SetProficiencies(ProficiencyType.Weapon,
                        WeaponTypeDefinitions.LongbowType.Name,
                        WeaponTypeDefinitions.ShortbowType.Name)
                    .SetCustomSubFeatures(
                        new ZenArcherMarker(),
                        new RangedAttackInMeleeDisadvantageRemover(Monk.IsMonkWeapon,
                            CharacterValidators.NoArmor, CharacterValidators.NoShield),
                        new AddTagToWeaponAttack(ZenArrowTag, IsZenArrowAttack)
                    )
                    .AddToDB(),
                BuildZenArrow()
            };
        }

        private static FeatureDefinition BuildZenArrow()
        {
            var technique = FeatureDefinitionPowerSharedPoolBuilder
                .Create("ClassMonkZenArrowTechnique", Monk.GUID)
                .SetGuiPresentation(Category.Power, ZenArrows)
                .SetShortTitle("Power/&ClassMonkZenArrowTechniqueSortTitle")
                .SetSharedPool(Monk.KiPool)
                .SetActivationTime(ActivationTime.OnAttackHit)
                .SetRechargeRate(RechargeRate.ShortRest)
                .SetCostPerUse(1)
                .SetCustomSubFeatures(new ReactionAttackModeRestriction(
                    (mode, _, _) => mode.AttackTags.Contains(ZenArrowTag)
                ))
                .AddToDB();

            var prone = FeatureDefinitionPowerSharedPoolBuilder
                .Create("ClassMonkZenArrowProne", Monk.GUID)
                .SetGuiPresentation(Category.Power)
                .SetSharedPool(Monk.KiPool)
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
                        AttributeDefinitions.Dexterity,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Wisdom
                    )
                    .SetEffectForms(new EffectFormBuilder()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                        .SetMotionForm(MotionForm.MotionType.FallProne, 0)
                        .Build())
                    .Build())
                .AddToDB();

            var push = FeatureDefinitionPowerSharedPoolBuilder
                .Create("ClassMonkZenArrowPush", Monk.GUID)
                .SetGuiPresentation(Category.Power)
                .SetSharedPool(Monk.KiPool)
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
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Wisdom
                    )
                    .SetEffectForms(new EffectFormBuilder()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                        .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
                        .Build())
                    .Build())
                .AddToDB();

            var distract = FeatureDefinitionPowerSharedPoolBuilder
                .Create("ClassMonkZenArrowDistract", Monk.GUID)
                .SetGuiPresentation(Category.Power)
                .SetSharedPool(Monk.KiPool)
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
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Wisdom
                    )
                    .SetEffectForms(new EffectFormBuilder()
                        .HasSavingThrow(EffectSavingThrowType.None)
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetConditionForm(ConditionDefinitionBuilder
                            .Create("ClassMonkZenArrowDistractCondition", Monk.GUID)
                            .SetGuiPresentation(Category.Condition,
                                ConditionDefinitions.ConditionDazzled.GuiPresentation.SpriteReference)
                            .SetDuration(DurationType.Round, 1)
                            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                            .SetConditionType(ConditionType.Detrimental)
                            .SetSpecialInterruptions(ConditionInterruption.Attacks)
                            .SetFeatures(FeatureDefinitionCombatAffinityBuilder
                                .Create("ClassMonkZenArrowDistractFeature", Monk.GUID)
                                .SetGuiPresentationNoContent(true)
                                .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                                .AddToDB())
                            .AddToDB(), ConditionForm.ConditionOperation.Add)
                        .Build())
                    .Build())
                .AddToDB();

            PowerBundleContext.RegisterPowerBundle(technique, true, prone, push, distract);

            return technique;
        }

        private static FeatureDefinition[] BuildLevel06Features()
        {
            var extraFlurryAttack1 = FeatureDefinitionAdditionalActionBuilder
                .Create("ClassMonkZenArcherFlurryOfArrowsExtraAttacks1", Monk.GUID)
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(new AddExtraMainHandAttack(ActionDefinitions.ActionType.Bonus, true,
                        CharacterValidators.NoArmor, CharacterValidators.NoShield, WieldsZenArcherWeapon)
                    .SetTags(Monk.FlurryTag)) //Do we need flurry tag here?
                .SetActionType(ActionDefinitions.ActionType.Bonus)
                .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
                .AddToDB();

            var extraFlurryAttack2 = FeatureDefinitionAdditionalActionBuilder
                .Create("ClassMonkZenArcherFlurryOfArrowsExtraAttacks2", Monk.GUID)
                .SetGuiPresentationNoContent(true)
                .SetActionType(ActionDefinitions.ActionType.Bonus)
                .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
                .AddToDB();

            var flurryOfArrows = FeatureDefinitionPowerSharedPoolBuilder
                .Create("ClassMonkZenArcherFlurryOfArrows", Monk.GUID)
                .SetGuiPresentation(Category.Power , FlurryOfArrows)
                .SetSharedPool(Monk.KiPool)
                .SetActivationTime(ActivationTime.BonusAction)
                .SetCostPerUse(2)
                .SetRechargeRate(RechargeRate.ShortRest)
                .SetShowCasting(false)
                .SetCustomSubFeatures(new PowerUseValidity(Monk.attackedWithMonkWeapon,
                    CharacterValidators.NoShield, CharacterValidators.NoArmor))
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .AddEffectForm(new EffectFormBuilder()
                        .SetConditionForm(ConditionDefinitionBuilder
                                .Create("ClassMonkZenArcherFlurryOfArrowsCondition", Monk.GUID)
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
                .Create("ClassMonkZenArcherKiPoweredArows", Monk.GUID)
                .SetGuiPresentation(Category.Feature)
                .SetCustomSubFeatures(new AddTagToWeaponAttack(TagsDefinitions.Magical,
                    (mode, _, character) => IsMonkWeapon(character, mode.SourceDefinition as ItemDefinition)))
                .AddToDB();

            return new[]
            {
                kiPoweredArrows,
                flurryOfArrows
            };
        }

        private static FeatureDefinition[] BuildLevel11Features()
        {
            return System.Array.Empty<FeatureDefinition>();
        }

        private static FeatureDefinition[] BuildLevel17Features()
        {
            return System.Array.Empty<FeatureDefinition>();
        }

        private static bool IsZenArrowAttack(RulesetAttackMode mode, RulesetItem weapon, RulesetCharacter character)
        {
            return mode is {Reach: false, Magical: false}
                   && (mode.Ranged || mode.Thrown)
                   && IsMonkWeapon(character, mode.SourceDefinition as ItemDefinition);
        }

        private static bool WieldsZenArcherWeapon(RulesetCharacter character)
        {
            var mainHandItem = character.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand]
                .EquipedItem;
            
            return IsMonkWeapon(character, mainHandItem?.ItemDefinition);
        }

        private class ZenArcherMarker
        {
            //Used for easier detecton of Zen Archer characters to extend their Monk weapon list
        }

        private class ExtendWeaponRange : IModifyAttackModeForWeapon
        {
            public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon)
            {
                if (attackMode == null || attackMode.Magical || (!attackMode.Ranged && !attackMode.Thrown))
                {
                    return;
                }

                if (!Monk.IsMonkWeapon(character, attackMode))
                {
                    return;
                }

                attackMode.CloseRange = Math.Min(16, attackMode.CloseRange * 2);
                attackMode.MaxRange = Math.Min(32, attackMode.MaxRange * 2);
            }
        }
    }
}