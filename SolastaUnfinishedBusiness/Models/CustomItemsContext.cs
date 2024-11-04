using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class CustomItemsContext
{
    private static readonly Dictionary<string, TagsDefinitions.Criticity> Tags = [];
    private static ItemDefinition _helmOfAwareness;
    private static ItemDefinition _glovesOfThievery;

    internal static ItemDefinition HelmOfAwareness => _helmOfAwareness ??= BuildHelmOfAwareness();
    internal static ItemDefinition GlovesOfThievery => _glovesOfThievery ??= BuildGlovesOfThievery();

    internal static void Load()
    {
        _helmOfAwareness ??= BuildHelmOfAwareness();
        _glovesOfThievery ??= BuildGlovesOfThievery();

        LoadAfterRestIdentify();

        SwitchAllowClubsToBeThrown();
        SwitchUniversalSylvanArmorAndLightbringer();
        SwitchMagicStaffFoci();
    }

    private static ItemDefinition BuildHelmOfAwareness()
    {
        var item = ItemDefinitionBuilder
            .Create("HelmOfAwareness")
            .SetGuiPresentation(Category.Item, HelmOfComprehendingLanguages)
            .SetItemPresentation(HelmOfComprehendingLanguages)
            .SetMerchantCategory(MerchantCategoryDefinitions.MagicDevice)
            .SetItemRarity(ItemRarity.Rare)
            .MakeMagical()
            .RequireAttunement()
            .SetSlotTypes(SlotTypeDefinitions.HeadSlot, SlotTypeDefinitions.ContainerSlot)
            .SetSlotsWhereActive(SlotTypeDefinitions.HeadSlot)
            .SetGold(1250)
            .SetWeight(2)
            .SetStaticProperties(
                ItemPropertyDescriptionsContext.BuildFrom(
                    FeatureDefinitionCombatAffinitys.CombatAffinityEagerForBattle, false),
                ItemPropertyDescriptionsContext.BuildFrom(
                    FeatureDefinitionPerceptionAffinityBuilder
                        .Create("PerceptionAffinityHelmOfAwareness")
                        .SetGuiPresentation(Gui.NoLocalization,
                            "Feature/&PerceptionAffinityHelmOfAwarenessDescription")
                        .CannotBeSurprised()
                        .AddToDB(),
                    false))
            .AddToDB();

        item.inDungeonEditor = Main.Settings.AddNewWeaponsAndRecipesToEditor;

        MerchantContext.AddItem(item, ShopItemType.MagicItemRare);
        return item;
    }

    private static ItemDefinition BuildGlovesOfThievery()
    {
        var item = ItemDefinitionBuilder
            .Create("GlovesOfThievery")
            .SetGuiPresentation(Category.Item, GlovesOfMissileSnaring)
            .SetItemPresentation(GlovesOfMissileSnaring)
            .SetMerchantCategory(MerchantCategoryDefinitions.MagicDevice)
            .SetItemRarity(ItemRarity.Uncommon)
            .MakeMagical()
            .NoAttunement()
            .SetSlotTypes(SlotTypeDefinitions.GlovesSlot, SlotTypeDefinitions.ContainerSlot)
            .SetSlotsWhereActive(SlotTypeDefinitions.GlovesSlot)
            .SetGold(120)
            .SetWeight(0.5f)
            .SetStaticProperties(ItemPropertyDescriptionsContext
                .BuildFrom(
                    FeatureDefinitionAbilityCheckAffinityBuilder
                        .Create("AbilityCheckAffinityGlovesOfThievery")
                        .SetGuiPresentation("GlovesOfThievery", Category.Item, Gui.NoLocalization)
                        .BuildAndSetAffinityGroups(
                            CharacterAbilityCheckAffinity.None,
                            DieType.D1, 5,
                            AbilityCheckGroupOperation.AddDie,
                            (AttributeDefinitions.Dexterity, SkillDefinitions.SleightOfHand),
                            (AttributeDefinitions.Dexterity, ToolDefinitions.ThievesToolsType))
                        .AddToDB(), false))
            .AddToDB();

        item.inDungeonEditor = Main.Settings.AddNewWeaponsAndRecipesToEditor;

        MerchantContext.AddItem(item, ShopItemType.MagicItemUncommon);
        return item;
    }

    private static void LoadAfterRestIdentify()
    {
        const string AfterRestIdentifyName = "PowerAfterRestIdentify";

        RestActivityDefinitionBuilder
            .Create("RestActivityShortRestIdentify")
            .SetGuiPresentation(AfterRestIdentifyName, Category.Feature)
            .AddCustomSubFeatures(new ValidateRestActivity(false, false))
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.None,
                PowerBundleContext.UseCustomRestPowerFunctorName,
                AfterRestIdentifyName)
            .AddToDB();

        RestActivityDefinitionBuilder
            .Create("RestActivityLongRestIdentify")
            .SetGuiPresentation(AfterRestIdentifyName, Category.Feature)
            .AddCustomSubFeatures(new ValidateRestActivity(false, false))
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.LongRest,
                RestActivityDefinition.ActivityCondition.None,
                PowerBundleContext.UseCustomRestPowerFunctorName,
                AfterRestIdentifyName)
            .AddToDB();

        var afterRestIdentifyCondition = ConditionDefinitionBuilder
            .Create("AfterRestIdentify")
            .SetGuiPresentation(Category.Condition)
            .AddCustomSubFeatures(OnConditionAddedOrRemovedIdentifyItems.Mark)
            .AddToDB();

        FeatureDefinitionPowerBuilder
            .Create(AfterRestIdentifyName)
            .SetGuiPresentation(Category.Feature, hidden: true)
            .AddCustomSubFeatures(CanIdentifyOnRest.Mark)
            .SetUsesFixed(ActivationTime.Rest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(
                        Side.Ally,
                        RangeType.Self,
                        1,
                        TargetType.Self)
                    .SetDurationData(
                        DurationType.Minute,
                        1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                afterRestIdentifyCondition,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();
    }

    internal static void SwitchAllowClubsToBeThrown()
    {
        var db = DatabaseRepository.GetDatabase<ItemDefinition>();

        foreach (var itemDefinition in db
                     .Where(x => x.IsWeapon &&
                                 x.WeaponDescription.WeaponTypeDefinition == WeaponTypeDefinitions.ClubType))
        {
            if (Main.Settings.AllowClubsToBeThrown)
            {
                itemDefinition.WeaponDescription.WeaponTags.Add(TagsDefinitions.WeaponTagThrown);
                itemDefinition.WeaponDescription.maxRange = 10;
            }
            else
            {
                itemDefinition.WeaponDescription.WeaponTags.Remove(TagsDefinitions.WeaponTagThrown);
                itemDefinition.WeaponDescription.maxRange = 5;
            }
        }
    }

    internal static void SwitchUniversalSylvanArmorAndLightbringer()
    {
        GreenmageArmor.RequiredAttunementClasses.Clear();
        WizardClothes_Alternate.RequiredAttunementClasses.Clear();

        if (Main.Settings.AllowAnyClassToWearSylvanArmor)
        {
            return;
        }

        var allowedClasses = new[] { Wizard, Sorcerer, Warlock };

        GreenmageArmor.RequiredAttunementClasses.AddRange(allowedClasses);
        WizardClothes_Alternate.RequiredAttunementClasses.AddRange(allowedClasses);
    }

    internal static void SwitchMagicStaffFoci()
    {
        if (!Main.Settings.MakeAllMagicStaveArcaneFoci)
        {
            return;
        }

        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>()
                     .Where(x => x.IsWeapon) // WeaponDescription could be null
                     .Where(x => x.WeaponDescription.WeaponType == EquipmentDefinitions.WeaponTypeQuarterstaff)
                     .Where(x => x.Magical && !x.Name.Contains("OfHealing")))
        {
            item.IsFocusItem = true;
            item.FocusItemDescription.focusType = EquipmentDefinitions.FocusType.Arcane;
        }
    }

    internal static bool IsAttackModeInvalid(RulesetCharacter character, RulesetAttackMode mode)
    {
        if (character is not RulesetCharacterHero hero)
        {
            return false;
        }

        return IsHandCrossbowUseInvalid(mode.sourceObject as RulesetItem, hero,
            hero.GetItemInSlot(EquipmentDefinitions.SlotTypeMainHand),
            hero.GetItemInSlot(EquipmentDefinitions.SlotTypeOffHand));
    }

    internal static bool IsHandCrossbowUseInvalid(
        RulesetItem item,
        RulesetCharacterHero hero,
        RulesetItem main,
        RulesetItem off)
    {
        if (Main.Settings.IgnoreHandXbowFreeHandRequirements)
        {
            return false;
        }

        if (item == null || hero == null)
        {
            return false;
        }

        Tags.Clear();
        item.FillTags(Tags, hero, true);

        if (!Tags.ContainsKey(TagsDefinitions.WeaponTagAmmunition) ||
            Tags.ContainsKey(TagsDefinitions.WeaponTagTwoHanded))
        {
            return false;
        }

        if (main == item && off != null)
        {
            return true;
        }

        return off == item
               && main != null
               && main.ItemDefinition.WeaponDescription?.WeaponType != WeaponTypeDefinitions.UnarmedStrikeType.Name;
    }


    private sealed class CanIdentifyOnRest : IValidatePowerUse
    {
        private CanIdentifyOnRest()
        {
        }

        public static CanIdentifyOnRest Mark { get; } = new();

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            //does this work properly for wild-shaped heroes?
            if (character is not RulesetCharacterHero hero)
            {
                return false;
            }

            return Main.Settings.IdentifyAfterRest && hero.HasNonIdentifiedItems();
        }
    }

    private sealed class OnConditionAddedOrRemovedIdentifyItems : IOnConditionAddedOrRemoved
    {
        public static OnConditionAddedOrRemovedIdentifyItems Mark { get; } = new();

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            target.GetOriginalHero()?.AutoIdentifyInventoryItems();
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }
}
