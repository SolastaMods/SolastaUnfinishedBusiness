using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.StackedMaterialComponent;

/// <summary>
///     Allow spells that require consumption of a material component (e.g. a gem of value >= 1000gp) use a stack
///     of lesser value components (e.g. 4 x 300gp diamonds).
///     Note that this implementation will only work with identical components - e.g. 'all diamonds', it won't consider
///     combining
///     different types of items with the tag 'gem'.
///     TODO: if anyone requests it we can improve with GroupBy etc...
/// </summary>
[HarmonyPatch(typeof(RulesetCharacter), "IsComponentMaterialValid")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_IsComponentMaterialValid
{
    public static void Postfix(RulesetCharacter __instance, SpellDefinition spellDefinition, ref string failure,
        ref bool __result)
    {
        if (!Main.Settings.AllowStackedMaterialComponent)
        {
            return;
        }

        if (__result)
        {
            return;
        }

        // Repeats the last section of the original method but adds 'approximateCostInGold * item.StackCount'
        var items = new List<RulesetItem>();

        __instance.CharacterInventory.EnumerateAllItems(items);

        foreach (var item in items)
        {
            var approximateCostInGold = EquipmentDefinitions.GetApproximateCostInGold(item.ItemDefinition.Costs);

            if (!item.ItemDefinition.ItemTags.Contains(spellDefinition.SpecificMaterialComponentTag) ||
                approximateCostInGold * item.StackCount < spellDefinition.SpecificMaterialComponentCostGp)
            {
                continue;
            }

            __result = true;

            failure = string.Empty;
        }
    }
}

[HarmonyPatch(typeof(RulesetCharacter), "SpendSpellMaterialComponentAsNeeded")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_SpendSpellMaterialComponentAsNeeded
{
    // Modify original code to spend enough of a stack to meet component cost
    public static bool Prefix(RulesetCharacter __instance, RulesetEffectSpell activeSpell)
    {
        if (!Main.Settings.AllowStackedMaterialComponent)
        {
            return true;
        }

        var spellDefinition = activeSpell.SpellDefinition;
        if (spellDefinition.MaterialComponentType != RuleDefinitions.MaterialComponentType.Specific
            || !spellDefinition.SpecificMaterialComponentConsumed
            || string.IsNullOrEmpty(spellDefinition.SpecificMaterialComponentTag)
            || spellDefinition.SpecificMaterialComponentCostGp <= 0
            || __instance.CharacterInventory == null)
        {
            return false;
        }

        var items = new List<RulesetItem>();

        __instance.CharacterInventory.EnumerateAllItems(items);

        var itemToUse = items
            .Where(item => item.ItemDefinition.ItemTags.Contains(spellDefinition.SpecificMaterialComponentTag))
            .Select(item => new
            {
                RulesetItem = item,
                // Note original code is "int cost = rulesetItem2.ItemDefinition.Costs[1];" which doesn't agree with IsComponentMaterialValid which
                // uses GetApproximateCostInGold
                Cost = EquipmentDefinitions.GetApproximateCostInGold(item.ItemDefinition.Costs)
            })
            .Select(item => new
            {
                item.RulesetItem,
                item.Cost,
                StackCountRequired =
                    (int)Math.Ceiling(spellDefinition.SpecificMaterialComponentCostGp / (double)item.Cost)
            })
            .Where(item => item.StackCountRequired <= item.RulesetItem.StackCount)
            .Select(item => new
            {
                item.RulesetItem,
                item.Cost,
                item.StackCountRequired,
                TotalCost = item.StackCountRequired * item.Cost
            })
            .Where(item => item.TotalCost >= activeSpell.SpellDefinition.SpecificMaterialComponentCostGp)
            .OrderBy(item => item.TotalCost) // min total cost used
            .ThenBy(item => item.StackCountRequired) // min items used
            .FirstOrDefault();

        if (itemToUse == null)
        {
            Main.Log("Didn't find item.");

            return false;
        }

        Main.Log($"Spending stack={itemToUse.StackCountRequired}, cost={itemToUse.TotalCost}");

        var componentConsumed = __instance.SpellComponentConsumed;

        if (componentConsumed != null)
        {
            for (var i = 0; i < itemToUse.StackCountRequired; i++)
            {
                componentConsumed(__instance, spellDefinition, itemToUse.RulesetItem);
            }
        }

        var rulesetItem = itemToUse.RulesetItem;

        if (rulesetItem.ItemDefinition.CanBeStacked && rulesetItem.StackCount > 1 &&
            itemToUse.StackCountRequired < rulesetItem.StackCount)
        {
            Main.Log($"Spending stack={itemToUse.StackCountRequired}, cost={itemToUse.TotalCost}");

            rulesetItem.SpendStack(itemToUse.StackCountRequired);
        }
        else
        {
            Main.Log("Destroy item");

            __instance.CharacterInventory.DestroyItem(rulesetItem);
        }

        return false;
    }
}
