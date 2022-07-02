using System.Linq;
using SolastaCommunityExpansion.Api.Extensions;

namespace SolastaCommunityExpansion.Models;

public delegate bool CharacterValidator(RulesetCharacter character);

public static class CharacterValidators
{
    public static readonly CharacterValidator HasAttacked = character => character.ExecutedAttacks > 0;
    public static readonly CharacterValidator NoArmor = character => !character.IsWearingArmor();
    public static readonly CharacterValidator NoShield = character => !character.IsWearingShield();
    public static readonly CharacterValidator HasShield = character => character.IsWearingShield();

    // public static readonly CharacterValidator EmptyOffhand = character =>
    //     character.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand].EquipedItem == null;

    public static readonly CharacterValidator HasPolearm = character =>
    {
        var slotsByName = character.CharacterInventory.InventorySlotsByName;
        return WeaponValidators.IsPolearm(slotsByName[EquipmentDefinitions.SlotTypeMainHand].EquipedItem)
               || WeaponValidators.IsPolearm(slotsByName[EquipmentDefinitions.SlotTypeOffHand].EquipedItem);
    };

    public static readonly CharacterValidator MainHandIsMeleeWeapon = character =>
        WeaponValidators.IsMelee(character.GetItemInSlot(EquipmentDefinitions.SlotTypeMainHand));

    // public static readonly CharacterValidator FullyUnarmed = character =>
    // {
    //     var slotsByName = character.CharacterInventory.InventorySlotsByName;
    //     return WeaponValidators.IsUnarmedWeapon(slotsByName[EquipmentDefinitions.SlotTypeMainHand].EquipedItem)
    //            && WeaponValidators.IsUnarmedWeapon(slotsByName[EquipmentDefinitions.SlotTypeOffHand].EquipedItem);
    // };

    public static readonly CharacterValidator HasUnarmedHand = character =>
    {
        var slotsByName = character.CharacterInventory.InventorySlotsByName;

        return WeaponValidators.IsUnarmedWeapon(slotsByName[EquipmentDefinitions.SlotTypeMainHand].EquipedItem)
               || WeaponValidators.IsUnarmedWeapon(slotsByName[EquipmentDefinitions.SlotTypeOffHand].EquipedItem);
    };

    // public static readonly CharacterValidator UsedAllMainAttacks = character =>
    //     character.ExecutedAttacks >= character.GetAttribute(AttributeDefinitions.AttacksNumber).CurrentValue;

    public static readonly CharacterValidator InBattle = _ =>
        ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress;

    public static CharacterValidator HasAnyOfConditions(params ConditionDefinition[] conditions)
    {
        return character => conditions.Any(c => character.HasConditionOfType(c.Name));
    }

    // public static CharacterValidator HasAnyOfConditions(params string[] conditions)
    // {
    //     return character => conditions.Any(character.HasConditionOfType);
    // }
}
