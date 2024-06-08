using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Models;

internal static class CustomItemsContext
{
    private static ItemDefinition _helmOfAwareness;
    private static ItemDefinition _glovesOfThievery;

    internal static ItemDefinition HelmOfAwareness => _helmOfAwareness ??= BuildHelmOfAwareness();
    internal static ItemDefinition GlovesOfThievery => _glovesOfThievery ??= BuildGlovesOfThievery();

    internal static void Load()
    {
        _helmOfAwareness ??= BuildHelmOfAwareness();
        _glovesOfThievery ??= BuildGlovesOfThievery();
    }

    private static ItemDefinition BuildHelmOfAwareness()
    {
        var item = ItemDefinitionBuilder
            .Create("HelmOfAwareness")
            .SetGuiPresentation(Category.Item, ItemDefinitions.HelmOfComprehendingLanguages)
            .SetItemPresentation(ItemDefinitions.HelmOfComprehendingLanguages)
            .SetMerchantCategory(MerchantCategoryDefinitions.MagicDevice)
            .SetItemRarity(RuleDefinitions.ItemRarity.Rare)
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
            .SetGuiPresentation(Category.Item, ItemDefinitions.GlovesOfMissileSnaring)
            .SetItemPresentation(ItemDefinitions.GlovesOfMissileSnaring)
            .SetMerchantCategory(MerchantCategoryDefinitions.MagicDevice)
            .SetItemRarity(RuleDefinitions.ItemRarity.Uncommon)
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
                            RuleDefinitions.CharacterAbilityCheckAffinity.None,
                            RuleDefinitions.DieType.D1, 5,
                            RuleDefinitions.AbilityCheckGroupOperation.AddDie,
                            (AttributeDefinitions.Dexterity, SkillDefinitions.SleightOfHand),
                            (AttributeDefinitions.Dexterity, ToolDefinitions.ThievesToolsType))
                        .AddToDB(), false))
            .AddToDB();

        item.inDungeonEditor = Main.Settings.AddNewWeaponsAndRecipesToEditor;

        MerchantContext.AddItem(item, ShopItemType.MagicItemUncommon);
        return item;
    }
}
