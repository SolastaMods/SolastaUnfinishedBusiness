using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Api.Extensions;

public static class RulesetCharacterHeroExtensions
{
    public static RulesetAttackMode RefreshAttackModePublic(
        [NotNull] this RulesetCharacterHero instance,
        ActionDefinitions.ActionType actionType,
        ItemDefinition itemDefinition,
        WeaponDescription weaponDescription,
        bool freeOffHand,
        bool canAddAbilityDamageBonus,
        string slotName,
        List<IAttackModificationProvider> attackModifiers,
        Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin,
        [CanBeNull] RulesetItem weapon = null)
    {
        var attackMode = instance.RefreshAttackMode(actionType, itemDefinition, weaponDescription,
            freeOffHand, canAddAbilityDamageBonus, slotName, attackModifiers, featuresOrigin, weapon);

        return attackMode;
    }

    [NotNull]
    public static List<(string, T)> GetTaggedFeaturesByType<T>([NotNull] this RulesetCharacterHero hero) where T : class
    {
        var list = new List<(string, T)>();

        foreach (var pair in hero.ActiveFeatures)
        {
            list.AddRange(GetTaggedFeatures<T>(pair.Key, pair.Value));
        }

        return list;
    }

    [NotNull]
    private static IEnumerable<(string, T)> GetTaggedFeatures<T>(
        string tag,
        [NotNull] IEnumerable<FeatureDefinition> features)
        where T : class
    {
        var list = new List<(string, T)>();
        foreach (var feature in features)
        {
            switch (feature)
            {
                case FeatureDefinitionFeatureSet {Mode: FeatureDefinitionFeatureSet.FeatureSetMode.Union} set:
                    list.AddRange(GetTaggedFeatures<T>(tag, set.FeatureSet));
                    break;

                case T typedFeature:
                    list.Add((tag, typedFeature));
                    break;
            }
        }

        return list;
    }

    public static bool IsWearingLightArmor([NotNull] this RulesetCharacterHero hero)
    {
        var equipedItem = hero.characterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeTorso].EquipedItem;

        if (equipedItem == null || !equipedItem.ItemDefinition.IsArmor)
        {
            return false;
        }

        var armorDescription = equipedItem.ItemDefinition.ArmorDescription;
        var element = DatabaseRepository.GetDatabase<ArmorTypeDefinition>().GetElement(armorDescription.ArmorType);

        return DatabaseRepository.GetDatabase<ArmorCategoryDefinition>()
            .GetElement(element.ArmorCategory).IsPhysicalArmor
               && element.ArmorCategory == EquipmentDefinitions.LightArmorCategory;
    }

    public static bool IsWearingMediumArmor([NotNull] this RulesetCharacterHero hero)
    {
        var equipedItem = hero.characterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeTorso].EquipedItem;

        if (equipedItem == null || !equipedItem.ItemDefinition.IsArmor)
        {
            return false;
        }

        var armorDescription = equipedItem.ItemDefinition.ArmorDescription;
        var element = DatabaseRepository.GetDatabase<ArmorTypeDefinition>().GetElement(armorDescription.ArmorType);

        return DatabaseRepository.GetDatabase<ArmorCategoryDefinition>()
            .GetElement(element.ArmorCategory).IsPhysicalArmor
               && element.ArmorCategory == EquipmentDefinitions.MediumArmorCategory;
    }

    public static bool IsWieldingTwoHandedWeapon([NotNull] this RulesetCharacterHero hero)
    {
        var equipedItem = hero.characterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand].EquipedItem;

        if (equipedItem != null && equipedItem.ItemDefinition.IsWeapon)
        {
            return equipedItem.ItemDefinition.activeTags.Contains("TwoHanded");
        }

        return false;
    }
}
