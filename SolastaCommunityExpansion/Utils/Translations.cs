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

    private static string TranslateBaidu(string sourceText, string targetCode)
    {
        const string BASE_URL = "https://api.fanyi.baidu.com/api/trans/vip/translate";

        var encoded = HttpUtility.UrlEncode(sourceText);
        var r = new Random();
        var salt = "";

        for (var i = 0; i < 9; i++)
        {
            salt += r.Next(1, 11);
        }

        try
        {
            var sign = "20181009000216890" + sourceText + salt + "TcAihQsIFCsOdnA14NyA";
            var signHash = GetMd5Hash(sign).ToLower();
            var finalUrl =
                $"{BASE_URL}?appid=20181009000216890&from=en&to={targetCode}&q={encoded}&salt={salt}&sign={signHash}";
            var payload = GetPayload(finalUrl);
            var json = JsonConvert.DeserializeObject<Model>(payload);

            return json.trans_result.First().dst;
        }
        catch
        {
            return sourceText;
        }
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
        var md5 = GetMd5Hash(sourceText);

        if (TranslationsCache.TryGetValue(md5, out var cachedTranslation))
        {
            return cachedTranslation;
        }

        var translation = Main.Settings.TranslationEngine switch
        {
            Engine.Baidu => TranslateBaidu(sourceText, targetCode),
            Engine.Google => TranslateGoogle(sourceText, targetCode),
            _ => sourceText
        };

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
                var splitted = line.Split(new[] {'\t'}, 2);

                words.Add(splitted[0], splitted[1]);
            }
            catch
            {
                Main.Error($"invalid dictionary line \"{line}\".");
            }
        }

        return words;
    }

    [ItemCanBeNull]
    internal static IEnumerable<string> GetTranslations(string languageCode)
    {
        using var zipStream = new MemoryStream(Resources.Translations);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);

        foreach (var entry in zip.Entries
                     .Where(x => x.FullName.StartsWith(languageCode))
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

            text = Glossary.Aggregate(text, (current, kvp) => current.Replace(kvp.Key, kvp.Value));

            var termData = languageSourceData.GetTermData(term);

            if (termData != null && termData.Languages[languageIndex] != null)
            {
                Main.Log($"term {term} overwritten with text {text}");

                termData.Languages[languageIndex] = text;
            }
            else
            {
                languageSourceData.AddTerm(term).Languages[languageIndex] = text;
            }
        }
    }

    public sealed class Model
    {
        public string from { get; set; }
        public string to { get; set; }
        public List<ResultModel> trans_result { get; set; }
    }

    public class ResultModel
    {
        public string src { get; set; }
        public string dst { get; set; }
    }
}
