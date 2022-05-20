using System.Linq;

namespace SolastaCommunityExpansion.Features;

public delegate bool CharacterValidator(RulesetCharacter character);

public static class CharacterValidators
{
    public static readonly CharacterValidator NoArmor = character => !character.IsWearingArmor();

    public static readonly CharacterValidator NoShield = character => !character.IsWearingShield();

    public static readonly CharacterValidator EmptyOffhand = character =>
        character.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand].EquipedItem == null;

    public static readonly CharacterValidator UsedAllMainAttacks = character =>
        character.ExecutedAttacks >= character.GetAttribute(AttributeDefinitions.AttacksNumber).CurrentValue;

    public static readonly CharacterValidator InBattle = _ =>
        ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress;

    public static CharacterValidator HasAnyOfConditions(params ConditionDefinition[] conditions)
    {
        return character => conditions.Any(c => character.HasConditionOfType(c.Name));
    }
}