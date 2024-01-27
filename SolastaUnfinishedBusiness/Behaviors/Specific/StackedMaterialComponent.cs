using System;
using System.Collections.Generic;
using System.Linq;

namespace SolastaUnfinishedBusiness.Behaviors.Specific;

/// <summary>
///     Allow spells that require consumption of a material component (e.g. a gem of value >= 1000gp) use a stack
///     of lesser value components (e.g. 4 x 300gp diamonds).
///     Note that this implementation will only work with identical components - e.g. 'all diamonds', it won't consider
///     combining different types of items with the tag 'gem'.
/// </summary>
internal static class StackedMaterialComponent
{
    internal static void IsComponentMaterialValid(
        RulesetCharacter character,
        SpellDefinition spellDefinition,
        ref string failure,
        ref bool result)
    {
        if (!Main.Settings.AllowStackedMaterialComponent)
        {
            return;
        }

        if (result)
        {
            return;
        }

        // Repeats the last section of the original method but adds 'approximateCostInGold * item.StackCount'
        var items = new List<RulesetItem>();

        character.CharacterInventory.EnumerateAllItems(items);

        if (!items.Any(item => item.ItemDefinition.ItemTags.Contains(spellDefinition.SpecificMaterialComponentTag) &&
                               EquipmentDefinitions.GetApproximateCostInGold(item.ItemDefinition.Costs) *
                               item.StackCount >= spellDefinition.SpecificMaterialComponentCostGp))
        {
            return;
        }

        result = true;
        failure = string.Empty;
    }

    //Modify original code to spend enough of a stack to meet component cost
    internal static bool SpendSpellMaterialComponentAsNeeded(RulesetCharacter character, RulesetEffectSpell activeSpell)
    {
        if (!Main.Settings.AllowStackedMaterialComponent)
        {
            return true;
        }

        var spell = activeSpell.SpellDefinition;

        if (spell.MaterialComponentType != RuleDefinitions.MaterialComponentType.Specific
            || !spell.SpecificMaterialComponentConsumed
            || string.IsNullOrEmpty(spell.SpecificMaterialComponentTag)
            || spell.SpecificMaterialComponentCostGp <= 0
            || character.CharacterInventory == null)
        {
            return false;
        }

        var items = new List<RulesetItem>();

        character.CharacterInventory.EnumerateAllItems(items);

        var itemToUse = items
            .Where(item => item.ItemDefinition.ItemTags.Contains(spell.SpecificMaterialComponentTag))
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
                StackCountRequired = (int)Math.Ceiling(spell.SpecificMaterialComponentCostGp / (double)item.Cost)
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
            // ReSharper disable once InvocationIsSkipped
            Main.Log("Didn't find item.");

            return false;
        }

        // ReSharper disable once InvocationIsSkipped
        Main.Log($"Spending stack={itemToUse.StackCountRequired}, cost={itemToUse.TotalCost}");

        var componentConsumed = character.SpellComponentConsumed;

        if (componentConsumed != null)
        {
            for (var i = 0; i < itemToUse.StackCountRequired; i++)
            {
                componentConsumed(character, spell, itemToUse.RulesetItem);
            }
        }

        var rulesetItem = itemToUse.RulesetItem;

        if (rulesetItem.ItemDefinition.CanBeStacked && rulesetItem.StackCount > 1 &&
            itemToUse.StackCountRequired < rulesetItem.StackCount)
        {
            // ReSharper disable once InvocationIsSkipped
            Main.Log($"Spending stack={itemToUse.StackCountRequired}, cost={itemToUse.TotalCost}");

            rulesetItem.SpendStack(itemToUse.StackCountRequired);
        }
        else
        {
            // ReSharper disable once InvocationIsSkipped
            Main.Log("Destroy item");

            character.CharacterInventory.DestroyItem(rulesetItem);
        }

        return false;
    }
}
