using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomUI;
using UnityEngine;
using UnityModManagerNet;
#if DEBUG
using SolastaUnfinishedBusiness.DataMiner;
#endif

namespace SolastaUnfinishedBusiness.Models;

internal static class BootContext
{
    private const string BaseURL = "https://github.com/SolastaMods/SolastaUnfinishedBusiness/releases/latest/download";

    private static string InstalledVersion { get; } = GetInstalledVersion();
    private static string LatestVersion { get; set; }
    private static string PreviousVersion { get; } = GetPreviousVersion();

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

        // AI Context
        AiContext.Load();

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

        // Backgrounds may rely on spells and powers being in the DB before they can properly load.
        BackgroundsContext.Load();

        // Subclasses may rely on spells and powers being in the DB before they can properly load.
        SubclassesContext.Load();

        // Deities may rely on spells and powers being in the DB before they can properly load.
        // DeitiesContext.Load();

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
            InventorClass.LateLoadSpellStoringItem();

            // Save by location initialization depends on services to be ready
            SaveByLocationContext.LateLoad();

            // Recache all gui collections
            GuiWrapperContext.Recache();

            // Cache CE definitions for diagnostics and export
            DiagnosticsContext.CacheCeDefinitions();

            // Dump documentations to mod folder
            if (!Directory.Exists($"{Main.ModFolder}/Documentation"))
            {
                Directory.CreateDirectory($"{Main.ModFolder}/Documentation");
            }

            DumpClasses("UnfinishedBusiness", x => x.ContentPack == CeContentPackContext.CeContentPack);
            DumpClasses("Solasta", x => x.ContentPack != CeContentPackContext.CeContentPack);
            DumpSubclasses("UnfinishedBusiness", x => x.ContentPack == CeContentPackContext.CeContentPack);
            DumpSubclasses("Solasta", x => x.ContentPack != CeContentPackContext.CeContentPack);
            DumpRaces("UnfinishedBusiness", x => x.ContentPack == CeContentPackContext.CeContentPack);
            DumpRaces("Solasta", x => x.ContentPack != CeContentPackContext.CeContentPack);
            DumpOthers<FeatDefinition>("UnfinishedBusinessFeats",
                x => x.ContentPack == CeContentPackContext.CeContentPack);
            DumpOthers<FeatDefinition>("SolastaFeats", x => x.ContentPack != CeContentPackContext.CeContentPack);
            DumpOthers<FightingStyleDefinition>("UnfinishedBusinessFightingStyles",
                x => x.ContentPack == CeContentPackContext.CeContentPack);
            DumpOthers<FightingStyleDefinition>("SolastaFightingStyles",
                x => x.ContentPack != CeContentPackContext.CeContentPack);
            DumpOthers<InvocationDefinition>("UnfinishedBusinessInvocations",
                x => x.ContentPack == CeContentPackContext.CeContentPack && x is not InvocationDefinitionCustom);
            DumpOthers<InvocationDefinition>("SolastaInvocations",
                x => x.ContentPack != CeContentPackContext.CeContentPack);
            DumpOthers<SpellDefinition>("UnfinishedBusinessSpells",
                x => x.ContentPack == CeContentPackContext.CeContentPack && !x.Name.StartsWith("SpellPower"));
            DumpOthers<SpellDefinition>("SolastaSpells",
                x => x.ContentPack != CeContentPackContext.CeContentPack);
            DumpOthers<ItemDefinition>("UnfinishedBusinessItems",
                x => x.ContentPack == CeContentPackContext.CeContentPack &&
                     x is ItemDefinition item &&
                     (item.IsArmor || item.IsWeapon));
            DumpOthers<ItemDefinition>("SolastaItems",
                x => x.ContentPack != CeContentPackContext.CeContentPack &&
                     x is ItemDefinition item &&
                     (item.IsArmor || item.IsWeapon));
            DumpOthers<MetamagicOptionDefinition>("UnfinishedBusinessMetamagic",
                x => x.ContentPack == CeContentPackContext.CeContentPack);
            DumpOthers<MetamagicOptionDefinition>("SolastaMetamagic",
                x => x.ContentPack != CeContentPackContext.CeContentPack);

            // really don't have a better place for these fixes here ;-)
            ExpandColorTables();
            AddExtraTooltipDefinitions();

            // avoid folks tweaking max party size directly on settings as we don't need to stress cloud servers
            Main.Settings.OverridePartySize = Math.Min(Main.Settings.OverridePartySize, ToolsContext.MaxPartySize);

