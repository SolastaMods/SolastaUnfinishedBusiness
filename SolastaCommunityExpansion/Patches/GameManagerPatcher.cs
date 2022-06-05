using System.Diagnostics.CodeAnalysis;
using System.IO;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Utils;
using SolastaMonsters.Models;
using UnityModManagerNet;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            // Translations must load first
            Translations.LoadTranslations("Modui");
            Translations.LoadTranslations("Translations");

            // Resources must load second
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
            CustomReactionsContext.Load();
            CustomWeaponsContext.Load();
            DmProEditorContext.Load();
            FaceUnlockContext.Load();
            FlexibleBackgroundsContext.Switch();
            GameUiContext.Load();
            InitialChoicesContext.Load();
            InventoryManagementContext.Load();
            ItemCraftingContext.Load();
            ItemOptionsContext.Load();
            Level20Context.Load();
            LevelDownContext.Load();
            PickPocketContext.Load();
            PowerBundleContext.Load();
            RemoveBugVisualModelsContext.Load();
            RespecContext.Load();
            ShieldStrikeContext.Load();

            // Fighting Styles must be loaded before feats to allow feats to generate corresponding fighting style ones.
            FightingStyleContext.Load();

            // Powers needs to be added to db before spells because of summoned creatures that have new powers defined here.
            PowersContext.Load();

            // There are spells that rely on new monster definitions with powers loaded during the PowersContext. So spells should get added to db after powers.
            SpellsContext.Load();

            // Races may rely on spells and powers being in the DB before they can properly load.
            RacesContext.Load();

            // Classes may rely on spells and powers being in the DB before they can properly load.
            ClassesContext.Load();

            // Subclasses may rely on classes being loaded (as well as spells and powers) in order to properly refer back to the class.
            SubclassesContext.Load();

            // Multiclass blueprints should always load to avoid issues with heroes saves and after classes and subclasses
            MulticlassContext.Load();

            // Load SRD and House rules last in case they change previous blueprints
            SrdAndHouseRulesContext.Load();

            // Custom High Level Monsters
            MonsterContext.Load();

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

                if (CheckForUpdates(out var version, out var changeLog))
                {
                    DisplayUpdateMessage(version, changeLog);
                }
                else
                {
                    DisplayWelcomeMessage();
                }
            };
        }

        private static string GetInstalledVersion()
        {
            var infoPayload = File.ReadAllText(Path.Combine(Main.MOD_FOLDER, "Info.json"));
            var infoJson = JsonConvert.DeserializeObject<JObject>(infoPayload);

            return infoJson["Version"].Value<string>();
        }

        private static bool CheckForUpdates(out string version, out string changeLog)
        {
            const string BASE_URL = "https://raw.githubusercontent.com/SolastaMods/SolastaCommunityExpansion/master/SolastaCommunityExpansion";

            var hasUpdate = false;

            version = "";
            changeLog = "";

            using var wc = new WebClient();

            wc.Encoding = Encoding.UTF8;

            try
            {
                var infoPayload = wc.DownloadString($"{BASE_URL}/Info.json");
                var infoJson = JsonConvert.DeserializeObject<JObject>(infoPayload);

                version = infoJson["Version"].Value<string>();
                hasUpdate = version.CompareTo(GetInstalledVersion()) > 0;

                changeLog = wc.DownloadString($"{BASE_URL}/Changelog.txt");
            }
            catch
            {
                Main.Logger.Log("cannot fetch update data.");
            }

            return hasUpdate;
        }

        private static void UpdateMod(string version)
        {
            using var wc = new WebClient();

            wc.Encoding = Encoding.UTF8;

            try
            {
                var files = new[] {"SolastaCommunityExpansion.dll", "Info.json"};

                foreach (var file in files)
                {
                    var fullFileName = Path.Combine(Main.MOD_FOLDER, file);

                    wc.DownloadFile(
                        $"https://github.com/SolastaMods/SolastaCommunityExpansion/releases/download/{version}/{file}",
                        $"{fullFileName}.UPD");

                    File.Delete(fullFileName);
                    File.Move($"{fullFileName}.UPD", fullFileName);
                }

                Gui.GuiService.ShowMessage(
                    MessageModal.Severity.Informative1,
                    "Message/&MessageModWelcomeTitle",
                    "Update successful. Please restart.",
                    "Message/&MessageOkTitle",
                    string.Empty,
                    null,
                    null);
            }
            catch
            {
                Main.Logger.Log("cannot fetch update payload.");
            }
        }

        private static void DisplayUpdateMessage(string version, string changeLog)
        {
            if (changeLog == string.Empty)
            {
                changeLog = ". no changelog found";
            }

            Gui.GuiService.ShowMessage(
                MessageModal.Severity.Attention2,
                "Message/&MessageModWelcomeTitle",
                $"Version {version} is now available.\n\n{changeLog}\n\nWould you like to update?",
                "Message/&MessageOkTitle",
                "Message/&MessageCancelTitle",
                () => UpdateMod(version),
                null);
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
