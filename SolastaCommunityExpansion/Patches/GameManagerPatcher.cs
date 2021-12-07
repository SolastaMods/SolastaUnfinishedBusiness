using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Feats;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Subclasses.Rogue;
using SolastaCommunityExpansion.Subclasses.Wizard;

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
                BugFixContext.Load();
                AdditionalNamesContext.Load();
                AsiAndFeatContext.Load();
                InitialChoicesContext.Load();
                ItemCraftingContext.Load();
                GameUiContext.Load();
                // Fighting Styles should be loaded before feats in
                // order to generate feats of new fighting styles.
                FightingStyleContext.Load();
                FeatsContext.Load();
                SubclassesContext.Load();
                FlexibleBackgroundsContext.Load();
                FlexibleRacesContext.Load();
                VisionContext.Load();
                PickPocketContext.Load();
                EpicArrayContext.Load();
                RespecContext.Load();
                RemoveIdentificationContext.Load();
                Level20Context.Load();
                DruidArmorContext.Load();
                CharacterExportContext.Load();
                InventoryManagementContext.Load();
                RemoveBugVisualModelsContext.Load();
                SrdAndHouseRulesContext.Load();

                Main.Enabled = true;
            }
        }
    }
}
