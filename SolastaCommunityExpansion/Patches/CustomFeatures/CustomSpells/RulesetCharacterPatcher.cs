using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells;

// Allows valid Somatic component if specific material component is held in main hand or off hand slots
[HarmonyPatch(typeof(RulesetCharacter), "IsComponentSomaticValid")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_IsComponentSomaticValid
{
    internal static void Postfix(RulesetCharacter __instance, ref bool __result,
        SpellDefinition spellDefinition, ref string failure)
    {
        if (__result || spellDefinition.MaterialComponentType != RuleDefinitions.MaterialComponentType.Specific)
        {
            return;
        }

        var materialTag = spellDefinition.SpecificMaterialComponentTag;
        var inventorySlotsByName = __instance.CharacterInventory.InventorySlotsByName;
        var mainHand = inventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand].EquipedItem;
        var offHand = inventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand].EquipedItem;
        var tagsMap = new Dictionary<string, TagsDefinitions.Criticity>();

        mainHand?.FillTags(tagsMap, __instance, true);
        offHand?.FillTags(tagsMap, __instance, true);

        if (!tagsMap.Keys.Contains(materialTag))
        {
            return;
        }

        __result = true;
        failure = string.Empty;
    }
}

// Allows spells to satisfy specific material components by actual active tags on an item that are not directly defined in ItemDefinition (like "Melee")
[HarmonyPatch(typeof(RulesetCharacter), "IsComponentMaterialValid")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_IsComponentMaterialValid
{
    internal static void Postfix(RulesetCharacter __instance, ref bool __result,
        SpellDefinition spellDefinition, ref string failure)
    {
        if (__result || spellDefinition.MaterialComponentType != RuleDefinitions.MaterialComponentType.Specific)
        {
            return;
        }

        var materialTag = spellDefinition.SpecificMaterialComponentTag;
        var requiredCost = spellDefinition.SpecificMaterialComponentCostGp;

        List<RulesetItem> items = new();
        __instance.CharacterInventory.EnumerateAllItems(items);
        var tagsMap = new Dictionary<string, TagsDefinitions.Criticity>();
        foreach (var rulesetItem in items)
        {
            tagsMap.Clear();
            rulesetItem.FillTags(tagsMap, __instance, true);
            var itemItemDefinition = rulesetItem.ItemDefinition;
            var costInGold = EquipmentDefinitions.GetApproximateCostInGold(itemItemDefinition.Costs);
            if (tagsMap.Keys.Contains(materialTag) && costInGold >= requiredCost)
            {
                __result = true;
                failure = String.Empty;
            }
        }
    }
}

//Modifies validity of ready cantrip action to include attack cantrips even if they don't have damage forms
[HarmonyPatch(typeof(RulesetCharacter), "IsValidReadyCantrip")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_IsValidReadyCantrip
{
    internal static void Postfix(RulesetCharacter __instance, ref bool __result,
        SpellDefinition cantrip)
    {
        if (__result)
        {
            return;
        }

        var effect = CustomFeaturesContext.ModifySpellEffect(cantrip, __instance);
        var hasDamage = effect.HasFormOfType(EffectForm.EffectFormType.Damage);
        var hasAttack = cantrip.HasSubFeatureOfType<IPerformAttackAfterMagicEffectUse>();
        var notGadgets = effect.TargetFilteringMethod != RuleDefinitions.TargetFilteringMethod.GadgetOnly;
        var componentsValid = __instance.AreSpellComponentsValid(cantrip);

        __result = (hasDamage || hasAttack) && notGadgets && componentsValid;
    }
}
