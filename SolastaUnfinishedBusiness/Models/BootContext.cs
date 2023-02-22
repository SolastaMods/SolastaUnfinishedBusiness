using System;
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
    private const string BaseURL = "https://github.com/SolastaMods/SolastaUnfinishedBusiness/releases/latest/download";

    internal static void Startup()
    {
#if DEBUG
        ItemDefinitionVerification.Load();
        EffectFormVerification.Load();
#endif

        // STEP 0: Cache TA definitions for diagnostics and export
        DiagnosticsContext.CacheTaDefinitions();

        // Load Translations and Resources Locator after
        TranslatorContext.Load();
        ResourceLocatorContext.Load();

        // Create our Content Pack for anything that gets further created
        CeContentPackContext.Load();
        CustomActionIdContext.Load();

        // Cache all Merchant definitions and what item types they sell
        MerchantTypeContext.Load();

        // Custom Conditions must load as early as possible
        CustomConditionsContext.Load();

        //
        // custom stuff that can be loaded in any order
        //

        CustomReactionsContext.Load();
        CustomWeaponsContext.Load();
        CustomItemsContext.Load();
        PowerBundleContext.Load();

        //
        // other stuff can be loaded in any order
        //

        ToolsContext.Load();
        CharacterExportContext.Load();
        DmProEditorContext.Load();
        GameUiContext.Load();

        // Start will all options under Character
        CharacterContext.Load();

        // Fighting Styles must be loaded before feats to allow feats to generate corresponding fighting style ones.
        FightingStyleContext.Load();

        // Races may rely on spells and powers being in the DB before they can properly load.
        RacesContext.Load();

        // Subclasses may rely on spells and powers being in the DB before they can properly load.
        SubclassesContext.Load();

        // Classes may rely on spells and powers being in the DB before they can properly load.
        ClassesContext.Load();

        // Load SRD and House rules towards the load phase end in case they change previous blueprints
        SrdAndHouseRulesContext.Load();

        // Level 20 must always load after classes and subclasses
        Level20Context.Load();

        // Item Options must be loaded after Item Crafting
        ItemCraftingMerchantContext.Load();
        RecipeHelper.AddRecipeIcons();

        MerchantContext.Load();

        ServiceRepository.GetService<IRuntimeService>().RuntimeLoaded += _ =>
        {
            // DelegatesContext.LateLoad();

            // Late initialized to allow feats and races from other mods
            CharacterContext.LateLoad();

            // There are feats that need all character classes loaded before they can properly be setup.
            FeatsContext.LateLoad();

            // Custom invocations
            InvocationsContext.LateLoad();

            // Custom metamagic
            MetamagicContext.LateLoad();

            // Divine Smite fixes and final switches
            SrdAndHouseRulesContext.LateLoad();

            // Level 20 - patching and final configs
            Level20Context.LateLoad();

            // Multiclass - patching and final configs
            MulticlassContext.LateLoad();

            // Spells context need Level 20 and Multiclass to properly register spells
            SpellsContext.LateLoad();

            // Shared Slots - patching and final configs
            SharedSpellsContext.LateLoad();

            // Set anything on subs that depends on spells and others
            SubclassesContext.LateLoad();

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
        //BUGFIX: expand color tables
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

        var definition = gui.tooltipClassDefinitions[GuiFeatDefinition.tooltipClass];

        var index = definition.tooltipFeatures.FindIndex(f =>
            f.scope == TooltipDefinitions.Scope.All &&
            f.featurePrefab.GetComponent<TooltipFeature>() is TooltipFeaturePrerequisites);

        if (index >= 0)
        {
            var custom = GuiTooltipClassDefinitionBuilder
                .Create(gui.tooltipClassDefinitions["ItemDefinition"], CustomItemTooltipProvider.ItemWithPreReqsTooltip)
                .SetGuiPresentationNoContent()
                .AddTooltipFeature(definition.tooltipFeatures[index])
                //TODO: figure out why only background widens, but not content
                // .SetPanelWidth(400f) //items have 340f by default
                .AddToDB();

            gui.tooltipClassDefinitions.Add(custom.Name, custom);
        }

        //make condition description visible on both modes
        definition = gui.tooltipClassDefinitions[GuiActiveCondition.tooltipClass];
        index = definition.tooltipFeatures.FindIndex(f =>
            f.scope == TooltipDefinitions.Scope.Simplified &&
            f.featurePrefab.GetComponent<TooltipFeature>() is TooltipFeatureDescription);

        if (index < 0)
        {
            return;
        }

        //since FeatureInfo is a struct we get here a copy
        var info = definition.tooltipFeatures[index];
        //modify it
        info.scope = TooltipDefinitions.Scope.All;
        //and then put copy back
        definition.tooltipFeatures[index] = info;
    }

    private static void Load()
    {
        if (ShouldUpdate(out var version, out var changeLog))
        {
            DisplayUpdateMessage(version, changeLog);
        }
        else if (!Main.Settings.HideWelcomeMessage)
        {
            DisplayWelcomeMessage();

            Main.Settings.HideWelcomeMessage = true;
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
        var hasUpdate = false;

        version = "";
        changeLog = "";

        using var wc = new WebClient();

        wc.Encoding = Encoding.UTF8;

        try
        {
            var installedVersion = GetInstalledVersion();
            var infoPayload = wc.DownloadString($"{BaseURL}/Info.json");
            var infoJson = JsonConvert.DeserializeObject<JObject>(infoPayload);

            // ReSharper disable once AssignNullToNotNullAttribute
            version = infoJson["Version"].Value<string>();

            var a1 = installedVersion.Split('.');
            var a2 = version.Split('.');
            var v1 = a1[0] + a1[1] + a1[2] + Int32.Parse(a1[3]).ToString("D3");
            var v2 = a2[0] + a2[1] + a2[2] + Int32.Parse(a2[3]).ToString("D3");

            hasUpdate = String.Compare(v2, v1, StringComparison.Ordinal) > 0;
            changeLog = wc.DownloadString($"{BaseURL}/Changelog.txt");
        }
        catch
        {
            Main.Error("cannot fetch update data.");
        }

        return hasUpdate;
    }

    private static void UpdateMod(string version)
    {
        var destFiles = new[] { "Info.json", "SolastaUnfinishedBusiness.dll" };

        using var wc = new WebClient();

        wc.Encoding = Encoding.UTF8;

        string message;
        var zipFile = $"SolastaUnfinishedBusiness-{version}.zip";
        var fullZipFile = Path.Combine(Main.ModFolder, zipFile);
        var fullZipFolder = Path.Combine(Main.ModFolder, "SolastaUnfinishedBusiness");
        var url = $"{BaseURL}/{zipFile}";

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

        Gui.GuiService.ShowMessage(
            MessageModal.Severity.Attention2,
            "Message/&MessageModWelcomeTitle",
            message,
            "Donate",
            "Message/&MessageOkTitle",
            OpenDonatePayPal,
            () => { }); // keep like this - don't use null here
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
            $"Version {version} is now available.\n\n{changeLog}\n\nWould you like to update?\n\nThe donate button will take you to PayPal by default. There are other donation options under Mod UI > Character.",
            "Message/&MessageOkTitle",
            "Message/&MessageCancelTitle",
            () => UpdateMod(version),
            () => { }); // keep like this - don't use null here
    }

    private static void DisplayWelcomeMessage()
    {
        Gui.GuiService.ShowMessage(
            MessageModal.Severity.Attention2,
            "Message/&MessageModWelcomeTitle",
            "Message/&MessageModWelcomeDescription",
            "Donate",
            "Message/&MessageOkTitle",
            OpenDonatePayPal,
            () => { }); // keep like this - don't use null here
    }

    internal static void OpenWiki()
    {
        OpenUrl("https://github.com/SolastaMods/SolastaUnfinishedBusiness/wiki");
    }

    internal static void OpenDonateGithubSponsors()
    {
        OpenUrl("https://github.com/sponsors/ThyWoof");
    }

    internal static void OpenDonatePatreon()
    {
        OpenUrl("https://patreon.com/SolastaMods");
    }

    internal static void OpenDonatePayPal()
    {
        OpenUrl("https://www.paypal.com/donate/?business=JG4FX47DNHQAG&item_name=Support+Solasta+Unfinished+Business");
    }

    internal static void OpenDiscord()
    {
        OpenUrl("https://discord.com/invite/uu8utaF");
    }

    internal static void OpenChangeLog()
    {
        OpenUrl(
            "https://raw.githubusercontent.com/SolastaMods/SolastaUnfinishedBusiness/master/SolastaUnfinishedBusiness/ChangelogHistory.txt");
    }

    private static void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // because of this: https://github.com/dotnet/corefx/issues/10361
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
