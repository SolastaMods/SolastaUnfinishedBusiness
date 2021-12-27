using HarmonyLib;
using SolastaCommunityExpansion.Models;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameManager_BindPostDatabase
    {
        internal static void Postfix()
        {
            PowersContext.Load();
            SpellsContext.Load();
            AdditionalNamesContext.Load();
            AsiAndFeatContext.Load();
            BugFixContext.Load();
            CharacterExportContext.Load();
            ConjurationsContext.Load();
            DruidArmorContext.Load();
            DungeonMakerContext.Load();
            EncountersSpawnContext.Load();
            EpicArrayContext.Load();
            FaceUnlockContext.Load();
            FightingStyleContext.Load(); // Fighting Styles should be loaded before feats in order to generate feats of new fighting styles
            FeatsContext.Load();
            FlexibleBackgroundsContext.Load();
            InitialChoicesContext.Load();
            GameUiContext.Load();
            InventoryManagementContext.Load();
            ItemCraftingContext.Load();
            ItemOptionsContext.Load();
            Level20Context.Load();
            PickPocketContext.Load();
            RemoveBugVisualModelsContext.Load();
            RemoveIdentificationContext.Load();
            RespecContext.Load();
            SrdAndHouseRulesContext.Load();
            SubclassesContext.Load();
            TelemaCampaignContext.Load();
            VisionContext.Load();
            ClassesContext.Load();

            Main.Enabled = true;
        }
    }
}
