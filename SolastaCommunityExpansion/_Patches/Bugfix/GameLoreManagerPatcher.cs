using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.BugFix
{
    [HarmonyPatch(typeof(GameLoreManager), "SerializeElements")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLoreManager_SerializeElements
    {
        // If a recipe can't be found in the database but was previously known, the serialization
        // puts a null in the list of known recipes. Since everything that uses the list assumes
        // every entry is a valid item that causes major issues. To prevent that remove null entries
        // from the list of known recipes.
        internal static void Postfix(GameLoreManager __instance)
        {
            if (!Main.Settings.BugFixNullRecipesOnGameSerialization)
            {
                return;
            }

            __instance.KnownRecipes.RemoveAll(item => item == null);
        }
    }
}
