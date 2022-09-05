using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

internal static class RecipesByTooltypeLinePatcher
{
    //PATCH: sort the recipes by crafted item title
    [HarmonyPatch(typeof(RecipesByTooltypeLine), "Load")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Load_Patch
    {
        internal static void Prefix(List<RecipeDefinition> recipes)
        {
            recipes.Sort((a, b) =>
                String.Compare(a.CraftedItem.FormatTitle(), b.CraftedItem.FormatTitle(),
                    StringComparison.CurrentCultureIgnoreCase));
        }
    }

    //PATCH: adds a filter to the crafting panel screen
    [HarmonyPatch(typeof(RecipesByTooltypeLine), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Refresh_Patch
    {
        internal static void Prefix(ref List<RecipeDefinition> knownRecipes)
        {
            ItemCraftingContext.FilterRecipes(ref knownRecipes);
        }
    }
}
