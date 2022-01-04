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
            ServiceRepository.GetService<IRuntimeService>().RuntimeLoaded += (runtime) =>
            {
                AdditionalNamesContext.Load();
                AsiAndFeatContext.Load();
                BugFixContext.Load();
                CharacterExportContext.Load();
                ClassesContext.Load();
                ConjurationsContext.Load();
                DruidArmorContext.Load();
                DungeonMakerContext.Load();
                EncountersSpawnContext.Load();
                EpicArrayContext.Load();
                FaceUnlockContext.Load();
                // fighting Styles must be loaded before feats to allow feats to generate corresponding fighting style ones
                FightingStyleContext.Load(); 
                FeatsContext.Load();
                FlexibleBackgroundsContext.Switch();
                FlexibleRacesContext.Switch();
                InitialChoicesContext.Load();
                GameUiContext.Load();
                InventoryManagementContext.Load();
                ItemCraftingContext.Load();
                ItemOptionsContext.Load();
                Level20Context.Load();
                PickPocketContext.Load();
                PowersContext.Load();
                RemoveBugVisualModelsContext.Load();
                RemoveIdentificationContext.Load();
                RespecContext.Load();
                SrdAndHouseRulesContext.Load();
                SpellsContext.Load();
                SubclassesContext.Load();
                TelemaCampaignContext.Load();
                TeleporterContext.Load();
                VisionContext.Load();
                // recache must be loaded at the very last step
                GuiWrapperContext.Recache();

                Main.Enabled = true;
            };
        }
    }
}
