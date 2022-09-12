using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.FightingStyles;

namespace SolastaUnfinishedBusiness.Models;

internal static class DisplayFightingStyleContext
{
    private static Dictionary<FightingStyleDefinition, List<FeatureDefinitionFightingStyleChoice>>
        FightingStylesChoiceList { get; } = new();

    internal static HashSet<FightingStyleDefinition> FightingStyles { get; private set; } = new();

    internal static void Load()
    {
        LoadStyle(new Crippling());
        LoadStyle(new Pugilist());
        LoadStyle(new TitanFighting());
        LoadStyle(new Merciless());

        FightingStyles = FightingStyles.OrderBy(x => x.FormatTitle()).ToHashSet();
    }

    private static void LoadStyle([NotNull] AbstractFightingStyle styleBuilder)
    {
        var style = styleBuilder.GetStyle();

        if (!FightingStyles.Contains(style))
        {
            FightingStylesChoiceList.Add(style, styleBuilder.GetChoiceLists());
            FightingStyles.Add(style);
        }

        UpdateStyleVisibility(style);
    }

    private static void UpdateStyleVisibility([NotNull] FightingStyleDefinition fightingStyleDefinition)
    {
        var name = fightingStyleDefinition.Name;
        var choiceLists = FightingStylesChoiceList[fightingStyleDefinition];

        foreach (var fightingStyles in choiceLists.Select(cl => cl.FightingStyles))
        {
            if (Main.Settings.FightingStyleEnabled.Contains(name))
            {
                fightingStyles.TryAdd(name);
            }
            else
            {
                fightingStyles.Remove(name);
            }
        }
    }

    internal static void Switch(FightingStyleDefinition fightingStyleDefinition, bool active)
    {
        if (!FightingStyles.Contains(fightingStyleDefinition))
        {
            return;
        }

        var name = fightingStyleDefinition.Name;

        if (active)
        {
            Main.Settings.FightingStyleEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.FightingStyleEnabled.Remove(name);
        }

        UpdateStyleVisibility(fightingStyleDefinition);
    }

    internal static void RefreshFightingStylesPatch(RulesetCharacterHero hero)
    {
        foreach (var trainedFightingStyle in hero.trainedFightingStyles)
        {
            bool? isActive = null;
            switch (trainedFightingStyle.Condition)
            {
                // Make hand crossbows benefit from Archery Fighting Style
                //TODO: check what happens with off-hand
                case FightingStyleDefinition.TriggerCondition.RangedWeaponAttack:
                    var rulesetInventorySlot =
                        hero.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand];

                    if (rulesetInventorySlot.EquipedItem != null
                        && rulesetInventorySlot.EquipedItem.ItemDefinition.IsWeapon
                        && rulesetInventorySlot.EquipedItem.ItemDefinition.WeaponDescription.WeaponType ==
                        "CEHandXbowType")
                    {
                        isActive = true;
                    }

                    break;

                // Make Shield Expert benefit from Two Weapon Fighting Style
                case FightingStyleDefinition.TriggerCondition.TwoMeleeWeaponsWielded:
                    var hasShieldExpert = hero.TrainedFeats.Any(x => x.Name == "FeatShieldExpert");
                    var mainHandSlot =
                        hero.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand];
                    var offHandSlot =
                        hero.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand];

                    if (hasShieldExpert
                        && mainHandSlot.EquipedItem != null
                        && mainHandSlot.EquipedItem.ItemDefinition.IsWeapon)
                    {
                        var dbWeaponTypeDefinition = DatabaseRepository.GetDatabase<WeaponTypeDefinition>();
                        var weaponType = mainHandSlot.EquipedItem.ItemDefinition.WeaponDescription.WeaponType;

                        if (dbWeaponTypeDefinition.GetElement(weaponType).WeaponProximity ==
                            RuleDefinitions.AttackProximity.Melee
                            && offHandSlot.EquipedItem != null
                            && offHandSlot.EquipedItem.ItemDefinition.IsArmor)
                        {
                            isActive = true;
                        }
                    }

                    break;
            }

            if (trainedFightingStyle is ICustomFightingStyle customFightingStyle)
            {
                isActive = customFightingStyle.IsActive(hero);
            }

            if (isActive == null)
            {
                continue;
            }

            if (isActive.Value)
            {
                hero.activeFightingStyles.TryAdd(trainedFightingStyle);
            }
            else
            {
                hero.activeFightingStyles.Remove(trainedFightingStyle);
            }
        }
    }
}
