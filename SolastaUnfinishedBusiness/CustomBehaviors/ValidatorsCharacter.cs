using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal delegate bool IsCharacterValidHandler(RulesetCharacter character);

internal static class ValidatorsCharacter
{
    internal static readonly IsCharacterValidHandler HasLightSourceOffHand = character =>
    {
        // required for wildshape scenarios
        if (character is not RulesetCharacterHero)
        {
            return false;
        }

        var offItem = character.GetOffhandWeapon();

        return offItem != null && offItem.ItemDefinition != null && offItem.ItemDefinition.IsLightSourceItem;
    };

    internal static readonly IsCharacterValidHandler HasFreeHand = character =>
        character.HasFreeHandSlot() && !ValidatorsWeapon.IsTwoHanded(character.GetMainWeapon());

    internal static readonly IsCharacterValidHandler HasAttacked = character => character.ExecutedAttacks > 0;

    internal static readonly IsCharacterValidHandler HasNoArmor = character => !character.IsWearingArmor();

    internal static readonly IsCharacterValidHandler HasNoShield = character => !character.IsWearingShield();

    internal static readonly IsCharacterValidHandler HasShield = character => character.IsWearingShield();

    internal static readonly IsCharacterValidHandler HasPolearm = character =>
        ValidatorsWeapon.IsPolearm(character.GetMainWeapon()) ||
        ValidatorsWeapon.IsPolearm(character.GetOffhandWeapon());

    internal static readonly IsCharacterValidHandler HasTwoHandedRangedWeapon = character =>
        ValidatorsWeapon.IsWeaponType(character.GetMainWeapon(),
            LongbowType, ShortbowType, HeavyCrossbowType, LightCrossbowType);

    internal static readonly IsCharacterValidHandler MainHandIsFinesseWeapon = character =>
        ValidatorsWeapon.HasAnyWeaponTag(character.GetMainWeapon(),
            TagsDefinitions.WeaponTagFinesse);

    internal static readonly IsCharacterValidHandler MainHandIsVersatileWeaponNoShield = character =>
        ValidatorsWeapon.HasAnyWeaponTag(character.GetMainWeapon(),
            TagsDefinitions.WeaponTagVersatile) && IsFreeOffhand(character);

    internal static readonly IsCharacterValidHandler MainHandIsMeleeWeapon = character =>
        ValidatorsWeapon.IsMelee(character.GetMainWeapon());

    internal static readonly IsCharacterValidHandler MainHandIsUnarmed = character =>
        ValidatorsWeapon.IsUnarmedWeapon(character.GetMainWeapon());

    internal static readonly IsCharacterValidHandler LightArmor = character =>
        HasArmorCategory(character, EquipmentDefinitions.LightArmorCategory);

    internal static readonly IsCharacterValidHandler HeavyArmor = character =>
        HasArmorCategory(character, EquipmentDefinitions.HeavyArmorCategory);

    internal static readonly IsCharacterValidHandler NotHeavyArmor = character =>
        !HasArmorCategory(character, EquipmentDefinitions.HeavyArmorCategory);
    // internal static readonly IsCharacterValidHandler EmptyOffhand = character =>
    //     character.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand].EquipedItem == null;

    internal static IsCharacterValidHandler HasAnyOfConditions(params string[] conditions)
    {
        return character => conditions.Any(character.HasConditionOfType);
    }

    internal static IsCharacterValidHandler HasNoneOfConditions(params string[] conditions)
    {
        return character => !conditions.Any(character.HasConditionOfType);
    }

    // does character has free offhand in TA's terms as used in RefreshAttackModes for bonus unarmed attack for Monk?
    internal static bool IsFreeOffhandForUnarmedTa(RulesetCharacter character)
    {
        var offHand = character.GetOffhandWeapon();

        return offHand == null || !offHand.ItemDefinition.IsWeapon;
    }

    internal static bool IsFreeOffhand(RulesetCharacter character)
    {
        return character.GetOffhandWeapon() == null;
    }

    internal static bool HasConditionWithSubFeatureOfType<T>(this RulesetCharacter character) where T : class
    {
        return character.conditionsByCategory
            .Any(keyValuePair => keyValuePair.Value
                .Any(rulesetCondition => rulesetCondition.ConditionDefinition.HasSubFeatureOfType<T>()));
    }

    private static bool HasArmorCategory(RulesetCharacter character, string category)
    {
        // required for wildshape scenarios
        if (character is not RulesetCharacterHero)
        {
            return false;
        }

        var equipedItem = character.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeTorso]
            .EquipedItem;

        if (equipedItem == null || !equipedItem.ItemDefinition.IsArmor)
        {
            return false;
        }

        var armorDescription = equipedItem.ItemDefinition.ArmorDescription;
        var element = DatabaseHelper.GetDefinition<ArmorTypeDefinition>(armorDescription.ArmorType);

        return DatabaseHelper.GetDefinition<ArmorCategoryDefinition>(element.ArmorCategory)
            .IsPhysicalArmor && element.ArmorCategory == category;
    }
}
