using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.LoadFixers
{
    internal static class GameLoreManagerPatcher
    {
        [HarmonyPatch(typeof(GameLoreManager), "SerializeElements")]
        internal static class GameLoreManager_SerializeElements_Patch
        {
            // If a recipe can't be found in the database but was previously known, the serialization
            // puts a null in the list of known recipes. Since everything that uses the list assumes
            // every entry is a valid item that causes major issues. To prevent that remove null entries
            // from the list of known recipes.
            internal static void Postfix(GameLoreManager __instance)
            {
                __instance.KnownRecipes.RemoveAll(item => item == null);
            }
        }
    }
}