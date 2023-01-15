using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RecipesByTooltypeLinePatcher
{
    [HarmonyPatch(typeof(RecipesByTooltypeLine), "Load")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Load_Patch
    {
        [UsedImplicitly]
        public static void Prefix(List<RecipeDefinition> recipes)
        {
            //PATCH: sort the recipes by crafted item title
            recipes.Sort((a, b) =>
                String.Compare(a.CraftedItem.FormatTitle(), b.CraftedItem.FormatTitle(),
                    StringComparison.CurrentCultureIgnoreCase));
        }
    }

    [HarmonyPatch(typeof(RecipesByTooltypeLine), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        [UsedImplicitly]
        public static void Prefix(ref List<RecipeDefinition> knownRecipes)
        {
            //PATCH: adds a filter to the crafting panel screen
            CraftingContext.FilterRecipes(ref knownRecipes);
        }
    }
}
