using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    internal static class GameManagerPatcher
    {
        [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class GameManager_BindPostDatabase_Patch
        {
            internal static void Postfix()
            {
                Models.BugFixContext.Load();

                Models.AdditionalNamesContext.Load();
                Models.AsiAndFeatContext.Load();
                Models.InitialChoicesContext.Load();
                Models.ItemCraftingContext.Load();
                Models.GameUiContext.Load();
                // Fighting Styles should be loaded before feats in
                // order to generate feats of new fighting styles.
                Models.FightingStyleContext.Load();
                Models.FeatsContext.Load();
                Models.SubclassesContext.Load();
                Models.FlexibleBackgroundsContext.Load();
                Models.FlexibleRacesContext.Load();
                Models.VisionContext.Load();
                Models.PickPocketContext.Load();
                Models.EpicArrayContext.Load();
                Models.RespecContext.Load();
                Models.RemoveIdentificationContext.Load();
                Models.Level20Context.Load();
                Models.DruidArmorContext.Load();
                Models.CharacterExportContext.Load();
                Models.InventoryManagementContext.Load();
                Models.RemoveBugVisualModelsContext.Load();

                Main.Enabled = true;
            }
        }
    }
}
