using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameManager_BindPostDatabase
    {
        internal static void Postfix()
        {
            GuiPresentationCheck.PostDatabaseLoadCheck();

            AdditionalNamesContext.Load();
            AsiAndFeatContext.Load();
            BugFixContext.Load();
            CharacterExportContext.Load();
            ConjurationsContext.Load();
            DungeonMakerContext.Load();
            DmProEditorContext.Load();
            EpicArrayContext.Load();
            FaceUnlockContext.Load();
            // Fighting Styles must be loaded before feats to allow feats to generate corresponding fighting style ones.
            FightingStyleContext.Load();
            FlexibleBackgroundsContext.Switch();
            InitialChoicesContext.Load();
            GameUiContext.Load();
            InventoryManagementContext.Load();
            ItemCraftingContext.Load();
            ItemOptionsContext.Load();
            Level20Context.Load();
            PickPocketContext.Load();
            // Powers needs to be added to db before spells because of summoned creatures that have new powers defined here.
            PowersContext.AddToDB();
            RemoveBugVisualModelsContext.Load();
            RemoveIdentificationContext.Load();
            LevelDownContext.Load();
            RespecContext.Load();
            // There are spells that rely on new monster definitions with powers loaded during the PowersContext. So spells should get added to db after powers.
            SpellsContext.AddToDB();
            SrdAndHouseRulesContext.Load();
            TelemaCampaignContext.Load();
            VisionContext.Load();

            // Classes may rely on spells and powers being in the DB before they can properly load.
            ClassesContext.Load();
            // Subclasses may rely on classes being loaded (as well as spells and powers) in order to properly refer back to the class.
            SubclassesContext.Load();

            ServiceRepository.GetService<IRuntimeService>().RuntimeLoaded += (_) =>
            {
                FlexibleRacesContext.Switch();
                InitialChoicesContext.RefreshFirstLevelTotalFeats();

                // There are feats that need all character classes loaded before they can properly be setup.
                FeatsContext.Load();
                // Generally available powers need all classes in the db before they are initialized here.
                PowersContext.Switch();
                // Spells context needs character classes (specifically spell lists) in the db in order to do it's work.
                SpellsContext.Load();

                // the spells cache is required by both level down or multiclass
                Multiclass.Models.CacheSpellsContext.Load();

                GuiPresentationCheck.PostCELoadCheck();

                GuiWrapperContext.Recache();

                if (Main.Settings.EnableMulticlass)
                {
                    Multiclass.Models.IntegrationContext.Load();
                    Multiclass.Models.InspectionPanelContext.Load();
                    Multiclass.Models.LevelUpContext.Load();
                    Multiclass.Models.SharedSpellsContext.Load();
                }

                Main.Enabled = true;
            };
        }
    }
}
