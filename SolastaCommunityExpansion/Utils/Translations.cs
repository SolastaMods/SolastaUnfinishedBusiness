using System.IO;
using I2.Loc;

namespace SolastaCommunityExpansion.Utils
{
    public static class Translations
    {
        public static void LoadTranslations(string category)
        {
            var code = LocalizationManager.CurrentLanguageCode;
            var path = Path.Combine(Main.MOD_FOLDER, $"{category}-{code}.txt");

            if (!File.Exists(path))
            {
                path = Path.Combine(Main.MOD_FOLDER, $"{category}-en.txt");
            }

            var languageSourceData = LocalizationManager.Sources[0];
            var languageIndex = languageSourceData.GetLanguageIndexFromCode(code);

            foreach (var line in File.ReadLines(path))
            {
                string term;
                string text;

                try
                {
                    var splitted = line.Split(new[] { '\t', ' ' }, 2);

                    term = splitted[0];
                    text = splitted[1];
                }
                catch
                {
                    Main.Error($"invalid translation line \"{line}\".");

                    continue;
                }

                var termData = languageSourceData.GetTermData(term);

                if (termData != null && termData.Languages[languageIndex] != null)
                {
                    Main.Log($"term {term} overwritten with {code} text {text}");

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
