using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaMulticlass.Models;
using UnityModManagerNet;
#if DEBUG
using SolastaCommunityExpansion.Patches.Diagnostic;
#endif

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameManager_BindPostDatabase
    {
        [System.Obsolete]
        internal static void Postfix()
        {
#if DEBUG
            ItemDefinitionVerification.Load();
            EffectFormVerification.Load();
#endif
            ResourceLocatorContext.Load();

            // Cache TA definitions for diagnostics and export
            DiagnosticsContext.CacheTADefinitions();

            // Needs to be after CacheTADefinitions
            CeContentPackContext.Load();

            AdditionalNamesContext.Load();
            AsiAndFeatContext.Load();
            BugFixContext.Load();
            CharacterExportContext.Load();
            ConjurationsContext.Load();
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

            MonsterContext.AddNewMonsters();

            ServiceRepository.GetService<IRuntimeService>().RuntimeLoaded += (_) =>
            {
                // Both are late initialized to allow feats and races from other mods
                FlexibleRacesContext.Switch();
                InitialChoicesContext.RefreshFirstLevelTotalFeats();

                // There are feats that need all character classes loaded before they can properly be setup.
                FeatsContext.Load();

                // Generally available powers need all classes in the db before they are initialized here.
                PowersContext.Switch();

                // Spells context needs character classes (specifically spell lists) in the db in order to do it's work.
                SpellsContext.Load();

                // Save by location initialization depends on services to be ready
                SaveByLocationContext.Load();

                // Recache all gui collections
                GuiWrapperContext.Recache();

                // Cache CE definitions for diagnostics and export
                DiagnosticsContext.CacheCEDefinitions();

                Main.Enabled = true;
                Main.Logger.Log("Enabled.");

                DisplayWelcomeMessage();
            };

            ServiceRepository.GetService<IRuntimeService>().RuntimeLoaded += (_) =>
            {
                // always load custom definitions even when multiclass is disabled to avoid errors parsing MC heroes during startup
                LevelUpContext.Load();

                if (!(Main.Settings.EnableMulticlass || Main.Settings.EnableLevelDown))
                {
                    return;
                }

                CacheSpellsContext.Load();

                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                IntegrationContext.Load();
                InspectionPanelContext.Load();
                LevelDownContext.Load();
                SharedSpellsContext.Load();

                // tells CE that MC is in the building and will handle pact magic from now on
                Main.IsMulticlassInstalled = Main.Settings.EnableMulticlass;
            };
        }

        private static void DisplayWelcomeMessage()
        {
            if (!Main.Settings.DisplayWelcomeMessage)
            {
                return;
            }

            Main.Settings.DisplayWelcomeMessage = false;

            Gui.GuiService.ShowMessage(
                MessageModal.Severity.Informative1,
                "Message/&MessageModWelcomeTitle",
                "Message/&MessageModWelcomeDescription",
                "Message/&MessageOkTitle",
                string.Empty,
                () => UnityModManager.UI.Instance.ToggleWindow(),
                null);
        }
    }
}
