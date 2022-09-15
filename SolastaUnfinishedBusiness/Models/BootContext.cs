using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolastaUnfinishedBusiness.Utils;
using UnityEngine;
using UnityModManagerNet;
#if DEBUG
using SolastaUnfinishedBusiness.DataMiner;
#endif

namespace SolastaUnfinishedBusiness.Models;

internal static class BootContext
{
    internal static readonly HashSet<string> SupportedLanguages = new() { "zh-CN" };

    public static void Startup()
    {
#if DEBUG
        ItemDefinitionVerification.Load();
        EffectFormVerification.Load();
#endif

        // Translations must load first
        Translations.Load();

        // Resources must load second
        ResourceLocatorContext.Load();

        // Cache TA definitions for diagnostics and export
        DiagnosticsContext.CacheTaDefinitions();

        // Needs to be after CacheTADefinitions
        CeContentPackContext.Load();

        // Cache all Merchant definitions and what item types they sell
        MerchantTypeContext.Load();

        // These can be loaded in any order so we bump them at the beginning
        CharacterExportContext.Load();
        ConjurationsContext.Load();
        CustomReactionsContext.Load();
        CustomWeaponsContext.Load();
        DmProEditorContext.Load();
        FlexibleBackgroundsContext.Load();
        GameUiContext.Load();
        InitialChoicesContext.Load();
        InventoryManagementContext.Load();
        ItemCraftingContext.Load();
        Level20Context.Load();
        PickPocketContext.Load();
        PowersBundleContext.Load();
        RespecContext.Load();
        ShieldStrikeContext.Load();

        // Fighting Styles must be loaded before feats to allow feats to generate corresponding fighting style ones.
        FightingStyleContext.Load();

        // Powers needs to be added to db before spells because of summoned creatures that have new powers defined here.
        PowersContext.Load();

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

        // Item Options must be loaded after Item Crafting
        ItemOptionsContext.Load();

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
            MulticlassIntegrationContext.LateLoad();

            // Divine Smite fixes
            SrdAndHouseRulesContext.LateLoad();

            // Level 20
            Level20Context.LateLoad();

            // Multiclass
            MulticlassContext.LateLoad();

            // Classes Features Sorting
            ClassesContext.LateLoad();

            // Save by location initialization depends on services to be ready
            SaveByLocationContext.LateLoad();

            // Register mod delegates
            // DelegatesContext.LateLoad();

            // Recache all gui collections
            GuiWrapperContext.Recache();

            // Cache CE definitions for diagnostics and export
            DiagnosticsContext.CacheCeDefinitions();

            // Manages update or welcome messages
            Load();

            ExpandColorTables();
            Main.Enable();
        };
    }

    private static void ExpandColorTables()
    {
        //
        // BUGFIX: expand color tables
        //

        for (var i = 21; i < 33; i++)
        {
            Gui.ModifierColors.Add(i, new Color32(0, 164, byte.MaxValue, byte.MaxValue));
            Gui.CheckModifierColors.Add(i, new Color32(0, 36, 77, byte.MaxValue));
        }
    }

    private static void Load()
    {
        if (ShouldUpdate(out var version, out var changeLog))
        {
            DisplayUpdateMessage(version, changeLog);
        }
        else if (Main.Settings.DisplayWelcomeMessage)
        {
            DisplayWelcomeMessage();

            Main.Settings.DisplayWelcomeMessage = false;
        }
    }

    private static string GetInstalledVersion()
    {
        var infoPayload = File.ReadAllText(Path.Combine(Main.ModFolder, "Info.json"));
        var infoJson = JsonConvert.DeserializeObject<JObject>(infoPayload);

        // ReSharper disable once AssignNullToNotNullAttribute
        return infoJson["Version"].Value<string>();
    }

    private static bool ShouldUpdate(out string version, [NotNull] out string changeLog)
    {
        const string BASE_URL =
            "https://raw.githubusercontent.com/SolastaMods/SolastaUnfinishedBusiness/master/SolastaUnfinishedBusiness";

        var hasUpdate = false;

        version = "";
        changeLog = "";

        using var wc = new WebClient();

        wc.Encoding = Encoding.UTF8;

        try
        {
            var infoPayload = wc.DownloadString($"{BASE_URL}/Info.json");
            var infoJson = JsonConvert.DeserializeObject<JObject>(infoPayload);

            // ReSharper disable once AssignNullToNotNullAttribute
            version = infoJson["Version"].Value<string>();
            // ReSharper disable once StringCompareToIsCultureSpecific
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
        const string BASE_URL = "https://github.com/SolastaMods/SolastaUnfinishedBusiness";

        var destFiles = new[] { "Info.json", "SolastaUnfinishedBusiness.dll" };

        using var wc = new WebClient();

        wc.Encoding = Encoding.UTF8;

        string message;
        var zipFile = $"SolastaUnfinishedBusiness-{version}.zip";
        var fullZipFile = Path.Combine(Main.ModFolder, zipFile);
        var fullZipFolder = Path.Combine(Main.ModFolder, "SolastaUnfinishedBusiness");
        var url = $"{BASE_URL}/releases/download/{version}/{zipFile}";

        try
        {
            wc.DownloadFile(url, fullZipFile);

            if (Directory.Exists(fullZipFolder))
            {
                Directory.Delete(fullZipFolder, true);
            }

            ZipFile.ExtractToDirectory(fullZipFile, Main.ModFolder);
            File.Delete(fullZipFile);

            foreach (var destFile in destFiles)
            {
                var fullDestFile = Path.Combine(Main.ModFolder, destFile);

                File.Delete(fullDestFile);
                File.Move(
                    Path.Combine(fullZipFolder, destFile),
                    fullDestFile);
            }

            Directory.Delete(fullZipFolder, true);

            message = "Update successful. Please restart.";
        }
        catch
        {
            message = $"Cannot fetch update payload. Try again or download from:\n{url}.";
        }

        Main.Logger.Log(message);
        Gui.GuiService.ShowMessage(
            MessageModal.Severity.Informative1,
            "Message/&MessageModWelcomeTitle",
            message,
            "Message/&MessageOkTitle",
            string.Empty,
            null,
            null);
    }

    private static void DisplayUpdateMessage(string version, string changeLog)
    {
        if (changeLog == string.Empty)
        {
            changeLog = "- no changelog found";
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
