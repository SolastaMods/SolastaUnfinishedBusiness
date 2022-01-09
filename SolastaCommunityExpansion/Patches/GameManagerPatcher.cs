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
            // fighting Styles must be loaded before feats to allow feats to generate corresponding fighting style ones
            FightingStyleContext.Load();
            FlexibleBackgroundsContext.Switch();
            InitialChoicesContext.Load();
            GameUiContext.Load();
            InventoryManagementContext.Load();
            ItemCraftingContext.Load();
            ItemOptionsContext.Load();
            Level20Context.Load();
            PickPocketContext.Load();
            PowersContext.AddToDB();
            RemoveBugVisualModelsContext.Load();
            RemoveIdentificationContext.Load();
            RespecContext.Load();
            SpellsContext.AddToDB();
            SrdAndHouseRulesContext.Load();
            TelemaCampaignContext.Load();
            TeleporterContext.Load();
            VisionContext.Load();

            // Classes may rely on spells and powers being in the DB before they can properly load.
            ClassesContext.Load();
            // Subclasses may rely on classes being loaded in order to properly refer back to the class.
            SubclassesContext.Load();

            ServiceRepository.GetService<IRuntimeService>().RuntimeLoaded += (runtime) =>
            {
                FlexibleRacesContext.Switch();
                InitialChoicesContext.RefreshFirstLevelTotalFeats();

                // There are feats that need all character classes loaded before they can properly be setup.
                FeatsContext.Load();
                // Generally available powers need all clases in the db before they are initialized here.
                PowersContext.Switch();
                SpellsContext.Load();

                GuiWrapperContext.Recache();

                Main.Enabled = true;
            };
        }
    }
}
