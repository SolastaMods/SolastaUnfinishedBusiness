using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Utils;
using SolastaMonsters.Models;
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
        //
        // Skyrim Load Order (ops, Solasta, lol)
        //
        internal static void Postfix()
        {
#if DEBUG
            ItemDefinitionVerification.Load();
            EffectFormVerification.Load();
#endif
            Translations.LoadTranslations("Modui");
            Translations.LoadTranslations("Translations");

            ResourceLocatorContext.Load();

            // Cache TA definitions for diagnostics and export
            DiagnosticsContext.CacheTADefinitions();

            // Needs to be after CacheTADefinitions
            CeContentPackContext.Load();

            // Cache all Merchant definitions and what item types they sell
            MerchantTypeContext.Load();

            // These can be loaded in any order so we bump them at the beginning
            AdditionalNamesContext.Load();
            BugFixContext.Load();
            CharacterExportContext.Load();
            ConjurationsContext.Load();
            DmProEditorContext.Load();
            FaceUnlockContext.Load();

            // Fighting Styles must be loaded before feats to allow feats to generate corresponding fighting style ones.
            FightingStyleContext.Load();

            FlexibleBackgroundsContext.Switch();
            InitialChoicesContext.Load();
            GameUiContext.Load();
            InventoryManagementContext.Load();

            CustomWeapons.Load();
            ShieldStrikeContext.Load();
            ItemCraftingContext.Load();
            ItemOptionsContext.Load();
            Level20Context.Load();
            PickPocketContext.Load();
            CustomReactionsContext.Load();

            // Powers needs to be added to db before spells because of summoned creatures that have new powers defined here.
            PowersContext.Load();

            RemoveBugVisualModelsContext.Load();
            RespecContext.Load();
            LevelDownContext.Load();

            // There are spells that rely on new monster definitions with powers loaded during the PowersContext. So spells should get added to db after powers.
            SpellsContext.Load();
            SrdAndHouseRulesContext.Load();

            // Races may rely on spells and powers being in the DB before they can properly load.
            RacesContext.Load();

            // Classes may rely on spells and powers being in the DB before they can properly load.
            ClassesContext.Load();

            // Subclasses may rely on classes being loaded (as well as spells and powers) in order to properly refer back to the class.
            SubclassesContext.Load();

            // Multiclass blueprints should always load to avoid issues with heroes saves and after classes and subclasses
            MulticlassContext.Load();

            // Custom High Level Monsters
            MonsterContext.Load();

            // Only a functor registration
            PowerBundleContext.Load();

            ServiceRepository.GetService<IRuntimeService>().RuntimeLoaded += _ =>
            {
                // Late initialized to allow feats and races from other mods
                FlexibleRacesContext.LateLoad();
                InitialChoicesContext.LateLoad();

                // There are feats that need all character classes loaded before they can properly be setup.
                FeatsContext.LateLoad();

                // Generally available powers need all classes in the db before they are initialized here.
                PowersContext.LateLoad();

                // Spells context needs character classes (specifically spell lists) in the db in order to do it's work.
                SpellsContext.LateLoad();

                // Integration Context
                IntegrationContext.LateLoad();

                // Divine Smite fixes
                HouseFeatureContext.LateLoad();

                // Multiclass
                MulticlassContext.LateLoad();

                // Classes Features Sorting
                ClassesContext.LateLoad();

                // Save by location initialization depends on services to be ready
                SaveByLocationContext.LateLoad();

                // Recache all gui collections
                GuiWrapperContext.Recache();

                // Cache CE definitions for diagnostics and export
                DiagnosticsContext.CacheCEDefinitions();

                Main.Enabled = true;
                Main.Logger.Log("Enabled.");

                DisplayWelcomeMessage();
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
