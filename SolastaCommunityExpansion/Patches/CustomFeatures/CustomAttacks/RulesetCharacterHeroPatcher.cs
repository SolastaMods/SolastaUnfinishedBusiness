using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomAttacks;

// Allows changing what attribute is used for weapon's attack and damage rolls
[HarmonyPatch(typeof(RulesetCharacterHero), "ComputeAttackModeAbilityScoreReplacement")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacterHero_ComputeAttackModeAbilityScoreReplacement
{
    internal static void Prefix(RulesetCharacterHero __instance, RulesetAttackMode attackMode, RulesetItem weapon)
    {
        var attributeModifiers = __instance.GetSubFeaturesByType<IModifyAttackAttributeForWeapon>();

        foreach (var modifier in attributeModifiers)
        {
            modifier.ModifyAttribute(__instance, attackMode, weapon);
        }
    }
}

// Allows changing damage and other stats of an attack mode
[HarmonyPatch(typeof(RulesetCharacterHero), "RefreshAttackMode")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacterHero_RefreshAttackMode
{
    internal static void Postfix(RulesetCharacterHero __instance,
        ref RulesetAttackMode __result,
        ActionDefinitions.ActionType actionType,
        ItemDefinition itemDefinition,
        WeaponDescription weaponDescription,
        bool freeOffHand,
        bool canAddAbilityDamageBonus,
        string slotName,
        List<IAttackModificationProvider> attackModifiers,
        Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin,
        RulesetItem weapon = null)
    {
        var attributeModifiers = __instance.GetSubFeaturesByType<IModifyAttackModeForWeapon>();

        foreach (var modifier in attributeModifiers)
        {
            modifier.ModifyAttackMode(__instance, __result, weapon);
        }
    }
}

// Allows adding extra attack modes
[HarmonyPatch(typeof(RulesetCharacterHero), "RefreshAttackModes")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacterHero_RefreshAttackModes
{
    private static bool _callRefresh;

    internal static void Prefix(RulesetCharacterHero __instance, ref bool callRefresh)
    {
        _callRefresh = callRefresh;
        callRefresh = false;
    }

    internal static void Postfix(RulesetCharacterHero __instance, bool callRefresh = false)
    {
        var providers = __instance.GetSubFeaturesByType<IAddExtraAttack>();

        foreach (var provider in providers)
        {
            provider.TryAddExtraAttack(__instance);
        }

        if (!_callRefresh || __instance.CharacterRefreshed == null)
        {
            return;
        }

        __instance.CharacterRefreshed(__instance);
    }
}

// Support for `IHeroRefreshedListener`
[HarmonyPatch(typeof(RulesetCharacterHero), "RefreshAll")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacterHero_RefreshAll
{
    internal static void Prefix(RulesetCharacterHero __instance)
    {
        var listeners = __instance.GetSubFeaturesByType<IHeroRefreshedListener>();
        if (listeners == null)
        {
            return;
        }

        foreach (var listener in listeners)
        {
            listener.OnHeroRefreshed(__instance);
        }
    }
}

[HarmonyPatch(typeof(RulesetCharacterHero), "RefreshActiveFightingStyles")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacterHero_RefreshActiveFightingStyles
{
    internal static void Postfix(RulesetCharacterHero __instance)
    {
        foreach (var trainedFightingStyle in __instance.trainedFightingStyles
                     .Where(x => x.Condition == FightingStyleDefinition.TriggerCondition.RangedWeaponAttack))
        {
            switch (trainedFightingStyle.Condition)
            {
                // Make hand crossbows benefit from Archery Fighting Style
                case FightingStyleDefinition.TriggerCondition.RangedWeaponAttack:
                    var rulesetInventorySlot =
                        __instance.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand];

                    if (rulesetInventorySlot.EquipedItem != null
                        && rulesetInventorySlot.EquipedItem.ItemDefinition.IsWeapon
                        && rulesetInventorySlot.EquipedItem.ItemDefinition.WeaponDescription.WeaponType ==
                        "CEHandXbowType")
                    {
                        __instance.ActiveFightingStyles.Add(trainedFightingStyle);
                    }

                    break;

                // Make Shield Expert benefit from Two Weapon Fighting Style
                case FightingStyleDefinition.TriggerCondition.TwoMeleeWeaponsWielded:
                    var hasShieldExpert = __instance.TrainedFeats.Any(x => x.Name == "FeatShieldExpert");
                    var mainHandSlot =
                        __instance.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand];
                    var offHandSlot =
                        __instance.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand];

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
                            __instance.ActiveFightingStyles.Add(trainedFightingStyle);
                        }
                    }

                    break;
            }
        }
    }
}
