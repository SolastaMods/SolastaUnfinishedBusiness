using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using I2.Loc;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using TMPro;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Models;

internal struct LanguageEntry
{
    public string Code;
    public string Text;
    public string Directory;
    [UsedImplicitly] public string SourceCode;
}

internal static class TranslatorContext
{
    private const string UnofficialLanguagesFolderPrefix = "UnofficialTranslations/";

    internal const string English = "en";

    private static readonly Dictionary<string, string> TranslationsCache = new();

    internal static readonly string[] AvailableLanguages =
    [
        "de", "en", "es", "fr", "ja", "it", "ko", "pt", "ru", "zh-CN"
    ];

    internal static readonly List<LanguageEntry> Languages = [];

    private static readonly Regex RegexHasCJK = new(@"\p{IsCJKUnifiedIdeographs}", RegexOptions.Compiled);

    /// <summary>
    ///     Maps unofficial language codes to official language codes.
    /// </summary>
    private static Dictionary<string, string> SourceCodeCache { get; } = new();

    public static bool IsCJKChar(char c)
    {
        return c >= 0x4E00 && c <= 0x9FA5;
    }

    public static bool HasCJKChar(string s)
    {
        return s.Length > 0 && RegexHasCJK.IsMatch(s);
    }

    public static bool HasCJKCharQuick(string s)
    {
        return s.Length > 0 && IsCJKChar(s[0]);
    }

    internal static void EarlyLoad()
    {
        if (Main.Settings.DisableUnofficialTranslations)
        {
            Main.Info("Unofficial translations support disabled.");

            return;
        }

        if (!Directory.Exists(Path.Combine(Main.ModFolder, UnofficialLanguagesFolderPrefix)))
        {
            Main.Error("Unofficial translations not found.");

            return;
        }

        LoadCustomLanguages();
        LoadCustomTerms();

        // LOAD CUSTOM FONTS

        var allFonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();

        // JAPANESE

        var fullFilename = Path.Combine(Main.ModFolder, $"{UnofficialLanguagesFolderPrefix}JapaneseHanSans.unity3d");

        if (!File.Exists(fullFilename))
        {
            Main.Error($"Loading the font bundle {fullFilename}.");
        }
        else
        {
            var fontBundle = AssetBundle.LoadFromFile(fullFilename);

            AddFont("NotoSansJP-Light SDF", fontBundle, allFonts, "Noto-Light SDF", "Noto-Thin SDF");
            AddFont("NotoSansJP-Regular SDF", fontBundle, allFonts, "Noto-Regular SDF", "LiberationSans SDF");
            AddFont("NotoSansJP-Bold SDF", fontBundle, allFonts, "Noto-Bold SDF");
        }

        // KOREAN

        fullFilename = Path.Combine(Main.ModFolder, $"{UnofficialLanguagesFolderPrefix}KoreanHanSans.unity3d");

        if (!File.Exists(fullFilename))
        {
            Main.Error($"Loading the font bundle {fullFilename}.");
        }
        else
        {
            var fontBundle = AssetBundle.LoadFromFile(fullFilename);

            AddFont("SourceHanSansK-Light SDF", fontBundle, allFonts, "Noto-Light SDF", "Noto-Thin SDF");
            AddFont("SourceHanSansK-Regular SDF", fontBundle, allFonts, "Noto-Regular SDF", "LiberationSans SDF");
            AddFont("SourceHanSansK-Bold SDF", fontBundle, allFonts, "Noto-Bold SDF");
        }
    }

    private static void LoadCustomLanguages()
    {
        var cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures);
        var directoryInfo = new DirectoryInfo($@"{Main.ModFolder}/{UnofficialLanguagesFolderPrefix}");
        var directories = directoryInfo.GetDirectories();

