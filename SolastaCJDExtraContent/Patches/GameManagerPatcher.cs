using HarmonyLib;

namespace SolastaCJDExtraContent.Patches
{
    internal static class GameManagerPatcher
    {
        [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
        internal static class GameManager_BindPostDatabase_Patch
        {
            internal static void Postfix()
            {
                Models.AsiAndFeatContext.Load();
                Models.InitialChoicesContext.Load();
                Models.ItemCraftingContext.Load();
                Models.GameUiContext.Load();
                Models.FeatsContext.Load();
            }
        }
    }
}
