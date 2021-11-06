using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    internal static class GameManagerPatcher
    {
        [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
        internal static class GameManager_BindPostDatabase_Patch
        {
            internal static void Postfix()
            {
                Models.AdditionalNamesContext.Load();
                Models.AsiAndFeatContext.Load();
                Models.InitialChoicesContext.Load();
                Models.ItemCraftingContext.Load();
                Models.GameUiContext.Load();
                Models.FeatsContext.Load();
                Models.SubclassesContext.Load();
                Models.FlexibleBackgroundsContext.Load();
                Models.FlexibleRacesContext.Load();
                Models.VisionContext.Load();
                Models.PickPocketContext.Load();
                Models.EpicArrayContext.Load();
                Models.RemoveIdentificationContext.Load();

                Main.Enabled = true;
            }
        }
    }
}