            // Manages update or welcome messages
            Load();
            Main.Enable();
        };
    }

    private static string LazyManStripXml(string input)
    {
        return input
            .Replace("<color=#add8e6ff>", string.Empty)
            .Replace("<#57BCF4>", "\n\t")
            .Replace("</color>", string.Empty)
            .Replace("<b>", string.Empty)
            .Replace("<i>", string.Empty)
            .Replace("</b>", string.Empty)
            .Replace("</i>", string.Empty);
    }

    private static void DumpClasses(string groupName, Func<BaseDefinition, bool> filter)
    {
        var outString = new StringBuilder();

        foreach (var klass in DatabaseRepository.GetDatabase<CharacterClassDefinition>()
                     .Where(x => filter(x))
                     .OrderBy(x => x.FormatTitle()))
        {
            outString.Append($"# {klass.FormatTitle()}\n\n");
            outString.Append(klass.FormatDescription());
            outString.Append("\n\n");

            var level = 0;

            foreach (var featureUnlockByLevel in klass.FeatureUnlocks
                         .Where(x => !x.FeatureDefinition.GuiPresentation.hidden)
                         .OrderBy(x => x.level))
            {
                if (level != featureUnlockByLevel.level)
                {
                    outString.Append($"\n## Level {featureUnlockByLevel.level}\n\n");
                    level = featureUnlockByLevel.level;
                }

                var featureDefinition = featureUnlockByLevel.FeatureDefinition;
                var description = LazyManStripXml(featureDefinition.FormatDescription());

                outString.Append($"* {featureDefinition.FormatTitle()}\n\n");
                outString.Append($"{description}\n\n");
            }

            outString.Append("\n\n\n");
        }

        using var sw = new StreamWriter($"{Main.ModFolder}/Documentation/{groupName}Classes.md");
        sw.WriteLine(outString.ToString());
    }

    private static void DumpSubclasses(string groupName, Func<BaseDefinition, bool> filter)
    {
        var outString = new StringBuilder();
        var db = DatabaseRepository.GetDatabase<FeatureDefinitionSubclassChoice>();

        foreach (var subclassChoices in db
                     .OrderBy(x => x.FormatTitle()))
        {
            foreach (var subclass in subclassChoices.Subclasses
                         .Select(DatabaseHelper.GetDefinition<CharacterSubclassDefinition>)
                         .Where(x => filter(x))
                         .OrderBy(x => x.FormatTitle()))
            {
                outString.Append($"# {subclass.FormatTitle()}\n\n");
                outString.Append(subclass.FormatDescription());
                outString.Append("\n\n");

                var level = 0;

                foreach (var featureUnlockByLevel in subclass.FeatureUnlocks
                             .Where(x => !x.FeatureDefinition.GuiPresentation.hidden)
                             .OrderBy(x => x.level))
                {
                    if (level != featureUnlockByLevel.level)
                    {
                        outString.Append($"\n## Level {featureUnlockByLevel.level}\n\n");
                        level = featureUnlockByLevel.level;
                    }

                    var featureDefinition = featureUnlockByLevel.FeatureDefinition;
                    var description = LazyManStripXml(featureDefinition.FormatDescription());

                    outString.Append($"* {featureDefinition.FormatTitle()}\n\n");
                    outString.Append($"{description}\n\n");
                }

                outString.Append("\n\n\n");
            }
        }

        using var sw = new StreamWriter($"{Main.ModFolder}/Documentation/{groupName}Subclasses.md");
        sw.WriteLine(outString.ToString());
    }

    private static void DumpRaces(string groupName, Func<BaseDefinition, bool> filter)
    {
        var outString = new StringBuilder();

        foreach (var race in DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
                     .Where(x => filter(x))
                     .OrderBy(x => x.FormatTitle()))
        {
            outString.Append($"# {race.FormatTitle()}\n\n");
            outString.Append(race.FormatDescription());
            outString.Append("\n\n");

            var level = 0;

            foreach (var featureUnlockByLevel in race.FeatureUnlocks
                         .Where(x => !x.FeatureDefinition.GuiPresentation.hidden)
                         .OrderBy(x => x.level))
            {
                if (level != featureUnlockByLevel.level)
                {
                    outString.Append($"\n## Level {featureUnlockByLevel.level}\n\n");
                    level = featureUnlockByLevel.level;
                }

                var featureDefinition = featureUnlockByLevel.FeatureDefinition;
                var description = LazyManStripXml(featureDefinition.FormatDescription());

                outString.Append($"* {featureDefinition.FormatTitle()}\n\n");
                outString.Append($"{description}\n\n");
            }

            outString.Append("\n\n\n");
        }

        using var sw = new StreamWriter($"{Main.ModFolder}/Documentation/{groupName}Races.md");
        sw.WriteLine(outString.ToString());
    }

    private static void DumpOthers<T>(string groupName, Func<BaseDefinition, bool> filter) where T : BaseDefinition
    {
        var outString = new StringBuilder();
        var db = DatabaseRepository.GetDatabase<T>();

        foreach (var subclass in db
                     .Where(x => filter(x))
                     .OrderBy(x => x.FormatTitle()))
        {
            outString.Append($"# {subclass.FormatTitle()}\n\n");
            outString.Append(subclass.FormatDescription());
            outString.Append("\n\n");
        }

        using var sw = new StreamWriter($"{Main.ModFolder}/Documentation/{groupName}.md");
        sw.WriteLine(outString.ToString());
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
        LatestVersion = GetLatestVersion(out var shouldUpdate);

        if (shouldUpdate && !Main.Settings.DisableUpdateMessage)
        {
            DisplayUpdateMessage();
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

    private static string GetPreviousVersion()
    {
        var a1 = InstalledVersion.Split('.');
        var minor = Int32.Parse(a1[3]);

        a1[3] = (--minor).ToString();

        // ReSharper disable once AssignNullToNotNullAttribute
        return string.Join(".", a1);
    }

    private static string GetLatestVersion(out bool shouldUpdate)
    {
        var version = "";

        shouldUpdate = false;

        using var wc = new WebClient();

        wc.Encoding = Encoding.UTF8;

        try
        {
            var infoPayload = wc.DownloadString($"{BaseURL}/Info.json");
            var infoJson = JsonConvert.DeserializeObject<JObject>(infoPayload);

            // ReSharper disable once AssignNullToNotNullAttribute
            version = infoJson["Version"].Value<string>();

            var a1 = InstalledVersion.Split('.');
            var a2 = version.Split('.');
            var v1 = a1[0] + a1[1] + a1[2] + Int32.Parse(a1[3]).ToString("D3");
            var v2 = a2[0] + a2[1] + a2[2] + Int32.Parse(a2[3]).ToString("D3");

            shouldUpdate = String.Compare(v2, v1, StringComparison.Ordinal) > 0;
        }
        catch
        {
            Main.Error("cannot fetch update data.");
        }

        return version;
    }

    internal static void UpdateMod(bool toLatest = true)
    {
        UnityModManager.UI.Instance.ToggleWindow(false);

        var version = toLatest ? LatestVersion : PreviousVersion;
        var destFiles = new[] { "Info.json", "SolastaUnfinishedBusiness.dll" };

        using var wc = new WebClient();

        wc.Encoding = Encoding.UTF8;

        string message;
        var zipFile = $"SolastaUnfinishedBusiness-{version}.zip";
        var fullZipFile = Path.Combine(Main.ModFolder, zipFile);
        var fullZipFolder = Path.Combine(Main.ModFolder, "SolastaUnfinishedBusiness");
        var baseUrlByVersion = BaseURL.Replace("latest/download", $"download/{version}");
        var url = $"{baseUrlByVersion}/{zipFile}";

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

            message = "Mod version change successful. Please restart.";
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
            () => { });
    }

    internal static void DisplayRollbackMessage()
    {
        UnityModManager.UI.Instance.ToggleWindow(false);

        Gui.GuiService.ShowMessage(
            MessageModal.Severity.Attention2,
            "Message/&MessageModWelcomeTitle",
            $"Would you like to rollback to {PreviousVersion}?",
            "Message/&MessageOkTitle",
            "Message/&MessageCancelTitle",
            () => UpdateMod(false),
            () => { });
    }

    private static void DisplayUpdateMessage()
    {
        Gui.GuiService.ShowMessage(
            MessageModal.Severity.Attention2,
            "Message/&MessageModWelcomeTitle",
            $"Version {LatestVersion} is now available. Open Mod UI > Gameplay > Tools to update.",
            "Changelog",
            "Message/&MessageOkTitle",
            OpenChangeLog,
            () => { });
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
            () => { });
    }

    internal static void OpenDonateGithub()
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

    internal static void OpenChangeLog()
    {
        OpenUrl(
            "https://raw.githubusercontent.com/SolastaMods/SolastaUnfinishedBusiness/master/SolastaUnfinishedBusiness/ChangelogHistory.txt");
    }

    internal static void OpenDocumentation(string filename)
    {
        OpenUrl($"file://{Main.ModFolder}/Documentation/{filename}");
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
