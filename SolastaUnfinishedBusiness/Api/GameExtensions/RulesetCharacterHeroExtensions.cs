using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class RulesetCharacterHeroExtensions
{
#if false
    [NotNull]
    internal static List<(string, T)> GetTaggedFeaturesByType<T>([NotNull] this RulesetCharacterHero hero)
        where T : class
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
                case FeatureDefinitionFeatureSet { Mode: FeatureDefinitionFeatureSet.FeatureSetMode.Union } set:
                    list.AddRange(GetTaggedFeatures<T>(tag, set.FeatureSet));
                    break;

                case T typedFeature:
                    list.Add((tag, typedFeature));
                    break;
            }
        }

        return list;
    }

    internal static bool IsWearingLightArmor([NotNull] this RulesetCharacterHero hero)
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

    internal static bool IsWearingMediumArmor([NotNull] this RulesetCharacterHero hero)
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

    internal static bool IsWieldingTwoHandedWeapon([NotNull] this RulesetCharacterHero hero)
    {
        var equipedItem = hero.characterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand]
            .EquipedItem;

        if (equipedItem != null && equipedItem.ItemDefinition.IsWeapon)
        {
            return equipedItem.ItemDefinition.activeTags.Contains("TwoHanded");
        }

        return false;
    }
#endif

    internal static void GrantAcquiredSpellWithTagFromSubclassPool(this RulesetCharacterHero hero, string tag)
    {
        var heroBuildingData = hero.GetHeroBuildingData();
        var selectedClass = LevelUpHelper.GetSelectedClass(hero);
        var classLevel = LevelUpHelper.GetSelectedClassLevel(hero);
        // it's indeed TagClass as this is how spell pools are offered in vanilla when from subclass
        var poolName = $"{AttributeDefinitions.TagClass}{selectedClass!.Name}{classLevel}{tag}";

        if (!heroBuildingData.AcquiredSpells.TryGetValue(poolName, out var spells))
        {
            return;
        }

        var spellRepertoire = hero.GetClassSpellRepertoire(selectedClass);

        foreach (var spell in spells)
        {
            hero.GrantSpell(spell, spellRepertoire!.SpellCastingFeature);
        }
    }

    internal static bool HasEmptyMainHand([NotNull] this RulesetCharacterHero hero)
    {
        return hero.GetMainWeapon() == null;
    }

    internal static bool HasEmptyOffHand([NotNull] this RulesetCharacterHero hero)
    {
        return hero.GetOffhandWeapon() == null;
    }

    internal static int GetClassLevel(this RulesetCharacterHero hero, CharacterClassDefinition classDefinition)
    {
        return classDefinition && hero.ClassesAndLevels.TryGetValue(classDefinition, out var classLevel)
            ? classLevel
            : 0;
    }

    internal static int GetClassLevel(this RulesetCharacterHero hero, string className)
    {
        if (string.IsNullOrEmpty(className))
        {
            return 0;
        }

        return hero.GetClassLevel(DatabaseRepository.GetDatabase<CharacterClassDefinition>()
            .FirstOrDefault(x => x.Name == className));
    }

    [CanBeNull]
    internal static RulesetItem GetMainWeapon(this RulesetCharacterHero hero)
    {
        return hero.GetItemInSlot(EquipmentDefinitions.SlotTypeMainHand);
    }

    [CanBeNull]
    internal static RulesetItem GetOffhandWeapon(this RulesetCharacterHero hero)
    {
        return hero.GetItemInSlot(EquipmentDefinitions.SlotTypeOffHand);
    }

    internal static int GetAttunementLimit([CanBeNull] this RulesetCharacterHero hero)
    {
        var limit = 3;

        if (Main.Settings.IncreaseMaxAttunedItems && hero != null)
        {
            var characterLevel = hero.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);

            limit = characterLevel switch
            {
                >= 18 => 5,
                >= 10 => 4,
                _ => limit
            };
        }

        if (hero == null)
        {
            return limit;
        }

        var mods = hero.GetSubFeaturesByType<ModifyAttunementLimit>();

        limit += mods.Sum(mod => mod.Value);

        return limit;
    }

    internal static bool HasNonIdentifiedItems([CanBeNull] this RulesetCharacterHero hero)
    {
        if (hero == null)
        {
            return false;
        }

        hero.CharacterInventory.EnumerateAllItems(hero.Items);

        return hero.Items.Any(item => item.NeedsIdentification());
    }
}
