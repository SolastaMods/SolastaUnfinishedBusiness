using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(RulesetCharacter), "IsComponentMaterialValid")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_IsComponentMaterialValid
    {
        public static void Postfix(RulesetCharacter __instance, SpellDefinition spellDefinition, ref string failure)
        {
            //this.CharacterInventory.EnumerateAllItems(this.items);
            //foreach (RulesetItem rulesetItem in this.items)
            //{
            //    int approximateCostInGold = EquipmentDefinitions.GetApproximateCostInGold(rulesetItem.ItemDefinition.Costs);
            //    if (rulesetItem.ItemDefinition.ItemTags.Contains(spellDefinition.SpecificMaterialComponentTag) && approximateCostInGold >= spellDefinition.SpecificMaterialComponentCostGp)
            //        return true;
            //}
            //this.items.Clear();

        }
    }
}
