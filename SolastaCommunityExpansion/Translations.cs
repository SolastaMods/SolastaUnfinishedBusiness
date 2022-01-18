using System.IO;
using I2.Loc;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace SolastaCommunityExpansion
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    internal static class Translations
    {
        internal static void Load(string fromFolder)
        {
            var languageSourceData = LocalizationManager.Sources[0];

            foreach (var path in Directory.EnumerateFiles(fromFolder, "Translations-??.txt"))
            {
                var filename = Path.GetFileName(path);
                var code = filename.Substring(13, 2);
                var languageIndex = languageSourceData.GetLanguageIndexFromCode(code);

                if (languageIndex < 0)
                {
                    Main.Error($"language {code} not currently loaded.");
                    continue;
                }

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

                    if (termData?.Languages[languageIndex] != null)
                    {
                        Main.Warning($"term {term} overwritten with {code} text {text}");

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
}
