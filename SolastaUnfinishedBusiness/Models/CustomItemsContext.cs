using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Models;

internal static class CustomItemsContext
{
    private static ItemDefinition _helmOfAwareness;
    internal static ItemDefinition HelmOfAwareness => _helmOfAwareness ??= BuildHelmOfAwareness();

    internal static void Load()
    {
        _helmOfAwareness ??= BuildHelmOfAwareness();
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
            .SetRequiresIdentification(false)
            .SetSlotTypes(SlotTypeDefinitions.HeadSlot, SlotTypeDefinitions.ContainerSlot)
            .SetSlotsWhereActive(SlotTypeDefinitions.HeadSlot)
            .SetGold(250)
            .SetWeight(2)
            .SetStaticProperties(
                ItemPropertyDescriptionsContext.BuildFrom(FeatureDefinitionCombatAffinitys.CombatAffinityEagerForBattle,
                    false, EquipmentDefinitions.KnowledgeAffinity.ActiveAndHidden),
                ItemPropertyDescriptionsContext.BuildFrom(FeatureDefinitionPerceptionAffinityBuilder
                    .Create("PerceptionAffinityHelmOfAwareness")
                    .SetGuiPresentationNoContent()
                    .CannotBeSurprised()
                    .AddToDB(), false, EquipmentDefinitions.KnowledgeAffinity.ActiveAndHidden)
            )
            .AddToDB();
        
        

        MerchantContext.AddItem(item, ShopItemType.MagicItemMinor);
        return item;
    }
}