        foreach (var directory in directories)
        {
            var code = directory.Name;
            var cultureInfo = cultureInfos.FirstOrDefault(o => o.Name == code);

            if (File.Exists($"{directory.FullName}/info.json"))
            {
                var info = JsonConvert.DeserializeObject<JObject>(File.ReadAllText($"{directory.FullName}/info.json"));
                var sourceCode = info["SourceCode"]?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(sourceCode))
                {
                    SourceCodeCache.Add(code, sourceCode);
                }

                Languages.Add(new LanguageEntry
                {
                    Code = code,
                    Text = info["NativeName"]!.ToString(),
                    Directory = directory.FullName,
                    SourceCode = sourceCode
                });

                Main.Info($"Language {code} detected.");
            }
            else if (cultureInfo != null)
            {
                if (LocalizationManager.HasLanguage(cultureInfo.DisplayName))
                {
                    Main.Error($"Language {code} from {directory.Name} already in game.");
                }
                else
                {
                    Languages.Add(new LanguageEntry
                    {
                        Code = code,
                        Text = cultureInfo.TextInfo.ToTitleCase(cultureInfo.NativeName),
                        Directory = directory.FullName
                    });

                    Main.Info($"Language {code} detected.");
                }
            }
            else
            {
                Main.Error($"Language {code} illegal!");
            }
        }
    }

    private static void LoadCustomTerms()
    {
        var languageSourceData = LocalizationManager.Sources[0];

        // load new language terms
        foreach (var language in Languages)
        {
            // add language
            languageSourceData.AddLanguage(language.Text, language.Code);

            var languageIndex = languageSourceData.GetLanguageIndex(language.Text);

            // add terms
            var directoryInfo = new DirectoryInfo(language.Directory);
            var files = directoryInfo.GetFiles("*.txt");
            var separator = new[] { '=' };

            foreach (var file in files)
            {
                using var sr = new StreamReader(file.FullName);

                while (sr.ReadLine() is { } line)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    try
                    {
                        var split = line.Split(separator, 2);
                        var term = split[0];
                        var text = split[1];

                        languageSourceData.AddTerm(term).Languages[languageIndex] = text;
                    }
                    catch
                    {
                        Main.Error($"Skipping line [{line}] in file [{file.FullName}]");
                    }
                }
            }
        }
    }

    private static void AddFont(
        string fontName,
        AssetBundle fontBundle,
        IEnumerable<TMP_FontAsset> allFonts,
        params string[] fontsToAppend)
    {
        var modFontAsset = fontBundle.LoadAsset<TMP_FontAsset>($"{fontName}.asset");

        if (!modFontAsset)
        {
            Main.Error($"Font asset {fontName} not found.");

            return;
        }

        foreach (var tmpFontAsset in allFonts.Where(x => fontsToAppend.Contains(x.name)))
        {
            tmpFontAsset.fallbackFontAssetTable.Add(modFontAsset);

            Main.Info($"Font asset {fontName} loaded.");
        }
    }

    [NotNull]
    private static string GetPayload([NotNull] string url)
    {
        using var wc = new WebClient();

        wc.Headers.Add("user-agent",
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
        wc.Encoding = Encoding.UTF8;

        return wc.DownloadString(url);
    }

    [NotNull]
    private static string GetMd5Hash([NotNull] string input)
    {
        var builder = new StringBuilder();
        var md5Hash = MD5.Create();
        var payload = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        foreach (var item in payload)
        {
            builder.Append(item.ToString("x2"));
        }

        return builder.ToString();
    }

    private static string TranslateGoogle(string sourceText, string targetCode)
    {
        const string BASE = "https://translate.googleapis.com/translate_a/single";

        try
        {
            var encoded = HttpUtility.UrlEncode(sourceText);
            var url = $"{BASE}?client=gtx&sl=auto&tl={targetCode}&dt=t&q={encoded}";
            var payload = GetPayload(url);
            var json = JsonConvert.DeserializeObject(payload);
            var result = string.Empty;

            if (json is not JArray outerArray)
            {
                return sourceText;
            }

            return outerArray.First() is not JArray terms
                ? sourceText
                : terms.Aggregate(result, (current, term) => current + term.First());
        }
        catch
        {
            Main.Error("Failed translating: " + sourceText);

            return sourceText;
        }
    }

    private static string Translate([NotNull] string sourceText, string targetCode)
    {
        if (sourceText == string.Empty)
        {
            return string.Empty;
        }

        var md5 = GetMd5Hash(sourceText);

        if (TranslationsCache.TryGetValue(md5, out var cachedTranslation))
        {
            return cachedTranslation;
        }

        var translation = TranslateGoogle(sourceText.Replace("_", " "), targetCode);

        TranslationsCache.Add(md5, translation);

        return translation;
    }

    private static bool IsModTerm(string fullName, string languageCode)
    {
        return fullName.StartsWith(languageCode) && fullName.EndsWith($"{languageCode}.txt");
    }

    private static bool IsFixedTerm(string fullName, string languageCode)
    {
        return fullName == $"Fixes-{languageCode}.txt";
    }

    [UsedImplicitly]
    internal static IEnumerable<string> GetTranslations(string languageCode, Func<string, string, bool> validate)
    {
        using var zipStream = new MemoryStream(Properties.Resources.Translations);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);

        foreach (var entry in zip.Entries.Where(x => validate(x.FullName, languageCode)))
        {
            using var dataStream = entry.Open();
            using var data = new StreamReader(dataStream);

            while (!data.EndOfStream)
            {
                yield return data.ReadLine();
            }
        }
    }

    private static Dictionary<string, string> GetTermsDict(
        string languageCode,
        Func<string, string, bool> validate)
    {
        var result = new Dictionary<string, string>();
        var separator = new[] { '=' };

        if (SourceCodeCache.TryGetValue(languageCode, out var sourceCode))
        {
            // if has source language, use it
            languageCode = sourceCode;
        }

        foreach (var line in GetTranslations(languageCode, validate))
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            var split = line.Split(separator, 2);

            if (split.Length != 2)
            {
                Main.Error($"cannot parse line {line}");
                continue;
            }

            var term = split[0];
            var text = split[1];

            if (result.ContainsKey(term))
            {
                Main.Error($"duplicate term {term}");
            }
            else
            {
                result.Add(term, text);
            }
        }

        return result;
    }

    internal static void Load()
    {
        var languageCode = LocalizationManager.CurrentLanguageCode;

        var englishTerms = GetTermsDict(English, IsModTerm);
        var currentLanguageTerms = languageCode != English ? GetTermsDict(languageCode, IsModTerm) : englishTerms;
        var fixedTerms = GetTermsDict(languageCode, IsFixedTerm);

        var languageSourceData = LocalizationManager.Sources[0];
        var languageIndex = languageSourceData.GetLanguageIndex(LocalizationManager.CurrentLanguage);

        // loads mod translations
        // we loop on default EN terms collection as this is the one to be trusted
        var lineCount = 0;

        foreach (var term in englishTerms.Keys)
        {
            // if we find a translated term them we use it otherwise fall back to EN default
            if (!currentLanguageTerms.TryGetValue(term, out var text))
            {
                text = englishTerms[term];
            }

            AddTerm(term, text);

            lineCount++;
        }

        Main.Info($"{lineCount} {languageCode} translation terms loaded of {currentLanguageTerms.Count} provided.");

        // loads official translations fixes
        lineCount = 0;

        foreach (var term in fixedTerms.Keys)
        {
            var text = fixedTerms[term];

            AddTerm(term, text);

            lineCount++;
        }

        Main.Info($"{lineCount} {languageCode} translation fixes loaded.");

        // creates a report on missing terms
        if (languageCode == English)
        {
            return;
        }

        var termsToAdd = englishTerms.Keys.Except(currentLanguageTerms.Keys).ToArray();

        if (termsToAdd.Length != 0)
        {
            Main.Info("ADD THESE TERMS:");

            foreach (var term in termsToAdd)
            {
                Main.Info($"{term} is missing from {languageCode} translation assets");
            }
        }

        var termsToDelete = currentLanguageTerms.Keys.Except(englishTerms.Keys);

        if (!termsToDelete.Any())
        {
            return;
        }

        Main.Info("DELETE THESE TERMS:");

        foreach (var term in currentLanguageTerms.Keys.Except(englishTerms.Keys))
        {
            Main.Info($"{term} must be deleted from {languageCode} translation assets");
        }

        return;

        void AddTerm(string term, string text)
        {
            var termData = languageSourceData.GetTermData(term);

            if (termData?.Languages[languageIndex] != null)
            {
                // ReSharper disable once InvocationIsSkipped
                Main.Log($"term {term} overwritten with text {text}");
                termData.Languages[languageIndex] = text;
            }
            else
            {
                languageSourceData.AddTerm(term).Languages[languageIndex] = text;
            }
        }
    }

    internal static bool HasTranslation(string term)
    {
        return LocalizationManager.Sources[0].ContainsTerm(term);
    }

    internal sealed class TranslatorBehaviour : MonoBehaviour
    {
        internal const string UbTranslationTag = "UB auto translation\n";

        private static TranslatorBehaviour _exporter;

        internal static readonly Dictionary<string, ExportStatus> CurrentExports = new();

        [NotNull]
        private static TranslatorBehaviour Exporter
        {
            get
            {
                if (_exporter)
                {
                    return _exporter;
                }

                _exporter = new GameObject().AddComponent<TranslatorBehaviour>();
                DontDestroyOnLoad(_exporter.gameObject);

                return _exporter;
            }
        }

        internal static void Cancel([NotNull] string exportName)
        {
            if (!CurrentExports.TryGetValue(exportName, out var value) || value.Coroutine == null)
            {
                return;
            }

            Exporter.StopCoroutine(value.Coroutine);
            CurrentExports.Remove(exportName);
        }

        internal static void TranslateUserCampaign(string languageCode, [NotNull] string exportName,
            [NotNull] UserCampaign userCampaign)
        {
            var newUserCampaign = userCampaign.DeepCopy();

            var oldUserCampaignTitle = Regex.Replace(userCampaign.Title, @"\[.+\] - (.*)", "$1");

            newUserCampaign.Title = $"[{languageCode}] - {oldUserCampaignTitle}";
            newUserCampaign.IsWorkshopItem = false;

            var coroutine = TranslateUserCampaignRoutine(languageCode, exportName, newUserCampaign);

            CurrentExports.Add(exportName,
                new ExportStatus
                {
                    Coroutine = Exporter.StartCoroutine(coroutine),
                    PercentageComplete = 0f,
                    LanguageCode = languageCode
                });
        }

        private static IEnumerator TranslateUserCampaignRoutine(
            string languageCode,
            [NotNull] string exportName,
            UserCampaign userCampaign)
        {
            // const string LoreOperation = "LoreOperation";
            // const string SetLocationStatus = "SetLocationStatus";

            yield return null;

            var current = 0;
            var total =
                userCampaign.UserDialogs
                    .SelectMany(x => x.AllDialogStates)
                    .SelectMany(x => x.DialogLines)
                    .Count() +
                userCampaign.UserItems.Count +
                userCampaign.UserMonsters.Count +
                userCampaign.UserNpcs.Count +
                userCampaign.UserMerchantInventories.Count +
                userCampaign.UserLootPacks.Count +
                userCampaign.UserLocations
                    .SelectMany(x => x.GadgetsByName)
                    .Count() +
                userCampaign.UserQuests
                    .SelectMany(x => x.AllQuestStepDescriptions)
                    .SelectMany(x => x.OutcomesTable)
                    .Count() +
                userCampaign.UserBiomes
                    .SelectMany(x => x.narrativeEventBasicLines)
                    .Count() +
                userCampaign.UserEncounterTables.Count +
                userCampaign.userEncounters.Count +
                userCampaign.campaignMapNodes.Count;

            userCampaign.Description = Translate(userCampaign.Description, languageCode);
            userCampaign.TechnicalInfo = UbTranslationTag + Translate(userCampaign.TechnicalInfo, languageCode);

            // Translate locations first, so that the translated cache of the location function can be used later
            // USER LOCATIONS
            foreach (var userLocation in userCampaign.UserLocations)
            {
                // userLocation.Title = Translate(userLocation.Title, languageCode);
                userLocation.Description = Translate(userLocation.Description, languageCode);

                foreach (var gadget in userLocation.GadgetsByName.Values)
                {
                    yield return Update();

                    foreach (var parameterValue in gadget.ParameterValues)
                    {
                        switch (parameterValue.GadgetParameterDescription.Name)
                        {
                            case "Speech":
                            case "Banter":
                            case "BanterLines":
                            // case "DestinationLocation":
                            case "ExitLore":
                                // case "WaypointTitle":
                                parameterValue.StringValue =
                                    Translate(parameterValue.StringValue, languageCode);
                                parameterValue.StringsList = parameterValue.StringsList
                                    .Select(stringValue => Translate(stringValue, languageCode))
                                    .ToList();

                                break;

                            case "LocationsList":
                                foreach (var destination in parameterValue.DestinationsList)
                                {
                                    destination.DisplayedTitle = Translate(destination.DisplayedTitle, languageCode);
                                    // magicSkySword : the location name is actually the location id, so we must let it equal to the location id
                                    // destination.UserLocationName =
                                    //     Translate(destination.UserLocationName, languageCode);
                                }

                                break;
                        }
                    }
                }
            }

            // USER DIALOGS
            foreach (var dialog in userCampaign.UserDialogs)
            {
                dialog.Title = Translate(dialog.Title, languageCode);
                dialog.Description = Translate(dialog.Description, languageCode);

                foreach (var userDialogState in dialog.AllDialogStates
                             .Where(x => x.Type is "AnswerChoice" or "CharacterSpeech" or "NpcSpeech"))
                {
                    foreach (var dialogLine in userDialogState.DialogLines)
                    {
                        yield return Update();

                        dialogLine.TextLine = Translate(dialogLine.TextLine, languageCode);
                    }
                }
            }

            // USER ITEMS
            foreach (var item in userCampaign.UserItems)
            {
                yield return Update();

                item.Title = Translate(item.Title, languageCode);
                item.Description = Translate(item.Title, languageCode);

                if (item.DocumentFragments.Count == 0)
                {
                    continue;
                }

                item.DocumentFragments = item.DocumentFragments
                    .Select(documentFragment => Translate(documentFragment, languageCode)).ToList();
            }

            // USER QUESTS
            foreach (var quest in userCampaign.UserQuests)
            {
                quest.Title = Translate(quest.Title, languageCode);
                quest.Description = Translate(quest.Description, languageCode);

                foreach (var userQuestStep in quest.AllQuestStepDescriptions)
                {
                    userQuestStep.Title = Translate(userQuestStep.Title, languageCode);
                    userQuestStep.Description = Translate(userQuestStep.Description, languageCode);

                    foreach (var outcome in userQuestStep.OutcomesTable)
                    {
                        yield return Update();

                        outcome.DescriptionText = Translate(outcome.DescriptionText, languageCode);

                        if (outcome.validatorDescription.type is not (QuestDefinitions.QuestValidatorType.EnterLocation
                            or
                            QuestDefinitions.QuestValidatorType.EnterCampaignMap or
                            QuestDefinitions.QuestValidatorType.LeaveLocation or
                            QuestDefinitions.QuestValidatorType.LeaveCampaignMap))
                        {
                            outcome.validatorDescription.StringParameter =
                                Translate(outcome.validatorDescription.StringParameter, languageCode);
                        }
                    }
                }
            }

            // USER MONSTERS
            foreach (var monster in userCampaign.UserMonsters)
            {
                yield return Update();

                monster.Title = Translate(monster.Title, languageCode);
                monster.Description = Translate(monster.Description, languageCode);
            }

            // USER NPCs
            foreach (var npc in userCampaign.UserNpcs)
            {
                yield return Update();

                npc.Title = Translate(npc.Title, languageCode);
                npc.Description = Translate(npc.Description, languageCode);
            }

            // USER MERCHANT INVENTORIES
            foreach (var merchantInventory in userCampaign.UserMerchantInventories)
            {
                yield return Update();

                merchantInventory.Title = Translate(merchantInventory.Title, languageCode);
                merchantInventory.Description = Translate(merchantInventory.Description, languageCode);
            }

            // USER LOOT PACKS
            foreach (var lootPack in userCampaign.UserLootPacks)
            {
                yield return Update();

                lootPack.Title = Translate(lootPack.Title, languageCode);
                lootPack.Description = Translate(lootPack.Description, languageCode);
            }

            // USER BIOMES
            foreach (var userBiome in userCampaign.userBiomes)
            {
                userBiome.Title = Translate(userBiome.Title, languageCode);
                userBiome.Description = Translate(userBiome.Description, languageCode);

                for (var index = 0; index < userBiome.NarrativeEventBasicLines.Count; index++)
                {
                    yield return Update();

                    var narrativeEventBasicLine = userBiome.NarrativeEventBasicLines[index];
                    userBiome.NarrativeEventBasicLines[index] = Translate(narrativeEventBasicLine, languageCode);
                }
            }

            // USER ENCOUNTER TABLES
            foreach (var userEncounterTable in userCampaign.userEncounterTables)
            {
                yield return Update();

                userEncounterTable.Title = Translate(userEncounterTable.Title, languageCode);
                userEncounterTable.Description = Translate(userEncounterTable.Description, languageCode);
            }

            // USER ENCOUNTERS
            foreach (var userEncounter in userCampaign.userEncounters)
            {
                yield return Update();

                userEncounter.Title = Translate(userEncounter.Title, languageCode);
                userEncounter.Description = Translate(userEncounter.Description, languageCode);
            }

            // USER CAMPAIGN MAP NODES
            foreach (var userCampaignMapNode in userCampaign.campaignMapNodes)
            {
                yield return Update();

                userCampaignMapNode.unchartedTitle = Translate(userCampaignMapNode.unchartedTitle, languageCode);
                userCampaignMapNode.overriddenTitle = Translate(userCampaignMapNode.overriddenTitle, languageCode);
                userCampaignMapNode.overriddenDescription =
                    Translate(userCampaignMapNode.overriddenDescription, languageCode);
            }

            CurrentExports.Remove(exportName);

            var userCampaignPoolService = ServiceRepository.GetService<IUserCampaignPoolService>();

            userCampaignPoolService.SaveUserCampaign(userCampaign);

            yield break;

            IEnumerator Update()
            {
                current++;
                CurrentExports[exportName].PercentageComplete = (float)current / total;

                yield return null;
            }
        }

        internal sealed class ExportStatus
        {
            internal Coroutine Coroutine;
            internal string LanguageCode;
            internal float PercentageComplete;
        }
    }
}
