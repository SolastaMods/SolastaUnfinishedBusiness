using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.FightingStyles;

namespace SolastaUnfinishedBusiness.Models;

internal static class FightingStyleContext
{
    private static Dictionary<FightingStyleDefinition, List<FeatureDefinitionFightingStyleChoice>>
        FightingStylesChoiceList { get; } = new();

    internal static HashSet<FightingStyleDefinition> FightingStyles { get; private set; } = new();

    internal static void Load()
    {
        LoadStyle(new BlindFighting());
        LoadStyle(new Crippling());
        LoadStyle(new Executioner());
        LoadStyle(new HandAndAHalf());
        LoadStyle(new Merciless());
        LoadStyle(new PolearmExpert());
        LoadStyle(new Pugilist());
        LoadStyle(new Sentinel());
        LoadStyle(new ShieldExpert());
        LoadStyle(new Torchbearer());

        // sorting
        FightingStyles = FightingStyles.OrderBy(x => x.FormatTitle()).ToHashSet();

        // settings paring
        foreach (var name in Main.Settings.FightingStyleEnabled
                     .Where(name => FightingStyles.All(x => x.Name != name))
                     .ToList())
        {
            Main.Settings.FightingStyleEnabled.Remove(name);
        }
    }

    private static void LoadStyle([NotNull] AbstractFightingStyle styleBuilder)
    {
        var style = styleBuilder.FightingStyle;

        if (!FightingStyles.Contains(style))
        {
            FightingStylesChoiceList.Add(style, styleBuilder.FightingStyleChoice);
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
            var isActive = trainedFightingStyle.contentPack == CeContentPackContext.CeContentPack;

            if (isActive)
            {
                hero.activeFightingStyles.TryAdd(trainedFightingStyle);

                continue;
            }

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (trainedFightingStyle.Condition)
            {
                // Make hand crossbows benefit from Archery Fighting Style
                case FightingStyleDefinition.TriggerCondition.RangedWeaponAttack:
                {
                    bool HasHandXbowInHands(string slotName)
                    {
                        var rulesetInventorySlot =
                            hero.CharacterInventory.InventorySlotsByName[slotName];

                        return rulesetInventorySlot.EquipedItem != null
                               && rulesetInventorySlot.EquipedItem.ItemDefinition.IsWeapon
                               && rulesetInventorySlot.EquipedItem.ItemDefinition.WeaponDescription.WeaponType ==
                               CustomWeaponsContext.CeHandXbowType;
                    }

                    isActive = HasHandXbowInHands(EquipmentDefinitions.SlotTypeMainHand) ||
                               HasHandXbowInHands(EquipmentDefinitions.SlotTypeOffHand);
                    break;
                }

                case FightingStyleDefinition.TriggerCondition.TwoMeleeWeaponsWielded:
                {
                    // Make Shield Expert benefit from Two Weapon Fighting Style
                    var hasShieldExpert = hero.TrainedFeats.Any(x =>
                                              x.Name.Contains(ShieldExpert.ShieldExpertName)) ||
                                          hero.TrainedFightingStyles.Any(x =>
                                              x.Name.Contains(ShieldExpert.ShieldExpertName));

                    var mainHandSlot =
                        hero.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand];
                    var offHandSlot =
                        hero.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand];

                    if (hasShieldExpert
                        && mainHandSlot.EquipedItem != null
                        && mainHandSlot.EquipedItem.ItemDefinition.IsWeapon)
                    {
                        var weaponType = mainHandSlot.EquipedItem.ItemDefinition.WeaponDescription.WeaponType;

                        if (DatabaseHelper.GetDefinition<WeaponTypeDefinition>(weaponType).WeaponProximity ==
                            RuleDefinitions.AttackProximity.Melee
                            && offHandSlot.EquipedItem != null
                            && offHandSlot.EquipedItem.ItemDefinition.IsArmor)
                        {
                            isActive = true;
                        }
                    }

                    // Make One Handed Crossbow not benefit from Two Weapon Fighting Style
                    if (mainHandSlot.EquipedItem != null && ValidatorsWeapon.IsRanged(mainHandSlot.EquipedItem) &&
                        ValidatorsWeapon.IsOneHanded(mainHandSlot.EquipedItem))
                    {
                        isActive = false;
                    }

                    if (offHandSlot.EquipedItem != null && ValidatorsWeapon.IsRanged(offHandSlot.EquipedItem) &&
                        ValidatorsWeapon.IsOneHanded(offHandSlot.EquipedItem))
                    {
                        isActive = false;
                    }

                    break;
                }
            }

            if (isActive)
            {
                hero.activeFightingStyles.TryAdd(trainedFightingStyle);
            }
        }
    }
}
