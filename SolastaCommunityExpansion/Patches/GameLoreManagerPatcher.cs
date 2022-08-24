using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Patches;

internal static class GameLoreManagerPatcher
{
    [HarmonyPatch(typeof(GameLoreManager), "SerializeElements")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SerializeElements_Patch
    {
        internal static void Postfix([NotNull] GameLoreManager __instance)
        {
            // PATCH: BUGFIX: null recipes on game serialization
            // If a recipe can't be found in the database but was previously known, the serialization
            // puts a null in the list of known recipes. Since everything that uses the list assumes
            // every entry is a valid item that causes major issues. To prevent that remove null entries
            // from the list of known recipes.
            __instance.KnownRecipes.RemoveAll(item => item == null);
        }
    }
}