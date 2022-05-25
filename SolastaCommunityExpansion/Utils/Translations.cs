using System;
using System.Collections.Generic;
using System.IO;
using I2.Loc;
using SolastaCommunityExpansion.Properties;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Utils
{
    public static class Translations
    {
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
                    var splitted = line.Split(new[] { '\t', ' ' }, 2);

                    words.Add(splitted[0], splitted[1]);
                }
                catch
                {
                    Main.Error($"invalid dictionary line \"{line}\".");

                    continue;
                }
            }

            return words;
        }

        public static void LoadTranslations(string category)
        {
            var languageSourceData = LocalizationManager.Sources[0];
            var languageIndex = languageSourceData.GetLanguageIndex(LocalizationManager.CurrentLanguage);
            var languageCode = ("frpt-BRruzh-CN".Contains(LocalizationManager.CurrentLanguageCode)
                ? LocalizationManager.CurrentLanguageCode
                : "en")
                .Replace("-", "_");
            var payload = (string)typeof(Resources).GetProperty(category + "_" + languageCode).GetValue(null);
            var lines = new List<string>(payload.Split(new[] { Environment.NewLine }, StringSplitOptions.None));

            foreach (var line in lines)
            {
                string term;
                string text;

                try
                {
                    var splitted = line.Split(new[] { '\t', ' ' }, 2);

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
}
