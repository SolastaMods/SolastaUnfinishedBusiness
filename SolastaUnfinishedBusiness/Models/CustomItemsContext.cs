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
                ItemPropertyDescriptionsContext.BuildFrom(FeatureDefinitionPerceptionAffinityBuilder
                        .Create("PerceptionAffinityHelmOfAwareness")
                        .SetGuiPresentation(Category.Feature)
                        .CannotBeSurprised()
                        .AddToDB(),
                    false))
            .AddToDB();

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
                .BuildFrom(FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create("AbilityCheckAffinityGlovesOfThievery")
                    .SetGuiPresentationNoContent()
                    .BuildAndSetAffinityGroups(RuleDefinitions.CharacterAbilityCheckAffinity.None,
                        RuleDefinitions.DieType.D1, 5,
                        (AttributeDefinitions.Dexterity, SkillDefinitions.SleightOfHand),
                        (AttributeDefinitions.Dexterity, ToolDefinitions.ThievesToolsType))
                    .AddToDB(), false))
            .AddToDB();

        MerchantContext.AddItem(item, ShopItemType.MagicItemUncommon);
        return item;
    }
}
