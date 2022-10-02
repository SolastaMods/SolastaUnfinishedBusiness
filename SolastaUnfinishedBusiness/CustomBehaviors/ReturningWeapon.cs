using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class ReturningWeapon
{
    private const string ActiveteReturningFormat = "Feedback/&ReturningWeaponActivates";
    private const string TagReturningWeapon = "ReturningWeapon";

    private ReturningWeapon()
    {
    }

    public static ReturningWeapon Instance { get; } = new();

    public static RuleDefinitions.AttackProximity Process(RulesetCharacterHero hero, RulesetAttackMode mode,
        RuleDefinitions.AttackProximity proximity)
    {
        if (proximity != RuleDefinitions.AttackProximity.Range || !mode.Thrown) { return proximity; }

        var inventory = hero.characterInventory;
        var num = inventory.CurrentConfiguration;
        var configurations = inventory.WieldedItemsConfigurations;
        if (inventory.CurrentConfiguration == configurations.Count - 1)
        {
            num = configurations[num].MainHandSlot.ShadowedSlot != configurations[0].MainHandSlot
                ? 1
                : 0;
        }

        var itemCfg = configurations[num];
        RulesetItem droppedItem = null;
        if (mode.SlotName == EquipmentDefinitions.SlotTypeMainHand &&
            itemCfg.MainHandSlot.EquipedItem != null &&
            itemCfg.MainHandSlot.EquipedItem.ItemDefinition == mode.SourceDefinition)
        {
            droppedItem = itemCfg.MainHandSlot.EquipedItem;
        }
        else if (mode.SlotName == EquipmentDefinitions.SlotTypeOffHand &&
                 itemCfg.OffHandSlot.EquipedItem != null &&
                 itemCfg.OffHandSlot.EquipedItem.ItemDefinition == mode.SourceDefinition)
        {
            droppedItem = itemCfg.OffHandSlot.EquipedItem;
        }

        if (droppedItem == null) { return proximity; }

        if (droppedItem.HasSubFeatureOfType<ReturningWeapon>())
        {
            proximity = RuleDefinitions.AttackProximity.Melee;
            GameConsoleHelper.LogCharacterActivatesAbility(hero, droppedItem.ItemDefinition.GuiPresentation.Title,
                ActiveteReturningFormat, tooltipClass: "ItemDefinition", tooltipContent: droppedItem.Name);
        }

        return proximity;
    }

    public static void AddCustomTags(RulesetItem item, Dictionary<string, TagsDefinitions.Criticity> tags)
    {
        if (item.HasSubFeatureOfType<ReturningWeapon>())
        {
            tags.TryAdd(TagReturningWeapon, TagsDefinitions.Criticity.Normal);
        }
    }
}
