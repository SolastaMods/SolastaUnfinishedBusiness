using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using I2.Loc;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolastaCommunityExpansion.Properties;

namespace SolastaCommunityExpansion.Utils;

public static class Translations
{
    public enum Engine
    {
        Baidu,
        Google
    }

    private static readonly Dictionary<string, string> TranslationsCache = new();

    internal static readonly string[] AvailableLanguages = {"de", "en", "es", "fr", "it", "pt", "ru", "zh-CN"};

    internal static readonly string[] AvailableEngines = Enum.GetNames(typeof(Engine));

    private static readonly Dictionary<string, string> Glossary = GetWordsDictionary();

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
            Main.Logger.Log("Failed translating: " + sourceText);

            return sourceText;
        }
    }

    internal static string Translate([NotNull] string sourceText, string targetCode)
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

        var translation = TranslateGoogle(sourceText, targetCode);

        TranslationsCache.Add(md5, translation);

        return translation;
    }

    [NotNull]
    private static Dictionary<string, string> GetWordsDictionary()
    {
        var words = new Dictionary<string, string>();
        var path = Path.Combine(Main.ModFolder, "dictionary.txt");

        if (!File.Exists(path))
        {
            return words;
        }

        foreach (var line in File.ReadLines(path))
        {
            try
            {
                var columns = line.Split(new[] {'='}, 2);

                words.Add(columns[0], columns[1]);
            }
            catch
            {
                Main.Error($"invalid dictionary line \"{line}\".");
            }
        }

        return words;
    }

    [ItemCanBeNull]
#if DEBUG
    internal
#else
    private
#endif
        static IEnumerable<string> GetTranslations(string languageCode)
    {
        using var zipStream = new MemoryStream(Resources.Translations);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);

        foreach (var entry in zip.Entries
                     .Where(x => x.FullName.EndsWith($"{languageCode}.txt")))
        {
            using var dataStream = entry.Open();
            using var data = new StreamReader(dataStream);

            while (!data.EndOfStream)
            {
                yield return data.ReadLine();
            }
        }
    }

    internal static void LoadTranslations(string languageCode)
    {
        var languageSourceData = LocalizationManager.Sources[0];
        var languageIndex = languageSourceData.GetLanguageIndex(LocalizationManager.CurrentLanguage);

        foreach (var line in GetTranslations(languageCode))
        {
            if (line == null)
            {
                continue;
            }

            var split = line.Split(new[] {'='}, 2);

            if (split.Length != 2)
            {
                continue;
            }

            var term = split[0];
            var text = split[1];

            if (term == string.Empty)
            {
                continue;
            }

            text = Glossary.Aggregate(text, (current, kvp) => current.Replace(kvp.Key, kvp.Value));

            var termData = languageSourceData.GetTermData(term);

            if (termData?.Languages[languageIndex] != null)
            {
                if (languageIndex == 0)
                {
                    Main.Logger.Log($"term {term} overwritten with text {text}");
                }

                termData.Languages[languageIndex] = text;
            }
            else
            {
                languageSourceData.AddTerm(term).Languages[languageIndex] = text;
            }
        }
    }
}
