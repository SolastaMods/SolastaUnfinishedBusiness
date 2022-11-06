using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using UnityEngine;
#if DEBUG
using SolastaUnfinishedBusiness.DataMiner;
#endif

namespace SolastaUnfinishedBusiness.Models;

internal static class BootContext
{
    internal static readonly HashSet<string> SupportedLanguages = new() { "none" };

    internal static void Startup()
    {
#if DEBUG
        ItemDefinitionVerification.Load();
        EffectFormVerification.Load();
#endif

        // STEP 0: Cache TA definitions for diagnostics and export
        DiagnosticsContext.CacheTaDefinitions();

        // Load Translations and Resources Locator after
        Load();
        ResourceLocatorContext.Load();

        // Create our Content Pack for anything that gets further created
        CeContentPackContext.Load();

        // Cache all Merchant definitions and what item types they sell
        MerchantTypeContext.Load();

        // can be loaded in any order so we bump them at the beginning
        CharacterExportContext.Load();
        ToolsContext.Load();

        CustomReactionsContext.Load();
        CustomWeaponsContext.Load();
        CustomItemsContext.Load();
        PowerBundleContext.Load();

        DmProEditorContext.Load();
        GameUiContext.Load();

        // Start will all options under Character
        CharacterContext.Load();

        // Fighting Styles must be loaded before feats to allow feats to generate corresponding fighting style ones.
        FightingStyleContext.Load();

        // Races may rely on spells and powers being in the DB before they can properly load.
        RacesContext.Load();

        // Classes may rely on spells and powers being in the DB before they can properly load.
        ClassesContext.Load();

        // Subclasses may rely on classes being loaded (as well as spells and powers) in order to properly refer back to the class.
        SubclassesContext.Load();

        // Custom invocations
        InvocationsContext.Load();

        // Load SRD and House rules towards the load phase end in case they change previous blueprints
        SrdAndHouseRulesContext.Load();

        // Level 20 must always load after classes and subclasses
        Level20Context.Load();

        // Item Options must be loaded after Item Crafting
        ItemCraftingMerchantContext.Load();

        MerchantContext.Load();

        ServiceRepository.GetService<IRuntimeService>().RuntimeLoaded += _ =>
        {
            // Late initialized to allow feats and races from other mods
            CharacterContext.LateLoad();

            // There are feats that need all character classes loaded before they can properly be setup.
            FeatsContext.LateLoad();

            // Spells context needs character classes (specifically spell lists) in the db in order to do it's work.
            SpellsContext.LateLoad();

            // Divine Smite fixes and final switches
            SrdAndHouseRulesContext.LateLoad();

            // Level 20 - patching and final configs
            Level20Context.LateLoad();

            // Multiclass - patching and final configs
            MulticlassContext.LateLoad();

            // Shared Slots - patching and final configs
            SharedSpellsContext.LateLoad();

            // Save by location initialization depends on services to be ready
            SaveByLocationContext.LateLoad();

            // Recache all gui collections
            GuiWrapperContext.Recache();

            // Cache CE definitions for diagnostics and export
            DiagnosticsContext.CacheCeDefinitions();

            // really don't have a better place for these fixes here ;-)
            ExpandColorTables();
            AddExtraTooltipDefinitions();

            // Manages update or welcome messages
            Load();
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

    private static void AddExtraTooltipDefinitions()
    {
        if (ServiceRepository.GetService<IGuiService>() is not GuiManager gui)
        {
            return;
        }

        var featDef = gui.tooltipClassDefinitions["FeatDefinition"];

        var prerequisites = featDef.tooltipFeatures.FindIndex(f =>
            f.scope == TooltipDefinitions.Scope.All &&
            f.featurePrefab.GetComponent<TooltipFeature>() is TooltipFeaturePrerequisites);

        if (prerequisites < 0)
        {
            return;
        }

        var custom = GuiTooltipClassDefinitionBuilder
            .Create(gui.tooltipClassDefinitions["ItemDefinition"], CustomItemTooltipProvider.ItemWithPreReqsTooltip)
            .SetGuiPresentationNoContent()
            .AddTooltipFeature(featDef.tooltipFeatures[prerequisites])
            //TODO: figure out why only background widens, but not content
            // .SetPanelWidth(400f) //items have 340f by default
            .AddToDB();

        gui.tooltipClassDefinitions.Add(custom.Name, custom);
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
            MessageModal.Severity.Attention2,
            "Message/&MessageModWelcomeTitle",
            message,
            "Message/&MessageOkTitle",
            "Donate",
            null,
            OpenDonate);
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
            MessageModal.Severity.Attention2,
            "Message/&MessageModWelcomeTitle",
            "Message/&MessageModWelcomeDescription",
            "Message/&MessageOkTitle",
            "Donate",
            null,
            OpenDonate);
    }

    internal static void OpenWiki()
    {
        OpenUrl("https://github.com/SolastaMods/SolastaUnfinishedBusiness/wiki");
    }

    internal static void OpenDonate()
    {
        OpenUrl("https://www.paypal.com/donate/?business=JG4FX47DNHQAG&item_name=Support+Solasta+Unfinished+Business");
    }

    private static void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}
