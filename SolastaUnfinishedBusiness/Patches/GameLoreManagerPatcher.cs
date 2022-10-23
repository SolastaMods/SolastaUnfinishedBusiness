using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

public static class GameLoreManagerPatcher
{
    [HarmonyPatch(typeof(GameLoreManager), "SerializeElements")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class SerializeElements_Patch
    {
        public static void Postfix([NotNull] GameLoreManager __instance)
        {
            //BUGFIX: avoid null recipes on game serialization
            // if a recipe can't be found in the database but was previously known, the serialization
            // puts a null in the list of known recipes. Since everything that uses the list assumes
            // every entry is a valid item that causes major issues. To prevent that remove null entries
            // from the list of known recipes.
            __instance.KnownRecipes.RemoveAll(item => item == null);
        }
    }
}
