using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using I2.Loc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolastaCommunityExpansion.Properties;

namespace SolastaCommunityExpansion.Utils;

public static class Translations
{
    internal static string Translate(string sourceText, string targetCode)
    {
        var translation = string.Empty;

        try
        {
            var translationFromGoogle = "";

            //Using secret translate.googleapis.com API that is internally used by the Google Translate extension for Chrome and requires no authentication
            var url = string.Format(
                "https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                "auto", targetCode, HttpUtility.UrlEncode(sourceText));

            using (var wc = new WebClient())
            {
                wc.Headers.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                wc.Encoding = Encoding.UTF8;

                translationFromGoogle = wc.DownloadString(url);
            }

            // Get translated text
            var json = JsonConvert.DeserializeObject(translationFromGoogle);

            translation = ((((json as JArray).First() as JArray).First() as JArray).First() as JValue).Value.ToString();
        }
        catch
        {
            Main.Logger.Log("Failed translating: " + sourceText);
        }

        return translation;
    }

    private static Dictionary<string, string> GetWordsDictionary()
    {
        var words = new Dictionary<string, string>();
        var path = Path.Combine(Main.MOD_FOLDER, "dictionary.txt");

        if (!File.Exists(path))
        {
            return words;
        }

        foreach (var line in File.ReadLines(path))
        {
            try
            {
                var splitted = line.Split(new[] {'\t', ' '}, 2);

                words.Add(splitted[0], splitted[1]);
            }
            catch
            {
                Main.Error($"invalid dictionary line \"{line}\".");
            }
        }

        return words;
    }

    public static void LoadTranslations(string category)
    {
        var languageSourceData = LocalizationManager.Sources[0];
        var languageIndex = languageSourceData.GetLanguageIndex(LocalizationManager.CurrentLanguage);
        var languageCode = LocalizationManager.CurrentLanguageCode.Replace("-", "_");
        var payload = (string)typeof(Resources).GetProperty(category + '_' + languageCode).GetValue(null);
        var lines = new List<string>(payload.Split(new[] {Environment.NewLine}, StringSplitOptions.None));

        foreach (var line in lines)
        {
            string term;
            string text;

            try
            {
                var splitted = line.Split(new[] {'\t', ' '}, 2);

                term = splitted[0];
                text = splitted[1];

                foreach (var kvp in GetWordsDictionary())
                {
                    text = text.Replace(kvp.Key, kvp.Value);
                }
            }
            catch
            {
                Main.Error($"invalid translation line \"{line}\".");

                continue;
            }

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
}
