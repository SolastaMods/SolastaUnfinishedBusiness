using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Validators;

namespace SolastaUnfinishedBusiness.BehaviorsSpecific;

internal class ReturningWeapon
{
    private const string ActivateReturningFormat = "Feedback/&ReturningWeaponActivates";
    private const string TagReturningWeapon = "ReturningWeapon";

    internal static readonly ReturningWeapon AlwaysValid = new(ValidatorsWeapon.AlwaysValid);
    private readonly IsWeaponValidHandler _isWeaponValidHandler;

    internal ReturningWeapon(IsWeaponValidHandler isWeaponValidHandler)
    {
        _isWeaponValidHandler = isWeaponValidHandler;
    }

    internal static RuleDefinitions.AttackProximity Process(
        RulesetCharacterHero hero,
        RulesetAttackMode mode,
        RuleDefinitions.AttackProximity proximity)
    {
        if (proximity != RuleDefinitions.AttackProximity.Range || !mode.Thrown)
        {
            return proximity;
        }

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

        if (droppedItem == null)
        {
            return proximity;
        }

        var isWeaponValid = droppedItem.GetSubFeaturesByType<ReturningWeapon>().Aggregate(
            false,
            (current, returningWeapon) => current || returningWeapon._isWeaponValidHandler(mode, null, hero));

        isWeaponValid = hero.GetSubFeaturesByType<ReturningWeapon>().Aggregate(
            isWeaponValid,
            (current, returningWeapon) => current || returningWeapon._isWeaponValidHandler(mode, null, hero));

        if (!isWeaponValid)
        {
            return proximity;
        }

        proximity = RuleDefinitions.AttackProximity.Melee;

        hero.LogCharacterActivatesAbility(droppedItem.ItemDefinition.GuiPresentation.Title,
            ActivateReturningFormat, tooltipClass: "ItemDefinition", tooltipContent: droppedItem.Name);

        return proximity;
    }

    internal static void AddReturningWeaponTag(RulesetItem item, Dictionary<string, TagsDefinitions.Criticity> tags)
    {
        if (item.HasSubFeatureOfType<ReturningWeapon>())
        {
            tags.TryAdd(TagReturningWeapon, TagsDefinitions.Criticity.Normal);
        }
    }
}
