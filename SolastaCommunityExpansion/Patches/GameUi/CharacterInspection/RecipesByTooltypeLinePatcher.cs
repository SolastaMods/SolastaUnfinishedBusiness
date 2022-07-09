using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection;

// sort the recipes by crafted item title
[HarmonyPatch(typeof(RecipesByTooltypeLine), "Load")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RecipesByTooltypeLine_Load
{
    internal static void Prefix(List<RecipeDefinition> recipes)
    {
        recipes.Sort((a, b) =>
            a.CraftedItem.FormatTitle().CompareTo(b.CraftedItem.FormatTitle()));
    }
}

// adds a filter to the crafting panel screen
[HarmonyPatch(typeof(RecipesByTooltypeLine), "Refresh")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RecipesByTooltypeLine_Refresh
{
    internal static void Prefix(ref List<RecipeDefinition> knownRecipes)
    {
        ItemCraftingContext.FilterRecipes(ref knownRecipes);
    }
}
